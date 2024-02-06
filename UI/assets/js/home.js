const htmldiv = document.getElementById('homediv')

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


fetch(`https://localhost:44394/api/Product/GetTopProducts`)
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
                            <form action="" class="addtocart">
                                <button  onclick="addToCart(event, ${item.product_id})"><a href="#"><i class="fa fa-cart-plus" aria-hidden="true" ></i></a></button>
                                <input type="number" name="cart_quantity" value="1" maxlength="2" minvalue="0">
                            </form>
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

function addToCart(event, id) {
    event.preventDefault();

    var formElement = event.target.closest('form');
    var quantityInput = formElement.querySelector('input[name="cart_quantity"]');
    var quantity = quantityInput.value;
    
    const userid = sessionStorage.getItem("userid");

    console.log(quantity);

    if (quantity <= 0){
        customPopup("Your product must have a quantity");
        return;
    }
    const formData = new FormData();
    formData.append("cart_userid", userid);
    formData.append("cart_productid", id);
    formData.append("cart_quantity", quantity);

    fetch(`https://localhost:44394/AddToCart/`, {
        method: "POST",
        body: formData,
    })

    .then(response => {
        if (response.ok) {
            return response.json();
        } else {
            customPopup("Unable to add item to cart");
            console.error(response.status);
            throw new Error("Error adding item to cart");
        }
    })
    .then(data => {
        customPopup("The item has been added to your cart");
        // Additional logic after adding to cart (if needed)
    })
    .catch(error => {
        console.error(error);
        customPopup("An error occurred while adding the item to your cart");
    });
}
