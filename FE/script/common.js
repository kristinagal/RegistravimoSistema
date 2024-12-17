const baseUrl = "https://localhost:7094/api";
const token = localStorage.getItem("token");
const role = localStorage.getItem("role");

// Helper: Render Menu
function renderMenu() {
    const menuContainer = document.getElementById("menu");
    if (menuContainer) {
        menuContainer.innerHTML = `
            <button onclick="window.location.href='create-person.html'">Sukurti</button>
            <button onclick="window.location.href='update-person.html'">Atnaujinti</button>
            <button onclick="window.location.href='retrieve-person.html'">Ieškoti</button>
            ${
                role === "Admin"
                    ? `<button onclick="window.location.href='delete-user.html'">Šalinti paskyrą</button>`
                    : ""
            }
            <button id="logout-btn">Atsijungti</button>
        `;

        document.getElementById("logout-btn").addEventListener("click", () => {
            localStorage.removeItem("token");
            localStorage.removeItem("role");
            alert("Atsijungėte sėkmingai.");
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

/**
 * Updates the profile picture display.
 * @param {string} base64Image - Base64 encoded image string.
 * @param {string} imgElementId - The ID of the image element to update.
 */

function displayProfilePicture(base64Image, imageElementId) {
    const profilePreview = document.getElementById(imageElementId);

    if (base64Image) {
        profilePreview.src = `data:image/png;base64,${base64Image}`;
        profilePreview.classList.remove("hidden");
    } else {
        profilePreview.src = ""; // Clear invalid src
        profilePreview.classList.add("hidden");
    }
}

// Shared: Access Restriction
function restrictAccess(allowedRoles = []) {
    if (!token) {
        alert("Norėdami tęsti prisijunkite.");
        window.location.href = "login.html";
        return;
    }

    if (!role) {
        alert("Jūsų rolė neapibrėžta. Prisijunkite iš naujo.");
        window.location.href = "login.html";
        return;
    }

    if (allowedRoles.length > 0 && !allowedRoles.includes(role)) {
        alert("Neturite teisių atlikti šį veiksmą.");
        window.location.href = "menu.html";
    }
}

// Render menu on page load
document.addEventListener("DOMContentLoaded", renderMenu);
