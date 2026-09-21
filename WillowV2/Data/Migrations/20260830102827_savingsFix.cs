using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WillowV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class savingsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentAmmount",
                table: "Savings",
                newName: "CurrentAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentAmount",
                table: "Savings",
                newName: "CurrentAmmount");
        }
    }
}
