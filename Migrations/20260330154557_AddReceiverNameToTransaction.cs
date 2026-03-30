using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemweb.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiverNameToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Transactions");
        }
    }
}
