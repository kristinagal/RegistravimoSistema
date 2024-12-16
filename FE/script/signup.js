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
            alert("Signup successful! You can now log in.");
            window.location.href = "login.html";
        } else {
            document.getElementById("signup-message").textContent = result.message || "Signup failed.";
        }
    } catch {
        document.getElementById("signup-message").textContent = "Error occurred during signup.";
    }
});
