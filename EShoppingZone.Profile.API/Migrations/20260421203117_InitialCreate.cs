using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EShoppingZone.Profile.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MobileNumber = table.Column<long>(type: "bigint", nullable: false),
                    About = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.ProfileId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StreetName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ColonyName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Pincode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_UserProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "ProfileId", "About", "DateOfBirth", "EmailId", "FullName", "Gender", "Image", "MobileNumber", "Password", "Role" },
                values: new object[] { 1, null, null, "admin@eshoppingzone.com", "Platform Admin", null, null, 9000000000L, "AQAAAAIAAYagAAAAEBftsdtAWoBp1cN0zoBFPk62QTwR16lTEVUUHQv+a56x/vlRtBGvgU0B2n9y/eTl7w==", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProfileId",
                table: "Addresses",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EmailId",
                table: "UserProfiles",
                column: "EmailId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
