// Restrict access to authorized roles
restrictAccess(["Admin", "User"]);

// Global variable to track initial values
let initialValues = {};

// Fetch and populate person details
document.getElementById("fetch-person-btn")?.addEventListener("click", async () => {
    const personIdInput = document.getElementById("update-id");
    const personId = personIdInput.value.trim();
    const generalError = document.getElementById("general-error");

    clearError(personIdInput);
    generalError.textContent = "";

    if (!personId) {
        showError(personIdInput, "Please enter a valid Person ID.");
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Person/${personId}`, {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (!response.ok) {
            switch (response.status) {
                case 401:
                    showError(personIdInput, "Unauthorized: Please log in again.");
                    break;
                case 403:
                    showError(personIdInput, "Forbidden: You do not have permission to access this resource.");
                    break;
                case 404:
                    showError(personIdInput, "Person not found.");
                    break;
                default:
                    showError(personIdInput, "Failed to fetch person details.");
            }
            return;
        }

        const person = await response.json();
        populatePersonFields(person);

        document.getElementById("update-form").classList.remove("hidden");
    } catch (error) {
        console.error("Error fetching person details:", error);
        showError(personIdInput, "An error occurred while fetching person details.");
    }
});

// Populate form fields
function populatePersonFields(person) {
    document.getElementById("update-vardas").value = person.vardas || "";
    document.getElementById("update-pavarde").value = person.pavarde || "";
    document.getElementById("update-asmens-kodas").value = person.asmensKodas || "";
    document.getElementById("update-telefono-numeris").value = person.telefonoNumeris || "";
    document.getElementById("update-el-pastas").value = person.elPastas || "";

    if (person.address) {
        document.getElementById("update-miestas").value = person.address.miestas || "";
        document.getElementById("update-gatve").value = person.address.gatve || "";
        document.getElementById("update-namo-numeris").value = person.address.namoNumeris || "";
        document.getElementById("update-buto-numeris").value = person.address.butoNumeris || "";
    }

    const profilePreview = document.getElementById("update-profile-preview");
    if (person.profilioNuotrauka) {
        profilePreview.src = `data:image/png;base64,${person.profilioNuotrauka}`;
        profilePreview.classList.remove("hidden");
    } else {
        profilePreview.classList.add("hidden");
    }

    initialValues = {
        vardas: person.vardas || "",
        pavarde: person.pavarde || "",
        asmenskodas: person.asmensKodas || "",
        telefononumeris: person.telefonoNumeris || "",
        elpastas: person.elPastas || "",
        miestas: person.address?.miestas || "",
        gatve: person.address?.gatve || "",
        namonumeris: person.address?.namoNumeris || "",
        butonumeris: person.address?.butoNumeris || "",
    };
}

// Update a single field
async function updateField(personId, endpoint, fieldName, value, inputElement) {
    try {
        const payload = { [fieldName]: value };

        const response = await fetch(`${baseUrl}/Person/${personId}/${endpoint}`, {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(payload),
        });

        if (!response.ok) {
            const result = await response.json();
            switch (response.status) {
                case 401:
                    showError(inputElement, "Unauthorized: Please log in again.");
                    break;
                case 403:
                    showError(inputElement, "Forbidden: You do not have permission to update this field.");
                    break;
                case 404:
                    showError(inputElement, "Resource not found.");
                    break;
                case 400:
                    showError(inputElement, result.message || `Invalid data for ${fieldName}.`);
                    break;
                default:
                    showError(inputElement, "An unexpected error occurred.");
            }
            return false;
        }

        clearError(inputElement);
        return true;
    } catch (error) {
        console.error(`Error updating ${fieldName}:`, error);
        showError(inputElement, `An error occurred while updating ${fieldName}.`);
        return false;
    }
}

// Save all updates
document.getElementById("save-updates-btn")?.addEventListener("click", async () => {
    const personId = document.getElementById("update-id").value.trim();

    if (!personId) {
        const personIdInput = document.getElementById("update-id");
        showError(personIdInput, "Person ID is required.");
        return;
    }

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
            const updated = await updateField(personId, endpoint, field, input.value, input);
            if (!updated) success = false;
        }
    }

    const fileInput = document.getElementById("update-profile-picture");
    if (fileInput.files.length > 0) {
        try {
            const profilePicture = await getBase64(fileInput.files[0]);
            const updated = await updateField(personId, "UpdateProfilioNuotrauka", "ProfilioNuotrauka", profilePicture, fileInput);
            if (!updated) success = false;
        } catch (error) {
            console.error("Error converting file to Base64:", error);
            showGeneralError("Failed to process profile picture.");
            success = false;
        }
    }

    showGeneralError(success ? "All updates were successfully saved!" : "Some fields failed to update.");
});

// Show and clear errors
function showError(inputElement, message) {
    clearError(inputElement);
    const errorDiv = document.createElement("div");
    errorDiv.className = "error-message";
    errorDiv.style.color = "red";
    errorDiv.textContent = message;
    inputElement.insertAdjacentElement("afterend", errorDiv);
}

function clearError(inputElement) {
    inputElement.parentNode.querySelectorAll(".error-message").forEach((el) => el.remove());
}

function showGeneralError(message) {
    const generalError = document.getElementById("general-error");
    generalError.textContent = message;
    generalError.style.color = "red";
}

async function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]);
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
}
