document.querySelector(".img-btn").addEventListener("click", function () {
  document.querySelector(".cont").classList.toggle("s-signup");
});

document.querySelector(".theme").addEventListener("click", function (e) {
  document.querySelector(".box_theme").classList.toggle(".box-active");
  document.querySelector(".theme").classList.toggle("theme-active");
});

function validateEmail(email) {
  const re =
    /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return re.test(email);
}

function CheckEmail(event) {
  var email = document.getElementById("login_email").value;
  var error_email = document.getElementById("error_email");
  event.preventDefault();
  if (email == "") {
    document.getElementById("login_email").style.borderColor = "red";
    error_email.innerHTML = "Not Invalid Email";
  } else {
    if (!validateEmail(email)) {
      document.getElementById("login_email").style.borderColor = "red";
      error_email.innerHTML = "Email Error";
    } else {
      document.getElementById("login_email").style.borderColor = "#2ecc71";
      document.getElementById("login_email").style.backgroundColor = "#fff";
      error_email.innerHTML = "";
    }
  }
}

function CheckPassword(event) {
  event.preventDefault();
  var password = document.getElementById("login_password").value;
  var error_password = document.getElementById("error_password");
  if (password == "") {
    document.getElementById("login_password").style.borderColor = "red";
    error_password.innerHTML = "Not Invalid Password";
  } else {
    document.getElementById("login_password").style.borderColor = "#2ecc71";
    document.getElementById("login_password").style.backgroundColor = "#fff";
    error_password.innerHTML = "";
  }
}

// Function Sign-In

function CheckName(event) {
  event.preventDefault();
  var name = document.getElementById("name").value;
  var error_name = document.getElementById("error_name");
  if (name == "") {
    error_name.innerHTML = "Name is not empty";
    document.getElementById("name").style.borderColor = "red";
  } else {
    document.getElementById("name").style.backgroundColor = "#fff";
    document.getElementById("name").style.borderColor = "#2ecc71";
    error_name.innerHTML = "";
  }
}

function CheckEmail_SignUp(event) {
  var email = document.getElementById("email").value;
  var error_email = document.getElementById("error_email_sign_up");
  if (email == "") {
    error_email.innerHTML = "Email is not empty";
    document.getElementById("email").style.borderColor = "red";
  } else {
    if (!validateEmail(email)) {
      error_email.innerHTML = "Email not correct";
      document.getElementById("email").style.borderColor = "red";
    } else {
      document.getElementById("email").style.backgroundColor = "#fff";
      document.getElementById("email").style.borderColor = "#2ecc71";
      error_email.innerHTML = "";
    }
  }
}

function CheckPassword_SignUp(event) {
  var password = document.getElementById("password").value;
  var error_password = document.getElementById("error_password_sign_up");
  if (password == "") {
    error_password.innerHTML = "Password is not empty";
    document.getElementById("password").style.borderColor = "red";
  } else {
    document.getElementById("password").style.backgroundColor = "#fff";
    document.getElementById("password").style.borderColor = "#2ecc71";
    error_password.innerHTML = "";
  }
}

function CheckConfirmPassword(event) {
  var password = document.getElementById("password").value;
  event.preventDefault();
  var confirm_password = document.getElementById("confirm_password").value;
  var error_confirm_password = document.getElementById(
    "error_confirm_password"
  );
  if (confirm_password == "") {
    error_confirm_password.innerHTML = "Confirm Password is not empty";
    document.getElementById("confirm_password").style.borderColor = "red";
  } else {
    if (password != confirm_password) {
      error_confirm_password.innerHTML = "Confirm Password is not correct";
      document.getElementById("confirm_password").style.borderColor = "red";
    } else {
      document.getElementById("confirm_password").style.backgroundColor =
        "#fff";
      document.getElementById("confirm_password").style.borderColor = "#2ecc71";
      error_confirm_password.innerHTML = "";
    }
  }
}

function CheckValidation(event, data, errorData, name) {
  event.preventDefault();
  var dataValue = document.getElementById(`${data}`).value;
  var error = document.getElementById(`${errorData}`);
  if (dataValue === "") {
    document.getElementById(`${data}`).style.borderColor = "red";
    error.innerHTML = `Not Invalid ${name}`;
  } else {
    document.getElementById(`${data}`).style.borderColor = "#2ecc71";
    document.getElementById(`${data}`).style.backgroundColor = "#fff";
    error.innerHTML = "";
  }
}
