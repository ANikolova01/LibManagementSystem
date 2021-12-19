using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddedLibraryChanges11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AvailabilityStatuses_AvailabilityStatusId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AvailabilityStatusId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AvailabilityStatusId",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "AvailabilityStatus",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailabilityStatus",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "AvailabilityStatusId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AvailabilityStatusId",
                table: "Books",
                column: "AvailabilityStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AvailabilityStatuses_AvailabilityStatusId",
                table: "Books",
                column: "AvailabilityStatusId",
                principalTable: "AvailabilityStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
