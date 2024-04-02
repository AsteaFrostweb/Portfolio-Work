using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Highscores_WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedHighscoresAndMapsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Highscores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Player_Id = table.Column<int>(type: "int", nullable: false),
                    Map_Id = table.Column<int>(type: "int", nullable: false),
                    Fastest_Lap = table.Column<int>(type: "int", nullable: false),
                    Best_Combo_Score = table.Column<int>(type: "int", nullable: false),
                    Best_Combo_Time = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Highscores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Highscores");

            migrationBuilder.DropTable(
                name: "Maps");
        }
    }
}
