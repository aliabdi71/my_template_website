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
        return;
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
        error: function(){
            $("#login_btn").html('ورود').removeAttr('disabled');
            //$("#loading_button").css('display', 'none');
            $("#message").html('خطا در برقراری ارتباط با سرور');
        },
    });
}