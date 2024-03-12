using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AfghanWheelzz.Migrations
{
    /// <inheritdoc />
    public partial class roleAssign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a449d13-d599-4545-8711-197906c1e999", null, "Admin", "ADMIN" },
                    { "1b81092f-fe66-4ec1-bd02-73df51ad416e", null, "Finance", "FINANCE" },
                    { "54145be1-7a88-4ad5-9983-bfaa2c963ec4", null, "Marketing", "MARKETING" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a449d13-d599-4545-8711-197906c1e999");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b81092f-fe66-4ec1-bd02-73df51ad416e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "54145be1-7a88-4ad5-9983-bfaa2c963ec4");
        }
    }
}
