using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260710163813_RestrictPasswordResetRequestUserDeletion")]
public partial class RestrictPasswordResetRequestUserDeletion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "f_k_password_reset_requests_users_user_id",
            table: "password_reset_requests",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "f_k_password_reset_requests_users_user_id",
            table: "password_reset_requests");
    }
}
