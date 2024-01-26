function sellproduct(event) {
    console.log(event);
    event.preventDefault();

    const loader = document.getElementById("preloader");
    const sellproductform = new FormData(document.getElementById("sellproduct"));


    const name = sellproductform.get("product_name");
    const description = sellproductform.get("product_description");
    const category = sellproductform.get("product_category");
    const price = sellproductform.get("product_price");
    const image1 = sellproductform.get("productimg_img1");
    const image2 = sellproductform.get("productimg_img2");

    const nameerrorr = document.getElementById("nameerror");
    const descriptionerror = document.getElementById("descriptionerror");
    const categoryerror = document.getElementById("categoryerror");
    const priceerror = document.getElementById("priceerror");
    const imageerror = document.getElementById("imgerror");


    let nopass = false;

    // gets the id of the first image
    const newimg = document.getElementById("images1");
    const check = newimg.files[0];
// looks to see if the first image has been updated
    if (!check){
        imageerror.innerHTML = ("You need to enter an image for your product");
        nopass = true;
        event.preventDefault();
    }else{
        imageerror.innerHTML =(null);
    }
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
        console.log(userid);
        sellproductform.append("product_userid", userid);



        const imagedata = new FormData();
        imagedata.append("productimg_img", image1);
        imagedata.append("productimg_img", image2);

        sellproductform.delete("productimg_img1");
        sellproductform.delete("productimg_img2");

        // goes to the api to make the product 

        fetch('https://localhost:44394/CreateProduct',{
            method:"POST",
            
            body: sellproductform
        })
        .then(response => {
            if (response.status === 201 || response.status === 200){
                return response.json();
                
            }else if( response.status === 409){
                loader.style.display = "none";
                return response.json().then(error => {
                    return Promise.reject(error.message);''
                    // if 404 send user a pop up with error message from the api
                })
            }else {
                // unexpected error  
                loader.style.display = "none";
                return response.json().then(error => {
                    return Promise.reject("You have run into an error. please try again later.");
                })
            }
        })
        .then(data => {
            const productid = data.product_id;
            console.log(productid);
            imagedata.append("productimg_productid", productid)
            fetch(`https://localhost:44394/createproductimg`, {
                method: "POST",
                credentials: "include",
                body: imagedata
            })
            // if the data was send successfully 
            .then(response => {
                if (response.status === 201 || response.status === 200 ){
                    return response.json();
                }else if (response.status === 409){
                    return response.json().then(error => {
                        return Promise.reject(error.message);''
                    })
                    
                }else{
                    // if not show an error
                return response.json().then(error => {
                    return Promise.reject("You have run into an error. please try again later.");
                })
                }
            })
            .then(data => {
                // once product is made send the to page to validate 
                window.location.href= "success.html";
            })
            // how error
            .catch(error => {
                console.error(error);
            })
            .finally(() => {
                // stop loading animation 
                loader.style.display = "none";
            })
        })
        .catch(error => {
            // 
            console.error(error);
            return customPopup(error);
        })
        .finally(() => {
            loader.style.display = "none";
        })
    }


}
 document.getElementById('sellproduct').addEventListener("submit", sellproduct);
let currentImgIndex = 1;
// allows user to upload images
 function handleMultipleImages(event){
    event.preventDefault();
    var image = document.getElementById(`images${currentImgIndex}`);
    
    if (currentImgIndex > 2){
        console.error("You cannot add more than 2 images");
        return;
    }
    console.log(currentImgIndex);

    currentImgIndex++;
    
    image.click();
    if (image.files == null){
        currentImgIndex--;
    }
    
 }