const baseUrl = "https://localhost:7094/api";
const token = localStorage.getItem("token");
const role = localStorage.getItem("role");

// Helper: Render Menu
function renderMenu() {
    const menuContainer = document.getElementById("menu");
    if (menuContainer) {
        menuContainer.innerHTML = `
            <button onclick="window.location.href='create-person.html'">Create Person</button>
            <button onclick="window.location.href='update-person.html'">Update Person</button>
            <button onclick="window.location.href='retrieve-person.html'">Retrieve Person</button>
            ${
                role === "Admin"
                    ? `<button onclick="window.location.href='delete-user.html'">Delete User</button>`
                    : ""
            }
            <button id="logout-btn">Logout</button>
        `;

        document.getElementById("logout-btn").addEventListener("click", () => {
            localStorage.removeItem("token");
            localStorage.removeItem("role");
            alert("You have been logged out.");
            window.location.href = "index.html";
        });
    }
}

// Helper: Convert Image to Base64
async function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const base64Data = reader.result.split(",")[1]; // Remove data URL prefix
            resolve(base64Data);
        };
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
}

// Shared: Access Restriction
function restrictAccess(allowedRoles = []) {
    if (!token) {
        alert("Please log in first.");
        window.location.href = "login.html";
        return;
    }

    if (!role) {
        alert("Your role is not defined. Please log in again.");
        window.location.href = "login.html";
        return;
    }

    if (allowedRoles.length > 0 && !allowedRoles.includes(role)) {
        alert("Access denied.");
        window.location.href = "menu.html";
    }
}

// Render menu on page load
document.addEventListener("DOMContentLoaded", renderMenu);
