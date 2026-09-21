using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WillowV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "RecurringTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "RecurringTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
