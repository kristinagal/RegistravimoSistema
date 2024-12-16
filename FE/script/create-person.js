restrictAccess(["Admin", "User"]); // Restrict access to logged-in users

document.getElementById("create-person-btn")?.addEventListener("click", async () => {
    // Validate that the token exists
    if (!token) {
        showGeneralError("You are not logged in. Please log in to continue.");
        return;
    }

    // Get the file input and handle Base64 conversion
    const fileInput = document.getElementById("profile-picture");
    let profilePicture = null;

    try {
        if (fileInput?.files?.length > 0) {
            profilePicture = await getBase64(fileInput.files[0]);
        }
    } catch (error) {
        console.error("Error converting file to Base64:", error);
        showGeneralError("Failed to process profile picture. Please try again.");
        return;
    }

    // Safely retrieve input values
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

    // Validate required fields before sending
    if (!data.vardas || !data.pavarde || !data.asmensKodas || !data.elPastas || !profilePicture) {
        showGeneralError("All fields, including a profile picture, are required.");
        return;
    }
    console.log(data);
    
    // Send data to the API
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
            alert("Person created successfully!");
            document.getElementById("create-person-form")?.reset();
            clearAllErrors();
        } else {
            const result = await response.json();
            console.error("API Error:", result);
            if (result.errors) showValidationErrors(result.errors);
            else showGeneralError(result.message || "Failed to create person.");
        }
    } catch (error) {
        console.error("Error creating person:", error);
        showGeneralError("An unexpected error occurred. Please try again later.");
    }
});

// Helper to convert file to Base64
async function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]); // Remove metadata
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
}

// Functions to show errors (reused where needed)
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
        alert(message); // Fallback if container doesn't exist
    }
}
