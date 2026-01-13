using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NivelAccesDate_CodeFirst.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    md5 = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    size = table.Column<int>(type: "int", nullable: false),
                    format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    limit = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    planId = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_plans",
                        column: x => x.planId,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "conversions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    imageId_from = table.Column<int>(type: "int", nullable: false),
                    imageId_to = table.Column<int>(type: "int", nullable: false),
                    datetime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversions", x => x.id);
                    table.ForeignKey(
                        name: "FK_conversions_images_imageId_from",
                        column: x => x.imageId_from,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversions_images_imageId_to",
                        column: x => x.imageId_to,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_conversions_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "plans",
                columns: new[] { "id", "limit", "name", "price" },
                values: new object[,]
                {
                    { 1, 10, "Free", 0 },
                    { 2, 1000, "Enterprise", 10 },
                    { 3, 1000, "Premium", 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_conversions_imageId_from",
                table: "conversions",
                column: "imageId_from");

            migrationBuilder.CreateIndex(
                name: "IX_conversions_imageId_to",
                table: "conversions",
                column: "imageId_to");

            migrationBuilder.CreateIndex(
                name: "IX_conversions_userId",
                table: "conversions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_users_planId",
                table: "users",
                column: "planId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conversions");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "plans");
        }
    }
}
