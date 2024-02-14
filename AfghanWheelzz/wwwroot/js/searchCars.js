function searchCars() {
    var searchInput = document.getElementById("searchInput").value.toLowerCase();

    var cards = document.querySelectorAll(".card");
    cards.forEach(function (card) {
        var cardTitle = card.querySelector(".card-title").innerText.toLowerCase();
        var cardText = card.querySelector(".card-text").innerText.toLowerCase();
        if (cardTitle.includes(searchInput) || cardText.includes(searchInput)) {
            card.style.display = "block";
        } else {
            card.style.display = "none";
        }
    });
}
