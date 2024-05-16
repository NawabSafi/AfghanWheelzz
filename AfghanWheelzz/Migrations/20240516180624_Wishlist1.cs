using Microsoft.EntityFrameworkCore.Migrations;

namespace AfghanWheelzz.Migrations
{
    public partial class Wishlist1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the following line as WishlistCars table is already dropped in Down method
            // migrationBuilder.DropTable(name: "WishlistCars");

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Wishlists_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_CarId",
                table: "Wishlists",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the following block as WishlistCars table is already dropped in Up method
            // migrationBuilder.CreateTable(
            //    name: "WishlistCars",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CarId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WishlistCars", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_WishlistCars_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_WishlistCars_Cars_CarId",
            //            column: x => x.CarId,
            //            principalTable: "Cars",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            // migrationBuilder.CreateIndex(
            //    name: "IX_WishlistCars_CarId",
            //    table: "WishlistCars",
            //    column: "CarId");

            // migrationBuilder.CreateIndex(
            //    name: "IX_WishlistCars_UserId",
            //    table: "WishlistCars",
            //    column: "UserId");

            // Remove the following line as WishlistCars table is already dropped in Up method
            // migrationBuilder.DropTable(name: "Wishlists");
        }
    }
}
