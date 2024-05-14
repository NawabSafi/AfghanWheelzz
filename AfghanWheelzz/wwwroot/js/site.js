﻿
'use strict';

(function ($) {

    /*------------------
        Preloader
    --------------------*/
    $(window).on('load', function () {
        $(".loader").fadeOut();
        $("#preloder").delay(200).fadeOut("slow");


    /*------------------
        Background Set
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg');
        $(this).css('background-image', 'url(' + bg + ')');
    });

    //Canvas Menu
    $(".canvas__open").on('click', function () {
        $(".offcanvas-menu-wrapper").addClass("active");
        $(".offcanvas-menu-overlay").addClass("active");
    });

    $(".offcanvas-menu-overlay").on('click', function () {
        $(".offcanvas-menu-wrapper").removeClass("active");
        $(".offcanvas-menu-overlay").removeClass("active");
    });



 

  
  

})(jQuery);
function ConfrimPasswordShow() {
    var x = document.getElementById("ConfirmPassword");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}



function PasswordShow() {
    var x = document.getElementById("Password");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

//function Car Delete
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


document.getElementById("postAddButton").addEventListener("click", function (event) {
    // Check if the user is authenticated
    var isAuthenticated = '@User.Identity.IsAuthenticated';
    console.log(isAuthenticated);
    // If the user is not authenticated, show an alert and prevent the default action
    if (isAuthenticated != 'True') {
        alert("Please login to post a car.");
        event.preventDefault(); // Prevent the default action (redirecting to the listing page)
    }
});
