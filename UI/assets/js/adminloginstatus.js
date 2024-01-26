

// looks to see if the admin should be logged in
function checkLoginStatusAdmin(){
    console.log("START")
    const exptime = getCookie("admincookieexpiry");
    const usertime = getCookie("usercookieexpiry");
    var url = window.location.pathname;
    var file = url.substring(url.lastIndexOf("/")+1);
    if (exptime){
        console.log("Admin exists");
        if(file === "adminlogin.html"){
            console.log("adminlogin")
            window.location.href = "adminhome.html";
        }
    } else if (usertime){
        console.log("I detect a user cookie");
        window.location.href = "home.html";
    }
    else{
        console.log("else");
        if (file === "adminlogin.html"){
            console.log("else if")
        }else{
            console.log("adminhome")
            window.location.href = "adminhome.html";
        }
    }
    console.log("END")
}

checkLoginStatusAdmin();
// validates the users cookie
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