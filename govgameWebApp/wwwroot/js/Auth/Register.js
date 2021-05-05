function registerFormSubmitted() {
    if (document.getElementById("password").value.length < 6) {
        alert("Your password must be 4 or more characters long.");
        return;
    }
    if (document.getElementById("password").value != document.getElementById("password-confirm").value) {
        alert("The passwords you entered do not match.");
        return;
    }
    var form = $("#register-form");
    $.post("/Auth/RegisterPOST", form.serialize(), function (data, success) {
        if (data == "success") {
            location.href = "/Auth/CheckVerificationEmail";
        }
        else {
            alert(data);
        }
    });
}
//# sourceMappingURL=Register.js.map