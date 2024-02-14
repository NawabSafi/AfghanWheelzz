
    document.addEventListener("DOMContentLoaded", function () {
    var deleteButtons = document.querySelectorAll(".delete-btn");
    var modal = document.getElementById("myModal");
    var modalTitle = modal.querySelector(".modal-title");
    var modalBody = modal.querySelector(".modal-body");
    var confirmDeleteButton = modal.querySelector(".btn-danger");

    deleteButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            var carId = this.getAttribute("data-id");
            modalTitle.textContent = "Delete Car";
            modalBody.innerHTML = "<p>Are you sure you want to delete the car with ID " + carId + "?</p>";
            confirmDeleteButton.onclick = function () {
                fetch('/Cars/Delete/' + carId, {
                    method: 'POST'
                }).then(function (response) {
                    if (response.ok) {
                        // Reload the page after successful deletion
                        window.location.reload();
                    } else {
                        console.error('Failed to delete the car.');
                    }
                }).catch(function (error) {
                    console.error('Error deleting car:', error);
                });
            };
            $('#myModal').modal('show');
        });
    });
});
