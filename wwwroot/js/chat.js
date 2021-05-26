"use strict";

var connection = new signalR.HubConnectionBuilder()
  .withUrl("/chathub")
  .build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
  var msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  var encodedMsg = user + " says " + msg;
  var li = document.createElement("li");
  li.textContent = encodedMsg;
  document.getElementById("messagesList").appendChild(li);
});

connection.on("BroadcastMessage", function (message) {
  var msg = message
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  var encodedMsg = msg;
  var li = document.createElement("li");
  li.textContent = encodedMsg;
  document.getElementById("messagesList").appendChild(li);
});

connection
  .start()
  .then(function () {
    document.getElementById("sendButton").disabled = false;
    connection.invoke("AddToGroup", document.getElementById("roomID").innerHTML).catch(err => {
      return console.error(err.toString());
    });
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

document
  .getElementById("sendButton")
  .addEventListener("click", function (event) {
    // var user = document.getElementById("userInput").value;
    var user = "Bạn";
    var message = document.getElementById("messageInput").value;
    document.getElementById("messageInput").value = "";
    connection.invoke("SendMessage", user, document.getElementById("roomID").innerHTML, message).catch(function (err) {
      return console.error(err.toString());
    });
    event.preventDefault();
  });

