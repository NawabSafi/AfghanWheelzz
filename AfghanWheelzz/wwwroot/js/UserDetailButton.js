
// Get the modal
var modal = document.getElementById("myModal");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks on the button, open the modal
function openModal(carId) {
    modal.style.display = "block";

    // Fetch user details using AJAX
    fetch(`/Home/GetSellerDetails?carId=${carId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const userDetailsHtml = `
        <h2>Seller's Details</h2>
        <p><strong>Name:</strong> ${data.name}</p>
        <p><strong>Email:</strong> ${data.email}</p>
        <p><strong>Phone Number:</strong> ${data.PhoneNumber}</p>
        `;
            document.getElementById("userDetails").innerHTML = userDetailsHtml;
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
