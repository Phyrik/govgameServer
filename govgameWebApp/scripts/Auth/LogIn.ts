function loginFormSubmitted() {
    var form: JQuery<HTMLFormElement> = $("#login-form");

    $.post("/Auth/LogInPOST", form.serialize(), function (data, success) {
        if (data == "success") {
            location.href = decodeURIComponent(form.attr("action"));
        }
        else {
            alert(data);
        }
    });
}