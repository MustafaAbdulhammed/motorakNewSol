function confirmDelete() {
    return confirm('Are you absolutely sure you want to delete this car? This action cannot be undone and will permanently remove all associated data.');
}

document.addEventListener('DOMContentLoaded', function () {
    // Add keyboard shortcuts
    document.addEventListener('keydown', function (e) {
        // ESC key to cancel - navigate to the cars index page
        if (e.key === 'Escape') {
            window.location.href = '/Car/Index'; // Fixed: Use direct URL instead of Razor syntax
        }
    });

    // Add double-click protection on delete button
    const deleteButton = document.querySelector('.btn-danger');
    let deleteClicked = false;

    if (deleteButton) { // Check if button exists
        deleteButton.addEventListener('click', function (e) {
            if (deleteClicked) {
                e.preventDefault();
                return false;
            }

            if (confirmDelete()) {
                deleteClicked = true;
                this.innerHTML = '<i class="ri-loader-4-line"></i> Deleting...';
                // Don't disable the button immediately - let the form submit first

                // Find the form and submit it
                const form = this.closest('form');
                if (form) {
                    // Disable the button after a short delay to allow form submission
                    setTimeout(() => {
                        this.disabled = true;
                    }, 100);
                    return true; // Allow form submission
                }
            } else {
                e.preventDefault();
                return false;
            }
        });
    }
});