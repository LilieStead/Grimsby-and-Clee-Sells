const urlParams = new URLSearchParams(window.location.search);
const userID = urlParams.get('id');


async function imageFetch(product){
    const images = [];
    
    for (let index = 0; index < 2; index++){
        const response = await fetch(`https://localhost:44394/GetImgThumbnailByProductId/${product.product_id}/${index}`);
        if (response.status === 204){
            console.log("One or more images were not found");
            return images;
        }
        const imgArray = await response.arrayBuffer();
        let img = new Blob([imgArray], {type: "image/jpeg"});
        let imgUrl = URL.createObjectURL(img);
        images.push({imgUrl, product});
    
    }
    
    return images;

}

function getUserItems(){
    const div = document.getElementById('unapprovedlist');
    fetch(`https://localhost:44394/getallproducts/user/${userID}`)
    .then(response => {
        if (response.status === 200){
            return response.json();
        } else if (response.status === 404){
            return response.json().then(error =>{
                return Promise.reject(error.message);
            })
        } else{
            console.error(response.status);
        }
    })
    .then(data => {
        console.log(data);
        document.getElementById('pageusername').innerHTML = `${data[0].user.users_username}`;
        data.forEach(item =>{
            var approveButton = `<h1><a onclick ="updateStatus(${item.product_id});">Approve</a></h1>`;
            if (item.product_status == 2){
                approveButton = '';
            }
            imageFetch(item).then(image => {
                var img2;
                if (image.length > 1){
                    img2 = `<img src="${image[1].imgUrl}" alt="">`
                }else{
                    img2 = "";
                }
                
                div.innerHTML += `<div class="usersproduct flexcontainer">
                        <div class="imgdiv">
                            <img src="${image[0].imgUrl}" alt="">
                            ${img2}
                        </div>
                        <div class="usersproductinfo">
                            <h1> ${item.product_name} </h1>
                            <h2><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i></h2>
                            <div class="statusoptions">
                                ${approveButton} <h1><a onclick ="rejectStatus(${item.product_id});">Rejected</a></h1>
                            </div>
                            <h1 class="info">${item.category.category_name} || &pound;${item.product_price}</h1>
                            <p>${item.product_description}</p>
                            <p>${item.status.status_name}</p>
                        </div>
                    </div>`
                    
            })
            
        })
    })
    .catch(error => {
        console.error(error);
        return customPopup(error);
    })
}



function updateStatus (item){
    const statusId = 2;
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

window.addEventListener('DOMContentLoaded', getUserItems);