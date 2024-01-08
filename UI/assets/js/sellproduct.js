function sellproduct(event) {
    event.preventDefault();

    const loader = document.getElementById("preloader");
    const sellproductform = new FormData(document.getElementById("sellproduct"));


    const name = sellproductform.get("name");
    const description = sellproductform.get("description");
    const category = sellproductform.get("category");
    const price = sellproductform.get("price");

    const nameerrorr = document.getElementById("nameerror");
    const descriptionerror = document.getElementById("descriptionerror");
    const categoryerror = document.getElementById("categoryerror");
    const priceerror = document.getElementById("priceerror");


    let nopass = false;
    

    // name validation
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
        loader.style.display = "block";
        const userid = sessionStorage.getItem("userid")
        console.log(userid)
        const productData = {
            product_name: name,
            product_description: description,
            product_category: category,
            product_userid: userid,
            product_price: price
        };


        fetch('https://localhost:44394/CreateProduct',{
            method:"POST",
            headers:{
                "Content-Type": "application/json",
            },
            body: JSON.stringify(productData),
        })
        .then(response => {
            if (response.status === 201 || response.status === 200){
                return response.json();
                
            }else if( response.status === 409){
                loader.style.display = "none";
                return response.json().then(error => {
                    return Promise.reject(error.message);''
                })
            }else {
                loader.style.display = "none";
                return response.json().then(error => {
                    return Promise.reject("You have run into an error. please try again later.");
                })
            }
        })
        .catch(error => {
            loader.style.display = "none";
            console.error(error);
            return customPopup(error);
        })
    }


}
 document.getElementById('sellproduct').addEventListener("submit", sellproduct);