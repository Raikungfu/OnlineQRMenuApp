const toggleMenuOpenButton = document.getElementById('toggle-menu-open');
const toggleMenuCloseButton = document.getElementById('toggle-menu-close');
const toggleMenuButton = document.getElementById('toggle-menu');
const sidebar = document.querySelector('.sidebar');
const mainNavMenu = document.querySelector('.main-nav-menu');

toggleMenuOpenButton.addEventListener('click', () => {
    sidebar.classList.toggle('show');
    mainNavMenu.classList.toggle('expanded');
});

toggleMenuCloseButton.addEventListener('click', () => {
    sidebar.classList.toggle('show');
    mainNavMenu.classList.toggle('expanded');
});
