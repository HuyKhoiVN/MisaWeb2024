function toggleDropdown(containerId, dropdownId) {
    const dropdown = document.getElementById(dropdownId);
    const comboboxContainer = document.getElementById(containerId);
    dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
    if (dropdown.style.display === 'block') {
        comboboxContainer.classList.add('active');
    } else {
        comboboxContainer.classList.remove('active');
    }
}

function selectItem(item, containerId, inputId) {
    const comboboxInput = document.getElementById(inputId);
    comboboxInput.innerHTML = item.innerText + ' <i class="fas fa-chevron-down"></i>';
    
    const selectedItem = document.querySelector(`#${containerId} .dropdown-item.selected`);
    if (selectedItem) {
        selectedItem.classList.remove('selected');
    }
    
    item.classList.add('selected');
    
    comboboxInput.classList.add('selected');
    
    toggleDropdown(containerId, item.parentNode.id);
}

//no icon
function selectItemNoIcon(item, containerId, inputId) {
    const comboboxInput = document.getElementById(inputId);
    comboboxInput.innerHTML = item.innerText;
    
    const selectedItem = document.querySelector(`#${containerId} .dropdown-item.selected`);
    if (selectedItem) {
        selectedItem.classList.remove('selected');
    }
    
    item.classList.add('selected');
    
    comboboxInput.classList.add('selected');
    
    toggleDropdown(containerId, item.parentNode.id);
}

function filterItems(containerId, inputId) {
    const comboboxInput = document.getElementById(inputId);
    const filter = comboboxInput.value.toLowerCase();
    const items = document.querySelectorAll(`#${containerId} .dropdown-item`);
    items.forEach(item => {
        const text = item.innerText.toLowerCase();
        item.style.display = text.includes(filter) ? '' : 'none';
    });
}

// Close the dropdown if the user clicks outside of it
window.onclick = function(event) {
    const containers = document.querySelectorAll('.combobox-container');
    containers.forEach(container => {
        const dropdown = container.querySelector('.dropdown');
        if (dropdown && dropdown.style.display === 'block') {
            if (!container.contains(event.target)) {
                dropdown.style.display = 'none';
                container.classList.remove('active');
            }
        }
    });
}
