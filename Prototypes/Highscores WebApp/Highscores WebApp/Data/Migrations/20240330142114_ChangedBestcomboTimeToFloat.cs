using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Highscores_WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBestcomboTimeToFloat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Best_Combo_Time",
                table: "Highscores",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Best_Combo_Time",
                table: "Highscores",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
