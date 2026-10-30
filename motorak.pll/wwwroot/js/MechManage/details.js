// Mechanic Details Page JavaScript
let currentMechanicId = null;

document.addEventListener('DOMContentLoaded', function () {
    // Auto-hide notifications
    const notifications = document.querySelectorAll('.notification.show');
    notifications.forEach(notification => {
        setTimeout(() => {
            notification.classList.remove('show');
        }, 5000);
    });
});

// Delete mechanic from details page
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
            // Success - redirect to mechanic list
            showNotification('Mechanic deleted successfully', 'success');
            setTimeout(() => {
                window.location.href = '/Mechanic/Index';
            }, 1500);
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