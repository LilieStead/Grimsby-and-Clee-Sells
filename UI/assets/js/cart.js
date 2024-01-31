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
                    getImages(element.cart_productid).then(Image => {
                        console.log(Image[0].imgUrl);
                        var productTotal = element.product.product_price * element.cart_quantity;
                        cartDiv.innerHTML += `
                    <div class="cartitem">
                            <div class="cartimgdiv">
                                <img src="${Image[0].imgUrl}" alt="">
                            </div>
                            <div class="cartinfo">
                                <h1>${element.product.product_name} <span>&pound;${element.product.product_price}</span></h1>
                                <form action="">
                                    <input type="number" value="${element.cart_quantity}">
                                    <button onclick="updateRequest(${element.cart_productid})" >update</button>
                                    <button onclick="removeRequest(${element.cart_productid})" >remove</button>
                                </form>
                                <h1>Total &pound;${productTotal}</h1>
                            </div>
                        </div>
                        <hr>
                        `
                    })
                });
            })
        })
        .catch(error => {
            console.error(error);
        })
}

getCartItems();


