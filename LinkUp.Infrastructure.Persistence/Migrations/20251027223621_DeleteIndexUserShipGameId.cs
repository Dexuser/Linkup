using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUp.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIndexUserShipGameId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ships_GameId_PlayerId_Type",
                table: "Ships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ships_GameId_PlayerId_Type",
                table: "Ships",
                columns: new[] { "GameId", "PlayerId", "Type" },
                unique: true);
        }
    }
}
