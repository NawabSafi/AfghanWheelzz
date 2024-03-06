using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfghanWheelzz.Migrations
{
    /// <inheritdoc />
    public partial class carcounadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarCount",
                table: "AspNetUsers");
        }
    }
}
