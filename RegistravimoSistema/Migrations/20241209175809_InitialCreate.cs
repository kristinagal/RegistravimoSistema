using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistravimoSistema.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vardas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pavarde = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AsmensKodas = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false, comment: "Format: 1YYMMDDXXXX"),
                    TelefonoNumeris = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, comment: "Starts with '8' or '+370'"),
                    ElPastas = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfilioNuotrauka = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Miestas = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gatve = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamoNumeris = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ButoNumeris = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PersonId",
                table: "Addresses",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_UserId",
                table: "Persons",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
