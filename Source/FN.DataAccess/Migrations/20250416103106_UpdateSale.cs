using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 793, DateTimeKind.Utc).AddTicks(1706),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 14, 55, 4, 278, DateTimeKind.Utc).AddTicks(399));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 792, DateTimeKind.Local).AddTicks(5719),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 277, DateTimeKind.Local).AddTicks(5183));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4255),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 14, 55, 4, 276, DateTimeKind.Utc).AddTicks(4550));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4846),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 22, 14, 55, 4, 276, DateTimeKind.Utc).AddTicks(4964));

            migrationBuilder.AddColumn<string>(
                name: "BannerImage",
                table: "sale_events",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "sale_events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "sale_events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "sale_event_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 790, DateTimeKind.Utc).AddTicks(9464),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 14, 55, 4, 275, DateTimeKind.Utc).AddTicks(9074));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 788, DateTimeKind.Local).AddTicks(2119),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 273, DateTimeKind.Local).AddTicks(5158));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 787, DateTimeKind.Local).AddTicks(3598),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 272, DateTimeKind.Local).AddTicks(6983));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7852),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 272, DateTimeKind.Local).AddTicks(1437));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7342),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 272, DateTimeKind.Local).AddTicks(784));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 785, DateTimeKind.Local).AddTicks(8329),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 271, DateTimeKind.Local).AddTicks(2862));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 784, DateTimeKind.Local).AddTicks(4079),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 270, DateTimeKind.Local).AddTicks(197));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImage",
                table: "sale_events");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "sale_events");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "sale_events");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "sale_event_products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 14, 55, 4, 278, DateTimeKind.Utc).AddTicks(399),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 793, DateTimeKind.Utc).AddTicks(1706));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 277, DateTimeKind.Local).AddTicks(5183),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 792, DateTimeKind.Local).AddTicks(5719));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 14, 55, 4, 276, DateTimeKind.Utc).AddTicks(4550),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4255));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 22, 14, 55, 4, 276, DateTimeKind.Utc).AddTicks(4964),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4846));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 14, 55, 4, 275, DateTimeKind.Utc).AddTicks(9074),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 790, DateTimeKind.Utc).AddTicks(9464));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 273, DateTimeKind.Local).AddTicks(5158),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 788, DateTimeKind.Local).AddTicks(2119));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 272, DateTimeKind.Local).AddTicks(6983),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 787, DateTimeKind.Local).AddTicks(3598));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 272, DateTimeKind.Local).AddTicks(1437),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7852));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 272, DateTimeKind.Local).AddTicks(784),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7342));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 271, DateTimeKind.Local).AddTicks(2862),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 785, DateTimeKind.Local).AddTicks(8329));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 55, 4, 270, DateTimeKind.Local).AddTicks(197),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 784, DateTimeKind.Local).AddTicks(4079));
        }
    }
}
