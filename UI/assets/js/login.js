function login(event){
    event.preventDefault();
    const loader = document.getElementById("preloader");
    const loginform = new FormData(document.getElementById("loginform"));
    const username = loginform.get('username');
    const password = loginform.get('password');

    const loginusernameerror = document.getElementById("loginusernameerror");
    const loginpassworderror = document.getElementById("loginpassworderror");

    nopass = false;

    if( username == null || username == ""){
        loginusernameerror.innerHTML = ("you need to enter your username");
        nopass = true;
        event.preventDefault();
    }else{
        loginusernameerror.innerHTML = (null);
    }

    if( password == null || password == ""){
        loginpassworderror.innerHTML = ("you need to enter your password");
        nopass = true;
        event.preventDefault();
    }else{
        loginusernameerror.innerHTML = (null);
    }
    console.log("before");
    if(nopass){
        return;
    }else{
        loader.style.display = "block";
        console.log("after");
        fetch(`https://192.168.0.135:44394/login/${username}/${password}`)
        .then(response => {
            if (response.status === 200){
                return response.json();
            }else if(response.status === 404 || response.status === 401){

                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
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
            loader.style.display = "none";
            return customPopup(error);
            
        })
    }
}

document.getElementById('loginform').addEventListener("submit", login);

