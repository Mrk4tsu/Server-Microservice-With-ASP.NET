using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTableProductOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 16, 57, 38, 384, DateTimeKind.Utc).AddTicks(7691),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 13, 2, 32, 293, DateTimeKind.Utc).AddTicks(1905));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 23, 57, 38, 382, DateTimeKind.Local).AddTicks(5118),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 20, 2, 32, 291, DateTimeKind.Local).AddTicks(8424));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 23, 57, 38, 382, DateTimeKind.Local).AddTicks(4623),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 20, 2, 32, 291, DateTimeKind.Local).AddTicks(8007));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 23, 57, 38, 381, DateTimeKind.Local).AddTicks(4155),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 20, 2, 32, 290, DateTimeKind.Local).AddTicks(8617));

            migrationBuilder.CreateTable(
                name: "product_owners",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_owners", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_product_owners_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_owners_items_ProductId",
                        column: x => x.ProductId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_product_owners_ProductId",
                table: "product_owners",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_owners");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 13, 2, 32, 293, DateTimeKind.Utc).AddTicks(1905),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 16, 57, 38, 384, DateTimeKind.Utc).AddTicks(7691));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 20, 2, 32, 291, DateTimeKind.Local).AddTicks(8424),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 23, 57, 38, 382, DateTimeKind.Local).AddTicks(5118));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 20, 2, 32, 291, DateTimeKind.Local).AddTicks(8007),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 23, 57, 38, 382, DateTimeKind.Local).AddTicks(4623));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 12, 20, 2, 32, 290, DateTimeKind.Local).AddTicks(8617),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 12, 23, 57, 38, 381, DateTimeKind.Local).AddTicks(4155));
        }
    }
}
