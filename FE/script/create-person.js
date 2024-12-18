restrictAccess(["Admin", "User"]);

document.getElementById("create-person-btn")?.addEventListener("click", async () => {
    if (!token) {
        showGeneralError("Jūs nesate prisijungę. Prisijunkite norėdami tęsti.");
        return;
    }

    const fileInput = document.getElementById("create-profilioNuotrauka");
    let profilePicture = null;

    try {
        if (fileInput?.files?.length > 0) {
            profilePicture = await getBase64(fileInput.files[0]);
        }
    } catch (error) {
        showGeneralError("Nepavyko apdoroti profilio nuotraukos.");
        return;
    }

    const getValue = (id) => document.getElementById(id)?.value?.trim() || "";

    const data = {
        vardas: getValue("create-vardas"),
        pavarde: getValue("create-pavarde"),
        asmensKodas: getValue("create-asmensKodas"),
        telefonoNumeris: getValue("create-telefonoNumeris"),
        elPastas: getValue("create-elPastas"),
        miestas: getValue("create-miestas"),
        gatve: getValue("create-gatve"),
        namoNumeris: getValue("create-namoNumeris"),
        butoNumeris: getValue("create-butoNumeris"),
        profilioNuotrauka: profilePicture,
    };

    clearAllErrors();

    try {
        const response = await fetch(`${baseUrl}/Person`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(data),
        });

        if (response.ok) {
            alert("Asmuo sėkmingai sukurtas!");
            document.getElementById("create-person-form")?.reset();
            clearAllErrors();
        } else {
            const result = await response.json();
            if (result.errors) showValidationErrors(result.errors);
            else showGeneralError(result.message || "Nepavyko sukurti asmens.");
        }
    } catch (error) {
        showGeneralError("Įvyko netikėta klaida. Bandykite dar kartą vėliau.");
    }
});

// Show validation errors
function showValidationErrors(errors) {
    clearAllErrors();
    for (const field in errors) {
        const inputId = `create-${toCamelCase(field)}`;
        const input = document.getElementById(inputId);

        if (input) {
            errors[field].forEach((error) => {
                const errorMsg = document.createElement("div");
                errorMsg.className = "error-message";
                errorMsg.textContent = error;
                input.insertAdjacentElement("afterend", errorMsg);
            });
        } else {
            console.warn(`No input field found for ${field}`);
        }
    }
}
