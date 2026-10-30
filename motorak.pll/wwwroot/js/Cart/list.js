// Updated Cart JavaScript with real backend integration

async function updateQuantity(itemId, action, value = null) {
    const quantityInput = document.querySelector(`[data-item-id="${itemId}"] .quantity-input`);
    let currentQuantity = parseInt(quantityInput.value);
    let newQuantity = currentQuantity;

    switch (action) {
        case 'increase':
            newQuantity = currentQuantity + 1;
            break;
        case 'decrease':
            newQuantity = Math.max(1, currentQuantity - 1);
            break;
        case 'set':
            newQuantity = Math.max(1, parseInt(value) || 1);
            break;
    }

    quantityInput.value = newQuantity;

    // Add visual feedback
    quantityInput.style.background = 'rgba(66, 133, 244, 0.1)';
    setTimeout(() => {
        quantityInput.style.background = '';
    }, 300);

    // Auto-update after quantity change
    if (action !== 'set') {
        await updateCartItem(itemId);
    }
}

async function updateDates(itemId) {
    try {
        const itemElement = document.querySelector(`[data-item-id="${itemId}"]`);
        const startDate = itemElement.querySelector('.start-date').value;
        const endDate = itemElement.querySelector('.end-date').value;

        // Client-side validation
        if (new Date(startDate) >= new Date(endDate)) {
            showNotification('End date must be after start date', 'error');
            return;
        }

        if (new Date(startDate) < new Date().toISOString().split('T')[0]) {
            showNotification('Start date cannot be in the past', 'error');
            return;
        }

        // Get CSRF token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const response = await fetch('/Cart/UpdateDates', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                itemId: parseInt(itemId),
                startDate: startDate,
                endDate: endDate
            })
        });

        const result = await response.json();

        if (result.success) {
            showNotification(result.message, 'success');

            // Add visual feedback
            const dateInputs = itemElement.querySelectorAll('.date-inputs input');
            dateInputs.forEach(input => {
                input.style.borderColor = '#4285f4';
                input.style.background = 'rgba(66, 133, 244, 0.05)';
            });

            setTimeout(() => {
                dateInputs.forEach(input => {
                    input.style.borderColor = '';
                    input.style.background = '';
                });
            }, 500);
        } else {
            showNotification(result.message || 'Failed to update dates', 'error');
        }
    } catch (error) {
        console.error('Error updating dates:', error);
        showNotification('Failed to update dates', 'error');
    }
}

async function updateCartItem(itemId) {
    try {
        const itemElement = document.querySelector(`[data-item-id="${itemId}"]`);
        const updateBtn = itemElement.querySelector('.update-btn');
        const quantityInput = itemElement.querySelector('.quantity-input');

        // Show loading state
        const originalContent = updateBtn.innerHTML;
        updateBtn.innerHTML = '<i class="ri-loader-4-line me-1" style="animation: spin 1s linear infinite;"></i>Updating...';
        updateBtn.disabled = true;

        // Get CSRF token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const response = await fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                itemId: parseInt(itemId),
                quantity: parseInt(quantityInput.value)
            })
        });

        const result = await response.json();

        if (result.success) {
            showNotification(result.message, 'success');

            // Update totals in the UI
            if (result.cartTotal) {
                document.getElementById('items-subtotal').textContent = result.cartTotal;
            }
            if (result.taxAmount) {
                document.getElementById('tax-amount').textContent = result.taxAmount;
            }
            if (result.finalTotal) {
                document.getElementById('total-amount').innerHTML = `<strong>${result.finalTotal}</strong>`;
            }
        } else {
            showNotification(result.message || 'Failed to update item', 'error');
            // Revert quantity on error
            location.reload();
        }

        // Restore button state
        updateBtn.innerHTML = originalContent;
        updateBtn.disabled = false;

    } catch (error) {
        console.error('Error updating cart item:', error);
        showNotification('Failed to update item', 'error');

        // Restore button and reload on error
        const updateBtn = document.querySelector(`[data-item-id="${itemId}"] .update-btn`);
        updateBtn.innerHTML = '<i class="ri-refresh-line me-1"></i>Update';
        updateBtn.disabled = false;
    }
}

