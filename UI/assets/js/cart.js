var user;
var totalPrice = 0;
const productToOrder = [];
const quantityToOrder = [];


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
                console.log(response.status);
                if (response.status === 200){
                    return response.json();
                }else if (response.status === 400 || response.status === 404){
                    cartDiv.innerHTML = `
                    <div id="nocart">
                        <h3>You have no products in your cart</h3>
                    <hr>
                </div>`
                    const address = document.getElementById("order_address");
                    address.disabled = true;
                    const postcode = document.getElementById("order_postcode")
                    postcode.disabled = true;
                    const name = document.getElementById("order_recipientname");
                    name.disabled = true;
                    const number = document.getElementById("order_detail1");
                    number.disabled = true;
                    const expiry = document.getElementById("order_detail2");
                    expiry.disabled = true;
                    const cvv = document.getElementById("order_detail3")
                    cvv.disabled = true;
                    const button = document.getElementById("orderbtn");
                    button.disabled = true;
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
                        productToOrder.push({
                            product: element.cart_productid,
                            quantity: element.cart_quantity
                        });
                        
                        
                        console.log(totalPrice);
                        cartDiv.innerHTML += `
                    <div class="cartitem">
                            <div class="cartimgdiv">
                                <img src="${Image[0].imgUrl}" alt="">
                            </div>
                            <div class="cartinfo">
                                <h1>${element.product.product_name} <span>&pound;${element.product.product_price.toFixed(2)}</span></h1>
                                <form action="">
                                    <input name="cart_quantity" type="number" value="${element.cart_quantity}">
                                    <button onclick="updateRequest(event,${element.cart_productid})" >update</button>
                                    <button onclick="removeRequest(event,${element.cart_productid})" >remove</button>
                                </form>
                                <h1>Total &pound;${productTotal.toFixed(2)}</h1>
                            </div>
                        </div>
                        <hr>
                        `
                    })
                });
                console.log(productToOrder);
                console.log(quantityToOrder);
                totalPrice = totalPrice.toFixed(2);
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


function order (event){
    event.preventDefault();
    const loader = document.getElementById("preloader");
    const form = new  FormData(document.getElementById("orderForm"));
    const address = form.get(`order_address`);
    const postcode = form.get(`order_postcode`);
    const name = form.get(`order_recipientname`);
    const cardnumber = form.get(`order_detail1`);
    const exp = form.get(`order_detail2`);
    const cvv = form.get(`order_detail3`);


    const addresserror = document.getElementById("addresserror");
    const postcodeerror = document.getElementById("postcodeerror");
    const nameerror = document.getElementById("nameerror");
    const cardnumbererror = document.getElementById("cardnumbererror")
    const experror = document.getElementById("experror");
    const cvverror = document.getElementById("cvverror")

    let nopass = false;
    
    if (address == null || address == ``){
        nopass = true;
        addresserror.innerHTML = (`You need to enter your address`)
    }else if (address.length >= 101 || address.length <= 20){
        nopass = true;
        addresserror.innerHTML = (`you address neeeds to be inbetween 20 & 100`)
    }else{
        addresserror.innerHTML = null;
    }

    if (postcode == null || postcode == ''){
        nopass = true;
        postcodeerror.innerHTML = (`You need to enter your postcode`)
    }else if (!/^[A-Z]{1,2}[0-9R][0-9A-Z]? [0-9][ABD-HJLNP-UW-Z]{2}$/.test(postcode)) {
        nopass = true;
        postcodeerror.innerHTML = (`You need to enter a valid post code`)
    }else{
        postcodeerror.innerHTML = null
    }

    if (name == null || name == ``){
        nopass = true;
        nameerror.innerHTML = (`you need to enter the name on the card`);
    }else if ( name.length >= 71 ||  name.length <= 10){
        nopass = true;
        nameerror.innerHTML = ("Name needs to be between 10 & 70")
    }

    if (!/^\d{16}$/.test(cardnumber)) {
        nopass = true;
        cardnumbererror.innerHTML = 'Your card number needs to be 16 digits and contain only numbers';
    } else {
        cardnumbererror.innerHTML = null;
    }
    
    if (!/^\d{3}$/.test(cvv)) {
        nopass = true;
        cvverror.innerHTML = 'CVV should be a 3-digit number';
    }else{
        cvverror.innerHTML = (null);
    }
    if (!/^(0?[1-9]|1[0-2])\/\d{2}$/.test(exp)) {
        nopass = true;
        experror.innerHTML = 'Expiration date should be in the format MM/YY';
    }
    if(nopass){
        return;
    }

    productToOrder.forEach(productitem => {
        console.log(productitem);
        form.append("productid", productitem.product);
        form.append("quantity", productitem.quantity);
    })
    form.append("userid", user);
    loader.style.display = 'block';

    fetch(`https://localhost:44394/createorder`, {
        method: "POST",
        credentials: "include",
        body: form
    })
    .then (response => {
        if (response.status === 200 || response.status === 203){
            return response.json();
        }else{
            console.error();
        }
    })
    .then (order => {
        loader.style.display = 'none';
        window.location.href = "usersorder.html"
    })


}

getCartItems();


document.getElementById(`orderForm`).addEventListener("submit", order)

