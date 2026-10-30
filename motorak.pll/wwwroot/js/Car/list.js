// Filter functionality
document.addEventListener('DOMContentLoaded', function () {
    const filterBtns = document.querySelectorAll('.filter-btn');
    const carCards = document.querySelectorAll('.car-card');

    filterBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            // Remove active class from all buttons
            filterBtns.forEach(b => b.classList.remove('active-featured'));
            // Add active class to clicked button
            this.classList.add('active-featured');

            const filter = this.getAttribute('data-filter');

            carCards.forEach(card => {
                const category = card.getAttribute('data-category');
                const status = card.getAttribute('data-status');

                let show = false;

                if (filter === 'all') {
                    show = true;
                } else if (filter === 'forsale' && category === 'forsale') {
                    show = true;
                } else if (filter === 'forrent' && category === 'forrent') {
                    show = true;
                } else if (filter === 'available' && status === 'available') {
                    show = true;
                }

                if (show) {
                    card.style.display = 'flex';
                    card.style.animation = 'fadeIn 0.5s ease';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
});

// Add to cart functionality
function addToCart(carId, itemType) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    fetch('/Cart/AddToCartSimple', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token || ''
        },
        body: `id=${carId}&itemType=${itemType}`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showNotification(data.message || `Car ${itemType === 'rent' ? 'added for rental' : 'added to cart'} successfully!`);
                updateCartCount(data.count);
            } else {
                showNotification(data.message || 'Failed to add item to cart', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showNotification('An error occurred. Please try again.', 'error');
        });
}

// Show notification
function showNotification(message, type = 'success') {
    const notification = document.getElementById('cart-notification');
    const messageEl = document.getElementById('notification-message');

    messageEl.textContent = message;
    notification.classList.add('show');

    if (type === 'error') {
        notification.style.background = '#dc3545';
    } else {
        notification.style.background = 'var(--first-color)';
    }

    setTimeout(() => {
        notification.classList.remove('show');
    }, 3000);
}

// Update cart count (if you have a cart counter in header)
function updateCartCount(count) {
    const cartCountEl = document.getElementById('cart-count');
    if (cartCountEl) {
        cartCountEl.textContent = count;
    }
}