async function removeFromCart(itemId) {
    if (!confirm('Are you sure you want to remove this item from cart?')) {
        return;
    }

    try {
        const itemElement = document.querySelector(`[data-item-id="${itemId}"]`);

        // Get CSRF token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const response = await fetch('/Cart/RemoveItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                itemId: parseInt(itemId)
            })
        });

        const result = await response.json();

        if (result.success) {
            // Animate removal
            itemElement.style.animation = 'slideOut 0.3s ease-out';

            setTimeout(() => {
                itemElement.remove();
                showNotification(result.message, 'success');

                // Check if cart is empty
                if (result.cartEmpty) {
                    showEmptyCart();
                } else {
                    // Update totals
                    if (result.cartTotal) {
                        document.getElementById('items-subtotal').textContent = result.cartTotal;
                    }
                    if (result.taxAmount) {
                        document.getElementById('tax-amount').textContent = result.taxAmount;
                    }
                    if (result.finalTotal) {
                        document.getElementById('total-amount').innerHTML = `<strong>${result.finalTotal}</strong>`;
                    }
                    if (result.itemCount !== undefined) {
                        updateCartCounter(result.itemCount);
                    }
                }
            }, 300);
        } else {
            showNotification(result.message || 'Failed to remove item', 'error');
        }
    } catch (error) {
        console.error('Error removing item:', error);
        showNotification('Failed to remove item', 'error');
    }
}

async function proceedToCheckout() {
    try {
        const btn = document.querySelector('.checkout-btn');
        const originalContent = btn.innerHTML;

        btn.innerHTML = '<i class="ri-loader-4-line me-2" style="animation: spin 1s linear infinite;"></i>Processing...';
        btn.disabled = true;

        // Get CSRF token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Cart/Checkout';

        // Add CSRF token
        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = token;
        form.appendChild(tokenInput);

        document.body.appendChild(form);
        form.submit();

    } catch (error) {
        console.error('Error during checkout:', error);
        showNotification('Failed to proceed to checkout', 'error');

        // Restore button
        const btn = document.querySelector('.checkout-btn');
        btn.innerHTML = '<i class="ri-secure-payment-line me-2"></i>Proceed to Checkout';
        btn.disabled = false;
    }
}

async function clearCart() {
    if (!confirm('Are you sure you want to clear all items from cart?')) {
        return;
    }

    try {
        const btn = document.querySelector('.clear-cart-btn');
        const originalContent = btn.innerHTML;

        btn.innerHTML = '<i class="ri-loader-4-line me-2" style="animation: spin 1s linear infinite;"></i>Clearing...';
        btn.disabled = true;

        // Get CSRF token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const response = await fetch('/Cart/ClearCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        });

        if (response.ok) {
            showNotification('Cart cleared successfully', 'success');
            setTimeout(() => {
                location.reload();
            }, 1000);
        } else {
            throw new Error('Failed to clear cart');
        }

    } catch (error) {
        console.error('Error clearing cart:', error);
        showNotification('Failed to clear cart', 'error');

        // Restore button
        const btn = document.querySelector('.clear-cart-btn');
        btn.innerHTML = '<i class="ri-delete-bin-line me-2"></i>Clear Cart';
        btn.disabled = false;
    }
}

function showEmptyCart() {
    const cartItemsContainer = document.getElementById('cart-items-container');
    const emptyCartElement = document.getElementById('empty-cart');

    if (cartItemsContainer) {
        cartItemsContainer.style.display = 'none';
    }
    if (emptyCartElement) {
        emptyCartElement.style.display = 'block';
    }
}

