// list.js

const cars = [/* same sample data */];
const carGrid = document.getElementById('carGrid');

// Load cars into grid
function loadCars() {
    carGrid.innerHTML = '';
    cars.forEach(car => {
        const card = document.createElement('article');
        card.className = `featured__card car-card ${car.status !== 'available' ? 'unavailable' : ''}`;
        card.dataset.category = car.type;
        card.dataset.status = car.status;

        card.innerHTML = `
            <div class="car-header">
                <h1 class="featured__title">${car.make}</h1>
                <h3 class="featured__subtitle">${car.model} (${car.year})</h3>
            </div>
            <div class="car-image-container">
                <img src="${car.image}" alt="${car.make} ${car.model}">
                ${car.status !== 'available' ? `<div class="status-badge">${car.status}</div>` : ''}
            </div>
            <div class="car-pricing">
                <h3 class="featured__price">$${car.price.toLocaleString()}</h3>
                ${car.type === 'forrent' ? `<small>Rent: $${car.rentalPrice}/day</small>` : ''}
            </div>
            <div class="car-actions">
                <button class="button" onclick="showCarDetails(${car.id})"><i class='ri-eye-line'></i> Details</button>
                <button class="button edit-btn" onclick="editCar(${car.id})"><i class='ri-edit-line'></i> Edit</button>
                <button class="button delete-btn" onclick="confirmDelete(${car.id})"><i class='ri-delete-bin-line'></i> Delete</button>
            </div>
        `;
        carGrid.appendChild(card);
    });
}

// Filter cars
function filterCars(filter) {
    document.querySelectorAll('.car-card').forEach(card => {
        if (filter === 'all') card.style.display = 'block';
        else if (filter === 'available') card.style.display = card.dataset.status === 'available' ? 'block' : 'none';
        else card.style.display = card.dataset.category === filter ? 'block' : 'none';
    });
}

// Show car details
function showCarDetails(carId) {
    const car = cars.find(c => c.id === carId);
    if (!car) return;

    document.getElementById('car-details').innerHTML = `
        <div class="car-detail-card">
            <h2>${car.make} ${car.model} (${car.year})</h2>
            <p>Price: $${car.price.toLocaleString()}</p>
            ${car.type === 'forrent' ? `<p>Rent: $${car.rentalPrice}/day</p>` : ''}
            <p>Status: ${car.status}</p>
        </div>
    `;
    showView('details');
}
