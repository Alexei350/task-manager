using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    password = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    salt = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    last_access = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    culture = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    observation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    time_spent = table.Column<TimeSpan>(type: "interval", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    completed_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task", x => x.id);
                    table.ForeignKey(
                        name: "fk_task_users_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_task_user_id",
                table: "task",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
