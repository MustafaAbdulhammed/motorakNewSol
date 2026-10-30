document.addEventListener('DOMContentLoaded', function () {
    // Debug: Log current model values
    console.log('Current Model Values:');
    console.log('ID:', '@Model.Id');
    console.log('Brand:', '@Model.Brand');
    console.log('Model:', '@Model.Model');
    console.log('Year:', '@Model.Year');
    console.log('Status:', '@Model.Status');
    console.log('Category:', '@Model.Category');

    // Add focus/blur effects for form inputs
    const inputs = document.querySelectorAll('.form-control, .form-select');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.style.transform = 'scale(1.02)';
        });

        input.addEventListener('blur', function () {
            this.style.transform = 'scale(1)';
        });
    });

    // Image preview functionality
    const imageInput = document.getElementById('imageFile');
    if (imageInput) {
        imageInput.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                console.log('New image selected:', file.name);
                // You can add image preview functionality here
            }
        });
    }

    // Form validation feedback
    const form = document.querySelector('form');
    form.addEventListener('submit', function (e) {
        console.log('Form submitted, checking validation...');

        const requiredFields = form.querySelectorAll('[required]');
        let allValid = true;

        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                allValid = false;
                field.style.borderColor = '#dc3545';
                console.log('Invalid field:', field.name || field.id);
            } else {
                field.style.borderColor = '#f59e0b';
                console.log('Valid field:', field.name || field.id, '=', field.value);
            }
        });

        if (!allValid) {
            e.preventDefault();
            console.log('Form validation failed, preventing submission');
            alert('Please fill in all required fields');
        } else {
            console.log('Form validation passed, submitting...');
        }
    });

    // Debug: Log form data before submission
    form.addEventListener('submit', function (e) {
        const formData = new FormData(form);
        console.log('Form Data being submitted:');
        for (let [key, value] of formData.entries()) {
            console.log(key + ':', value);
        }
    });
});