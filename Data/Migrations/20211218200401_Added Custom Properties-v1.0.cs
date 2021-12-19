using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Data.Migrations
{
    public partial class AddedCustomPropertiesv10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeLibraryBranch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatronAccId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PatronAccId",
                table: "AspNetUsers",
                column: "PatronAccId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Patrons_PatronAccId",
                table: "AspNetUsers",
                column: "PatronAccId",
                principalTable: "Patrons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade,
                onUpdate: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Patrons_PatronAccId",
                table: "AspNetUsers");


            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PatronAccId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HomeLibraryBranch",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PatronAccId",
                table: "AspNetUsers");
        }
    }
}
