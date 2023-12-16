function signup(event) {
    event.preventDefault();

    const signupform = new FormData(document.getElementById("signup"));

    // Gets all inputs from the signup form
    const username = signupform.get('username');
    const firstname = signupform.get('firstname');
    const lastname = signupform.get('lastname');
    const dob = signupform.get('dob');
    const email = signupform.get('email');
    const phone = signupform.get('phone');
    const password = signupform.get('password');

    // error message p tag 
    const signupusernameerror = document.getElementById('signupusernameerror');
    const firstnameerror = document.getElementById('firstnameerror');
    const lastnameerror = document.getElementById('lastnameerror');
    const doberror = document.getElementById('doberror');
    const emailerror = document.getElementById('emailerror');
    const phoneerror = document.getElementById('phoneerror');
    const passworderror = document.getElementById('passworderror');

    let nopass = false; // Use "let" instead of "const"

    // functions 

// over 18 check 
    function isover18(dobformate) {
        const currentdate = new Date();
        const datedifference = currentdate - dobformate;
        age = Math.floor(datedifference / (365.25 * 24 * 60 * 60 * 1000));
        return age;
    }
// special character check 
function hasspecialcharacter(password) {
    const specialcharregex = /[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]/;
    // Test the password against the regular expression
    return specialcharregex.test(password);
  }


    // Username validation
    if (username == '' || username == null){
        signupusernameerror.innerHTML = ("you need to enter a user name");
        nopass = true;
        event.preventDefault();
    } else if (username.length <= 4){
        signupusernameerror.innerHTML = ("your username needs to be loner than 4 characters");
        nopass = true;
        event.preventDefault();
    }else if (username.length > 20){
        signupusernameerror.innerHTML = ("your username can not be more than 20 characters");
        nopass = true;
        event.preventDefault();
    }else{
        signupusernameerror.innerHTML = (null);
    }
    // End of username validation

    //First name validation

    if (firstname == '' || firstname == null){
        firstnameerror.innerHTML =("you need to enter your first name");
        nopass = true;
        event.preventDefault();
    }else if (firstname.length < 3){
        firstnameerror.innerHTML = ("your first name can not be smaller than three characters");
        nopass = true;
        event.preventDefault();
    }else if (firstname.length > 20){
        firstnameerror.innerHTML = ("you first name can not be larger than 20 characters");
        nopass = true;
        event.preventDefault();
    }else{
        firstnameerror.innerHTML = (null);
    }

    // End of first name validation

    // Last name validation

    if (lastname == "" || lastname == null){
        lastnameerror.innerHTML = ("you need to enter your last name");
        nopass = true;
        event.preventDefault();
    }else if(lastname.length < 4){
        lastnameerror.innerHTML = ("your last name can not be less than 4 characters");
        nopass = true;
        event.preventDefault();
    }else if (lastname.length > 20){
        lastnameerror.innerHTML = ("your last name can not be more than 20 characters");
        nopass = true;
        event.preventDefault();
    } else{
        lastnameerror.innerHTML = (null);
    }

    // end of last name validation 

    // Email validation

    var emailformat = /^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$/;

    if (email == "" || email == null){
        emailerror.innerHTML = ("you need to enter your email address");
        nopass = true;
        event.preventDefault();
    } else if (!email.match(emailformat)){
        emailerror.innerHTML = ("your email does not follow the right format");
        nopass = true;
        event.preventDefault();
    }else{
        emailerror.innerHTML = (null);
    }

    // End of email validation



    // Phone number validation

    const phoneformat = /^07\d{9}$/;

    if (phone == "" || phone == null){
        phoneerror.innerHTML = ("you need to enter your phone number");
        nopass = true;nopass = true;
        event.preventDefault();
    } else if (!phone.match(phoneformat)){
        phoneerror.innerHTML = ("Please enter your phone number in the format 07262938121, without spaces or dashes")
        nopass = true;
        event.preventDefault();
    }else {
        phoneerror.innerHTML = (null)
    }

    // end of Phone number validation

    // password validation 

    if (password == '' || password == null){
        passworderror.innerHTML = ("you need to enter a password");
        nopass = true;
        event.preventDefault();
    }else if (password.length > 25){
        passworderror.innerHTML = ("your password cannot be more than 25 charters");
        nopass = true;
        event.preventDefault();
    }else if (password.length < 10){
        passworderror.innerHTML = ("your password cannot be less than 10 charters");
        nopass = true;
        event.preventDefault();
    }else if (!hasspecialcharacter(password)){
        passworderror.innerHTML = ("Your password must contain a special character");
        nopass = true;
        event.preventDefault();
    }else{
        passworderror.innerHTML = (null);
    }

    // end of password validation

    // Date of birth validation 
    let dobformate;
    let age;
    if (dob == '' || dob == null){
        doberror.innerHTML = ("you need to enter your date of birth");
        nopass = true;
    }else{
    // Validate the users age
        try {
             // Make the user dob in a date format
            const dobformate = new Date(dob);
            if (isNaN(dobformate)) {
                throw new Error("Invalid date");
            }
            isover18(dobformate);
        // works out if age is over 18 (dob error)
                if (age >= 18) {
                    doberror.innerHTML = (null);
                } else {
                    doberror.innerHTML = ("You need to be over 18 years old");
                    nopass = true;
                    event.preventDefault();
                } 
        } catch (error) {
            doberror.innerHTML = 'Your date of birth is in the wrong format';
            nopass = true;
        }
    // end of date of birth validation 

    }
    if (nopass) {
        return;
    }else{

        const user_dob = new Date(dob).toISOString();
        const userData = {

            users_username: username,
            users_firstname:firstname,
            users_lastname:lastname,
            users_email:email,
            users_phone:phone,
            users_dob:user_dob,
            users_password:password
        };


        fetch(`https://192.168.0.135:44394/signup`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        }, 
        body: JSON.stringify(userData),
        })
        .then(response => {
            if (response.status === 201 || response.status === 200) {
                return response.json();
            } else {
                // Reject the Promise with an error status
                throw new Error(response.status);
            }
        })
        .then(data => {
            // const usertoken = data.token;
            fetch(`https://192.168.0.135:44394/login/${username}/${password}`)
            .then(response => {
                if (response.status === 200){
                    return response.json();
                }else{
                    return console.error(response.status);
                }
            })
            .then (data => {
                console.log(data);
                var cookies = document.cookie.split(";");
                var cookieexp = cookies.find(cookie=>cookie.trim().startsWith("usercookieexpiry="));
                if (cookieexp){
                    window.location.href = "home.html";
                }
            })
            .catch(error => {
                return console.error(error);
            })

        })
        .catch(error => {
            if (error.message.includes("error : 409")) {
                // Extract the error message from the API response
                console.log(error.message);
                const errorMessage = error.message.split(":")[1].trim();
                console.log(errorMessage)
                customPopup(errorMessage);
            } else {
                // If no specific handling for this type of error, use the error message directly
                customPopup("Error: " + error);
            }
        })
        
    }
}

document.getElementById('signup').addEventListener("submit", signup);