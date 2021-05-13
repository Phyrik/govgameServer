function inviteMinister(username) {
    $.post("/Game/InviteMinister?ministry=" + ministryCode + "&invitedUsername=" + username, function (data, status) {
        if (data == "success") {
            location.href = "/Game/PrimeMinisterDashboard/ViewMinistry?ministry=" + ministryCode;
        }
        else {
            alert(data);
        }
    });
}
function searchUsers() {
    var searchText = $("#search-input").val();
    var lis = $("#user-ul")[0].getElementsByTagName("li");
    var hrs = $("#user-ul")[0].getElementsByTagName("hr");
    for (var i = 0; i < lis.length; i++) {
        var name = lis[i].getElementsByTagName("h2")[0].innerHTML;
        if (name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1) {
            lis[i].style.display = "grid";
            hrs[i].style.display = "";
        }
        else {
            lis[i].style.display = "none";
            hrs[i].style.display = "none";
        }
    }
}
document.getElementById("search-input").addEventListener("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        searchUsers();
    }
});
//# sourceMappingURL=InviteNewMinister.js.map