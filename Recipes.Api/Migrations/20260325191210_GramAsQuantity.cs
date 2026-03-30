using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Api.Migrations
{
    /// <inheritdoc />
    public partial class GramAsQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<double>(
                name: "Gram",
                table: "RecipeIngredients",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gram",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "RecipeIngredients",
                type: "text",
                nullable: true);
        }
    }
}
