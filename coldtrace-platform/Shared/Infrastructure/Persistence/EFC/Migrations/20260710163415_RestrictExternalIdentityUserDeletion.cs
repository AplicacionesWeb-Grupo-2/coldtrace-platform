using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260710163415_RestrictExternalIdentityUserDeletion")]
public partial class RestrictExternalIdentityUserDeletion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "i_x_external_identities_user_id",
            table: "external_identities",
            column: "user_id");

        migrationBuilder.AddForeignKey(
            name: "f_k_external_identities_users_user_id",
            table: "external_identities",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "f_k_external_identities_users_user_id",
            table: "external_identities");

        migrationBuilder.DropIndex(
            name: "i_x_external_identities_user_id",
            table: "external_identities");
    }
}
