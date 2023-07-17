using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restaurant_franchise.Migrations
{
    /// <inheritdoc />
    public partial class func : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "related_tagsId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_related_tagsId",
                table: "Products",
                column: "related_tagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_related_tagsId",
                table: "Products",
                column: "related_tagsId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_related_tagsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_related_tagsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "related_tagsId",
                table: "Products");
        }
    }
}
