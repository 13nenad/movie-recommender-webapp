function initialiseSearchBox(userId) {
    $("#movieSearch").tokenInput("/Movies/AutoCompleteSuggestions", {
        queryParam: "searchTerm",
        minChars: 3,
        hintText: "Search...",
        tokenLimit: 1,
        theme: "facebook",
        onAdd: function () {
            var selectedMovie = $("#movieSearch").tokenInput("get")[0].movieId;
            location.href = "/Movies/Details/" + selectedMovie + "?userId=" + userId;
        }
    });
}

function goToSearchResults(userId) {
    if ($("#token-input-movieSearch").val().length > 2) {
        var searchTerm = $("#token-input-movieSearch").val();

        if (searchTerm !== "") {
            location.href = "/Movies/Index?userId=" + userId + "&searchTerm=" + searchTerm;
        }
    }
}

function initialiseCarousel() {
    $(document).ready(function () {
        $('#filteredCarousel').carousel({
            interval: 3000
        });

        $('#recommendationsCarousel').carousel({
            interval: 3500
        });

        $('.carousel .carousel-item').each(function () {
            var minPerSlide = 4;
            var next = $(this).next();
            if (!next.length) {
                next = $(this).siblings(':first');
            }
            next.children(':first-child').clone().appendTo($(this));

            for (var i = 0; i < minPerSlide; i++) {
                next = next.next();
                if (!next.length) {
                    next = $(this).siblings(':first');
                }

                next.children(':first-child').clone().appendTo($(this));
            }
        });
    });
}

function startDatabricksJob(userId) {
    $(document).ready(function () {
        $.ajax({
            url: "/Movies/StartDatabricksJob/" + userId,
            type: "GET",
            success: function (data) {
                if (data !== "Success")
                    alert(data);
                else
                    alert("Databricks notebook job triggered successfully");
            },
            error: function (jxhr, msg, err) {
                alert(msg);
            }
        });
    });
}