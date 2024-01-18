function displayUsersOnSearch(){
    const userSearch = document.getElementById('usersearch').value;

    fetch(`https://localhost:44394/adminsearch/users?users=${userSearch}`)
    .then(response => {
        if (response.status === 200){
            return response.json();
        } else if (response.status === 404){
            return response.json().then(error => {
                return Promise.reject(error.message);
            });
        } else{
            console.error(response.status);
        }
    })
    .then(data => {
        console.log(data);
        const resultDiv = document.getElementById('userresultdiv');
        resultDiv.innerHTML = '';
        data.forEach(user => {
            var dob = user.users_dob;
            var convert = new Date(dob);
            var options = {day: '2-digit', month: '2-digit', year: 'numeric'};
            var formattedDate = convert.toLocaleDateString('en-GB', options);
            formattedDate = formattedDate.split('/').join('/');

            console.log(user);
            // Add to <a> tag's href: ${user.users_id} after creating the page
            resultDiv.innerHTML += `<div class="userresult"><h3>Name: ${user.users_firstname} ${user.users_lastname}</h3>
            <h3><a href='#'>Username: ${user.users_username}</a></h3>
            <h3>Email: ${user.users_email}</h3>
            <h3>DOB: ${formattedDate}</h3>
            <h3>Phone Number: ${user.users_phone}</h3>
            </div>`
        })
    })
    .catch(error => {
        console.error(error);
        return customPopup(error);
    })
}

document.addEventListener('DOMContentLoaded', function(){
    // function getThumbnail(){
        // Logic here 
    //}

    function displayUnapprovedProducts(){
        fetch(`https://localhost:44394/GetProductsByStatus/1`)
        .then(response => {
            if (response.status === 200){
                return response.json();
            } else if (response.status === 404){
                return response.json().then(error => {
                    return Promise.reject(error.message);
                });
            } else{
                console.error(response.status);
            }
        })
        .then(data => {
            console.log(data);
            const prodDiv = document.getElementById('unapprovedlist');
            prodDiv.innerHTML = '';
            data.forEach(item => {
                // functionName(data).then(image => {
                    console.log(item);
                    prodDiv.innerHTML += `<div class="unapproveditems"><h3>Name: ${item.product_name}</h3>
                    <h3>Description: ${item.product_description}</h3>
                    <h3>Price: $${item.product_price}</h3>
                    <h3>Status: ${item.status.status_name}</h3>
                    <h3>Category: ${item.category.category_name}</h3>
                    <h3>User: ${item.user.users_username}</h3>
                    </div>`
                // })
                
            })
        })
        .catch(error => {
            console.error(error);
            return customPopup(error);
        })
    }
    displayUnapprovedProducts();
});




document.getElementById('usersearch').addEventListener('keydown', function(event){
    if (event.key === "Enter"){
        displayUsersOnSearch();
    }
});