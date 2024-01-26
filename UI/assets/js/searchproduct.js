
const prama = new URLSearchParams(window.location.search);
const productnamesearch = prama.get(`productname`);
const htmldiv = document.getElementById('products')

async function getImages(data){

    const images = [];
    const response = await fetch(`https://localhost:44394/GetImgThumbnailByProductId/${data.product_id}/0`);
    if (response.status === 204){
        console.log("One or more images were not found");
        return;
    }
    const imgArray = await response.arrayBuffer();
    let img = new Blob([imgArray], {type: "image/jpeg"});
    let imgUrl = URL.createObjectURL(img);
    images.push({imgUrl, data});

    console.log(images)
    return images;
}


fetch(`https://localhost:44394/SearchByProductName/${productnamesearch}`)
.then (response => {
    if (response.status === 200){
        return response.json();
    }else if( response.status === 404){
        return response.json().then(error => {
            return Promise.reject(error.message);
        })
    } else{
        return console.error(response.status);
    }
})
.then (data => {
    console.log(data);
    data.forEach(item => {
        console.log(item);
        getImages(item).then(image => {
            console.log(image);

                htmldiv.innerHTML += `
                
                <div class="usersproduct">
                    <div class="imgdiv">
                        <img src="${image[0].imgUrl}" alt="">
                    </div>
                    <div class="usersproductinfo">
                        <div class="productflex">
                            <h1>${item.product_name}</h1>
                            <a href="#"><i class="fa fa-cart-plus" aria-hidden="true"></i></a>
                            <input type="number" id="" name="cart_quantity" value="1" maxlength="2">
                        </div>
                        <h2><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i></h2>
                        <h3><i class="fa fa-user-o" aria-hidden="true"> </i>${item.user.users_username}</h3>
                        <h1 class="info">${item.category.category_name} ||  &pound;${item.product_price}</h1>
                        <p>${item.product_description}</p>
                    </div>
                </div>
                
                `
                
            
        })
    })

})
.catch (error =>{
    loader.style.display = "none";
    return customPopup(error);
})


document.getElementsByName('').addEventListener