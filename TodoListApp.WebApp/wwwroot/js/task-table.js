document.addEventListener("DOMContentLoaded", function ()
{
    document.querySelectorAll(".clickable-row").forEach(row =>
    {
        row.addEventListener("click", () =>
        {
            window.location = row.getAttribute("data-url");
        });
    });
});
