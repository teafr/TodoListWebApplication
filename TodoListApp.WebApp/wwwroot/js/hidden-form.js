function toggleEdit(id)
{
    const container = document.getElementById(id);
    const display = container.querySelector(".comment-display");
    const form = container.querySelector(".comment-edit-form");
    const input = form.querySelector("input[name='newComment']");

    display.classList.toggle("d-none");
    form.classList.toggle("d-none");

    if (!form.classList.contains("d-none"))
    {
        input.focus();
    }
}

function toggleForm(formId)
{
    const form = document.getElementById(formId);
    if (form)
    {
        form.classList.toggle("d-none");
    }
}