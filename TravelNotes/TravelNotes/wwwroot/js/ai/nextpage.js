document.getElementById("submit").addEventListener("click", function () {
    var selectedValues = [];
    var checkboxes = document.querySelectorAll(".tag-checkradio:radio");
    checkboxes.forEach(function (checkbox) {
        selectedValues.push(checkbox.value);
    });
    console.log(selectedValues); // You can do anything with these selected values
    // Here you can implement logic to switch to the next page
    // For example, you can hide the current page and show the next one
    var currentPage = document.querySelector(".page:not(.hidden)");
    var nextPage = currentPage.nextElementSibling;
    if (nextPage) {
        currentPage.classList.add("hidden");
        nextPage.classList.remove("hidden");
    }
});