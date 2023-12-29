

function displayUser(){
    const cookie = getCookie("usercookieexpiry");
    const username = document.getElementById("username");
    if (cookie){
        fetch(`https://192.168.0.135:44394/api/User/decodetoken`, {
            method: "GET", 
            credentials: "include"
        })
        .then(response => {
            if (response.status === 200){
                return response.json();
            }
            else if(response.status === 500){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            else if(response.status === 400){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            else if(response.status === 401){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }
            
            else{
                console.error(response.status);

            }

        })
        .then(data => {
            username.innerHTML = "";
            username.innerHTML = data.username;
            sessionStorage.setItem("username",data.username);
            sessionStorage.setItem("userid",data.userid);
            sessionStorage.setItem("firstname",data.firstname);
            sessionStorage.setItem("lastname",data.lastname);
        })
        .catch(error => {
            return customPopup(error);
        })
    }
}


function logout(){
    const cookie = getCookie("usercookieexpiry");
    if(cookie){
        fetch(`https://192.168.0.135:44394/logout`,{
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
            window.location.href = "index.html";
        })
        .catch(error => {
            return customPopup(error);
        })
    }
}

document.getElementById("logout").addEventListener("click",logout);



displayUser();