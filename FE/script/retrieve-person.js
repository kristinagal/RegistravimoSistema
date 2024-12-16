restrictAccess();

document.getElementById("retrieve-btn")?.addEventListener("click", async () => {
    const id = document.getElementById("retrieve-id").value.trim();

    if (!id) {
        displayError("Please enter a valid Person ID.");
        return;
    }

    try {
        const response = await fetch(`${baseUrl}/Person/${id}`, {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) {
            const person = await response.json();

            let profilePic = "";
            if (person.profilioNuotrauka) {
                profilePic = `<img src="data:image/png;base64,${person.profilioNuotrauka}" 
                                alt="Profile Picture" class="profile-picture" />`;
            }

            // Display person data in a table
            document.getElementById("retrieve-result").innerHTML = `
                <h2>Person Details</h2>
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
                ${profilePic}
            `;
        } else {
            displayError("Failed to retrieve person. Invalid ID or no data found.");
        }
    } catch (error) {
        console.error("Error retrieving person details:", error);
        displayError("An error occurred during retrieval. Please try again later.");
    }
});

// Helper function to display error messages
function displayError(message) {
    document.getElementById("retrieve-result").innerHTML = `
        <p class="error-message">${message}</p>
    `;
}
