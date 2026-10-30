// Edit Mechanic Page JavaScript
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('editMechanicForm');
    const submitBtn = document.getElementById('submitBtn');

    if (form && submitBtn) {
        // Form validation
        form.addEventListener('submit', function (e) {
            // Clear previous errors
            clearValidationErrors();

            // Validate form
            if (!validateEditMechanicForm()) {
                e.preventDefault();
                return false;
            }

            // Set loading state
            setLoadingState(submitBtn, true, 'Saving Changes...');
        });

        // Real-time validation
        setupRealTimeValidation();

        // Track changes
        trackFormChanges();
    }
});

// Custom form validation for edit mechanic
function validateEditMechanicForm() {
    let isValid = true;

    // Name validation
    const nameInput = document.querySelector('input[name="Name"]');
    if (!nameInput.value.trim()) {
        showFieldError(nameInput, 'Name is required');
        isValid = false;
    } else if (nameInput.value.trim().length < 2) {
        showFieldError(nameInput, 'Name must be at least 2 characters long');
        isValid = false;
    }

    // Phone validation (optional but if provided, should be valid)
    const phoneInput = document.querySelector('input[name="PhoneNumber"]');
    if (phoneInput.value.trim()) {
        const phoneRegex = /^[\+]?[\d\s\-\(\)]{10,}$/;
        if (!phoneRegex.test(phoneInput.value.replace(/\s/g, ''))) {
            showFieldError(phoneInput, 'Please enter a valid phone number');
            isValid = false;
        }
    }

    // Work hours validation
    const workHoursSelect = document.querySelector('select[name="WorkHours"]');
    if (!workHoursSelect.value) {
        showFieldError(workHoursSelect, 'Please select work hours');
        isValid = false;
    }

    // Status validation
    const statusSelect = document.querySelector('select[name="Status"]');
    if (!statusSelect.value) {
        showFieldError(statusSelect, 'Please select a status');
        isValid = false;
    }

    return isValid;
}

// Show field-specific error
function showFieldError(input, message) {
    const infoItem = input.closest('.info-item');
    if (infoItem) {
        infoItem.classList.add('error');

        // Remove existing error message
        const existingError = infoItem.querySelector('.validation-error');
        if (existingError) {
            existingError.remove();
        }

        // Add new error message
        const errorElement = document.createElement('div');
        errorElement.className = 'validation-error';
        errorElement.innerHTML = `
            <i class="ri-error-warning-line"></i>
            <span>${message}</span>
        `;

        infoItem.appendChild(errorElement);
    }
}

// Clear all validation errors
function clearValidationErrors() {
    const errorItems = document.querySelectorAll('.info-item.error');
    errorItems.forEach(item => {
        item.classList.remove('error');
        const errorElement = item.querySelector('.validation-error');
        if (errorElement) {
            errorElement.remove();
        }
    });
}

// Setup real-time validation
function setupRealTimeValidation() {
    // Name validation
    const nameInput = document.querySelector('input[name="Name"]');
    if (nameInput) {
        nameInput.addEventListener('blur', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            if (!this.value.trim()) {
                showFieldError(this, 'Name is required');
            } else if (this.value.trim().length < 2) {
                showFieldError(this, 'Name must be at least 2 characters long');
            }
        });
    }

    // Phone validation
    const phoneInput = document.querySelector('input[name="PhoneNumber"]');
    if (phoneInput) {
        phoneInput.addEventListener('blur', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            if (this.value.trim()) {
                const phoneRegex = /^[\+]?[\d\s\-\(\)]{10,}$/;
                if (!phoneRegex.test(this.value.replace(/\s/g, ''))) {
                    showFieldError(this, 'Please enter a valid phone number');
                }
            }
        });

        // Format phone number as user types
        phoneInput.addEventListener('input', function () {
            formatPhoneNumber(this);
        });
    }

    // Work hours validation
    const workHoursSelect = document.querySelector('select[name="WorkHours"]');
    if (workHoursSelect) {
        workHoursSelect.addEventListener('change', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            if (!this.value) {
                showFieldError(this, 'Please select work hours');
            }
        });
    }

    // Status validation
    const statusSelect = document.querySelector('select[name="Status"]');
    if (statusSelect) {
        statusSelect.addEventListener('change', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            if (!this.value) {
                showFieldError(this, 'Please select a status');
            }
        });
    }
}

