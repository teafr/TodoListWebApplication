using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Database.Migrations
{
    public partial class AddEditorsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "editors",
                table: "todo_lists",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "editors",
                table: "todo_lists");
        }
    }
}
