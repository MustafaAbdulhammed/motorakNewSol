document.getElementById('createCustomerForm').addEventListener('submit', function () {
    const submitBtn = document.getElementById('submitBtn');
    submitBtn.innerHTML = '<i class="ri-loader-4-line"></i> Creating...';
    submitBtn.disabled = true;

    setTimeout(() => {
        submitBtn.innerHTML = '<i class="ri-check-line"></i> Create Customer';
        submitBtn.disabled = false;
    }, 3000);
});

// Real-time validation feedback
document.querySelectorAll('.form-control').forEach(input => {
    input.addEventListener('blur', function () {
        const infoItem = this.closest('.info-item');
        const hasError = infoItem.classList.contains('error');

        if (this.value.trim() && hasError) {
            infoItem.classList.remove('error');
            const errorMsg = infoItem.querySelector('.validation-error');
            if (errorMsg) errorMsg.style.display = 'none';
        }
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