// Track form changes
function trackFormChanges() {
    const form = document.getElementById('editMechanicForm');
    if (!form) return;

    // Store original values
    const originalValues = {};
    const inputs = form.querySelectorAll('input[type="text"], input[type="email"], input[type="tel"], select');

    inputs.forEach(input => {
        if (input.name && input.name !== '__RequestVerificationToken') {
            originalValues[input.name] = input.value;
        }
    });

    // Track changes
    let hasChanges = false;

    function checkForChanges() {
        hasChanges = false;
        inputs.forEach(input => {
            if (input.name && input.name !== '__RequestVerificationToken') {
                if (originalValues[input.name] !== input.value) {
                    hasChanges = true;
                }
            }
        });

        // Update submit button state
        const submitBtn = document.getElementById('submitBtn');
        if (submitBtn) {
            if (hasChanges) {
                submitBtn.classList.add('has-changes');
                submitBtn.innerHTML = '<i class="ri-save-line"></i> Save Changes';
            } else {
                submitBtn.classList.remove('has-changes');
                submitBtn.innerHTML = '<i class="ri-save-line"></i> No Changes';
            }
        }
    }

    // Add change listeners
    inputs.forEach(input => {
        input.addEventListener('input', checkForChanges);
        input.addEventListener('change', checkForChanges);
    });

    // Warn before leaving with unsaved changes
    window.addEventListener('beforeunload', function (e) {
        if (hasChanges) {
            const confirmationMessage = 'You have unsaved changes. Are you sure you want to leave?';
            e.returnValue = confirmationMessage;
            return confirmationMessage;
        }
    });

    // Remove warning when form is submitted
    form.addEventListener('submit', function () {
        window.removeEventListener('beforeunload', arguments.callee);
    });
}

// Format phone number as user types
function formatPhoneNumber(input) {
    let value = input.value.replace(/\D/g, '');
    let formattedValue = '';

    if (value.length > 0) {
        if (value.length <= 3) {
            formattedValue = value;
        } else if (value.length <= 6) {
            formattedValue = `${value.slice(0, 3)}-${value.slice(3)}`;
        } else if (value.length <= 10) {
            formattedValue = `${value.slice(0, 3)}-${value.slice(3, 6)}-${value.slice(6)}`;
        } else {
            formattedValue = `${value.slice(0, 3)}-${value.slice(3, 6)}-${value.slice(6, 10)}`;
        }
    }

    input.value = formattedValue;
}

// Show confirmation before discarding changes
function confirmDiscardChanges(callback) {
    const form = document.getElementById('editMechanicForm');
    if (!form) return callback();

    const inputs = form.querySelectorAll('input[type="text"], input[type="email"], input[type="tel"], select');
    let hasChanges = false;

    // Check if there are changes (simplified check)
    inputs.forEach(input => {
        if (input.name && input.dataset.originalValue && input.dataset.originalValue !== input.value) {
            hasChanges = true;
        }
    });

    if (hasChanges) {
        showConfirmDialog(
            'Discard Changes?',
            'You have unsaved changes. Are you sure you want to leave without saving?',
            callback,
            () => { } // Do nothing on cancel
        );
    } else {
        callback();
    }
}

// Override navigation links to check for changes
document.addEventListener('DOMContentLoaded', function () {
    const navigationLinks = document.querySelectorAll('a[href*="/Mechanic"]');
    navigationLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const href = this.href;
            confirmDiscardChanges(() => {
                window.location.href = href;
            });
        });
    });
});