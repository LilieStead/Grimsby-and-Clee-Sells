
// used to display all the admins details
function displayAdmin(){
    const cookie = getCookie("admincookieexpiry");
    const username = document.getElementById("username");
    if (cookie){
        fetch(`https://localhost:44394/api/Admin/decodeadmin`, {
            method: "GET", 
            credentials: "include"
        })
        .then(response => {
            if (response.status === 200){
                return response.json();
            }
            // error handing to custom pop up 
            else if(response.status === 500){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            // error handing to custom pop up 
            else if(response.status === 400){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            // error handing to custom pop up 
            else if(response.status === 401){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            // error handing to custom pop up 
            else{
                console.error(response.status);

            }

        })
        // sets session storage with admins details 
        .then(data => {
            username.innerHTML = "";
            username.innerHTML = data.username;
            sessionStorage.setItem('IsAdmin', "UserIsAdmin");
            sessionStorage.setItem("username",data.username);
            sessionStorage.setItem("userid",data.userid);
            sessionStorage.setItem("firstname",data.firstname);
            sessionStorage.setItem("lastname",data.lastname);
        })
        // details admin session storage if an error is present 
        .catch(error => {
            sessionStorage.setItem('IsAdmin');
            sessionStorage.removeItem("username");
            sessionStorage.removeItem("userid");
            sessionStorage.removeItem("firstname");
            sessionStorage.removeItem("lastname");
            return window.location.href = "adminlogin.html";
        })
    }else{
        window.location.href= "adminlogin.html";
    }
}

// gets the admins expiry cookie so the UI knows they should be logged in 
function logout(){
    const cookie = getCookie("admincookieexpiry");
    if(cookie){
        fetch(`https://localhost:44394/AdminLogout`,{
            method:"GET",
            credentials:"include"
        })
        .then(response => {
            if (response.status === 200){
                return response.json();
            } else if ( response.status === 400){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }else{
                console.error(response.status);
            }
        })

        .then(data => {
            sessionStorage.removeItem("username");
            sessionStorage.removeItem("userid");
            sessionStorage.removeItem("firstname");
            sessionStorage.removeItem("lastname");
            window.location.href = "adminlogin.html";
        })
        .catch(error => {
            return customPopup(error);
        })
    }
}

document.getElementById("logout").addEventListener("click",logout);



displayAdmin();