function previewImage(imageNumber) {
    const imageInput = document.getElementById('images' + imageNumber);
    var gallery = document.getElementById('gallery');
 
    const file = imageInput.files[0];
   
    let reader = new FileReader();
    reader.onload = function (e) {
        let galleryItem = document.createElement('div');
 
        let galleryImg = document.createElement('img');
        galleryImg.className = 'img-fluid';
        galleryImg.onload = function () {
            URL.revokeObjectURL(galleryImg.src); 
        };
        galleryImg.src = URL.createObjectURL(file);
 
        galleryItem.appendChild(galleryImg);
        gallery.appendChild(galleryItem);
    };
 
    reader.readAsDataURL(file);
}