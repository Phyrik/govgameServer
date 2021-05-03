import firebase from "@firebase/app";
import "@firebase/auth";

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
firebase.initializeApp(firebaseConfig);

firebase.auth().setPersistence(firebase.auth.Auth.Persistence.NONE);

var actionCodeSettings = {
    url: "https://govgame.crumble-technologies.co.uk/Auth/EmailVerified"
};

function formSubmitted() {
    if ((document.getElementById("password") as HTMLInputElement).value != (document.getElementById("password-confirm") as HTMLInputElement).value) {
        alert("The passwords you entered do not match.");
    }

    var form: HTMLFormElement = document.getElementById("register-form") as HTMLFormElement;

    var idTokenInput: HTMLInputElement = document.createElement("input");
    idTokenInput.setAttribute("id", "id-token");
    idTokenInput.setAttribute("name", "idToken");
    idTokenInput.setAttribute("type", "text");
    idTokenInput.style.display = "none";
    firebase.auth().createUserWithEmailAndPassword((document.getElementById("email") as HTMLInputElement).value, (document.getElementById("password") as HTMLInputElement).value).then(user => {
        firebase.auth().currentUser.getIdToken().then(idToken => {
            idTokenInput.setAttribute("value", idToken);
            try {
                document.getElementById("id-token").remove();
            } catch { }
            form.appendChild(idTokenInput);

            $.post("/Auth/RegisterPOST", $("form#register-form").serialize(), function (data, status) {
                if (data == "success") {
                    firebase.auth().currentUser.sendEmailVerification(actionCodeSettings).then(() => {
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
    }).catch(error => {
        alert(error.message);
    });
}