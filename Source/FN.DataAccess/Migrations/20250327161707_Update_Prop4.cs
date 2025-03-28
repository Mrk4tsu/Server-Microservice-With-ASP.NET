using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_Prop4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 324, DateTimeKind.Local).AddTicks(8855),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 390, DateTimeKind.Local).AddTicks(4889));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 16, 17, 7, 324, DateTimeKind.Utc).AddTicks(5082),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 15, 37, 40, 389, DateTimeKind.Utc).AddTicks(9959));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 322, DateTimeKind.Local).AddTicks(7037),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 383, DateTimeKind.Local).AddTicks(941));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 321, DateTimeKind.Local).AddTicks(9893),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 381, DateTimeKind.Local).AddTicks(9596));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 321, DateTimeKind.Local).AddTicks(5333),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 381, DateTimeKind.Local).AddTicks(2786));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 321, DateTimeKind.Local).AddTicks(4913),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 381, DateTimeKind.Local).AddTicks(2279));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 319, DateTimeKind.Local).AddTicks(5014),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 378, DateTimeKind.Local).AddTicks(9014));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 390, DateTimeKind.Local).AddTicks(4889),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 324, DateTimeKind.Local).AddTicks(8855));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 15, 37, 40, 389, DateTimeKind.Utc).AddTicks(9959),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 16, 17, 7, 324, DateTimeKind.Utc).AddTicks(5082));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 383, DateTimeKind.Local).AddTicks(941),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 322, DateTimeKind.Local).AddTicks(7037));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 381, DateTimeKind.Local).AddTicks(9596),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 321, DateTimeKind.Local).AddTicks(9893));

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 381, DateTimeKind.Local).AddTicks(2786),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 321, DateTimeKind.Local).AddTicks(5333));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 381, DateTimeKind.Local).AddTicks(2279),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 321, DateTimeKind.Local).AddTicks(4913));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 22, 37, 40, 378, DateTimeKind.Local).AddTicks(9014),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 23, 17, 7, 319, DateTimeKind.Local).AddTicks(5014));
        }
    }
}