function updateCartCounter(count) {
    // Update cart counter in navbar if it exists
    const cartCounter = document.querySelector('.cart-counter');
    if (cartCounter) {
        cartCounter.textContent = count;
        if (count === 0) {
            cartCounter.style.display = 'none';
        } else {
            cartCounter.style.display = 'inline-block';
        }
    }
}

function showNotification(message, type = 'success') {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.cart-notification');
    existingNotifications.forEach(notification => notification.remove());

    const notification = document.createElement('div');
    notification.className = 'cart-notification';
    notification.innerHTML = `
        <div style="
            position: fixed; 
            top: 20px; 
            right: 20px; 
            background: ${type === 'error' ? '#dc3545' : '#28a745'}; 
            color: white; 
            padding: 1rem 1.5rem; 
            border-radius: 0.5rem; 
            z-index: 9999;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            animation: slideInRight 0.3s ease-out;
            max-width: 350px;
            word-wrap: break-word;
        ">
            <i class="ri-${type === 'error' ? 'close' : 'check'}-circle-line me-2"></i>
            ${message}
        </div>
    `;

    // Add CSS animation if not already present
    if (!document.querySelector('#cart-notification-styles')) {
        const style = document.createElement('style');
        style.id = 'cart-notification-styles';
        style.textContent = `
            @keyframes slideInRight {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
            @keyframes slideOutRight {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(100%);
                    opacity: 0;
                }
            }
            @keyframes slideOut {
                from {
                    opacity: 1;
                    transform: translateX(0);
                }
                to {
                    opacity: 0;
                    transform: translateX(-100%);
                }
            }
            @keyframes spin {
                from {
                    transform: rotate(0deg);
                }
                to {
                    transform: rotate(360deg);
                }
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(notification);

    // Auto-remove after 4 seconds
    setTimeout(() => {
        if (notification && notification.parentNode) {
            notification.firstElementChild.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => {
                notification.remove();
            }, 300);
        }
    }, 4000);
}

// Add to cart function (for use from other pages)
async function addToCart(carId, itemType, count = 1, startDate = null, endDate = null) {
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const requestData = {
            carId: parseInt(carId),
            itemType: itemType,
            count: parseInt(count)
        };

        // Add rental dates if provided
        if (startDate) requestData.rentStartDate = startDate;
        if (endDate) requestData.rentEndDate = endDate;

        const response = await fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(requestData)
        });

        const result = await response.json();

        if (result.success) {
            showNotification(result.message, 'success');

            // Update cart counter
            await updateCartCount();
        } else {
            showNotification(result.message || 'Failed to add item to cart', 'error');
        }

        return result.success;
    } catch (error) {
        console.error('Error adding to cart:', error);
        showNotification('Failed to add item to cart', 'error');
        return false;
    }
}

// Update cart count in navbar
async function updateCartCount() {
    try {
        const response = await fetch('/Cart/GetCartCount');
        const result = await response.json();
        updateCartCounter(result.count || 0);
    } catch (error) {
        console.error('Error updating cart count:', error);
    }
}

// Initialize cart functionality when page loads
document.addEventListener('DOMContentLoaded', function () {
    // Update cart count on page load
    updateCartCount();

    // Auto-validate rental dates when they change
    const dateInputs = document.querySelectorAll('.start-date, .end-date');
    dateInputs.forEach(input => {
        input.addEventListener('change', function () {
            const itemElement = this.closest('[data-item-id]');
            const itemId = itemElement.getAttribute('data-item-id');
            updateDates(itemId);
        });
    });

    // Auto-update quantity when input changes (with debounce)
    const quantityInputs = document.querySelectorAll('.quantity-input');
    quantityInputs.forEach(input => {
        let timeout;
        input.addEventListener('input', function () {
            const itemElement = this.closest('[data-item-id]');
            const itemId = itemElement.getAttribute('data-item-id');

            clearTimeout(timeout);
            timeout = setTimeout(() => {
                updateCartItem(itemId);
            }, 500); // Wait 500ms after user stops typing
        });
    });
});