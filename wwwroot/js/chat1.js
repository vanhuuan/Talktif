("use strict");
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
var userCnnID;

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
  var msg = message
  // var encodedMsg = user + " says " + msg;
  var div = document.createElement("div");
  div.className =
    user != userCnnID
      ? "message-row other-message"
      : "message-row your-message";
var divContent = document.createElement("div");
divContent.className = "message-text";
divContent.textContent = msg;
  div.appendChild(divContent);
  document.getElementsByClassName("message-box-content")[0].appendChild(div);
  if (user != userCnnID) {
    var messAudio = new Audio('/message.mp3');
    messAudio.play();  
  }
  document.getElementsByClassName("message-box-content")[0].scrollTo(0,document.getElementsByClassName("message-box-content")[0].scrollHeight);
});

connection.on("BroadcastMessage", function (message) {
  var msg = message
  var encodedMsg = msg;
  var li = document.createElement("li");
  li.textContent = encodedMsg;
  document.getElementsByClassName("message-box-content")[0].appendChild(li);
  document.getElementsByClassName("message-box-content")[0].scrollTo(0,document.getElementsByClassName("message-box-content")[0].scrollHeight);
});

connection
  .start()
  .then(function () {
    if (!userCnnID) userCnnID = connection.connectionId;
    document.getElementById("sendButton").disabled = false;
    connection.invoke("AddToQueue", userID, username, null).catch((err) => {
      return console.error(err.toString());
    });
  })
  .catch(function (err) {
    return console.error(err.toString());
  });

window.onbeforeunload = function () {
  connection.invoke("LeaveChat", userID, username).catch((err) => {
    return console.error(err.toString());
  });
};

document
  .getElementsByTagName("form")[0]
  .addEventListener("submit", function (event) {
    event.preventDefault();
    var message = document.getElementById("message").value;
    document.getElementById("message").value = "";
    connection.invoke("SendMessage", message).catch(function (err) {
      return console.error(err.toString());
    });
  });

if (userID) {
  document
    .getElementById("addFriendButton")
    .addEventListener("click", function (event) {
      event.preventDefault();
      connection.invoke("AddFriend", token).catch((err) => {
        return console.error(err.toString());
      });
    });

  document
    .getElementById("saveFilter")
    .addEventListener("click", function (event) {
      event.preventDefault();
      var filterString = "";
      var filterInputs = document.getElementsByClassName("filter-option");
      if (filterInputs[0].checked) filterString += (filterString == "" ? "" : ",") + userHobbies;
      if (filterInputs[1].checked) filterString += (filterString == "" ? "" : ",") + userAddress;
      if (filterInputs[2].checked) filterString += (filterString == "" ? "" : ",") + (userGender ? "female" : "male");
      connection.invoke("SaveFilter", userID, username, filterString).catch((err) => {
        return console.error(err.toString());
      });
    });
}

document
  .getElementById("skipButton")
  .addEventListener("click", function (event) {
    event.preventDefault();
    connection.invoke("SkipChat", userID, username).catch((err) => {
      return console.error(err.toString());
    });
  });
