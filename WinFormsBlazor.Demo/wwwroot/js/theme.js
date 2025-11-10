// Theme management for Blazor UI
window.themeManager = {
    applyTheme: function (themeName) {
        const root = document.documentElement;

        // Define theme colors matching WinForms themes
        const themes = {
            'Light': {
                bg: 'rgb(255, 255, 255)',
                fg: 'rgb(0, 0, 0)',
                accent: 'rgb(0, 120, 215)',
                border: 'rgb(220, 220, 220)',
                hover: 'rgb(240, 240, 240)'
            },
            'Light Blue': {
                bg: 'rgb(240, 248, 255)',
                fg: 'rgb(20, 20, 20)',
                accent: 'rgb(30, 144, 255)',
                border: 'rgb(200, 220, 240)',
                hover: 'rgb(230, 240, 250)'
            },
            'Dark': {
                bg: 'rgb(30, 30, 30)',
                fg: 'rgb(255, 255, 255)',
                accent: 'rgb(45, 120, 210)',
                border: 'rgb(60, 60, 60)',
                hover: 'rgb(45, 45, 45)'
            },
            'Dark Purple': {
                bg: 'rgb(25, 20, 35)',
                fg: 'rgb(240, 240, 245)',
                accent: 'rgb(138, 43, 226)',
                border: 'rgb(50, 45, 60)',
                hover: 'rgb(40, 35, 50)'
            }
        };

        const theme = themes[themeName] || themes['Light'];

        root.style.setProperty('--bg-color', theme.bg);
        root.style.setProperty('--fg-color', theme.fg);
        root.style.setProperty('--accent-color', theme.accent);
        root.style.setProperty('--border-color', theme.border);
        root.style.setProperty('--hover-color', theme.hover);
    }
};
