using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGeneratorAPI.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class releases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "AppFiles");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AppFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "AppFiles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Filename",
                table: "AppFiles",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseId",
                table: "AppFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "S3Id",
                table: "AppFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Runtime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.ReleaseId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFiles_ReleaseId",
                table: "AppFiles",
                column: "ReleaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppFiles_Releases_ReleaseId",
                table: "AppFiles",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "ReleaseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppFiles_Releases_ReleaseId",
                table: "AppFiles");

            migrationBuilder.DropTable(
                name: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_AppFiles_ReleaseId",
                table: "AppFiles");

            migrationBuilder.DropColumn(
                name: "ReleaseId",
                table: "AppFiles");

            migrationBuilder.DropColumn(
                name: "S3Id",
                table: "AppFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "AppFiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Filename",
                table: "AppFiles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AddColumn<string>(
                name: "Runtime",
                table: "AppFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "AppFiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
