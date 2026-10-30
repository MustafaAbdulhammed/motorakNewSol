// Create Mechanic Page JavaScript
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('createMechanicForm');
    const submitBtn = document.getElementById('submitBtn');

    if (form && submitBtn) {
        // Form validation
        form.addEventListener('submit', function (e) {
            // Clear previous errors
            clearValidationErrors();

            // Validate form
            if (!validateCreateMechanicForm()) {
                e.preventDefault();
                return false;
            }

            // Set loading state
            setLoadingState(submitBtn, true, 'Creating Mechanic...');
        });

        // Real-time validation
        setupRealTimeValidation();
    }
});

// Custom form validation for create mechanic
function validateCreateMechanicForm() {
    let isValid = true;
    const errors = [];

    // Name validation
    const nameInput = document.querySelector('input[name="Name"]');
    if (!nameInput.value.trim()) {
        showFieldError(nameInput, 'Name is required');
        isValid = false;
    } else if (nameInput.value.trim().length < 2) {
        showFieldError(nameInput, 'Name must be at least 2 characters long');
        isValid = false;
    }

    // Email validation
    const emailInput = document.querySelector('input[name="Email"]');
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailInput.value.trim()) {
        showFieldError(emailInput, 'Email is required');
        isValid = false;
    } else if (!emailRegex.test(emailInput.value)) {
        showFieldError(emailInput, 'Please enter a valid email address');
        isValid = false;
    } else if (emailInput.value.length < 8) {
        showFieldError(emailInput, 'Email must be at least 8 characters long');
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

    // Password validation
    const passwordInput = document.querySelector('input[name="Password"]');
    if (!passwordInput.value) {
        showFieldError(passwordInput, 'Password is required');
        isValid = false;
    } else if (passwordInput.value.length < 6) {
        showFieldError(passwordInput, 'Password must be at least 6 characters long');
        isValid = false;
    }

    // Confirm password validation
    const confirmPasswordInput = document.querySelector('input[name="ConfirmPassword"]');
    if (confirmPasswordInput && passwordInput) {
        confirmPasswordInput.addEventListener('blur', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            if (!this.value) {
                showFieldError(this, 'Please confirm your password');
            } else if (this.value !== passwordInput.value) {
                showFieldError(this, 'Passwords do not match');
            }
        });

        // Also validate when password changes
        passwordInput.addEventListener('input', function () {
            if (confirmPasswordInput.value && confirmPasswordInput.value !== this.value) {
                showFieldError(confirmPasswordInput, 'Passwords do not match');
            } else {
                const confirmInfoItem = confirmPasswordInput.closest('.info-item');
                confirmInfoItem.classList.remove('error');
                const existingError = confirmInfoItem.querySelector('.validation-error');
                if (existingError) existingError.remove();
            }
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
}

// Password strength checker
function getPasswordStrength(password) {
    let score = 0;
    let feedback = [];

    if (password.length >= 8) score += 1;
    else feedback.push('At least 8 characters');

    if (/[a-z]/.test(password)) score += 1;
    else feedback.push('Lowercase letter');

    if (/[A-Z]/.test(password)) score += 1;
    else feedback.push('Uppercase letter');

    if (/\d/.test(password)) score += 1;
    else feedback.push('Number');

    if (/[^a-zA-Z\d]/.test(password)) score += 1;
    else feedback.push('Special character');

    const levels = {
        0: { level: 'weak', text: 'Very Weak', percent: 20 },
        1: { level: 'weak', text: 'Weak', percent: 25 },
        2: { level: 'fair', text: 'Fair', percent: 50 },
        3: { level: 'good', text: 'Good', percent: 75 },
        4: { level: 'strong', text: 'Strong', percent: 90 },
        5: { level: 'strong', text: 'Very Strong', percent: 100 }
    };

    return levels[score] || levels[0];
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

// Initialize phone formatting
document.addEventListener('DOMContentLoaded', function () {
    const phoneInput = document.querySelector('input[name="PhoneNumber"]');
    if (phoneInput) {
        phoneInput.addEventListener('input', function () {
            formatPhoneNumber(this);
        });
    }
}); querySelector('input[name="ConfirmPassword"]');
if (!confirmPasswordInput.value) {
    showFieldError(confirmPasswordInput, 'Please confirm your password');
    isValid = false;
} else if (confirmPasswordInput.value !== passwordInput.value) {
    showFieldError(confirmPasswordInput, 'Passwords do not match');
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

    // Email validation
    const emailInput = document.querySelector('input[name="Email"]');
    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!this.value.trim()) {
                showFieldError(this, 'Email is required');
            } else if (!emailRegex.test(this.value)) {
                showFieldError(this, 'Please enter a valid email address');
            } else if (this.value.length < 8) {
                showFieldError(this, 'Email must be at least 8 characters long');
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
    }

    // Password strength indicator
    const passwordInput = document.querySelector('input[name="Password"]');
    if (passwordInput) {
        passwordInput.addEventListener('input', function () {
            const infoItem = this.closest('.info-item');
            const strength = getPasswordStrength(this.value);

            // Remove existing strength indicator
            const existingIndicator = infoItem.querySelector('.password-strength');
            if (existingIndicator) {
                existingIndicator.remove();
            }

            if (this.value.length > 0) {
                const strengthIndicator = document.createElement('div');
                strengthIndicator.className = 'password-strength';
                strengthIndicator.innerHTML = `
                    <div class="strength-bar">
                        <div class="strength-fill strength-${strength.level}" style="width: ${strength.percent}%"></div>
                    </div>
                    <span class="strength-text">${strength.text}</span>
                `;
                infoItem.appendChild(strengthIndicator);
            }
        });

        passwordInput.addEventListener('blur', function () {
            const infoItem = this.closest('.info-item');
            infoItem.classList.remove('error');
            const existingError = infoItem.querySelector('.validation-error');
            if (existingError) existingError.remove();

            if (!this.value) {
                showFieldError(this, 'Password is required');
            } else if (this.value.length < 6) {
                showFieldError(this, 'Password must be at least 6 characters long');
            }
        });
    }

    // Confirm password validation
    const confirmPasswordInput = document.