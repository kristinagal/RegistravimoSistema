let personId = null; //  current user's ID
let initialValues = {}; // initial field values 

document.addEventListener("DOMContentLoaded", async () => {

    try {
        const response = await fetch(`${baseUrl}/Person/MyProfile`, {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (!response.ok) {
            switch (response.status) {
                case 401:
                    showGeneralError("Esate neprisijungęs. Prisijunkite iš naujo.");
                    break;
                case 403:
                    showGeneralError("Neturite teisės peržiūrėti šio profilio.");
                    break;
                default:
                    showGeneralError("Įvyko klaida gaunant jūsų profilio duomenis.");
            }
            return;
        }

        const person = await response.json();
        personId = person.id; // current user's ID
        populatePersonFields(person);
    } catch (error) {
        console.error("Klaida gaunant profilį:", error);
        showGeneralError("Įvyko serverio klaida. Bandykite vėliau.");
    }
});


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
    } else {
        document.getElementById("update-miestas").value = "";
        document.getElementById("update-gatve").value = "";
        document.getElementById("update-namo-numeris").value = "";
        document.getElementById("update-buto-numeris").value = "";
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

document.getElementById("save-updates-btn")?.addEventListener("click", async () => {
    clearAllErrors();

    if (!personId) {
        showGeneralError("Nepavyko nustatyti naudotojo ID. Atnaujinkite puslapį.");
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

    let changesMade = false;
    let success = true;

    for (const { id, field, endpoint } of fields) {
        const input = document.getElementById(id);
        if (input.value !== initialValues[field]) {
            changesMade = true;
            const updated = await updateField(personId, endpoint, field, input.value, input);
            if (!updated) success = false;
        }
    }

    const fileInput = document.getElementById("update-profile-picture");
    if (fileInput.files.length > 0) {
        changesMade = true;
        try {
            const profilePicture = await getBase64(fileInput.files[0]);
            const updated = await updateField(personId, "UpdateProfilioNuotrauka", "ProfilioNuotrauka", profilePicture, fileInput);
            if (!updated) success = false;
        } catch (error) {
            console.error("Klaida apdorojant nuotrauką:", error);
            showGeneralError("Nepavyko apdoroti profilio nuotraukos.");
            success = false;
        }
    }

    if (!changesMade) {
        showGeneralError("Nėra jokių pakeitimų, kuriuos būtų galima išsaugoti.");
        return;
    }

    if (success) {
        alert("Atnaujinimai buvo sėkmingai išsaugoti!");
    } else {
        showGeneralError("Kai kurių laukų atnaujinti nepavyko. Patikrinkite klaidas ir bandykite dar kartą.");
    }
});


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
            if (result.errors?.[fieldName]) {
                displayFieldError(inputElement, result.errors[fieldName]);
            } else {
                console.error(`Klaida atnaujinant ${fieldName}:`, result);
            }
            return false;
        }

        clearFieldError(inputElement);
        return true;
    } catch (error) {
        console.error(`Klaida atnaujinant ${fieldName}:`, error);
        return false;
    }
}

function displayFieldError(inputElement, errorMessages) {
    clearFieldError(inputElement);

    const errorContainer = document.createElement("div");
    errorContainer.className = "error-message";

    errorMessages.forEach((message) => {
        const errorLine = document.createElement("div");
        errorLine.textContent = message;
        errorContainer.appendChild(errorLine);
    });

    inputElement.insertAdjacentElement("afterend", errorContainer);
}

