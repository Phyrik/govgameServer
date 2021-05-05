function loginFormSubmitted() {
    var form = $("#login-form");
    $.post("/Auth/LogInPOST", form.serialize(), function (data, success) {
        if (data == "success") {
            location.href = decodeURIComponent(form.attr("action"));
        }
        else {
            alert(data);
        }
    });
}
//# sourceMappingURL=LogIn.js.map