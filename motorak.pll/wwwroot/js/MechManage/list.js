// Mechanic Management JavaScript
let currentMechanicId = null;

// Search functionality
document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const mechanicsGrid = document.getElementById('mechanicsGrid');

    if (searchInput && mechanicsGrid) {
        searchInput.addEventListener('input', function () {
            const searchTerm = this.value.toLowerCase().trim();
            const mechanicCards = mechanicsGrid.querySelectorAll('.customer-card');

            mechanicCards.forEach(card => {
                const mechanicName = card.querySelector('.customer-name')?.textContent.toLowerCase() || '';
                const mechanicEmail = card.querySelector('.customer-email')?.textContent.toLowerCase() || '';
                const mechanicPhone = card.querySelector('.ri-phone-line')?.parentElement?.textContent.toLowerCase() || '';

                const isVisible = mechanicName.includes(searchTerm) ||
                    mechanicEmail.includes(searchTerm) ||
                    mechanicPhone.includes(searchTerm);

                card.style.display = isVisible ? 'block' : 'none';
            });
        });
    }

    // Auto-hide notifications
    const notifications = document.querySelectorAll('.notification.show');
    notifications.forEach(notification => {
        setTimeout(() => {
            notification.classList.remove('show');
        }, 5000);
    });
});

// View mechanic details
function viewMechanic(mechanicId) {
    if (!mechanicId) {
        showNotification('Invalid mechanic ID', 'error');
        return;
    }

    // Navigate to mechanic details page
    window.location.href = `/Mechanic/MechanicDetails/${mechanicId}`;
}

// Edit mechanic
function editMechanic(mechanicId) {
    if (!mechanicId) {
        showNotification('Invalid mechanic ID', 'error');
        return;
    }

    // Navigate to edit page
    window.location.href = `/Mechanic/Edit/${mechanicId}`;
}

// Delete mechanic
function deleteMechanic(mechanicId) {
    if (!mechanicId) {
        showNotification('Invalid mechanic ID', 'error');
        return;
    }

    currentMechanicId = mechanicId;

    // Show delete confirmation modal
    const modal = document.getElementById('deleteModal');
    if (modal) {
        modal.style.display = 'block';

        // Set up confirm button
        const confirmBtn = document.getElementById('confirmDeleteBtn');
        if (confirmBtn) {
            confirmBtn.onclick = confirmDelete;
        }
    }
}

// Confirm delete action
async function confirmDelete() {
    if (!currentMechanicId) {
        showNotification('No mechanic selected for deletion', 'error');
        return;
    }

    try {
        // Get anti-forgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        if (!token) {
            showNotification('Security token not found. Please refresh the page.', 'error');
            return;
        }

        // Create form data
        const formData = new FormData();
        formData.append('__RequestVerificationToken', token);

        // Show loading state
        const confirmBtn = document.getElementById('confirmDeleteBtn');
        const originalText = confirmBtn.innerHTML;
        confirmBtn.innerHTML = '<i class="ri-loader-line"></i> Deleting...';
        confirmBtn.disabled = true;

        // Send delete request
        const response = await fetch(`/Mechanic/Delete/${currentMechanicId}`, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            // Success - reload page to show updated list
            showNotification('Mechanic deleted successfully', 'success');
            setTimeout(() => {
                window.location.reload();
            }, 1000);
        } else {
            // Error
            const errorText = await response.text();
            showNotification('Failed to delete mechanic: ' + errorText, 'error');

            // Reset button
            confirmBtn.innerHTML = originalText;
            confirmBtn.disabled = false;
        }

    } catch (error) {
        console.error('Delete error:', error);
        showNotification('An error occurred while deleting the mechanic', 'error');

        // Reset button
        const confirmBtn = document.getElementById('confirmDeleteBtn');
        confirmBtn.innerHTML = originalText;
        confirmBtn.disabled = false;
    }

    // Close modal
    closeDeleteModal();
}

// Close delete modal
function closeDeleteModal() {
    const modal = document.getElementById('deleteModal');
    if (modal) {
        modal.style.display = 'none';
    }
    currentMechanicId = null;
}

// Show notification
function showNotification(message, type = 'success') {
    const notification = document.getElementById('notification');
    const messageSpan = document.getElementById('notification-message');
    const icon = notification.querySelector('i');

    if (notification && messageSpan) {
        messageSpan.textContent = message;

        // Update icon based on type
        icon.className = type === 'success' ? 'ri-check-line' :
            type === 'error' ? 'ri-close-circle-line' :
                'ri-information-line';

        // Update background color
        notification.style.background = type === 'success' ? 'var(--success-color)' :
            type === 'error' ? 'var(--error-color)' :
                'var(--info-color)';

        // Show notification
        notification.classList.add('show');

        // Auto-hide after 3 seconds
        setTimeout(() => {
            notification.classList.remove('show');
        }, 3000);
    }
}

// Close modal when clicking outside
window.onclick = function (event) {
    const modal = document.getElementById('deleteModal');
    if (event.target === modal) {
        closeDeleteModal();
    }
}

// Handle escape key to close modal
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeDeleteModal();
    }
});