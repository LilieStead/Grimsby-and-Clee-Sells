const loader = document.getElementById("loading");
const userId = sessionStorage.getItem('userid');
fetch(`https://localhost:44394/api/Product/GetProductByUserId/${userId}`)
.then(response => {
    if (!response.ok) {
        document.getElementById('loadingcontainer').style.display
        throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return response.json();
})
.then (product => imageFetch(product))
.then(data => {

})
.catch(error => {
    loader.style.display = "none";
    console.log(error);
    return customPopup(error);
})

async function imageFetch(product){
    const images = [];
    for ( let imgNumber = 0; imgNumber < 4; imgNumber++){
        product.forEach(async iteam => {
            const response = await fetch(`https://localhost:44394/GetImgThumbnailByProductId/${iteam.product_id}/${imgNumber}`);
        if (response.status === 204){
            console.log("One or more images were not found");
        }
        const imgArray = await response.arrayBuffer();
        let img = new Blob([imgArray], {type: "image/jpeg"});
        let imgUrl = URL.createObjectURL(img);
        images.push({imgUrl, iteam});
        createProduct(imgUrl, iteam);

        })
    }
    console.log(images)

}



function createProduct(imgUrl, iteam){
    const productBody = document.querySelector('#usersproducts');
    console.log(imgUrl, iteam);
    productBody.innerHTML+=
    `<div class="usersproduct flexcontainer">
    <div class="imgdiv">
        <img src="${imgUrl}" alt="">
    </div>
    <div class="usersproductinfo">
        <h1> ${iteam.product_name} </h1>
        <h2><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i><i class="fa fa-star" aria-hidden="true"></i></h2>
        <h1 class="info">${iteam.status.status_name}</h1>
        <h1 class="info">${iteam.category.category_name} || $${iteam.product_price}</h1>
        <p>${iteam.product_description}</p>
    </div>
</div>`;
}