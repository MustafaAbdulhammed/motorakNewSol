document.addEventListener('DOMContentLoaded', function () {

    const inputs = document.querySelectorAll('.form-control, .form-select');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.style.transform = 'scale(1.02)';
        });

        input.addEventListener('blur', function () {
            this.style.transform = 'scale(1)';
        });
    });

    const imageInput = document.getElementById('imageFile');
    if (imageInput) {
        imageInput.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                console.log('Image selected:', file.name);
            }
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('createCarForm');

    form.addEventListener('submit', function (event) {
        // Simple client-side validation
        let isValid = true;
        const errors = [];

        // Clear previous errors
        document.querySelectorAll('.text-danger').forEach(el => el.textContent = '');
        document.getElementById('validationSummary').style.display = 'none';
        document.getElementById('validationErrors').innerHTML = '';

        // Validate required fields
        const brand = document.getElementById('Brand').value.trim();
        const model = document.getElementById('Model').value.trim();
        const year = document.getElementById('Year').value;
        const condition = document.getElementById('Condition').value;

        if (!brand) {
            isValid = false;
            document.querySelector('[data-valmsg-for="Brand"]').textContent = 'Brand is required.';
            errors.push('Brand is required.');
        }

        if (!model) {
            isValid = false;
            document.querySelector('[data-valmsg-for="Model"]').textContent = 'Model is required.';
            errors.push('Model is required.');
        }

        if (!year) {
            isValid = false;
            document.querySelector('[data-valmsg-for="Year"]').textContent = 'Year is required.';
            errors.push('Year is required.');
        } else if (year < 1900 || year > 2030) {
            isValid = false;
            document.querySelector('[data-valmsg-for="Year"]').textContent = 'Year must be between 1900 and 2030.';
            errors.push('Year must be between 1900 and 2030.');
        }

        if (!condition) {
            isValid = false;
            document.querySelector('[data-valmsg-for="Condition"]').textContent = 'Condition is required.';
            errors.push('Condition is required.');
        }

        if (!isValid) {
            event.preventDefault();
            const validationSummary = document.getElementById('validationSummary');
            const validationErrors = document.getElementById('validationErrors');

            validationSummary.style.display = 'block';
            errors.forEach(error => {
                const li = document.createElement('li');
                li.textContent = error;
                validationErrors.appendChild(li);
            });

            // Scroll to validation summary
            validationSummary.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    });
});