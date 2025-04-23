using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 824, DateTimeKind.Utc).AddTicks(7565),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 793, DateTimeKind.Utc).AddTicks(1706));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 821, DateTimeKind.Local).AddTicks(8168),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 792, DateTimeKind.Local).AddTicks(5719));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6344),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4255));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 28, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6830),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4846));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(1604),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 790, DateTimeKind.Utc).AddTicks(9464));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 817, DateTimeKind.Local).AddTicks(3478),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 788, DateTimeKind.Local).AddTicks(2119));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 816, DateTimeKind.Local).AddTicks(4555),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 787, DateTimeKind.Local).AddTicks(3598));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8524),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7852));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8016),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7342));

            migrationBuilder.AddColumn<string>(
                name: "Cover",
                table: "items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 814, DateTimeKind.Local).AddTicks(9047),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 785, DateTimeKind.Local).AddTicks(8329));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 813, DateTimeKind.Local).AddTicks(4187),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 784, DateTimeKind.Local).AddTicks(4079));

            migrationBuilder.AddColumn<string>(
                name: "Cover",
                table: "app_users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cover",
                table: "items");

            migrationBuilder.DropColumn(
                name: "Cover",
                table: "app_users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 793, DateTimeKind.Utc).AddTicks(1706),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 824, DateTimeKind.Utc).AddTicks(7565));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 792, DateTimeKind.Local).AddTicks(5719),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 821, DateTimeKind.Local).AddTicks(8168));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4255),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6344));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 10, 31, 5, 791, DateTimeKind.Utc).AddTicks(4846),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 28, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 10, 31, 5, 790, DateTimeKind.Utc).AddTicks(9464),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(1604));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 788, DateTimeKind.Local).AddTicks(2119),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 817, DateTimeKind.Local).AddTicks(3478));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 787, DateTimeKind.Local).AddTicks(3598),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 816, DateTimeKind.Local).AddTicks(4555));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7852),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8524));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 786, DateTimeKind.Local).AddTicks(7342),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8016));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 785, DateTimeKind.Local).AddTicks(8329),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 814, DateTimeKind.Local).AddTicks(9047));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 16, 17, 31, 5, 784, DateTimeKind.Local).AddTicks(4079),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 813, DateTimeKind.Local).AddTicks(4187));
        }
    }
}
