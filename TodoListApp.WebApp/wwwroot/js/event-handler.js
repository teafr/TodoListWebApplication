document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.dropdown-item').forEach(item => {
        item.addEventListener('click', function () {
            const selected = this.getAttribute('data-value');
            document.getElementById('searchProperty').value = selected;
            document.getElementById('propertyDropdown').innerText = selected;
        });
    });
});