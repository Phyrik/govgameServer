declare var ministryCode: string;

var ministerHeading = document.getElementById("minister")
var ministerButton = document.getElementById("minister-button")

function dismissMinister() {
    var okPressed = confirm("Are you sure you want to dismiss this minister?")

    if (!okPressed) return

    ministerHeading.innerHTML = "No minister"
    ministerButton.onclick = function () { location.href = `/Game/PrimeMinisterDashboard/InviteNewMinister?minister=${ministryCode}` }
    ministerButton.innerHTML = "Invite new minister"

    $.post(`/Game/DismissMinister?minister=${ministryCode}`, function (data, status) {
        if (data != "success") {
            alert(data)
            location.reload()
        }
    })
}

function cancelMinistryInvite() {
    ministerHeading.innerHTML = "No minister"
    ministerButton.onclick = function () { location.href = `/Game/PrimeMinisterDashboard/InviteNewMinister?minister=${ministryCode}` }
    ministerButton.innerHTML = "Invite new minister"

    $.post(`/Game/CancelMinistryInvite?ministry=${ministryCode}`, function (data, status) {
        if (data != "success") {
            alert(data)
            location.reload()
        }
    })
}