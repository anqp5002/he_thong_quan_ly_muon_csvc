using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVC_PTIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "rooms");
        }
    }
}
