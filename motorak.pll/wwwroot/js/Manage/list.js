// Search functionality
document.getElementById('searchInput').addEventListener('input', function (e) {
    const searchTerm = e.target.value.toLowerCase();
    const customerCards = document.querySelectorAll('.customer-card');

    customerCards.forEach(card => {
        const name = card.querySelector('.customer-name').textContent.toLowerCase();
        const email = card.querySelector('.customer-email').textContent.toLowerCase();

        if (name.includes(searchTerm) || email.includes(searchTerm)) {
            card.style.display = 'block';
            card.style.animation = 'fadeInUp 0.3s ease forwards';
        } else {
            card.style.display = 'none';
        }
    });
});

// Actions
function viewCustomer(id) {
    window.location.href = `/Customer/CustomerDetails/${id}`;
}

function editCustomer(id) {
    window.location.href = `/Customer/Edit/${id}`;
}

let deleteCustomerId = null;

function deleteCustomer(id) {
    deleteCustomerId = id;
    document.getElementById('deleteModal').style.display = 'block';
}

function closeDeleteModal() {
    document.getElementById('deleteModal').style.display = 'none';
    deleteCustomerId = null;
}

// Confirm delete
document.getElementById('confirmDeleteBtn').addEventListener('click', function () {
    if (deleteCustomerId) {
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Customer/Delete';

        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.name = 'id';
        idInput.value = deleteCustomerId;

        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

        form.appendChild(idInput);
        form.appendChild(tokenInput);
        document.body.appendChild(form);
        form.submit();
    }
});


// Load more
document.getElementById('loadMoreBtn').addEventListener('click', function () {
    showNotification('Loading more customers...', 'info');
});
