using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restaurant_franchise.Migrations
{
    /// <inheritdoc />
    public partial class CatTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_CategoryKey",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_CategoryKey",
                table: "Categories",
                newName: "IX_Categories_CategoryKey");

            migrationBuilder.AddColumn<string>(
                name: "ProductCategory",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_CategoryKey",
                table: "Categories",
                column: "CategoryKey",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_CategoryKey",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ProductCategory",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Employee");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_CategoryKey",
                table: "Employee",
                newName: "IX_Employee_CategoryKey");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_CategoryKey",
                table: "Employee",
                column: "CategoryKey",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}
