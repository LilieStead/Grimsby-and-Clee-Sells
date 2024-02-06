var productid;
var user;

function validateUser() {
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

validateUser();
