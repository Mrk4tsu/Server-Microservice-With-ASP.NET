using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FN.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddForum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 270, DateTimeKind.Utc).AddTicks(5136),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 6, 59, 48, 810, DateTimeKind.Utc).AddTicks(1620));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 269, DateTimeKind.Local).AddTicks(8387),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 807, DateTimeKind.Local).AddTicks(7919));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 268, DateTimeKind.Utc).AddTicks(5952),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 6, 59, 48, 806, DateTimeKind.Utc).AddTicks(8399));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 22, 4, 40, 51, 268, DateTimeKind.Utc).AddTicks(6494),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 30, 6, 59, 48, 806, DateTimeKind.Utc).AddTicks(8830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 268, DateTimeKind.Utc).AddTicks(672),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 6, 59, 48, 806, DateTimeKind.Utc).AddTicks(4008));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 264, DateTimeKind.Local).AddTicks(9972),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 804, DateTimeKind.Local).AddTicks(630));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 264, DateTimeKind.Local).AddTicks(245),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 803, DateTimeKind.Local).AddTicks(2486));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 263, DateTimeKind.Local).AddTicks(3811),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 802, DateTimeKind.Local).AddTicks(6403));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 263, DateTimeKind.Local).AddTicks(3229),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 802, DateTimeKind.Local).AddTicks(5973));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 262, DateTimeKind.Local).AddTicks(3093),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 801, DateTimeKind.Local).AddTicks(8672));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 260, DateTimeKind.Local).AddTicks(6495),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 800, DateTimeKind.Local).AddTicks(5734));

            migrationBuilder.CreateTable(
                name: "topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLocked = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 271, DateTimeKind.Utc).AddTicks(2934)),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 271, DateTimeKind.Utc).AddTicks(3680)),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_topics_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "replies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 272, DateTimeKind.Utc).AddTicks(1076)),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 272, DateTimeKind.Utc).AddTicks(1762)),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_replies_app_users_UserId",
                        column: x => x.UserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_replies_replies_ParentId",
                        column: x => x.ParentId,
                        principalTable: "replies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_replies_topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_IsDeleted",
                table: "replies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_replies_ParentId",
                table: "replies",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_TopicId",
                table: "replies",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_UpdatedAt",
                table: "replies",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_UserId",
                table: "replies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_IsDeleted",
                table: "topics",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_IsLocked",
                table: "topics",
                column: "IsLocked");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_UpdatedAt",
                table: "topics",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_UserId",
                table: "topics",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "replies");

            migrationBuilder.DropTable(
                name: "topics");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_product_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 6, 59, 48, 810, DateTimeKind.Utc).AddTicks(1620),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 270, DateTimeKind.Utc).AddTicks(5136));

            migrationBuilder.AlterColumn<DateTime>(
                name: "InteractionDate",
                table: "user_blog_interactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 807, DateTimeKind.Local).AddTicks(7919),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 269, DateTimeKind.Local).AddTicks(8387));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 6, 59, 48, 806, DateTimeKind.Utc).AddTicks(8399),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 268, DateTimeKind.Utc).AddTicks(5952));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "sale_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 30, 6, 59, 48, 806, DateTimeKind.Utc).AddTicks(8830),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 22, 4, 40, 51, 268, DateTimeKind.Utc).AddTicks(6494));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "product_prices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 6, 59, 48, 806, DateTimeKind.Utc).AddTicks(4008),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 4, 40, 51, 268, DateTimeKind.Utc).AddTicks(672));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "payments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 804, DateTimeKind.Local).AddTicks(630),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 264, DateTimeKind.Local).AddTicks(9972));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "orders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 803, DateTimeKind.Local).AddTicks(2486),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 264, DateTimeKind.Local).AddTicks(245));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 802, DateTimeKind.Local).AddTicks(6403),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 263, DateTimeKind.Local).AddTicks(3811));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 802, DateTimeKind.Local).AddTicks(5973),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 263, DateTimeKind.Local).AddTicks(3229));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "feedbacks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 801, DateTimeKind.Local).AddTicks(8672),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 262, DateTimeKind.Local).AddTicks(3093));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "app_users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 23, 13, 59, 48, 800, DateTimeKind.Local).AddTicks(5734),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 5, 15, 11, 40, 51, 260, DateTimeKind.Local).AddTicks(6495));
        }
    }
}
