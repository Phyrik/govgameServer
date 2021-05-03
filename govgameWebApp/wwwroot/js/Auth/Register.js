"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var app_1 = require("@firebase/app");
require("@firebase/auth");
var firebaseConfig = {
    apiKey: "AIzaSyBgcaRD71YU9H8TEnj82iDnkeNuOSXrfcM",
    authDomain: "government-game.firebaseapp.com",
    databaseURL: "https://government-game.firebaseio.com",
    projectId: "government-game",
    storageBucket: "government-game.appspot.com",
    messagingSenderId: "1038258061620",
    appId: "1:1038258061620:web:a2a96589a58a2c9570a561",
    measurementId: "G-EW4LVNGLVK"
};
app_1.default.initializeApp(firebaseConfig);
app_1.default.auth().setPersistence(app_1.default.auth.Auth.Persistence.NONE);
var actionCodeSettings = {
    url: "https://govgame.crumble-technologies.co.uk/Auth/EmailVerified"
};
function formSubmitted() {
    if (document.getElementById("password").value != document.getElementById("password-confirm").value) {
        alert("The passwords you entered do not match.");
    }
    var form = document.getElementById("register-form");
    var idTokenInput = document.createElement("input");
    idTokenInput.setAttribute("id", "id-token");
    idTokenInput.setAttribute("name", "idToken");
    idTokenInput.setAttribute("type", "text");
    idTokenInput.style.display = "none";
    app_1.default.auth().createUserWithEmailAndPassword(document.getElementById("email").value, document.getElementById("password").value).then(function (user) {
        app_1.default.auth().currentUser.getIdToken().then(function (idToken) {
            idTokenInput.setAttribute("value", idToken);
            try {
                document.getElementById("id-token").remove();
            }
            catch (_a) { }
            form.appendChild(idTokenInput);
            $.post("/Auth/RegisterPOST", $("form#register-form").serialize(), function (data, status) {
                if (data == "success") {
                    app_1.default.auth().currentUser.sendEmailVerification(actionCodeSettings).then(function () {
                        location.href = "/Auth/CheckVerificationEmail";
                    });
                }
                else if (data == "error: username taken") {
                    alert("Sorry! That username is already taken.");
                }
                else {
                    alert(data);
                }
            });
        });
    }).catch(function (error) {
        alert(error.message);
    });
}
//# sourceMappingURL=Register.js.map