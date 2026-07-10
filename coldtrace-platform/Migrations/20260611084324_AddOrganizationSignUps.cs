using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationSignUps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    label = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_roles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    resource = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    action = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_role_permissions", x => new { x.role_id, x.id });
                    table.ForeignKey(
                        name: "f_k_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    uuid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    organization_user_id = table.Column<int>(type: "int", nullable: true),
                    first_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    last_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                    table.ForeignKey(
                        name: "f_k_users_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "label", "name" },
                values: new object[,]
                {
                    { 1, "Super Administrator", "super-admin" },
                    { 2, "Administrator", "administrator" },
                    { 3, "Operations Manager", "operations-manager" },
                    { 4, "Operator", "operator" },
                    { 5, "Auditor", "auditor" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "id", "role_id", "action", "description", "resource" },
                values: new object[,]
                {
                    { 1, 1, "manage", "roles-permissions.permissions.manage-administrators", "administrators" },
                    { 2, 1, "manage", "roles-permissions.permissions.manage-users", "users" },
                    { 3, 1, "manage", "roles-permissions.permissions.manage-assets", "assets" },
                    { 4, 1, "view", "roles-permissions.permissions.view-reports", "reports" },
                    { 5, 1, "update", "roles-permissions.permissions.resolve-alerts", "alerts" },
                    { 6, 1, "view", "roles-permissions.permissions.monitor-assets", "assets" },
                    { 7, 1, "view", "roles-permissions.permissions.read-only", "operations" },
                    { 2, 2, "manage", "roles-permissions.permissions.manage-users", "users" },
                    { 3, 2, "manage", "roles-permissions.permissions.manage-assets", "assets" },
                    { 4, 2, "view", "roles-permissions.permissions.view-reports", "reports" },
                    { 5, 2, "update", "roles-permissions.permissions.resolve-alerts", "alerts" },
                    { 6, 2, "view", "roles-permissions.permissions.monitor-assets", "assets" },
                    { 7, 2, "view", "roles-permissions.permissions.read-only", "operations" },
                    { 3, 3, "manage", "roles-permissions.permissions.manage-assets", "assets" },
                    { 4, 3, "view", "roles-permissions.permissions.view-reports", "reports" },
                    { 5, 3, "update", "roles-permissions.permissions.resolve-alerts", "alerts" },
                    { 5, 4, "update", "roles-permissions.permissions.resolve-alerts", "alerts" },
                    { 6, 4, "view", "roles-permissions.permissions.monitor-assets", "assets" },
                    { 4, 5, "view", "roles-permissions.permissions.view-reports", "reports" },
                    { 7, 5, "view", "roles-permissions.permissions.read-only", "operations" }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_organization_id",
                table: "users",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
