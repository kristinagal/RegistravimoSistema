restrictAccess(["Admin"]);

document.getElementById("delete-btn")?.addEventListener("click", async () => {
    const id = document.getElementById("delete-id").value;

    try {
        const response = await fetch(`${baseUrl}/Accounts/DeleteUser/${id}`, {
            method: "DELETE",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) {
            alert("Vartotojas sėkmingai ištrintas!");
            document.getElementById("delete-id").value = "";
        } else {
            alert("Nepavyko ištrinti vartotojo.");
        }
    } catch {
        alert("Įvyko klaida šalinant vartotoją.");
    }
});
