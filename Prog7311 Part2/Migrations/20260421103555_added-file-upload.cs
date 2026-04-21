using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prog7311_Part2.Migrations
{
    /// <inheritdoc />
    public partial class addedfileupload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Contract",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Contract");
        }
    }
}
