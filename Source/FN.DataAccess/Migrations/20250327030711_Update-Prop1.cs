using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProp1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 210, DateTimeKind.Local).AddTicks(1060),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 307, DateTimeKind.Local).AddTicks(1294));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 3, 7, 7, 209, DateTimeKind.Utc).AddTicks(7172),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 2, 51, 39, 306, DateTimeKind.Utc).AddTicks(5934));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 207, DateTimeKind.Local).AddTicks(7043),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 303, DateTimeKind.Local).AddTicks(8053));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(9535),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(8003));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4792),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(1170));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4320),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(309));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 205, DateTimeKind.Local).AddTicks(2130),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 300, DateTimeKind.Local).AddTicks(4167));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 307, DateTimeKind.Local).AddTicks(1294),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 210, DateTimeKind.Local).AddTicks(1060));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 2, 51, 39, 306, DateTimeKind.Utc).AddTicks(5934),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 3, 7, 7, 209, DateTimeKind.Utc).AddTicks(7172));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 303, DateTimeKind.Local).AddTicks(8053),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 207, DateTimeKind.Local).AddTicks(7043));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(8003),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(9535));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(1170),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4792));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(309),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 206, DateTimeKind.Local).AddTicks(4320));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 300, DateTimeKind.Local).AddTicks(4167),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 10, 7, 7, 205, DateTimeKind.Local).AddTicks(2130));
        }
    }
}
