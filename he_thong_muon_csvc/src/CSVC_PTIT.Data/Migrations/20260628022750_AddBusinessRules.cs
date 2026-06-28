using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVC_PTIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LateMinutes",
                table: "returns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "borrow_requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateMinutes",
                table: "returns");

            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "borrow_requests");
        }
    }
}
