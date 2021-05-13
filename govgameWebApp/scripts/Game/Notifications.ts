declare var readNotOpenedColour: string;
declare var unreadNotOpenedColour: string;

function markNotificationAsRead(notificationId) {
    var notificationLi = document.getElementById("notification-li-" + notificationId);

    notificationLi.style.backgroundColor = readNotOpenedColour;

    $.post(`/Game/MarkNotificationAsRead?notificationId=${notificationId}`, function (data, status) {
        if (data != "success") {
            console.log("error setting notification as read");
            alert(data);
            notificationLi.style.backgroundColor = unreadNotOpenedColour;
        }
    })
}