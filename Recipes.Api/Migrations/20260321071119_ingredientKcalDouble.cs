using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Api.Migrations
{
    /// <inheritdoc />
    public partial class ingredientKcalDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kcal",
                table: "Ingredients");

            migrationBuilder.AddColumn<double>(
                name: "Kcal100g",
                table: "Ingredients",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kcal100g",
                table: "Ingredients");

            migrationBuilder.AddColumn<int>(
                name: "Kcal",
                table: "Ingredients",
                type: "integer",
                nullable: true);
        }
    }
}
