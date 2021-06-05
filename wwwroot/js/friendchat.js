("use strict");
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
var userCnnID;

function AddMessage(user, room, msg, token) {
  var xhr = new XMLHttpRequest();
  xhr.open(
    "POST",
    "https://talktifapi.azurewebsites.net/api/Chat/AddMessage",
    true
  );
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.setRequestHeader("Authorization", "Bearer " + token);
  xhr.send(`{"Message":"${msg}","IdSender":"${user}","idChatRoom":"${room}"}`);
}

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
  var msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  // var encodedMsg = user + " says " + msg;
  var span = document.createElement("span");
  span.className =
    user != userCnnID
      ? "msgtext p-2 mr-auto mb-1"
      : "msgtext-self p-2 ml-auto mb-1";
  span.textContent = msg;
  var div = document.createElement("div");
  div.className = "row w-100 m-0";
  div.appendChild(span);
  document.getElementById("chatMessages").appendChild(div);
});

connection.on("BroadcastMessage", function (message) {
  var msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  var encodedMsg = msg;
  var li = document.createElement("li");
  li.textContent = encodedMsg;
  document.getElementById("chatMessages").appendChild(li);
});

connection
  .start()
  .then(function () {
    if (!userCnnID) userCnnID = connection.connectionId;
    document.getElementById("sendButton").disabled = false;
    connection.invoke("JoinFriendChat", String(roomID)).catch((err) => {
      return console.error(err.toString());
    });
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

window.onbeforeunload = function () {
  connection.invoke("LeaveFriendChat", String(roomID)).catch((err) => {
    return console.error(err.toString());
  });
};

document
  .getElementsByTagName("form")[0]
  .addEventListener("submit", function (event) {
    event.preventDefault();

    // Send msg real-time
    var message = document.getElementById("messageInput").value;
    document.getElementById("messageInput").value = "";
    connection
      .invoke("SendFriendMessage", String(roomID), message)
      .catch(function (err) {
        return console.error(err.toString());
      });

    // Call API add message
    AddMessage(String(userID), String(roomID), message, token);
  });
