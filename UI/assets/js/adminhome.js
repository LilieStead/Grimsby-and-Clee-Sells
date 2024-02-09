const urlFile = window.location.pathname;
var file = urlFile.substring(urlFile.lastIndexOf("/") + 1);
//if the file is admin home , do the following 
if (file === "adminhome.html"){
    async function imageFetch(product){
        console.log(product);
        const images = [];
        const response = await fetch(`https://localhost:44394/GetImgThumbnailByProductId/${product.product_id}/0`);
        if (response.status === 204){
            console.log("One or more images were not found");
        }
        const imgArray = await response.arrayBuffer();
        let img = new Blob([imgArray], {type: "image/jpeg"});
        let imgUrl = URL.createObjectURL(img);
        //push the images so they can be used else where
        images.push({imgUrl, product});

        console.log(images)
        return images;
    
    }
    
        
        function displayUnapprovedProducts(){
            //api connection
            fetch(`https://localhost:44394/GetProductsByStatus/1`)
            .then(response => {
                if (response.status === 200){
                    return response.json();

                } else if (response.status === 404){
                    //error handling 
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
                    imageFetch(item).then(image => {
                        console.log(image);
                        //html for each part of data 
                        prodDiv.innerHTML += `
                    
                    
                    <div class="usersproduct flexcontainer">
                        <div class="imgdiv">
                            <img src="${image[0].imgUrl}" alt="">
                        </div>
                        <div class="usersproductinfo">
                            <h1> ${item.product_name} </h1>
                            <h2><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i></h2>
                            <div class="statusoptions">
                                <h1><a onclick ="updateStatus(${item.product_id});">Approve</a></h1> <h1><a onclick ="rejectStatus(${item.product_id});">Rejected</a></h1>
                            </div>
                            <h1 class="info">${item.category.category_name} || &pound;${item.product_price.toFixed(2)}</h1>
                            <p>${item.product_description}</p>
                        </div>
                    </div>`
                    })
                })
            })
            //error handling 
            .catch(error => {
                console.error(error);
                return customPopup(error);
            })
        }
        displayUnapprovedProducts();
   
} else if (file === "adminusersearch.html"){
    //if it is adminusersearch
function displayUsersOnSearch(){
    const userSearch = document.getElementById('usersearch').value;
//api connection
    fetch(`https://localhost:44394/adminsearch/users?users=${userSearch}`)
    .then(response => {
        if (response.status === 200){
            return response.json();
            //send error code 
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
            //for each part of data 
            data.forEach(user => {
                var dob = user.users_dob;
                var convert = new Date(dob);
                var options = {day: '2-digit', month: '2-digit', year: 'numeric'};
                var formattedDate = convert.toLocaleDateString('en-GB', options);
                formattedDate = formattedDate.split('/').join('/');
    
                console.log(user);
                // Add to <a> tag's href: ${user.users_id} after creating the page
                resultDiv.innerHTML += `
                <div class="usersproduct">
            <div class="usersproductinfo">
                <h1> <i class="fa fa-user" aria-hidden="true"> </i>${user.users_username}</h1>
                    <h5>first name: <span class="details">${user.users_firstname}</span></h5>
                    <h5>last name: <span class="details">${user.users_lastname}</span></h5>
                    <h5>email: <span class="details">${user.users_email}</span></h5>
                    <h5>phone number: <span class="details"> ${user.users_phone}</span></h5>
                    <h5>Date of birth: <span class="details"> ${formattedDate}</span></h5>
                    <h5><a href="adminuseritems.html?id=${user.users_id}">View ${user.users_username}'s Items</a></h5>
                </div>
            </div>
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

function updateStatus (item){
    const statusId = 2;
    const statusData = new FormData();
    statusData.append("product_status", statusId);
    console.log(item);
    //update status to approve
    fetch(`https://localhost:44394/ChangeProductStatus/${item}`,{
        method: "PUT",
        body: statusData
    })
    .then(response => {
        if (response.status === 200 || response.status === 201){
            return response.json();
        }else{
            console.error(response.status);
            customPopup("An unexpected error has occurred");
        }
    })
    .then(data => {
        console.log(statusId);
        window.location.reload();
    })
    .catch(error => {
        console.error(error);
    })
}
//update status to reject
function rejectStatus(item){
    const statusId = 3;
    const statusData = new FormData();
    statusData.append("product_status", statusId);
    console.log(item);
    fetch(`https://localhost:44394/ChangeProductStatus/${item}`,{
        method: "PUT",
        body: statusData
    })
    .then(response => {
        if (response.status === 200 || response.status === 201){
            return response.json();
        }else{
            console.error(response.status)
            customPopup("An unexpected error has occurred");
        }
    })
    
    .then(data => {
        console.log(statusId);
        window.location.reload();
    })
    .catch(error => {
        console.error(error);
    })
}

