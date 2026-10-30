// Show notification
function showNotification(message, type = 'success') {
    const notification = document.getElementById('notification');
    const messageEl = document.getElementById('notification-message');
    const icon = notification.querySelector('i');

    messageEl.textContent = message;

    switch (type) {
        case 'error':
            notification.style.background = 'var(--error-color)';
            icon.className = 'ri-close-circle-line';
            break;
        case 'warning':
            notification.style.background = 'var(--warning-color)';
            icon.className = 'ri-error-warning-line';
            break;
        case 'info':
            notification.style.background = 'var(--info-color)';
            icon.className = 'ri-information-line';
            break;
        default:
            notification.style.background = 'var(--success-color)';
            icon.className = 'ri-check-line';
    }

    notification.classList.add('show');
    setTimeout(() => notification.classList.remove('show'), 4000);
}

// Close modal when clicking outside
window.addEventListener('click', function (event) {
    const modal = document.getElementById('deleteModal');
    if (event.target === modal) {
        closeDeleteModal();
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const notifications = document.querySelectorAll('.notification.show');
    notifications.forEach(notification => {
        setTimeout(() => notification.classList.remove('show'), 4000);
    });
});
