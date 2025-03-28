using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_app_users_AppUserId",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_AppUserId",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "payments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 307, DateTimeKind.Local).AddTicks(1294),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 712, DateTimeKind.Local).AddTicks(4081));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 2, 51, 39, 306, DateTimeKind.Utc).AddTicks(5934),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 25, 17, 43, 8, 712, DateTimeKind.Utc).AddTicks(202));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 303, DateTimeKind.Local).AddTicks(8053),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 709, DateTimeKind.Local).AddTicks(9897));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(8003),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 709, DateTimeKind.Local).AddTicks(2585));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(1170),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 708, DateTimeKind.Local).AddTicks(7925));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(309),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 708, DateTimeKind.Local).AddTicks(7481));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 300, DateTimeKind.Local).AddTicks(4167),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 707, DateTimeKind.Local).AddTicks(5568));

            migrationBuilder.CreateIndex(
                name: "IX_payments_UserId",
                table: "payments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_app_users_UserId",
                table: "payments",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_app_users_UserId",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_UserId",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "payments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 712, DateTimeKind.Local).AddTicks(4081),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 307, DateTimeKind.Local).AddTicks(1294));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 25, 17, 43, 8, 712, DateTimeKind.Utc).AddTicks(202),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 2, 51, 39, 306, DateTimeKind.Utc).AddTicks(5934));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 709, DateTimeKind.Local).AddTicks(9897),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 303, DateTimeKind.Local).AddTicks(8053));

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "payments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 709, DateTimeKind.Local).AddTicks(2585),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(8003));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 708, DateTimeKind.Local).AddTicks(7925),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(1170));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 708, DateTimeKind.Local).AddTicks(7481),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 302, DateTimeKind.Local).AddTicks(309));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 26, 0, 43, 8, 707, DateTimeKind.Local).AddTicks(5568),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 3, 27, 9, 51, 39, 300, DateTimeKind.Local).AddTicks(4167));

            migrationBuilder.CreateIndex(
                name: "IX_payments_AppUserId",
                table: "payments",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_app_users_AppUserId",
                table: "payments",
                column: "AppUserId",
                principalTable: "app_users",
                principalColumn: "Id");
        }
    }
}
