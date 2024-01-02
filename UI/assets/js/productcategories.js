
function getCategories(){
    fetch('https://192.168.0.135:44394/getallcategory')
    .then(response => response.json())
    .then(data =>{
        const categoryselect = document.getElementById('category');

        data.forEach(element => {
            var options = [
                {value: element.category_id, text: element.category_name}
            ];

            var choice = document.createElement('option');
            choice.value = options[0].value;
            choice.text = options[0].text;
            categoryselect.add(choice);
        });
    })
    .catch(error =>{
        customPopup("failed to get category");
        console.log(error);
    })
    
}


getCategories()