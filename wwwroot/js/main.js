Exit = () => {
  var url = "index.html";
  var referLink = document.createElement("a");
  referLink.href = url;
  document.body.appendChild(referLink);
  referLink.click();
};
Home = () => {
  var url = "Index.html";
  var referLink = document.createElement("a");
  referLink.href = url;
  document.body.appendChild(referLink);
  referLink.click();
};

// const messages = document.getElementById("message-box");

// function appendMessage() {
//   const message = document.getElementsByClassName("message-div")[0];
//   const newMessage = message.cloneNode(true);
//   messages.appendChild(newMessage);
// }

// function getMessages() {
//   // Prior to getting your messages.
//   shouldScroll =
//     messages.scrollTop + messages.clientHeight === messages.scrollHeight;
//   /*
//    * Get your messages, we'll just simulate it by appending a new one syncronously.
//    */
//   appendMessage();
//   // After getting your messages.
//   if (!shouldScroll) {
//     scrollToBottom();
//   }
// }

// function scrollToBottom() {
//   messages.scrollTop = messages.scrollHeight;
// }

// scrollToBottom();

// setInterval(getMessages, 100);
GetValue = (event) => {
  //var email = document.getElementById("login_email").value;
  var text = document.getElementById("message").value;
  const messages = document.getElementById("content-mes");
  // messages.classList.add("border");
  messages.classList.add("border-message");
  messages.innerHTML = text;
  event.preventDefault();
  //alert(text);
};
