

function displayUser(){
    const cookie = getCookie("usercookieexpiry");
    const username = document.getElementById("username");
    if (cookie){
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
        // creates session storage with user details
        .then(data => {
            username.innerHTML = "";
            username.innerHTML = data.username;
            sessionStorage.setItem("username",data.username);
            sessionStorage.setItem("userid",data.userid);
            sessionStorage.setItem("firstname",data.firstname);
            sessionStorage.setItem("lastname",data.lastname);
        })
        .catch(error => {
            sessionStorage.removeItem("username");
            sessionStorage.removeItem("userid");
            sessionStorage.removeItem("firstname");
            sessionStorage.removeItem("lastname");
            return window.location.href = "index.html";
        })
    }else{
        window.location.href= "index.html";
    }
}

// deletes any instance of the user in session storage 
function logout(){
    const cookie = getCookie("usercookieexpiry");
    if(cookie){
        fetch(`https://localhost:44394/logout`,{
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

document.getElementById('search').addEventListener('keydown', (event) => {
    // Check if the pressed key is 'Enter'
    if (event.key === "Enter") {
        // Prevent the default form submission behavior
        event.preventDefault();

        // Get the value from the input field
        var productname = event.target.value.trim();

        // Check if the input is not empty before redirecting
        if (productname) {
            // Construct the URL for the search page with the product name as a query parameter
            const searchpage = "searchproduct.html";
            window.location.href = `${searchpage}?productname=${encodeURIComponent(productname)}`;
        } else {
            // Handle the case where the input is empty (optional)
            console.log("Please enter a product name.");
        }
    }
});
