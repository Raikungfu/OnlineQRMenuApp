const toggleMenuOpenButton = document.getElementById('toggle-menu-open');
const toggleMenuCloseButton = document.getElementById('toggle-menu-close');
const sidebar = document.querySelector('.sidebar');
const mainNavMenu = document.querySelector('.main-nav-menu');

function toggleMenu() {
    if (sidebar && mainNavMenu) {
        sidebar.classList.toggle('show');
        mainNavMenu.classList.toggle('expanded');
    }
}

if (toggleMenuOpenButton) {
    toggleMenuOpenButton.addEventListener('click', toggleMenu);
}

if (toggleMenuCloseButton) {
    toggleMenuCloseButton.addEventListener('click', toggleMenu);
}

document.addEventListener('DOMContentLoaded', function () {
    const mobileNav = document.getElementById('main-mobile-nav');
    const navToggle = document.getElementById('nav-toggle');
    const mobileNavClose = document.getElementById('mobile-nav-close');

    // Toggle the mobile navigation menu
    navToggle.addEventListener('click', function () {
        mobileNav.classList.toggle('hidden');
    });

    // Close the mobile navigation menu
    mobileNavClose.addEventListener('click', function () {
        mobileNav.classList.add('hidden');
    });
});