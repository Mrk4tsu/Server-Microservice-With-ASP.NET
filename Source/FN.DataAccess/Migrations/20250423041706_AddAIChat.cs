using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAIChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 4, 17, 5, 677, DateTimeKind.Utc).AddTicks(8063),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 824, DateTimeKind.Utc).AddTicks(7565));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 676, DateTimeKind.Local).AddTicks(7161),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 821, DateTimeKind.Local).AddTicks(8168));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 4, 17, 5, 675, DateTimeKind.Utc).AddTicks(287),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6344));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 30, 4, 17, 5, 675, DateTimeKind.Utc).AddTicks(850),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 28, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 4, 17, 5, 674, DateTimeKind.Utc).AddTicks(4966),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(1604));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 671, DateTimeKind.Local).AddTicks(1773),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 817, DateTimeKind.Local).AddTicks(3478));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 670, DateTimeKind.Local).AddTicks(1026),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 816, DateTimeKind.Local).AddTicks(4555));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 669, DateTimeKind.Local).AddTicks(4209),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8524));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 669, DateTimeKind.Local).AddTicks(3590),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8016));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 668, DateTimeKind.Local).AddTicks(2954),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 814, DateTimeKind.Local).AddTicks(9047));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 665, DateTimeKind.Local).AddTicks(8349),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 813, DateTimeKind.Local).AddTicks(4187));

            migrationBuilder.CreateTable(
                name: "chat_sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_sessions_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChatSessionId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SentAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_sessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "chat_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_ChatSessionId",
                table: "chat_messages",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_sessions_UserId",
                table: "chat_sessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropTable(
                name: "chat_sessions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 824, DateTimeKind.Utc).AddTicks(7565),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 4, 17, 5, 677, DateTimeKind.Utc).AddTicks(8063));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 821, DateTimeKind.Local).AddTicks(8168),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 676, DateTimeKind.Local).AddTicks(7161));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6344),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 4, 17, 5, 675, DateTimeKind.Utc).AddTicks(287));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 28, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(6830),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 30, 4, 17, 5, 675, DateTimeKind.Utc).AddTicks(850));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 15, 9, 27, 820, DateTimeKind.Utc).AddTicks(1604),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 4, 17, 5, 674, DateTimeKind.Utc).AddTicks(4966));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 817, DateTimeKind.Local).AddTicks(3478),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 671, DateTimeKind.Local).AddTicks(1773));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 816, DateTimeKind.Local).AddTicks(4555),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 670, DateTimeKind.Local).AddTicks(1026));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8524),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 669, DateTimeKind.Local).AddTicks(4209));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 815, DateTimeKind.Local).AddTicks(8016),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 669, DateTimeKind.Local).AddTicks(3590));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 814, DateTimeKind.Local).AddTicks(9047),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 668, DateTimeKind.Local).AddTicks(2954));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 22, 9, 27, 813, DateTimeKind.Local).AddTicks(4187),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 11, 17, 5, 665, DateTimeKind.Local).AddTicks(8349));
        }
    }
}
