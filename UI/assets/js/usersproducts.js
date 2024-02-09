const userId = sessionStorage.getItem('userid');
fetch(`https://localhost:44394/api/Product/GetProductByUserId/${userId}`)
.then(response => {
    const productBody = document.querySelector('#usersproducts');
    // if response inst ok it means there is an error 
    if (response.status === 404 || response.status === 400) {
        console.log("hdjahsdjsah")
        productBody.innerHTML = `
        <div id="noproducts">
                <h3>You have made no products</h3>
            </div>
        `;
        return response.json().then(error => {
            return Promise.reject(error.message);
        });
    }else if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return response.json();
})
.then (product => imageFetch(product))
.then(data => {
    // nothing is needed in here
})
.catch(error => {
    // any errors go here
    if (error === "No items found"){
        return;
    }else{
        return customPopup(error);
    }
    
})

async function imageFetch(product){
    // sets format of image stored in array
    
    const images = [];
    for ( let imgNumber = 0; imgNumber < 4; imgNumber++){

        console.log(product)
        product.forEach(async item => {
            const response = await fetch(`https://localhost:44394/GetImgThumbnailByProductId/${item.product_id}/${imgNumber}`);
        if (response.status === 204){
            console.log("One or more images were not found");
            return;
        }
        const imgArray = await response.arrayBuffer();
        let img = new Blob([imgArray], {type: "image/jpeg"});
        let imgUrl = URL.createObjectURL(img);
        images.push({imgUrl, item});
        createProduct(imgUrl, item);

        })
        
    }
    console.log(images)
    

}



function createProduct(imgUrl, item){
    const productBody = document.querySelector('#usersproducts');
    console.log(imgUrl, item);
    // Adds data to the page in HTML
    productBody.innerHTML+=
    `<div class="usersproduct flexcontainer">
    <div class="imgdiv">
        <img src="${imgUrl}" alt="">
    </div>
    <div class="usersproductinfo">
        <h1> ${item.product_name}<a href="editproduct.html?id=${item.product_id}"> Edit Product</a></h1>
        <h1 class="info">${item.status.status_name}</h1>
        <h1 class="info">${item.category.category_name} || &pound;${item.product_price.toFixed(2)}</h1>
        <p>${item.product_description}</p>
    </div>
</div>`;
}