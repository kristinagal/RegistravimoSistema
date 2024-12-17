restrictAccess();

document.getElementById("retrieve-btn")?.addEventListener("click", async () => {
    const id = document.getElementById("retrieve-id").value.trim();

    if (!id) {
        displayError("Įveskite teisingą asmens ID.");
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Person/${id}`, {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) {
            const person = await response.json();

            // Display profile picture using reusable function
            displayProfilePicture(person.profilioNuotrauka, "retrieve-profile-preview");

            // Display person data in a table
            document.getElementById("retrieve-result").innerHTML = `
                <table>
                    <tr><th>Vardas</th><td>${person.vardas || "N/A"}</td></tr>
                    <tr><th>Pavarde</th><td>${person.pavarde || "N/A"}</td></tr>
                    <tr><th>Asmens Kodas</th><td>${person.asmensKodas || "N/A"}</td></tr>
                    <tr><th>Telefono Numeris</th><td>${person.telefonoNumeris || "N/A"}</td></tr>
                    <tr><th>El. Paštas</th><td>${person.elPastas || "N/A"}</td></tr>
                    <tr><th>Miestas</th><td>${person.address?.miestas || "N/A"}</td></tr>
                    <tr><th>Gatvė</th><td>${person.address?.gatve || "N/A"}</td></tr>
                    <tr><th>Namo Numeris</th><td>${person.address?.namoNumeris || "N/A"}</td></tr>
                    <tr><th>Buto Numeris</th><td>${person.address?.butoNumeris || "N/A"}</td></tr>
                </table>
            `;
        } else {
            displayError("Nepavyko rasti asmens su šiuo ID.");
        }
    } catch (error) {
        console.error("Klaida:", error);
        displayError("Klaida. Pabandykite vėliau.");
    }
});

// Helper function to display error messages
function displayError(message) {
    document.getElementById("retrieve-result").innerHTML = `
        <p class="error-message">${message}</p>
    `;
}
