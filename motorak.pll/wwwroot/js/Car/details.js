function changeImage(thumbnail, imageSrc) {
    const mainImage = document.getElementById('mainImage');
    const thumbnails = document.querySelectorAll('.thumbnail');

    // Update main image
    mainImage.src = imageSrc;

    // Update active thumbnail
    thumbnails.forEach(thumb => thumb.classList.remove('active'));
    thumbnail.classList.add('active');
}

// Rent modal functionality
function showRentModal() {
    const modal = document.getElementById('rentModal');
    modal.style.display = 'block';

    // Set minimum dates
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);

    document.getElementById('startDate').min = today.toISOString().split('T')[0];
    document.getElementById('endDate').min = tomorrow.toISOString().split('T')[0];

    // Add event listeners for date calculation
    document.getElementById('startDate').addEventListener('change', calculateTotal);
    document.getElementById('endDate').addEventListener('change', calculateTotal);
}

function closeRentModal() {
    const modal = document.getElementById('rentModal');
    modal.style.display = 'none';
}

// Calculate rental total
function calculateTotal() {
    const startDate = new Date(document.getElementById('startDate').value);
    const endDate = new Date(document.getElementById('endDate').value);
    const dailyRate = @((Model.DailyRentPrice ?? 0).ToString("F2", System.Globalization.CultureInfo.InvariantCulture));

    if (startDate && endDate && endDate > startDate) {
        const days = Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24));
        const total = days * dailyRate;

        document.getElementById('numberOfDays').textContent = days;
        document.getElementById('totalCost').textContent = `${total.toFixed(2)}`;

        // Update end date minimum
        const minEnd = new Date(startDate);
        minEnd.setDate(minEnd.getDate() + 1);
        document.getElementById('endDate').min = minEnd.toISOString().split('T')[0];
    } else {
        document.getElementById('numberOfDays').textContent = '0';
        document.getElementById('totalCost').textContent = '$0.00';
    }
}

// Handle rent form submission
document.addEventListener('DOMContentLoaded', function () {
    const rentForm = document.getElementById('rentForm');
    if (rentForm) {
        rentForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const startDate = document.getElementById('startDate').value;
            const endDate = document.getElementById('endDate').value;

            if (!startDate || !endDate) {
                showNotification('Please select both start and end dates', 'error');
                return;
            }

            if (new Date(startDate) >= new Date(endDate)) {
                showNotification('End date must be after start date', 'error');
                return;
            }

            addToCartWithDates(@Model.Id, 'rent', startDate, endDate);
        });
    }
});

// Add to cart function
function addToCart(carId, itemType) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const formData = new URLSearchParams();
    formData.append('id', carId);
    formData.append('itemType', itemType);

    fetch('/Cart/AddToCartSimple', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token || ''
        },
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showNotification(data.message || `Car ${itemType === 'rent' ? 'added for rental' : 'added to cart'} successfully!`);
            } else {
                showNotification(data.message || 'Failed to add item to cart', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showNotification('An error occurred. Please try again.', 'error');
        });
}

// Add to cart with dates (for rentals)
function addToCartWithDates(carId, itemType, startDate, endDate) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const data = {
        CarId: carId,
        ItemType: itemType === 'rent' ? 1 : 0, // Assuming Rent = 1, Buy = 0
        StartDate: startDate,
        EndDate: endDate
    };

    fetch('/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token || ''
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showNotification(data.message || 'Car added to cart for rental!');
                closeRentModal();
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
    const notification = document.getElementById('notification');
    const messageEl = document.getElementById('notification-message');

    messageEl.textContent = message;
    notification.classList.add('show');

    if (type === 'error') {
        notification.style.background = '#dc3545';
        notification.querySelector('i').className = 'ri-close-circle-line';
    } else {
        notification.style.background = 'var(--first-color)';
        notification.querySelector('i').className = 'ri-check-line';
    }

    setTimeout(() => {
        notification.classList.remove('show');
    }, 4000);
}

// Close modal when clicking outside
window.addEventListener('click', function (event) {
    const modal = document.getElementById('rentModal');
    if (event.target === modal) {
        closeRentModal();
    }
});