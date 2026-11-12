using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceHistoryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_price_changes_product_id",
                table: "price_changes");

            migrationBuilder.CreateIndex(
                name: "ix_price_changes_product_id_changed_at",
                table: "price_changes",
                columns: new[] { "product_id", "changed_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_price_changes_product_id_changed_at",
                table: "price_changes");

            migrationBuilder.CreateIndex(
                name: "ix_price_changes_product_id",
                table: "price_changes",
                column: "product_id");
        }
    }
}
