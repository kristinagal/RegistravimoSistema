const baseUrl = "https://localhost:7094/api";
let token = "";

// Login
document.getElementById("login-btn").addEventListener("click", async () => {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    try {
        const response = await fetch(`${baseUrl}/Accounts/Login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ username, password })
        });

        const result = await response.json();
        if (response.ok) {
            token = result.token;
            document.getElementById("login-message").textContent = "Login successful!";
            document.getElementById("person-section").classList.remove("hidden");
            document.getElementById("retrieve-section").classList.remove("hidden");
            document.getElementById("delete-section").classList.remove("hidden");
        } else {
            document.getElementById("login-message").textContent = result.message || "Login failed.";
        }
    } catch (error) {
        document.getElementById("login-message").textContent = "Error occurred during login.";
    }
});

// Create Person
document.getElementById("create-person-btn").addEventListener("click", async () => {
    const data = {
        vardas: document.getElementById("vardas").value,
        pavarde: document.getElementById("pavarde").value,
        asmensKodas: document.getElementById("asmens-kodas").value,
        telefonoNumeris: document.getElementById("telefono-numeris").value,
        elPastas: document.getElementById("el-pastas").value,
        miestas: document.getElementById("miestas").value,
        gatve: document.getElementById("gatve").value,
        namoNumeris: document.getElementById("namo-numeris").value,
        butoNumeris: document.getElementById("buto-numeris").value
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
            document.getElementById("person-message").textContent = "Person created successfully!";
        } else {
            const result = await response.json();
            document.getElementById("person-message").textContent = result.message || "Failed to create person.";
        }
    } catch (error) {
        document.getElementById("person-message").textContent = "Error occurred during person creation.";
    }
});

// Retrieve Person
document.getElementById("retrieve-btn").addEventListener("click", async () => {
    const id = document.getElementById("retrieve-id").value;

    try {
        const response = await fetch(`${baseUrl}/Person/${id}`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        const result = await response.json();
        if (response.ok) {
            document.getElementById("retrieve-result").textContent = JSON.stringify(result, null, 2);
        } else {
            document.getElementById("retrieve-result").textContent = result.message || "Failed to retrieve person.";
        }
    } catch (error) {
        document.getElementById("retrieve-result").textContent = "Error occurred while retrieving person.";
    }
});

// Delete User (Admin)
document.getElementById("delete-btn").addEventListener("click", async () => {
    const id = document.getElementById("delete-id").value;

    try {
        const response = await fetch(`${baseUrl}/Accounts/DeleteUser/${id}`, {
            method: "DELETE",
            headers: {
                Authorization: `Bearer ${token}`
            }
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


// Function to convert image to Base64
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(',')[1]); // Remove metadata
        reader.onerror = error => reject(error);
        reader.readAsDataURL(file);
    });
}

// Create Person with Profile Picture
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
                Authorization: `Bearer ${token}`
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
