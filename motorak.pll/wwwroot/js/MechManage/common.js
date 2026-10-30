// Common JavaScript functions used across the application

// Global notification function
function showGlobalNotification(message, type = 'info', duration = 3000) {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.global-notification');
    existingNotifications.forEach(notification => notification.remove());

    // Create notification element
    const notification = document.createElement('div');
    notification.className = 'global-notification';
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 8px;
        color: white;
        font-weight: 500;
        z-index: 10000;
        transform: translateX(400px);
        transition: transform 0.3s ease;
        max-width: 400px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);
        display: flex;
        align-items: center;
        gap: 10px;
    `;

    // Set background color based on type
    const colors = {
        success: '#4CAF50',
        error: '#F44336',
        warning: '#FF9800',
        info: '#2196F3'
    };

    const icons = {
        success: 'ri-check-circle-line',
        error: 'ri-error-warning-line',
        warning: 'ri-alert-line',
        info: 'ri-information-line'
    };

    notification.style.backgroundColor = colors[type] || colors.info;

    // Add icon and message
    notification.innerHTML = `
        <i class="${icons[type] || icons.info}"></i>
        <span>${message}</span>
        <button onclick="this.parentElement.remove()" style="
            background: none;
            border: none;
            color: white;
            font-size: 18px;
            cursor: pointer;
            margin-left: auto;
            opacity: 0.8;
        ">×</button>
    `;

    // Add to document
    document.body.appendChild(notification);

    // Animate in
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);

    // Auto remove
    setTimeout(() => {
        notification.style.transform = 'translateX(400px)';
        setTimeout(() => {
            if (notification.parentElement) {
                notification.remove();
            }
        }, 300);
    }, duration);
}

// Form validation helper
function validateForm(formElement) {
    const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;
    let firstInvalidInput = null;

    inputs.forEach(input => {
        const value = input.value.trim();
        const isInputValid = value !== '';

        // Remove existing error styling
        input.classList.remove('error');
        const errorElement = input.parentElement.querySelector('.validation-error');

        if (!isInputValid) {
            isValid = false;
            input.classList.add('error');

            if (!firstInvalidInput) {
                firstInvalidInput = input;
            }

            // Add error message if not exists
            if (!errorElement) {
                const error = document.createElement('div');
                error.className = 'validation-error';
                error.innerHTML = `
                    <i class="ri-error-warning-line"></i>
                    <span>This field is required</span>
                `;
                input.parentElement.appendChild(error);
            }
        } else if (errorElement) {
            errorElement.remove();
        }
    });

    // Focus first invalid input
    if (firstInvalidInput) {
        firstInvalidInput.focus();
        firstInvalidInput.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }

    return isValid;
}

// Loading state helper
function setLoadingState(button, isLoading, loadingText = 'Loading...') {
    if (isLoading) {
        button.dataset.originalText = button.innerHTML;
        button.innerHTML = `<i class="ri-loader-line"></i> ${loadingText}`;
        button.disabled = true;
    } else {
        button.innerHTML = button.dataset.originalText || button.innerHTML;
        button.disabled = false;
    }
}

// Confirm dialog helper
function showConfirmDialog(title, message, onConfirm, onCancel = null) {
    // Remove existing dialogs
    const existingDialog = document.querySelector('.confirm-dialog');
    if (existingDialog) {
        existingDialog.remove();
    }

    // Create dialog
    const dialog = document.createElement('div');
    dialog.className = 'confirm-dialog';
    dialog.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.5);
        z-index: 10001;
        display: flex;
        align-items: center;
        justify-content: center;
        opacity: 0;
        transition: opacity 0.3s ease;
    `;

    dialog.innerHTML = `
        <div style="
            background: white;
            padding: 30px;
            border-radius: 12px;
            max-width: 400px;
            width: 90%;
            text-align: center;
            transform: scale(0.9);
            transition: transform 0.3s ease;
        ">
            <h3 style="margin: 0 0 15px 0; color: #333;">${title}</h3>
            <p style="margin: 0 0 25px 0; color: #666; line-height: 1.5;">${message}</p>
            <div style="display: flex; gap: 10px; justify-content: center;">
                <button class="btn-secondary cancel-btn" style="
                    padding: 10px 20px;
                    border: 1px solid #ddd;
                    background: #f5f5f5;
                    color: #333;
                    border-radius: 6px;
                    cursor: pointer;
                ">Cancel</button>
                <button class="btn-danger confirm-btn" style="
                    padding: 10px 20px;
                    border: none;
                    background: #F44336;
                    color: white;
                    border-radius: 6px;
                    cursor: pointer;
                ">Confirm</button>
            </div>
        </div>
    `;

    // Add event listeners
    const cancelBtn = dialog.querySelector('.cancel-btn');
    const confirmBtn = dialog.querySelector('.confirm-btn');

    const closeDialog = () => {
        dialog.style.opacity = '0';
        setTimeout(() => dialog.remove(), 300);
    };

    cancelBtn.onclick = () => {
        closeDialog();
        if (onCancel) onCancel();
    };

    confirmBtn.onclick = () => {
        closeDialog();
        onConfirm();
    };

    // Close on backdrop click
    dialog.onclick = (e) => {
        if (e.target === dialog) {
            closeDialog();
            if (onCancel) onCancel();
        }
    };

    // Add to document and show
    document.body.appendChild(dialog);
    setTimeout(() => {
        dialog.style.opacity = '1';
        dialog.querySelector('div').style.transform = 'scale(1)';
    }, 10);
}

// Format date helper
function formatDate(dateString, options = {}) {
    const defaultOptions = {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    };

    const finalOptions = { ...defaultOptions, ...options };
    return new Date(dateString).toLocaleDateString('en-US', finalOptions);
}

// Debounce helper
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Initialize common functionality
document.addEventListener('DOMContentLoaded', function () {
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert, .notification');
    alerts.forEach(alert => {
        if (alert.classList.contains('show')) {
            setTimeout(() => {
                alert.classList.remove('show');
                setTimeout(() => {
                    if (alert.parentElement) {
                        alert.remove();
                    }
                }, 300);
            }, 5000);
        }
    });

    // Add smooth scrolling to anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add loading states to forms
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function () {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                setLoadingState(submitBtn, true, 'Processing...');
            }
        });
    });
});