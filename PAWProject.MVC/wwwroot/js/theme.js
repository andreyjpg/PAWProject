(function () {
    const root = document.documentElement;
    const themeToggle = document.getElementById('themeToggle');
    const themeToggleIcon = document.getElementById('themeToggleIcon');
    const colorDots = document.querySelectorAll('.theme-color-dot');

    const DEFAULT_PRIMARY = '#D32F2F';

    const savedTheme = localStorage.getItem('theme') || 'light';
    const savedPrimaryColor = localStorage.getItem('primaryColor') || DEFAULT_PRIMARY;

    root.setAttribute('data-theme', savedTheme);
    root.style.setProperty('--primary-color', savedPrimaryColor);

    function updateThemeIcon(theme) {
        if (!themeToggleIcon) return;
        if (theme === 'dark') {
            themeToggleIcon.classList.remove('bi-moon-fill');
            themeToggleIcon.classList.add('bi-sun-fill');
        } else {
            themeToggleIcon.classList.remove('bi-sun-fill');
            themeToggleIcon.classList.add('bi-moon-fill');
        }
    }


    updateThemeIcon(savedTheme);

    function setActiveColorDot(color) {
        colorDots.forEach(dot => {
            dot.classList.toggle('active', dot.getAttribute('data-color') === color);
        });
    }

    setActiveColorDot(savedPrimaryColor);

    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            const currentTheme = root.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            root.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
            updateThemeIcon(newTheme);
        });
    }

    colorDots.forEach(dot => {
        dot.addEventListener('click', () => {
            const color = dot.getAttribute('data-color');
            root.style.setProperty('--primary-color', color);
            localStorage.setItem('primaryColor', color);
            setActiveColorDot(color);
        });
    });
})();
