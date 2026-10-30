document.getElementById('editCustomerForm').addEventListener('submit', function () {
    const submitBtn = document.getElementById('submitBtn');
    submitBtn.innerHTML = '<i class="ri-loader-4-line"></i> Saving...';
    submitBtn.disabled = true;

    setTimeout(() => {
        submitBtn.innerHTML = '<i class="ri-save-line"></i> Save Changes';
        submitBtn.disabled = false;
    }, 3000);
});

// Validation + UX
document.querySelectorAll('.form-control:not(.readonly-field)').forEach(input => {
    input.addEventListener('blur', function () {
        const infoItem = this.closest('.info-item');
        const hasError = infoItem.classList.contains('error');

        if (this.value.trim() && hasError) {
            infoItem.classList.remove('error');
            const errorMsg = infoItem.querySelector('.validation-error');
            if (errorMsg) errorMsg.style.display = 'none';
        }
    });

    input.addEventListener('focus', function () {
        const infoItem = this.closest('.info-item');
        infoItem.style.transform = 'translateX(5px)';
        infoItem.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.1)';
    });

    input.addEventListener('blur', function () {
        const infoItem = this.closest('.info-item');
        infoItem.style.transform = 'translateX(0)';
        infoItem.style.boxShadow = 'none';
    });
});

// Prevent Enter key submission
document.querySelectorAll('.form-control').forEach(input => {
    input.addEventListener('keypress', function (e) {
        if (e.key === 'Enter' && e.target.type !== 'submit') {
            e.preventDefault();
        }
    });
});
