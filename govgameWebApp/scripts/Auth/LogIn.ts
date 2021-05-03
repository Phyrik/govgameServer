import firebase from "@firebase/app";
import "@firebase/auth";

const firebaseConfig = {
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

function formSubmitted() {
    var form = document.getElementById("login-form") as HTMLFormElement;

    var idTokenInput = document.createElement("input");
    idTokenInput.setAttribute("name", "idToken");
    idTokenInput.setAttribute("type", "text");
    idTokenInput.style.display = "none";
    firebase.auth().signInWithEmailAndPassword((document.getElementById("email") as HTMLInputElement).value, (document.getElementById("password") as HTMLInputElement).value).then(user => {
        firebase.auth().currentUser.getIdToken().then(idToken => {
            idTokenInput.setAttribute("value", idToken);
            form.appendChild(idTokenInput);

            form.submit();
        });
    }).catch(error => {
        alert(error.message);
    });
}