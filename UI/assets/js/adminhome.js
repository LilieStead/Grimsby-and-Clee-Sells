const urlFile = window.location.pathname;
var file = urlFile.substring(urlFile.lastIndexOf("/") + 1);
if (file === "adminhome.html"){

        // function getThumbnail(){
            // Logic here 
        //}
        
        function displayUnapprovedProducts(){
            fetch(`https://localhost:44394/GetProductsByStatus/1`)
            .then(response => {
                if (response.status === 200){
                    return response.json();
                } else if (response.status === 404){
                    return response.json().then(error => {
                        return Promise.reject(error.message);
                    });
                } else{
                    console.error(response.status);
                }
            })
            .then(data => {
                console.log(data);
                const prodDiv = document.getElementById('unapprovedlist');
                prodDiv.innerHTML = '';
                data.forEach(item => {
                    // functionName(data).then(image => {
                        console.log(item);
                        prodDiv.innerHTML += `
                        
                        
                        <div class="usersproduct flexcontainer">
                            <div class="imgdiv">
                                <img src="Screenshot 2023-12-07 170845.png" alt="">
                            </div>
                            <div class="usersproductinfo">
                                <h1> ${item.product_name} </h1>
                                <h2><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i></h2>
                                <div class="statusoptions">
                                    <h1><a href="#">Approve</a></h1> <h1><a href="#">Rejected</a></h1>
                                </div>
                                <h1 class="info">${item.category.category_name} || &pound;${item.product_price}</h1>
                                <p>${item.product_description}</p>
                            </div>
                        </div>`
                    // })
                    
                })
            })
            .catch(error => {
                console.error(error);
                return customPopup(error);
            })
        }
        displayUnapprovedProducts();
   
} else if (file === "adminusersearch.html"){
function displayUsersOnSearch(){
    const userSearch = document.getElementById('usersearch').value;

    fetch(`https://localhost:44394/adminsearch/users?users=${userSearch}`)
    .then(response => {
        if (response.status === 200){
            return response.json();
        } else if (response.status === 404){
            return response.json().then(error => {
                return Promise.reject(error.message);
            });
        } else{
            console.error(response.status);
        }
    })
    .then(data => {
        console.log(data);
        const resultDiv = document.getElementById('userresultdiv');
        resultDiv.innerHTML = '';
        if (data == null || data == "" ){
            return customPopup("No users found")
        }else{
            data.forEach(user => {
                var dob = user.users_dob;
                var convert = new Date(dob);
                var options = {day: '2-digit', month: '2-digit', year: 'numeric'};
                var formattedDate = convert.toLocaleDateString('en-GB', options);
                formattedDate = formattedDate.split('/').join('/');
    
                console.log(user);
                // Add to <a> tag's href: ${user.users_id} after creating the page
                resultDiv.innerHTML += `<div class="userresult"><h3>Name: ${user.users_firstname} ${user.users_lastname}</h3>
                <h3><a href='#'>Username: ${user.users_username}</a></h3>
                <h3>Email: ${user.users_email}</h3>
                <h3>DOB: ${formattedDate}</h3>
                <h3>Phone Number: ${user.users_phone}</h3>
                </div>`
            })
        }
    })
    .catch(error => {
        console.error(error);
        return customPopup(error);
    })
}






document.getElementById('usersearch').addEventListener('keydown', function(event){
    if (event.key === "Enter"){
        displayUsersOnSearch();
    }
});
}



// <div class="unapproveditems"><h3>Name: ${item.product_name}</h3>
//                     <h3>Description: ${item.product_description}</h3>
//                     <h3>Price: $${item.product_price}</h3>
//                     <h3>Status: ${item.status.status_name}</h3>
//                     <h3>Category: ${item.category.category_name}</h3>
//                     <h3>User: ${item.user.users_username}</h3>
//                     </div>