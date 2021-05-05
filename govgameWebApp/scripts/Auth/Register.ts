function registerFormSubmitted() {
    if ((document.getElementById("password") as HTMLInputElement).value.length < 6) {
        alert("Your password must be 4 or more characters long.");
        return;
    }

    if ((document.getElementById("password") as HTMLInputElement).value != (document.getElementById("password-confirm") as HTMLInputElement).value) {
        alert("The passwords you entered do not match.");
        return;
    }

    var form: JQuery<HTMLFormElement> = $("#register-form");

    $.post("/Auth/RegisterPOST", form.serialize(), function (data, success) {
        if (data == "success") {
            location.href = "/Auth/CheckVerificationEmail";
        }
        else {
            alert(data);
        }
    });
}