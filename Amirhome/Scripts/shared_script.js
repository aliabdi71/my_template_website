$(document).ready(function(){
    var height = $(window).height();
    var body_height = $(".wrap").height();
    if (body_height < height)
        $(".wrap").css('height', height - 250);

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