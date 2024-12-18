using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistravimoSistema.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("9ed92855-07f3-4fe7-b49a-ab8101a1c9be"));

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: new Guid("b214935c-302f-468d-a977-f825e1af21f6"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f7ff97f2-06dd-43cf-89f0-624dbd6af84c"));

            migrationBuilder.AlterColumn<string>(
                name: "ButoNumeris",
                table: "Addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "PasswordSalt", "Role", "Username" },
                values: new object[] { new Guid("8a8f4006-beb0-4110-9f3f-9be0b049a3d2"), new byte[] { 151, 169, 155, 59, 113, 42, 157, 71, 210, 120, 113, 230, 111, 230, 120, 166, 113, 25, 182, 168, 205, 193, 24, 132, 136, 101, 212, 186, 221, 2, 254, 158, 252, 71, 206, 220, 215, 98, 232, 219, 222, 157, 213, 246, 166, 51, 108, 204, 71, 238, 19, 186, 117, 0, 134, 156, 31, 237, 33, 251, 9, 114, 42, 133 }, new byte[] { 224, 159, 121, 119, 126, 207, 228, 77, 11, 216, 198, 49, 72, 190, 94, 210 }, "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "AsmensKodas", "ElPastas", "Pavarde", "ProfilioNuotrauka", "TelefonoNumeris", "UserId", "Vardas" },
                values: new object[] { new Guid("e66c7520-ba75-4dcc-b7b1-7df65d35fb85"), "19901010001", "admin@example.com", "User", new byte[0], "+37060012345", new Guid("8a8f4006-beb0-4110-9f3f-9be0b049a3d2"), "Admin" });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "ButoNumeris", "Gatve", "Miestas", "NamoNumeris", "PersonId" },
                values: new object[] { new Guid("71af4f5e-d83d-4244-899c-1e326f546d2c"), "101", "Gedimino pr.", "Vilnius", "1", new Guid("e66c7520-ba75-4dcc-b7b1-7df65d35fb85") });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_AsmensKodas",
                table: "Persons",
                column: "AsmensKodas",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Persons_AsmensKodas",
                table: "Persons");

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("71af4f5e-d83d-4244-899c-1e326f546d2c"));

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: new Guid("e66c7520-ba75-4dcc-b7b1-7df65d35fb85"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8a8f4006-beb0-4110-9f3f-9be0b049a3d2"));

            migrationBuilder.AlterColumn<string>(
                name: "ButoNumeris",
                table: "Addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "PasswordSalt", "Role", "Username" },
                values: new object[] { new Guid("f7ff97f2-06dd-43cf-89f0-624dbd6af84c"), new byte[] { 200, 177, 199, 83, 204, 62, 130, 173, 199, 213, 122, 219, 41, 175, 85, 210, 219, 246, 64, 240, 120, 163, 130, 131, 248, 62, 143, 182, 39, 60, 212, 154, 13, 128, 129, 102, 217, 109, 34, 149, 86, 19, 110, 40, 143, 4, 46, 16, 38, 114, 167, 35, 239, 139, 144, 25, 84, 153, 83, 112, 37, 28, 16, 81 }, new byte[] { 54, 0, 190, 59, 21, 27, 119, 77, 5, 249, 239, 184, 219, 2, 244, 244 }, "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "AsmensKodas", "ElPastas", "Pavarde", "ProfilioNuotrauka", "TelefonoNumeris", "UserId", "Vardas" },
                values: new object[] { new Guid("b214935c-302f-468d-a977-f825e1af21f6"), "19901010001", "admin@example.com", "User", new byte[0], "+37060012345", new Guid("f7ff97f2-06dd-43cf-89f0-624dbd6af84c"), "Admin" });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "ButoNumeris", "Gatve", "Miestas", "NamoNumeris", "PersonId" },
                values: new object[] { new Guid("9ed92855-07f3-4fe7-b49a-ab8101a1c9be"), "101", "Gedimino pr.", "Vilnius", "1", new Guid("b214935c-302f-468d-a977-f825e1af21f6") });
        }
    }
}
