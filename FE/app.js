const baseUrl = "https://localhost:7094/api";
const token = localStorage.getItem("token");
const role = localStorage.getItem("role");

// Helper function to render the menu
function renderMenu() {
    const menuContainer = document.getElementById("menu");
    if (menuContainer) {
        menuContainer.innerHTML = `
            <button onclick="window.location.href='menu.html'">Home</button>
            <button onclick="window.location.href='create-person.html'">Create Person</button>
            <button onclick="window.location.href='update-person.html'">Update Person</button>
            <button onclick="window.location.href='retrieve-person.html'">Retrieve Person</button>
            ${role === "Admin" ? `<button onclick="window.location.href='delete-user.html'">Delete User</button>` : ""}
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
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]);
        reader.onerror = error => reject(error);
        reader.readAsDataURL(file);
    });
}

// Track initial values
let initialValues = {};

// Fetch person details
document.getElementById("fetch-person-btn")?.addEventListener("click", async () => {
    const id = document.getElementById("update-id").value;

    try {
        const response = await fetch(`${baseUrl}/Person/${id}`, {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) {
            const person = await response.json();

            // Populate fields
            document.getElementById("update-vardas").value = person.vardas;
            document.getElementById("update-pavarde").value = person.pavarde;
            document.getElementById("update-asmens-kodas").value = person.asmensKodas;
            document.getElementById("update-telefono-numeris").value = person.telefonoNumeris;
            document.getElementById("update-el-pastas").value = person.elPastas;
            document.getElementById("update-miestas").value = person.address.miestas;
            document.getElementById("update-gatve").value = person.address.gatve;
            document.getElementById("update-namo-numeris").value = person.address.namoNumeris;
            document.getElementById("update-buto-numeris").value = person.address.butoNumeris;

            if (person.profilioNuotrauka) {
                const profilePreview = document.getElementById("update-profile-preview");
                profilePreview.src = `data:image/png;base64,${person.profilioNuotrauka}`;
                profilePreview.classList.remove("hidden");
            }

            // Store initial values
            initialValues = {
                vardas: person.vardas,
                pavarde: person.pavarde,
                asmensKodas: person.asmensKodas,
                telefonoNumeris: person.telefonoNumeris,
                elPastas: person.elPastas,
                miestas: person.address.miestas,
                gatve: person.address.gatve,
                namoNumeris: person.address.namoNumeris,
                butoNumeris: person.address.butoNumeris,
            };

            document.getElementById("update-form").classList.remove("hidden");
        } else {
            const result = await response.json();
            alert(result.message || "Failed to fetch person details.");
        }
    } catch {
        alert("An error occurred while fetching person details.");
    }
});

// Update single field and show validation errors
async function updateField(endpoint, fieldName, value, inputElement) {
    try {
        const response = await fetch(`${baseUrl}/Person/${endpoint}`, {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({ [fieldName]: value }),
        });

        if (!response.ok) {
            const result = await response.json();
            showError(inputElement, result.message || `Error updating ${fieldName}.`);
            return false;
        } else {
            clearError(inputElement);
            return true;
        }
    } catch {
        showError(inputElement, `Error updating ${fieldName}.`);
        return false;
    }
}

// Show error message near the input field
function showError(inputElement, message) {
    clearError(inputElement);
    const error = document.createElement("div");
    error.className = "error-message";
    error.style.color = "red";
    error.textContent = message;
    inputElement.insertAdjacentElement("afterend", error);
}

// Clear error message
function clearError(inputElement) {
    const nextSibling = inputElement.nextElementSibling;
    if (nextSibling && nextSibling.classList.contains("error-message")) {
        nextSibling.remove();
    }
}

// Save all updates
document.getElementById("save-updates-btn")?.addEventListener("click", async () => {
    const id = document.getElementById("update-id").value;
    const fields = [
        { id: "update-vardas", field: "Vardas", endpoint: "UpdateVardas" },
        { id: "update-pavarde", field: "Pavarde", endpoint: "UpdatePavarde" },
        { id: "update-asmens-kodas", field: "AsmensKodas", endpoint: "UpdateAsmensKodas" },
        { id: "update-telefono-numeris", field: "TelefonoNumeris", endpoint: "UpdateTelefonoNumeris" },
        { id: "update-el-pastas", field: "ElPastas", endpoint: "UpdateElPastas" },
        { id: "update-miestas", field: "Miestas", endpoint: "UpdateAddress/Miestas" },
        { id: "update-gatve", field: "Gatve", endpoint: "UpdateAddress/Gatve" },
        { id: "update-namo-numeris", field: "NamoNumeris", endpoint: "UpdateAddress/NamoNumeris" },
        { id: "update-buto-numeris", field: "ButoNumeris", endpoint: "UpdateAddress/ButoNumeris" },
    ];

    let success = true;

    for (const { id, field, endpoint } of fields) {
        const input = document.getElementById(id);
        if (input.value !== initialValues[field.toLowerCase()]) {
            const updated = await updateField(`${id}/${endpoint}`, field, input.value, input);
            if (!updated) success = false;
        }
    }

    // Handle profile picture
    const fileInput = document.getElementById("update-profile-picture");
    if (fileInput.files.length > 0) {
        const base64Image = await getBase64(fileInput.files[0]);
        const profileInput = document.getElementById("update-profile-picture");
        const updated = await updateField("UpdateProfilioNuotrauka", "ProfilioNuotrauka", base64Image, profileInput);
        if (!updated) success = false;
    }

    if (success) {
        alert("All updates were successfully saved!");
    } else {
        alert("Some fields failed to update. Please check for errors.");
    }
});
