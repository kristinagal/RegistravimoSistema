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

        // Bind logout functionality
        document.getElementById("logout-btn").addEventListener("click", () => {
            localStorage.removeItem("token");
            localStorage.removeItem("role");
            alert("You have been logged out.");
            window.location.href = "index.html"; // Redirect to login/signup page
        });
    }
}

// Restrict access to certain pages based on authentication and role
const restrictedPages = ["delete-user.html", "create-person.html", "retrieve-person.html", "menu.html"];
const publicPages = ["login.html", "signup.html", "index.html"];
const currentPage = window.location.pathname.split("/").pop();

if (!publicPages.includes(currentPage) && restrictedPages.some(page => currentPage.includes(page))) {
    if (!token) {
        alert("Please log in first.");
        window.location.href = "login.html";
    } else if (currentPage === "delete-user.html" && role !== "Admin") {
        alert("Access denied. Only Admins can access this page.");
        window.location.href = "menu.html";
    }
}

// Render the menu dynamically if logged in
if (token) {
    renderMenu();
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
                const payload = JSON.parse(atob(result.token.split(".")[1])); // Decode JWT
                localStorage.setItem("token", result.token);
                localStorage.setItem("role", payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);

                alert("Login successful!");
                window.location.href = "menu.html";
            } else {
                document.getElementById("login-message").textContent = result.message || "Login failed.";
            }
        } catch (error) {
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
        } catch (error) {
            document.getElementById("signup-message").textContent = "Error occurred during signup.";
        }
    });
}

// Create Person Functionality
if (document.getElementById("create-person-btn")) {
    document.getElementById("create-person-btn").addEventListener("click", async () => {
        const fileInput = document.getElementById("profile-picture");
        let profilePicture = null;

        // If a file is selected, convert it to Base64
        if (fileInput.files.length > 0) {
            try {
                profilePicture = await getBase64(fileInput.files[0]);
            } catch (error) {
                document.getElementById("person-message").textContent = "Failed to process profile picture.";
                return;
            }
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
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                document.getElementById("person-message").textContent = "Person created successfully!";
            } else {
                const result = await response.json();
                document.getElementById("person-message").textContent = result.message || "Failed to create person.";
            }
        } catch (error) {
            document.getElementById("person-message").textContent = "Error occurred during person creation.";
        }
    });
}

// Update Person Functionality
document.getElementById("retrieve-update-btn").addEventListener("click", async () => {
    const personId = document.getElementById("update-id").value;

    if (!personId) {
        document.getElementById("update-message").textContent = "Please enter a valid Person ID.";
        return;
    }

    try {
        // Fetch existing person data
        const response = await fetch(`${baseUrl}/Person/${personId}`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        if (response.ok) {
            const person = await response.json();

            // Populate fields with existing data
            document.getElementById("update-vardas").value = person.vardas;
            document.getElementById("update-pavarde").value = person.pavarde;
            document.getElementById("update-asmens-kodas").value = person.asmensKodas;
            document.getElementById("update-telefono-numeris").value = person.telefonoNumeris;
            document.getElementById("update-el-pastas").value = person.elPastas;
            document.getElementById("update-miestas").value = person.address.miestas;
            document.getElementById("update-gatve").value = person.address.gatve;
            document.getElementById("update-namo-numeris").value = person.address.namoNumeris;
            document.getElementById("update-buto-numeris").value = person.address.butoNumeris;

            document.getElementById("update-message").textContent = "Person data loaded successfully!";
        } else {
            const result = await response.json();
            document.getElementById("update-message").textContent = result.message || "Failed to load person data.";
        }
    } catch (error) {
        document.getElementById("update-message").textContent = "Error occurred while retrieving person data.";
    }
});

document.getElementById("update-person-btn").addEventListener("click", async () => {
    const personId = document.getElementById("update-id").value;

    if (!personId) {
        document.getElementById("update-message").textContent = "Please enter a valid Person ID.";
        return;
    }

    const data = {
        vardas: document.getElementById("update-vardas").value,
        pavarde: document.getElementById("update-pavarde").value,
        asmensKodas: document.getElementById("update-asmens-kodas").value,
        telefonoNumeris: document.getElementById("update-telefono-numeris").value,
        elPastas: document.getElementById("update-el-pastas").value,
        miestas: document.getElementById("update-miestas").value,
        gatve: document.getElementById("update-gatve").value,
        namoNumeris: document.getElementById("update-namo-numeris").value,
        butoNumeris: document.getElementById("update-buto-numeris").value,
        profilioNuotrauka: null // Add logic for profile picture if needed
    };

    try {
        // Update person data
        const response = await fetch(`${baseUrl}/Person/${personId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            document.getElementById("update-message").textContent = "Person updated successfully!";
        } else {
            const result = await response.json();
            document.getElementById("update-message").textContent = result.message || "Failed to update person.";
        }
    } catch (error) {
        document.getElementById("update-message").textContent = "Error occurred while updating person.";
    }
});

// Retrieve Person by ID Functionality
if (document.getElementById("retrieve-btn")) {
    document.getElementById("retrieve-btn").addEventListener("click", async () => {
        const id = document.getElementById("retrieve-id").value;

        try {
            const response = await fetch(`${baseUrl}/Person/${id}`, {
                method: "GET",
                headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
            });

            if (response.ok) {
                const result = await response.json();
                document.getElementById("retrieve-result").textContent = JSON.stringify(result, null, 2);
            } else {
                const result = await response.json();
                document.getElementById("retrieve-result").textContent = result.message || "Failed to retrieve person.";
            }
        } catch (error) {
            document.getElementById("retrieve-result").textContent = "Error occurred during retrieval.";
        }
    });
}

// Delete User (Admin Only)
if (document.getElementById("delete-btn")) {
    document.getElementById("delete-btn").addEventListener("click", async () => {
        const id = document.getElementById("delete-id").value;

        try {
            const response = await fetch(`${baseUrl}/Accounts/DeleteUser/${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
            });

            if (response.ok) {
                document.getElementById("delete-message").textContent = "User deleted successfully!";
            } else {
                const result = await response.json();
                document.getElementById("delete-message").textContent = result.message || "Failed to delete user.";
            }
        } catch (error) {
            document.getElementById("delete-message").textContent = "Error occurred while deleting user.";
        }
    });
}

// Helper Function: Convert Image to Base64
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]); // Remove metadata
        reader.onerror = error => reject(error);
        reader.readAsDataURL(file);
    });
}
