restrictAccess(["Admin"]);

document.getElementById("delete-btn")?.addEventListener("click", async () => {
    const id = document.getElementById("delete-id").value;

    try {
        const response = await fetch(`${baseUrl}/Accounts/DeleteUser/${id}`, {
            method: "DELETE",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) {
            alert("User deleted successfully!");
            document.getElementById("delete-id").value = "";
        } else {
            alert("Failed to delete user.");
        }
    } catch {
        alert("Error occurred while deleting user.");
    }
});
