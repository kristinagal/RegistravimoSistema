document.getElementById("login-btn")?.addEventListener("click", async () => {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    try {
        const response = await fetch(`${baseUrl}/Accounts/Login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
        });
        console.log(response);
        const result = await response.json();
        if (response.ok) {
            const payload = JSON.parse(atob(result.token.split(".")[1]));
            localStorage.setItem("token", result.token);
            localStorage.setItem("role", payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);

console.log(payload);

            alert("Login successful!");
            window.location.href = "menu.html";
        } else {
            document.getElementById("login-message").textContent = result.message || "Login failed.";
        }
    } catch {
        document.getElementById("login-message").textContent = "Error occurred during login.";
    }
});
