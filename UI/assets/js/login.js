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
        // send the login request to the api
        fetch(`https://localhost:44394/login/${username}/${password}`)
        .then(response => {
            if (response.status === 200){
                return response.json();
                // if error found then use reject to send custom pop 
            }else if(response.status === 404 || response.status === 401){

                return response.json().then(error => {
                    return Promise.reject(error.message);
                })
            }else{
                // if error found then use reject to send custom pop 
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
// see if the admin has an expiry cookie
function checkAdminLogin(){
    const adminCookie = getCookie('admincookieexpiry');
    console.log("Checking Admin...");
    if (adminCookie){
        document.removeEventListener('DOMContentLoaded', preventLoginLoop);
        window.location.href = "adminhome.html";
        
    } else{
        return;
    }
}
// used so the users doest get stuck going from index to adminlogin 
function preventLoginLoop(){
    const adminCookie = getCookie('admincookieexpiry');

    if(adminCookie){
        fetch(`https://localhost:44394/api/Admin/decodeadmin`, {
            method: "GET",
            credentials: "include"
        })
        .then(response => {
            if (response.status === 200){
                return response.json();
            }
            else{
                console.error(response.status);
            }
        })
        .then(data => {
            checkAdminLogin();
            document.addEventListener('DOMContentLoaded', checkAdminLogin);
        })
        .catch(error => {
            return console.error(error);
        })
    } else{
        console.log("Admin Not Logged In");
    }
    
}
document.addEventListener('DOMContentLoaded', preventLoginLoop);
// preventLoginLoop();


document.getElementById('loginform').addEventListener("submit", login);

