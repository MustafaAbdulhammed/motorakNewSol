// Set up delete functionality for this specific customer
let deleteCustomerId = @Model.Id;

function deleteCustomer(id) {
    deleteCustomerId = id;
    document.getElementById('deleteModal').style.display = 'block';
}

function restoreCustomer(id) {
    if (confirm('Are you sure you want to restore this customer?')) {
        // Create form and submit for restore
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Customer/Restore';

        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.name = 'id';
        idInput.value = id;

        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

        form.appendChild(idInput);
        form.appendChild(tokenInput);
        document.body.appendChild(form);
        form.submit();
    }
}