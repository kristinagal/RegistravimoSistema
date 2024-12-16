restrictAccess();

document.addEventListener("DOMContentLoaded", () => {
    renderMenu();
    document.getElementById("welcome-message").textContent = `Welcome, ${role}!`;
});