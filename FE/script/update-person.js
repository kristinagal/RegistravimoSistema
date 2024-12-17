restrictAccess(["Admin", "User"]); // Restrict access to Admin and User roles

let initialValues = {}; // Store initial values for comparison

// Fetch and populate person details
document.getElementById("fetch-person-btn")?.addEventListener("click", async () => {
    const personIdInput = document.getElementById("update-id");
    const personId = personIdInput.value.trim();
    const generalError = document.getElementById("general-error");

    clearError(personIdInput);
    generalError.textContent = "";

    if (!personId) {
        showError(personIdInput, "Prašome įvesti galiojantį asmens ID.");
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
                    showError(personIdInput, "Esate neprisijungęs. Prisijunkite iš naujo.");
                    break;
                case 403:
                    showError(personIdInput, "Neturite teisių peržiūrėti šį asmenį.");
                    break;
                case 404:
                    showError(personIdInput, "Asmuo su tokiu ID nerastas.");
                    break;
                default:
                    showError(personIdInput, "Įvyko klaida gaunant asmens duomenis.");
            }
            return;
        }

        const person = await response.json();
        populatePersonFields(person);

        document.getElementById("update-form").classList.remove("hidden");
    } catch (error) {
        console.error("Klaida:", error);
        showError(personIdInput, "Įvyko serverio klaida. Bandykite vėliau.");
    }
});

// Populate form fields with fetched person data
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

    displayProfilePicture(person.profilioNuotrauka, "update-profile-preview");

    initialValues = {
        Vardas: person.vardas || "",
        Pavarde: person.pavarde || "",
        AsmensKodas: person.asmensKodas || "",
        TelefonoNumeris: person.telefonoNumeris || "",
        ElPastas: person.elPastas || "",
        Miestas: person.address?.miestas || "",
        Gatve: person.address?.gatve || "",
        NamoNumeris: person.address?.namoNumeris || "",
        ButoNumeris: person.address?.butoNumeris || "",
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
                    showError(inputElement, "Esate neprisijungęs. Prisijunkite iš naujo.");
                    break;
                case 403:
                    showError(inputElement, "Neturite teisės atnaujinti šio lauko.");
                    break;
                case 404:
                    showError(inputElement, "Asmuo nerastas.");
                    break;
                case 400:
                    showError(inputElement, result.message || `Neteisingi duomenys laukui ${fieldName}.`);
                    break;
                default:
                    showError(inputElement, "Įvyko netikėta klaida.");
            }
            return false;
        }

        clearError(inputElement);
        return true;
    } catch (error) {
        console.error(`Klaida atnaujinant ${fieldName}:`, error);
        showError(inputElement, `Įvyko klaida atnaujinant lauką ${fieldName}.`);
        return false;
    }
}

// Save all updates
document.getElementById("save-updates-btn")?.addEventListener("click", async () => {
    const personId = document.getElementById("update-id").value.trim();

    if (!personId) {
        const personIdInput = document.getElementById("update-id");
        showError(personIdInput, "Asmens ID yra privalomas.");
        return;
    }

    const fields = [
        { id: "update-vardas", field: "Vardas", endpoint: "UpdateVardas" },
        { id: "update-pavarde", field: "Pavarde", endpoint: "UpdatePavarde" },
        { id: "update-asmens-kodas", field: "AsmensKodas", endpoint: "UpdateAsmensKodas" },
        { id: "update-telefono-numeris", field: "TelefonoNumeris", endpoint: "UpdateTelefonoNumeris" },
        { id: "update-el-pastas", field: "ElPastas", endpoint: "UpdateElPastas" },
        { id: "update-miestas", field: "Miestas", endpoint: "UpdateMiestas" },
        { id: "update-gatve", field: "Gatve", endpoint: "UpdateGatve" },
        { id: "update-namo-numeris", field: "NamoNumeris", endpoint: "UpdateNamoNumeris" },
        { id: "update-buto-numeris", field: "ButoNumeris", endpoint: "UpdateButoNumeris" },
    ];

    let success = true;

    for (const { id, field, endpoint } of fields) {
        const input = document.getElementById(id);
        if (input.value !== initialValues[field]) {
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
            console.error("Klaida apdorojant nuotrauką:", error);
            showGeneralError("Nepavyko apdoroti nuotraukos.");
            success = false;
        }
    }

    if (success) {
        alert("Atnaujinimai buvo sėkmingai išsaugoti!");
    } else {
        showGeneralError("Kai kurių laukų atnaujinti nepavyko. Patikrinkite klaidas ir bandykite dar kartą.");
    }
});

// Display profile picture
function displayProfilePicture(base64Image, imageElementId) {
    const profilePreview = document.getElementById(imageElementId);
    if (base64Image) {
        profilePreview.src = `data:image/png;base64,${base64Image}`;
        profilePreview.classList.remove("hidden");
    } else {
        profilePreview.src = "";
        profilePreview.classList.add("hidden");
    }
}

// Error handling
function showError(inputElement, message) {
    clearError(inputElement);
    const errorDiv = document.createElement("div");
    errorDiv.className = "error-message";
    errorDiv.textContent = message;
    inputElement.insertAdjacentElement("afterend", errorDiv);
}

function clearError(inputElement) {
    inputElement.parentNode.querySelectorAll(".error-message").forEach((el) => el.remove());
}

function showGeneralError(message) {
    const generalError = document.getElementById("general-error");
    generalError.textContent = message;
    generalError.classList.add("error-message");
}

// Convert file to Base64
async function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]);
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
}
