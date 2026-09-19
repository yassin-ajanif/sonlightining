using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionCommerciale.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddProduitPpvRemise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Ppv",
                table: "Produits",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Remise",
                table: "Produits",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            // Keep existing purchase prices: no discount yet → Remise 0%, Ppv = PrixAchatHT
            migrationBuilder.Sql(
                """
                UPDATE "Produits"
                SET "Remise" = '0',
                    "Ppv" = "PrixAchatHT";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ppv",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Remise",
                table: "Produits");
        }
    }
}
