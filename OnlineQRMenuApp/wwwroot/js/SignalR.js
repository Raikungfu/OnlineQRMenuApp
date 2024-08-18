(document).ready(function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/AppHub")
        .build();

    connection.on("ReceiveNotification", function (message) {
        showNotification(message);
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });

    function showNotification(message) {
        const notificationContainer = $('#notificationContainer');
        const notification = $(`
                            <div class="notification">
                                <button class="close-btn">&times;</button>
                                <div class="message">${message}</div>
                            </div>
                        `);

        notification.find('.close-btn').click(function () {
            notification.remove();
        });

        notificationContainer.append(notification);
    }
});