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
    value = value.toString();
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

function inputNumberWithCommas(x) {
    var res = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return turn2PersianNumber(res);
}

function numberWithCommas(x) {
    var res = x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return turn2PersianNumber(res) + ' تومان';
}

$(document).ready(function () {
    /*var height = $(window).height();
    var body_height = $(".wrap").height();
    if (body_height < height)
        $(".wrap").css('height', height - 250);*/

    $(".price").each(function () {
        var val = $(this).html();
        $(this).html(numberWithCommas(val));
    });
    $(".number").each(function () {
        var val = $(this).html();
        $(this).html(turn2PersianNumber(val));
    });

    $("#login_btn").click(function () {

        var email = $("#email").val(),
            pass = $("#password").val();
        if (email === '') {
            $("#message").html("لطفا ایمیل را وارد کنید");
            return false;
        }
        if (pass === '') {
            $("#message").html("لطفا کلمه عبور را وارد کنید");
            return false;
        }
        $(this).css('display', 'none');
        $("#loading_button").css('display', 'inline');
        sendDataToServer(email, pass);
        return true;
    });
});

function sendDataToServer(email, pass) {
    var postData = { "email": email, "password": pass };
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
                $("#login_btn").css('display', 'inline');
                $("#loading_button").css('display', 'none');
                $("#message").html(data);
            }
        },
    });
}