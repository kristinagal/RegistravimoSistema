document.getElementById("signup-btn")?.addEventListener("click", async () => {
    const username = document.getElementById("signup-username").value;
    const password = document.getElementById("signup-password").value;

    try {
        const response = await fetch(`${baseUrl}/Accounts/SignUp`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
        });

        const result = await response.json();
        if (response.ok) {
            alert("Registracija sėkminga! Dabar galite prisijungti.");
            window.location.href = "login.html";
        } else {
            document.getElementById("signup-message").textContent = result.message || "Registracija nepavyko.";
        }
    } catch {
        document.getElementById("signup-message").textContent = "Įvyko klaida registracijos metu.";
    }
});
