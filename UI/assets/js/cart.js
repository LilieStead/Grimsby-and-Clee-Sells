var user;
var totalPrice = 0;
async function getImages(data){

    const images = [];
    const response = await fetch(`https://localhost:44394/GetImgThumbnailByProductId/${data}/0`);
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


function getCartItems(){
    const cartDiv = document.getElementById("cartproduct");
    const totalPriceDiv = document.getElementById("total");
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
            user = data.userid;
            fetch(`https://localhost:44394/SearchByUserId/${data.userid}`,{
                method: "GET",
                credentials: "include"
            })
            .then (response => {
                if (response.status === 200){
                    return response.json();
                }
            })
            .then( data => {
                console.log(data);
                data.forEach(element => {
                    console.log(element);
                    var productTotal = element.product.product_price * element.cart_quantity;
                    totalPrice += productTotal;
                    getImages(element.cart_productid).then(Image => {
                        console.log(Image[0].imgUrl);
                        
                        console.log(totalPrice);
                        cartDiv.innerHTML += `
                    <div class="cartitem">
                            <div class="cartimgdiv">
                                <img src="${Image[0].imgUrl}" alt="">
                            </div>
                            <div class="cartinfo">
                                <h1>${element.product.product_name} <span>&pound;${element.product.product_price}</span></h1>
                                <form action="">
                                    <input name="cart_quantity" type="number" value="${element.cart_quantity}">
                                    <button onclick="updateRequest(event,${element.cart_productid})" >update</button>
                                    <button onclick="removeRequest(event,${element.cart_productid})" >remove</button>
                                </form>
                                <h1>Total &pound;${productTotal}</h1>
                            </div>
                        </div>
                        <hr>
                        `
                    })
                });
                totalPriceDiv.innerHTML = "&pound;" + totalPrice;
            })
        })
        .catch(error => {
            console.error(error);
        })
}


function updateRequest(event, productid){
    event.preventDefault();
    var formElement = event.target.closest(`form`);
    var quantityInput = formElement.querySelector(`input[name="cart_quantity"]`)
    var quantity = quantityInput.value;
    const formData = new FormData();
    formData.append("cart_userid", user);
    formData.append("cart_productid", productid);
    formData.append("cart_quantity", quantity);
    fetch(`https://localhost:44394/api/Cart/EditCartItemQuantity`,{
        method: "PUT",
        body: formData,
    })
    .then(response => {
        if (response.ok){
            window.location.reload();
        }else if(response.status === 409){
            return response.json().then(error => {
                return Promise.reject(error.message);
            })
        }
        else{
            customPopup("Unable to update item to cart")
            console.error(response.status);
            throw new Error("Error adding item to cart");
        }
    })
    .catch(error => {
        console.error(error);
        customPopup(error);
    })
    
}

function removeRequest(event, productid){
    event.preventDefault();
    const formData = new FormData();
    formData.append("cart_userid", user);
    formData.append("cart_productid", productid);
    fetch(`https://localhost:44394/api/Cart/DeleteUsersCart`,{
        method: "DELETE",
        body: formData,
    })
    .then(response => {
        if (response.ok){
            window.location.reload();
        }else{
            customPopup("Unable to delete item from cart")
            console.error(response.status);
            throw new Error("Error deleting item to cart");
        }
    })
    .catch(error => {
        console.error(error);
        customPopup("An error occurred while deleting the item from your cart");
    })
}


function order (){
    
}

getCartItems();


document.getElementById(`orderForm`).addEventListener("submit", order)


