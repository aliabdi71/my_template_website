function notInRange(x, from, to) {
    if (x >= from && x <= to)
        return false;
    else
        return true;
}

function turn2EnglishNumber(value) {
    if (!value) {
        return;
    }
    value = value.toString().replace(/,/g, '');
    var englishNumbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"],
        persianNumbers = ["۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹", "۰"];

    for (var i = 0, numbersLen = persianNumbers.length; i < numbersLen; i++) {
        value = value.replace(new RegExp(persianNumbers[i], "g"), englishNumbers[i]);
    }
    return value;
}

function turn2PersianNumber(value) {
    if (!value) {
        return '0';
    }
    value = value.toString();
    var englishNumbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"],
        persianNumbers = ["۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹", "۰"];

    for (var i = 0, numbersLen = englishNumbers.length; i < numbersLen; i++) {
        value = value.replace(new RegExp(englishNumbers[i], "g"), persianNumbers[i]);
    }
    return value;
}

function inputNumberWithCommasEnglish(x) {
    var res = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return res;
}

function inputNumberWithCommas(x) {
    var res = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return turn2PersianNumber(res);
}

function numberWithCommas(x) {
    if (x === null)
        return 'ندارد';
    if (x.toString().indexOf('ندارد') > -1)
        return x;
    var res = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return turn2PersianNumber(res) + ' تومان';
}

function convertToJalaliDate(Gdate) {
    var g_date = Gdate.split('/');
    var j_date = toJalaali(parseInt(g_date[2]), parseInt(g_date[0]), parseInt(g_date[1]));
    return turn2PersianNumber(j_date.jy + "/" + j_date.jm + "/" + j_date.jd);
}

function fix_numbers() {
    $(".price").each(function () {
        var val = $(this).html().replace('تومان', '');
        if (val.indexOf('ندارد') > -1)
            return;
        $(this).html(numberWithCommas(val));
        $(this).removeClass("price");
    });
    $(".number").each(function () {
        var val = $(this).html();
        $(this).html(turn2PersianNumber(val));
        $(this).removeClass("number");
    });
}

$(document).ready(function () {
    /*var height = $(window).height();
    var body_height = $(".wrap").height();
    if (body_height < height)
        $(".wrap").css('height', height - 250);*/

    fix_numbers();

    $(".sys-msg").show().delay(5000).fadeOut();

    $("#login_btn").click(function () {

        var email = $("#email").val(),
            pass = $("#password").val(),
            remember = document.getElementById("remember").checked;
        if (email === '') {
            $("#message").html("لطفا ایمیل را وارد کنید");
            return false;
        }
        if (pass === '') {
            $("#message").html("لطفا کلمه عبور را وارد کنید");
            return false;
        }
        $(this).html('<i class="fa fa-refresh fa-spin"></i>').attr('disabled', 'disabled');
        sendDataToServer(email, pass, remember);
        return true;
    });

    $("#email, #password").keypress(function (e) {
        if (e.which == 13) {
            $('#login_btn').trigger('click');
        }
    });

    $(".banner-panel table td > select").select2();

});

function adverNextClick(elem) {
    var page = parseInt($(elem).data('index'));
    $(elem).html('<i class="fa fa-refresh fa-spin"></i>').attr('disabled', 'disabled');
    var postData = { "p": page };
    $.ajax({
        type: 'POST',
        url: "/Home/GetAdversByPage",
        dataType: 'json',
        data: postData,
        success: function (data) {
            $(elem).data('index', page + 1);
            $("#adverPrevPage").data('index', page - 1);
            $("#adverPrevPage").html('<img src="/Content/shared_images/arrow-left.png" />').removeAttr('disabled');
            createAdvertiseSectionForHomePage(data);
            $(elem).html('<img src="/Content/shared_images/arrow-right.png" />').removeAttr('disabled');
        },
        error: function () {
            alert('خطا در برقراری ارتباط با سرور');
        },
    });
}

function adverPrevClick(elem) {
    var page = parseInt($(elem).data('index'));
    $(elem).html('<i class="fa fa-refresh fa-spin"></i>').attr('disabled', 'disabled');
    var postData = { "p": page };
    $.ajax({
        type: 'POST',
        url: "/Home/GetAdversByPage",
        dataType: 'json',
        data: postData,
        success: function (data) {
            $(elem).data('index', page - 1);
            $("#adverNextPage").data('index', page + 1);
            $("#adverNextPage").removeAttr('disabled');
            createAdvertiseSectionForHomePage(data);
            if (page - 1 === 0) {
                $(elem).html('<img src="/Content/shared_images/arrow-left-dis.png" />').attr('disabled', 'disabled');
            }
            else {
                $(elem).html('<img src="/Content/shared_images/arrow-left.png" />').removeAttr('disabled');
            }
        },
        error: function () {
            alert('خطا در برقراری ارتباط با سرور');
        },
    });
}

function createAdvertiseSectionForHomePage(data) {
    var inner_html = "";
    for (var i = 0 ; i < data.length ; i = i + 3) {
        inner_html += '<div class="row" style="margin: 20px 20px 0;">';
        for (var j = i ; j < i + 3 && j < data.length ; j++) {
            inner_html += '<div class="col-md-4">';
            inner_html += '<a class="adver-link" href="/Advertise/AddvertiseDetail?AddvertiseID=' + data[j].ID + '">';
            inner_html += ' <div class="adver-content">';
            inner_html += ' <div class="adver-image"><img src="/Content/advertise_images/' + data[j].ImgUrl + '" />';
            inner_html += ' </div> <div class="adver-description text-center">';
            inner_html += ' <h1>' + data[j].Title + '</h1>';
            inner_html += ' <h2>' + data[j].Condition + ' - ' + data[j].District + '</h2>';
            inner_html += ' <br /><br />';
            inner_html += ' <h1>کل/رهن: ' + numberWithCommas(data[j].FirstPrice) + '</h1>';
            inner_html += ' <h2>متری/اجاره: ' + numberWithCommas(data[j].SecondPrice) + '</h2>';
            inner_html += ' <h2>' + turn2PersianNumber(data[j].Date) + '</h2>';
            inner_html += '</div></div></a></div>';
        }
        inner_html += '</div>';
    }
    $("#adverPartialSection").fadeOut('normal').html(inner_html).fadeIn('normal');
}

function sendDataToServer(email, pass, remember) {
    var postData = { "email": email, "password": pass, "remember": remember };
    $.ajax({
        type: 'POST',
        url: "/Account/Login",
        dataType: 'json',
        data: postData,
        success: function (data) {
            if (data === "success") {
                location.reload();
            }
            else {
                $("#login_btn").html('ورود').removeAttr('disabled');
                //$("#loading_button").css('display', 'none');
                $("#message").html(data);
            }
        },
        error: function () {
            $("#login_btn").html('ورود').removeAttr('disabled');
            //$("#loading_button").css('display', 'none');
            $("#message").html('خطا در برقراری ارتباط با سرور');
        },
    });
}