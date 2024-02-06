2
var user;


function getUserDetails(){

    fetch(`https://localhost:44394/api/User/decodetoken`, {
            method: "GET", 
            credentials: "include"
        })
        .then(response => {
            if (response.status === 200){
                return response.json();
            }
            // error handling 
            else if(response.status === 500){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            // error handling 
            else if(response.status === 400){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            // error handling 
            else if(response.status === 401){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            // unplanned error 
            else{
                console.error(response.status);

            }

        })
        .then (data => {
            user = data.userid;
            fetch (`https://localhost:44394/getuserbyid/${user}`)
            .then(responce => {
                if (responce.status === 404 || responce.status === 400){
                    return customPopup("User not found")
                }
                return responce.json();
            })
            .then (data =>{
                document.getElementById("usernameinput").value = data.users_username;
                document.getElementById("firstname").value = data.users_firstname;
                document.getElementById("lastname").value = data.users_lastname;

                var userConvert = data.users_dob.split("T");
                var newDate = new Date(userConvert[0]);
                var finalDate = newDate.toISOString().split('T')[0];

                document.getElementById("dob").value = finalDate;
                document.getElementById("email").value = data.users_email;
                document.getElementById("phone").value = data.users_phone;
                document.getElementById("balance").value = data.users_balance;




            })

            .catch(error => {
                return customPopup(error);
            })
        })
        .catch (error => {
            console.error(error);
            return customPopup(error);
        })

}










function editUserDetails (event){
    event.preventDefault();
    const loader = document.getElementById("preloader");
    const editform = new FormData(document.getElementById("changedetailsform"));
    const username = editform.get("username");
    const firstname = editform.get("firstname");
    const lastname = editform.get("lastname");
    const dob = editform.get('dob');
    const email = editform.get('email');
    const phone = editform.get('phone');
    const balance =editform.get(`balance`)

    const usernameerror = document.getElementById('usernameerror');
    const firstnameerror = document.getElementById('firstnameerror');
    const lastnameerror = document.getElementById('lastnameerror');
    const doberror = document.getElementById('doberror');
    const emailerror = document.getElementById('emailerror');
    const phoneerror = document.getElementById('phoneerror');
    const balanceerror = document.getElementById(`balanceerror`);

    let nopass = false;

    function isover18(dobformate) {
        const currentdate = new Date();
        const datedifference = currentdate - dobformate;
        age = Math.floor(datedifference / (365.25 * 24 * 60 * 60 * 1000));
        return age;
    }

    if (username == '' || username == null){
        usernameerror.innerHTML = ("you need to enter a user name");
        nopass = true;
        event.preventDefault();
    } else if (username.length <= 4){
        usernameerror.innerHTML = ("your username needs to be loner than 4 characters");
        nopass = true;
        event.preventDefault();
    }else if (username.length > 20){
        usernameerror.innerHTML = ("your username can not be more than 20 characters");
        nopass = true;
        event.preventDefault();
    }else{
        usernameerror.innerHTML = (null);
    }

    if (firstname == '' || firstname == null){
        firstnameerror.innerHTML =("you need to enter your first name");
        nopass = true;
        event.preventDefault();
    }else if (firstname.length < 3){
        firstnameerror.innerHTML = ("First name must be at least three characters long");
        nopass = true;
        event.preventDefault();
    }else if (firstname.length > 20){
        firstnameerror.innerHTML = ("First name must not exceed 20 characters.");
        nopass = true;
        event.preventDefault();
    }else{
        firstnameerror.innerHTML = (null);
    }

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


    if (balance == null || balance == ``){
        nopass = true;
        balanceerror.innerHTML = ("please enter your  balance");
    }

    }
    if (nopass) {
        return;
    }else{
        loader.style.display = "block";


        const user_dob = new Date(dob).toISOString();
        const userData = {
            users_id: user,
            users_username: username,
            users_firstname: firstname,
            users_lastname: lastname,
            users_email: email,
            users_phone: phone,
            users_dob: user_dob,
            users_balance: balance
        };


        console.log(userData);

        fetch(`https://localhost:44394/updateuser/details`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(userData),
        })
            .then(response => {
                if (response.ok) {
                    // If the response status is OK (200 or 201), redirect to home.html
                    window.location.href = "home.html";
                } else {
                    loader.style.display = "none";
                    // If the response status is not OK, extract error message and reject the Promise
                    return response.json().then(error => {
                        const errorMessage = error.message || "An error occurred. Please try again later.";
                        return Promise.reject(errorMessage);
                    });
                }
            })
            .catch(error => {
                loader.style.display = "none";
                console.error(error);
                return customPopup(error);
            });
        }



}


getUserDetails ();
document.getElementById(`changedetailsform`).addEventListener("submit", editUserDetails)