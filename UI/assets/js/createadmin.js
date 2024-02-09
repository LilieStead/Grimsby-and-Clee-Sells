function createAdmin(event) {
    event.preventDefault();

    const loader = document.getElementById("preloader");
    const form = new FormData(document.getElementById("createadmin"));

    // Gets all inputs from the signup form
    const username = form.get('admin_username');
    const firstname = form.get('admin_firstname');
    const lastname = form.get('admin_lastname');
    const dob = form.get('admin_dob');
    const email = form.get('admin_email');
    const phone = form.get('admin_phone');
    const password = form.get('admin_password');

    // error message p tag 
    const signupusernameerror = document.getElementById('usernameerror');
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
    } else if (username.length <= 4){
        signupusernameerror.innerHTML = ("your username needs to be loner than 4 characters");
        nopass = true;
    }else if (username.length > 20){
        signupusernameerror.innerHTML = ("your username can not be more than 20 characters");
        nopass = true;
    }else{
        signupusernameerror.innerHTML = (null);
    }
    // End of username validation

    //First name validation

    if (firstname == '' || firstname == null){
        firstnameerror.innerHTML =("you need to enter your first name");
        nopass = true;
    }else if (firstname.length < 3){
        firstnameerror.innerHTML = ("First name must be at least three characters long");
        nopass = true;
    }else if (firstname.length > 20){
        firstnameerror.innerHTML = ("First name must not exceed 20 characters.");
        nopass = true;
    }else{
        firstnameerror.innerHTML = (null);
    }

    // End of first name validation

    // Last name validation

    if (lastname == "" || lastname == null){
        lastnameerror.innerHTML = ("you need to enter your last name");
        nopass = true;
    }else if(lastname.length < 4){
        lastnameerror.innerHTML = ("your last name can not be less than 4 characters");
        nopass = true;
    }else if (lastname.length > 20){
        lastnameerror.innerHTML = ("your last name can not be more than 20 characters");
        nopass = true;
    } else{
        lastnameerror.innerHTML = (null);
    }

    // end of last name validation 

    // Email validation

    var emailformat = /^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$/;

    if (email == "" || email == null){
        emailerror.innerHTML = ("you need to enter your email address");
        nopass = true;
    } else if (!email.match(emailformat)){
        emailerror.innerHTML = ("your email does not follow the right format");
        nopass = true;
    }else{
        emailerror.innerHTML = (null);
    }

    // End of email validation



    // Phone number validation

    const phoneformat = /^07\d{9}$/;

    if (phone == "" || phone == null){
        phoneerror.innerHTML = ("you need to enter your phone number");
        nopass = true;nopass = true;
    } else if (!phone.match(phoneformat)){
        phoneerror.innerHTML = ("Please enter your phone number in the format 07262938121, without spaces or dashes")
        nopass = true;
    }else {
        phoneerror.innerHTML = (null)
    }

    // end of Phone number validation

    // password validation 

    if (password == '' || password == null){
        passworderror.innerHTML = ("you need to enter a password");
        nopass = true;
    }else if (password.length > 25){
        passworderror.innerHTML = ("your password cannot be more than 25 charters");
        nopass = true;
    }else if (password.length < 10){
        passworderror.innerHTML = ("your password cannot be less than 10 charters");
        nopass = true;
    }else if (!hasspecialcharacter(password)){
        passworderror.innerHTML = ("Your password must contain a special character");
        nopass = true;
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
        loader.style.display = "block";

        fetch(`https://localhost:44394/CreateAdmin`, {
        method: "POST",
        body: form,
        })
        .then(response => {
            if (response.status === 201) {
                return response.json();
            } else if(response.status === 409 || response.status === 400){
                
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })

            }else {
                // Reject the Promise with an error status
                return response.json().then(error => {
                    return Promise.reject("You have run into an error. please try again later.");
                })
            }
        })
        // once the user signs up sign them in using the same username and password
        .then(data => {
            var userConvert = data.admin_dob.split("T");
            var newDate = new Date(userConvert[0]);
            var finalDate = newDate.toISOString().split('T')[0];
            console.log(data);
            const adminDiv = document.getElementById('createadmin');
            adminDiv.innerHTML = `<h3>Admin Created. Confirm the details below are correct.
            <h5>Username: ${data.admin_username}</h5>
            <h5>First Name: ${data.admin_firstname}</h5>
            <h5>Last Name: ${data.admin_lastname}</h5>
            <h5>Email Address: ${data.admin_email}</h5>
            <h5>Phone Number: ${data.admin_phone}</h5>
            <h5>DOB: ${finalDate}</h5>
            <h5>Password: ${password}</h5>
            <p>Please provide these details to the new administrator</p>
            `;

        })
        .catch(error => {
            console.error(error)
            return customPopup(error);
        })
        .finally(() => {
            loader.style.display = 'none';
        })
        
    }
}

document.getElementById('createadmin').addEventListener('submit', createAdmin);