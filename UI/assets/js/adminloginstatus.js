function checkLoginStatusAdmin(){
    console.log("START")
    const exptime = getCookie("admincookieexpiry");
    console.log(exptime)
    var url = window.location.pathname;
    var file = url.substring(url.lastIndexOf("/")+1);
    if (exptime){
        console.log("if")
        if(file === "adminlogin.html"){
            console.log("adminlogin")
            window.location.href = "adminhome.html";
        }else{
            if (file === "adminhome.html"){
                console.log("inadminhome");
            }else{
                console.log("adminnotlogin")
            }
            
        }
    }else{
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