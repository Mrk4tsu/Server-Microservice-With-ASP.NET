using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProp2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_items_ProductId",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_items_ProductId",
                table: "payments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 505, DateTimeKind.Local).AddTicks(9377),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 210, DateTimeKind.Local).AddTicks(1060));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 3, 30, 30, 505, DateTimeKind.Utc).AddTicks(4852),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 3, 7, 7, 209, DateTimeKind.Utc).AddTicks(7172));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 500, DateTimeKind.Local).AddTicks(3302),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 207, DateTimeKind.Local).AddTicks(7043));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 499, DateTimeKind.Local).AddTicks(5981),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(9535));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 499, DateTimeKind.Local).AddTicks(1156),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4792));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 499, DateTimeKind.Local).AddTicks(694),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4320));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 497, DateTimeKind.Local).AddTicks(8236),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 205, DateTimeKind.Local).AddTicks(2130));

            migrationBuilder.AddForeignKey(
                name: "FK_orders_product_details_ProductId",
                table: "orders",
                column: "ProductId",
                principalTable: "product_details",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_product_details_ProductId",
                table: "payments",
                column: "ProductId",
                principalTable: "product_details",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_product_details_ProductId",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_product_details_ProductId",
                table: "payments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 210, DateTimeKind.Local).AddTicks(1060),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 505, DateTimeKind.Local).AddTicks(9377));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 3, 7, 7, 209, DateTimeKind.Utc).AddTicks(7172),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 3, 30, 30, 505, DateTimeKind.Utc).AddTicks(4852));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 207, DateTimeKind.Local).AddTicks(7043),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 500, DateTimeKind.Local).AddTicks(3302));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(9535),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 499, DateTimeKind.Local).AddTicks(5981));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4792),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 499, DateTimeKind.Local).AddTicks(1156));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4320),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 499, DateTimeKind.Local).AddTicks(694));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 205, DateTimeKind.Local).AddTicks(2130),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 30, 30, 497, DateTimeKind.Local).AddTicks(8236));

            migrationBuilder.AddForeignKey(
                name: "FK_orders_items_ProductId",
                table: "orders",
                column: "ProductId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_items_ProductId",
                table: "payments",
                column: "ProductId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
