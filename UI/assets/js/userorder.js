var user;
const orderDiv = document.getElementById("orders")

function getOrders(){
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
        .then (data =>{
            user = data.userid

//get orders from the api
            fetch(`https://localhost:44394/GetOrderByUserId/${user}`)
            .then (response =>{
                 if (response.status === 200){
                    return response.json();
                    
                 } else if (response.status === 400 || response.status === 404) {
                    orderDiv.innerHTML = `
                    <div id="noproducts">
                        <h3>You have no orders</h3>
                    </div>
                    `

                    return;
                 }
                 else{
                    return response.json().then(error => {
                        return Promise.reject(error.message);
                    })
                 }
            })
            .then (order => {
                //for each set of data
                order.forEach(orders =>{
                    console.log(orders);
                    
                    const dateTimeStr = orders.order_date;
                    // Create a new Date object from the string
                    const dateTime = new Date(dateTimeStr);

                    // Extract the date part
                    const dateOnlyStr = dateTime.toISOString().split('T')[0];


                    orderDiv.innerHTML += `
                    
                    <div class="usersproduct">
                    <div class="usersproductinfo">
                            <h5>recipient name: <span class="details">${orders.order_recipientname}</h1>
                            <h5>order date: <span class="details">${dateOnlyStr}</h1>
                            <h5>address: <span class="details">${orders.order_address}</span></h5>
                            <h5>Postcode: <span class="details">${orders.order_postcode}</span></h5>
                            <h5>Status: <span class="details"> ${orders.orderstatus.orderstatus_status}</span></h5>
                        </div>
                    </div>
                </div>

                
                    `
                })
            })
        })




}

getOrders();