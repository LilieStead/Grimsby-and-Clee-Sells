
function checkLoginStatus(){
    const exptime = getCookie("usercookieexpiry");
    var url = window.location.pathname;
    var file = url.substring(url.lastIndexOf("/")+1);
    if (exptime){
        if(file === "index.html"){
            window.location.href = "home.html";
        }
    }else{
        if (file === "index.html"){
            return;
        }else{
            window.location.href="index.html";
        }
    }

}

checkLoginStatus();






function getCookie(cookieName) {
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