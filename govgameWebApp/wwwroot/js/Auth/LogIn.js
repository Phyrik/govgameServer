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
function formSubmitted() {
    var form = document.getElementById("login-form");
    var idTokenInput = document.createElement("input");
    idTokenInput.setAttribute("name", "idToken");
    idTokenInput.setAttribute("type", "text");
    idTokenInput.style.display = "none";
    app_1.default.auth().signInWithEmailAndPassword(document.getElementById("email").value, document.getElementById("password").value).then(function (user) {
        app_1.default.auth().currentUser.getIdToken().then(function (idToken) {
            idTokenInput.setAttribute("value", idToken);
            form.appendChild(idTokenInput);
            form.submit();
        });
    }).catch(function (error) {
        alert(error.message);
    });
}
//# sourceMappingURL=LogIn.js.map