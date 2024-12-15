const baseUrl = "https://localhost:7094/api";
const token = localStorage.getItem("token");
const role = localStorage.getItem("role");

// Helper function to render the menu
function renderMenu() {
    const menuContainer = document.getElementById("menu");
    if (menuContainer) {
        menuContainer.innerHTML = `
            <button onclick="window.location.href='menu.html'">Home</button>
            <button onclick="window.location.href='create-person.html'">Create Person</button>
            <button onclick="window.location.href='update-person.html'">Update Person</button>
            <button onclick="window.location.href='retrieve-person.html'">Retrieve Person</button>
            ${role === "Admin" ? `<button onclick="window.location.href='delete-user.html'">Delete User</button>` : ""}
            <button id="logout-btn">Logout</button>
        `;
        document.getElementById("logout-btn").addEventListener("click", () => {
            localStorage.removeItem("token");
            localStorage.removeItem("role");
            alert("You have been logged out.");
            window.location.href = "index.html";
        });
    }
}

// Restrict access to pages
const restrictedPages = ["delete-user.html", "create-person.html", "retrieve-person.html", "update-person.html", "menu.html"];
const publicPages = ["login.html", "signup.html", "index.html"];
const currentPage = window.location.pathname.split("/").pop();

if (!publicPages.includes(currentPage) && restrictedPages.includes(currentPage)) {
    if (!token) {
        alert("Please log in first.");
        window.location.href = "login.html";
    } else if (currentPage === "delete-user.html" && role !== "Admin") {
        alert("Access denied. Only Admins can access this page.");
        window.location.href = "menu.html";
    }
}

if (token) renderMenu();

// Helper Function: Convert Image to Base64
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]);
        reader.onerror = error => reject(error);
        reader.readAsDataURL(file);
    });
}

// Login Functionality
if (currentPage === "login.html") {
    document.getElementById("login-btn").addEventListener("click", async () => {
        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        try {
            const response = await fetch(`${baseUrl}/Accounts/Login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            const result = await response.json();
            if (response.ok) {
                const payload = JSON.parse(atob(result.token.split(".")[1]));
                localStorage.setItem("token", result.token);
                localStorage.setItem("role", payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);

                alert("Login successful!");
                window.location.href = "menu.html";
            } else {
                document.getElementById("login-message").textContent = result.message || "Login failed.";
            }
        } catch {
            document.getElementById("login-message").textContent = "Error occurred during login.";
        }
    });
}

// Signup Functionality
if (currentPage === "signup.html") {
    document.getElementById("signup-btn").addEventListener("click", async () => {
        const username = document.getElementById("signup-username").value;
        const password = document.getElementById("signup-password").value;

        try {
            const response = await fetch(`${baseUrl}/Accounts/SignUp`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
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
}

// Retrieve Person Functionality
if (document.getElementById("retrieve-btn")) {
    document.getElementById("retrieve-btn").addEventListener("click", async () => {
        const id = document.getElementById("retrieve-id").value;

        try {
            const response = await fetch(`${baseUrl}/Person/${id}`, {
                method: "GET",
                headers: { Authorization: `Bearer ${token}` }
            });

            if (response.ok) {
                const result = await response.json();

                // Display profile picture if exists
                let profilePic = "";
                if (result.profilioNuotrauka) {
                    profilePic = `<img src="data:image/png;base64,${result.profilioNuotrauka}" 
                                  alt="Profile Picture" style="max-width: 200px;" />`;
                }

                document.getElementById("retrieve-result").innerHTML = `
                    <pre>${JSON.stringify(result, null, 2)}</pre>
                    ${profilePic}
                `;
            } else {
                const result = await response.json();
                document.getElementById("retrieve-result").textContent = result.message || "Failed to retrieve person.";
            }
        } catch {
            document.getElementById("retrieve-result").textContent = "Error occurred during retrieval.";
        }
    });
}

// Create Person Functionality
if (document.getElementById("create-person-btn")) {
    document.getElementById("create-person-btn").addEventListener("click", async () => {
        const fileInput = document.getElementById("profile-picture");
        let profilePicture = null;

        if (fileInput.files.length > 0) {
            profilePicture = await getBase64(fileInput.files[0]);
        }

        const data = {
            vardas: document.getElementById("vardas").value,
            pavarde: document.getElementById("pavarde").value,
            asmensKodas: document.getElementById("asmens-kodas").value,
            telefonoNumeris: document.getElementById("telefono-numeris").value,
            elPastas: document.getElementById("el-pastas").value,
            miestas: document.getElementById("miestas").value,
            gatve: document.getElementById("gatve").value,
            namoNumeris: document.getElementById("namo-numeris").value,
            butoNumeris: document.getElementById("buto-numeris").value,
            profilioNuotrauka: profilePicture
        };

        try {
            const response = await fetch(`${baseUrl}/Person`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                alert("Person created successfully!");
            } else {
                const result = await response.json();
                document.getElementById("person-message").textContent = result.message || "Failed to create person.";
            }
        } catch {
            document.getElementById("person-message").textContent = "Error occurred during person creation.";
        }
    });
}

// Delete User Functionality (Admin Only)
if (document.getElementById("delete-btn")) {
    document.getElementById("delete-btn").addEventListener("click", async () => {
        const id = document.getElementById("delete-id").value;

        try {
            const response = await fetch(`${baseUrl}/Accounts/DeleteUser/${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` }
            });

            if (response.ok) {
                alert("User deleted successfully!");
            } else {
                const result = await response.json();
                document.getElementById("delete-message").textContent = result.message || "Failed to delete user.";
            }
        } catch {
            document.getElementById("delete-message").textContent = "Error occurred while deleting user.";
        }
    });
}
