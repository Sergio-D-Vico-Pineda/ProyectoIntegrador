// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let $btntema = document.getElementById("tema");

function toggleDarkMode() {
    let switchToTheme = localStorage.getItem('theme') === 'dark' ? 'light' : 'dark';
    cambiarTxtBtn(switchToTheme);
    setTheme(switchToTheme);
}

function cambiarTxtBtn(theme) {
    $btntema.textContent = theme === 'light' ? '🌙' : '☀';
    $btntema.title = theme === 'light' ? 'Cambiar a modo oscuro' : 'Cambiar a modo claro';
}

const setTheme = (theme) => {
    document.documentElement.setAttribute('data-bs-theme', theme);
    localStorage.setItem('theme', theme);
}

const preferedColorScheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
setTheme(localStorage.getItem('theme') || preferedColorScheme);
cambiarTxtBtn(localStorage.getItem('theme') === 'dark' ? 'dark' : 'light');