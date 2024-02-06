var productid;
var user;

function getProductinfo() {
    const param = new URLSearchParams(window.location.search);
    productid = param.get('id');

    fetch(`https://localhost:44394/api/User/decodetoken`, {
        method: "GET",
        credentials: "include"
    })
    .then(response => {
        if (response.status === 200) {
            return response.json();
        } else if (response.status === 500 || response.status === 400 || response.status === 401) {
            return response.json().then(error => {
                return Promise.reject(error.message);
            });
        } else {
            console.error(response.status);
        }
    })
    .then(data => {
        user = data.userid;
        console.log(user);

         

        // Now you can proceed with the second fetch call or any other logic
        fetch(`https://localhost:44394/getproductbyid/${productid}`)
            .then(response => {
                if (response.status === 404 || response.status === 400) {
                    return response.json().then(error => {
                        return Promise.reject(error.message);
                    });
                } else if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                return response.json();
            })
            .then(product => {
                console.log(user);
                if (product.product_userid != user) {
                    window.location.href = "home.html";
                } else {
                    document.getElementById("name").value = product.product_name;
                    document.getElementById("description").value = product.product_description;
                    document.getElementById("price").value =  product.product_price.toFixed(2);

                    var categorySelect = product.product_category;
                    var selectElement = document.getElementById("category");
                    selectElement.value = categorySelect;
                

                }
            });
    })
    .catch(error => {
        console.error(error);
        // Handle the error as needed
    });
}




function editUsersProduct(event){


    event.preventDefault();

    const loader = document.getElementById("preloader");
    const  editproductform = new FormData(document.getElementById("sellproduct"));
    let nopass = false;

    const name = editproductform.get("product_name");
    const description = editproductform.get("product_description");
    const category = editproductform.get("product_category");
    const price = editproductform.get("product_price");

    const nameerrorr = document.getElementById("nameerror");
    const descriptionerror = document.getElementById("descriptionerror");
    const categoryerror = document.getElementById("categoryerror");
    const priceerror = document.getElementById("priceerror");
    
    if (name == "" || name == null){
        nameerrorr.innerHTML = ("You need to enter in a name for your product");
        nopass = true;
        event.preventDefault();
    }else if (name.length <= 10){
        nameerrorr.innerHTML = ("You Products name needs to be more than 10 characters");
        nopass = true;
        event.preventDefault();
    }else if (name.length > 50){
        nameerrorr.innerHTML = ("Your products name can not be over 50 characters");
        nopass = true;
        event.preventDefault();
    }else{
        nameerrorr.innerHTML = (null);
    }

    // description validation

    if (description == "" || description == null){
        descriptionerror.innerHTML = ("You need to enter a product description");
        nopass = true;
        event.preventDefault();
    }else if (description.length <= 25){
        descriptionerror.innerHTML = ("Your product description needs to be over 25 characters");
        nopass = true;
        event.preventDefault();
    }else if (description.length > 200){
        descriptionerror.innerHTML = ("Your product description can not be more than 200 characters"); 
    }else{
        descriptionerror.innerHTML = (null);
    }

    // category validations

    if (category == "" || category == null){
        categoryerror.innerHTML = ("you need to select a category")
        nopass = true;
        event.preventDefault();
    }else{
        categoryerror.innerHTML = (null);
    }

    // Price validation 
    if (price == "" || price == null){
        priceerror.innerHTML = ("You need to enter your products price");
        nopass = true;
        event.preventDefault();
    }else{
        priceerror.innerHTML = (null);
    }

    if (nopass) {
        return;
    }else{
        editproductform.append("product_userid", user);
        editproductform.append("product_id", productid);
        fetch(`https://localhost:44394/updateproduct`, {
            method: "PUT",
            credentials: "include",
            body: editproductform
        })
        .then( response => {
            if (response.status != 200){
                return response.json().then(error => {
                    const errorMessage = error.message || "An error occurred. Please try again later.";
                    return Promise.reject(errorMessage);
                })
            }else {
                window.location.href = "success.html";
            }
        })
        .catch(error => {
            return customPopup(error);
        });


    }   
}
getProductinfo();
document.getElementById(`sellproduct`).addEventListener("submit", editUsersProduct);