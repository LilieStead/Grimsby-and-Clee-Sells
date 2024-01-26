function checkLoginStatus(){
    // if the user has a user cookie it means they should be logged in
    const exptime = getCookie("usercookieexpiry");
    var url = window.location.pathname;
    var file = url.substring(url.lastIndexOf("/")+1);
    console.log(file);
    if (exptime){
        if(file === "index.html" || file.includes("admin")){
            console.log("gone home");
            window.location.href = "home.html";
        }
    }else{
        console.log("else")
        if (file === "index.html"){
            console.log("else if")
            return;
        }else{
            window.location.href = "index.html";
            
        }
    }

}

checkLoginStatus();

function getCookie(cookieName) {
    // used to get the users cookie
    var name = cookieName + "=";
    var ca = document.cookie.split(';');
    for(var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
