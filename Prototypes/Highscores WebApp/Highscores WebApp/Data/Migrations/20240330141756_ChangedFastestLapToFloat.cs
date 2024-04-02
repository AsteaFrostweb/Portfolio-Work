using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Highscores_WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFastestLapToFloat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Fastest_Lap",
                table: "Highscores",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Fastest_Lap",
                table: "Highscores",
                type: "time",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
