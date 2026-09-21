using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionCommerciale.Shared.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPaiementEstEncaisse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstEncaisse",
                table: "PaiementsFournisseurs",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstEncaisse",
                table: "PaiementsBonPreparation",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstEncaisse",
                table: "Paiements",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstEncaisse",
                table: "PaiementsFournisseurs");

            migrationBuilder.DropColumn(
                name: "EstEncaisse",
                table: "PaiementsBonPreparation");

            migrationBuilder.DropColumn(
                name: "EstEncaisse",
                table: "Paiements");
        }
    }
}
