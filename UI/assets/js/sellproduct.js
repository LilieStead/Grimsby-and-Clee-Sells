function sellproduct(event) {
    event.preventDefault();

    const loader = document.getElementById("preloader");
    const sellproductform = new FormData(document.getElementById("sellproduct"));


    const name = sellproduct.get("name");
    const description = sellproduct.get("description");
    const category = sellproduct.get("category");
    const manufacture = sellproduct.get("manufacture");
    const price = sellproduct.get("price");
    
}