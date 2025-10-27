using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUp.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BattleshipsEntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendShip",
                table: "FriendShip");

            migrationBuilder.RenameTable(
                name: "FriendShip",
                newName: "FriendShips");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendShips",
                table: "FriendShips",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BattleshipGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Player1Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Player2Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentTurnPlayerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    WinnerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastMoveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleshipGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    AttackerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    IsHit = table.Column<bool>(type: "bit", nullable: false),
                    AttackTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attacks_BattleshipGames_GameId",
                        column: x => x.GameId,
                        principalTable: "BattleshipGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    StartX = table.Column<int>(type: "int", nullable: false),
                    StartY = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsSunk = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ships_BattleshipGames_GameId",
                        column: x => x.GameId,
                        principalTable: "BattleshipGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipId = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    IsHit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipPositions_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attacks_AttackTime",
                table: "Attacks",
                column: "AttackTime");

            migrationBuilder.CreateIndex(
                name: "IX_Attacks_GameId_AttackerId",
                table: "Attacks",
                columns: new[] { "GameId", "AttackerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attacks_GameId_AttackerId_X_Y",
                table: "Attacks",
                columns: new[] { "GameId", "AttackerId", "X", "Y" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attacks_GameId_X_Y",
                table: "Attacks",
                columns: new[] { "GameId", "X", "Y" });

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_CurrentTurnPlayerId",
                table: "BattleshipGames",
                column: "CurrentTurnPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_LastMoveDate",
                table: "BattleshipGames",
                column: "LastMoveDate");

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Player1Id",
                table: "BattleshipGames",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Player2Id",
                table: "BattleshipGames",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Status",
                table: "BattleshipGames",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ShipPositions_ShipId_IsHit",
                table: "ShipPositions",
                columns: new[] { "ShipId", "IsHit" });

            migrationBuilder.CreateIndex(
                name: "IX_ShipPositions_ShipId_X_Y",
                table: "ShipPositions",
                columns: new[] { "ShipId", "X", "Y" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipPositions_X_Y",
                table: "ShipPositions",
                columns: new[] { "X", "Y" });

            migrationBuilder.CreateIndex(
                name: "IX_Ships_GameId_PlayerId",
                table: "Ships",
                columns: new[] { "GameId", "PlayerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ships_GameId_PlayerId_Type",
                table: "Ships",
                columns: new[] { "GameId", "PlayerId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ships_IsSunk",
                table: "Ships",
                column: "IsSunk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attacks");

            migrationBuilder.DropTable(
                name: "ShipPositions");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "BattleshipGames");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendShips",
                table: "FriendShips");

            migrationBuilder.RenameTable(
                name: "FriendShips",
                newName: "FriendShip");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendShip",
                table: "FriendShip",
                column: "Id");
        }
    }
}
