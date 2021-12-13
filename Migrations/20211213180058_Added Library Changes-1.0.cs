using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryManagementSystem.Migrations
{
    public partial class AddedLibraryChanges10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoOfPages",
                table: "Books",
                newName: "NoOfPages_LengthTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoOfPages_LengthTime",
                table: "Books",
                newName: "NoOfPages");
        }
    }
}
