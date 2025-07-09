using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoodTracking.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminPasswordHashSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaProtegida",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaProtegida",
                table: "Users");
        }
    }
}
