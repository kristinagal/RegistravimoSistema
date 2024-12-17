restrictAccess(["Admin", "User"]); 

document.getElementById("create-person-btn")?.addEventListener("click", async () => {
    // Patikrinkite, ar egzistuoja jwt token
    if (!token) {
        showGeneralError("Jūs nesate prisijungę. Prisijunkite norėdami tęsti.");
        return;
    }

    // Gauti failo įvestį ir konvertuoti į Base64
    const fileInput = document.getElementById("profile-picture");
    let profilePicture = null;

    try {
        if (fileInput?.files?.length > 0) {
            profilePicture = await getBase64(fileInput.files[0]);
        }
    } catch (error) {
        console.error("Klaida konvertuojant failą į Base64:", error);
        showGeneralError("Nepavyko apdoroti profilio nuotraukos. Bandykite dar kartą.");
        return;
    }

    const getValue = (id) => document.getElementById(id)?.value?.trim() || "";

    const data = {
        vardas: getValue("create-vardas"),
        pavarde: getValue("create-pavarde"),
        asmensKodas: getValue("create-asmens-kodas"),
        telefonoNumeris: getValue("create-telefono-numeris"),
        elPastas: getValue("create-el-pastas"),
        miestas: getValue("create-miestas"),
        gatve: getValue("create-gatve"),
        namoNumeris: getValue("create-namo-numeris"),
        butoNumeris: getValue("create-buto-numeris"),
        profilioNuotrauka: profilePicture,
    };

    console.log(data);

    // Privalomų laukų patikrinimas
    if (!data.vardas || !data.pavarde || !data.asmensKodas || !data.elPastas || !profilePicture) {
        showGeneralError("Visi laukai, įskaitant profilio nuotrauką, yra privalomi.");
        return;
    }
    console.log(data);
    
    // Siųsti duomenis į API
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
            console.error("API klaida:", result);
            if (result.errors) showValidationErrors(result.errors);
            else showGeneralError(result.message || "Nepavyko sukurti asmens.");
        }
    } catch (error) {
        console.error("Klaida kuriant asmenį:", error);
        showGeneralError("Įvyko netikėta klaida. Bandykite dar kartą vėliau.");
    }
});

// Pagalbinė funkcija konvertuoti failą į Base64
async function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]); // Pašalina metaduomenis
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
}

// Funkcijos klaidoms rodyti (naudojamos, kai reikia)
function showValidationErrors(errors) {
    clearAllErrors();
    for (const field in errors) {
        const input = document.getElementById(`create-${field}`);
        if (input) {
            const errorMsg = document.createElement("div");
            errorMsg.className = "error-message";
            errorMsg.textContent = errors[field];
            errorMsg.style.color = "red";
            input.insertAdjacentElement("afterend", errorMsg);
        }
    }
}

function clearAllErrors() {
    document.querySelectorAll(".error-message").forEach((el) => el.remove());
}

function showGeneralError(message) {
    const errorContainer = document.getElementById("general-error");
    if (errorContainer) {
        errorContainer.textContent = message;
        errorContainer.style.color = "red";
    } else {
        alert(message); // Atsarginis variantas, jei konteinerio nėra
    }
}
