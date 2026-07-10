using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddAiResolutionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_resolution_plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    incident_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    summary = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    probable_cause = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    corrective_action_draft = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    resolution_notes_draft = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    escalation_recommended = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    escalation_urgency = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    escalation_reason = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    model_provider = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    model_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    provider_metadata = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    generated_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    approved_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    approved_by = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    rejected_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    rejected_by = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    rejection_reason = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    final_corrective_action = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    final_resolution_notes = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ai_resolution_plans", x => x.id);
                    table.ForeignKey(
                        name: "f_k_ai_resolution_plans_incidents_incident_id",
                        column: x => x.incident_id,
                        principalTable: "incidents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_ai_resolution_plans_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ai_resolution_plan_required_evidences",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    value = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    ai_resolution_plan_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ai_resolution_plan_required_evidences", x => x.id);
                    table.ForeignKey(
                        name: "f_k_ai_resolution_plan_required_evidence_ai_resolution_plan_id",
                        column: x => x.ai_resolution_plan_id,
                        principalTable: "ai_resolution_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ai_resolution_plan_steps",
                columns: table => new
                {
                    sequence = table.Column<int>(type: "int", nullable: false),
                    ai_resolution_plan_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    rationale = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    expected_outcome = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ai_resolution_plan_steps", x => new { x.ai_resolution_plan_id, x.sequence });
                    table.ForeignKey(
                        name: "f_k_ai_resolution_plan_steps_ai_resolution_plan_id",
                        column: x => x.ai_resolution_plan_id,
                        principalTable: "ai_resolution_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ai_resolution_plan_uncertainty_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    value = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    ai_resolution_plan_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ai_resolution_plan_uncertainty_notes", x => x.id);
                    table.ForeignKey(
                        name: "f_k_ai_resolution_plan_uncertainty_notes_ai_resolution_plan_id",
                        column: x => x.ai_resolution_plan_id,
                        principalTable: "ai_resolution_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_ai_resolution_plan_required_evidences_ai_resolution_plan_id",
                table: "ai_resolution_plan_required_evidences",
                column: "ai_resolution_plan_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ai_resolution_plan_uncertainty_notes_ai_resolution_plan_id",
                table: "ai_resolution_plan_uncertainty_notes",
                column: "ai_resolution_plan_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ai_resolution_plans_incident_id",
                table: "ai_resolution_plans",
                column: "incident_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ai_resolution_plans_organization_id_incident_id_generated_at",
                table: "ai_resolution_plans",
                columns: new[] { "organization_id", "incident_id", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_ai_resolution_plans_organization_id_status",
                table: "ai_resolution_plans",
                columns: new[] { "organization_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_resolution_plan_required_evidences");

            migrationBuilder.DropTable(
                name: "ai_resolution_plan_steps");

            migrationBuilder.DropTable(
                name: "ai_resolution_plan_uncertainty_notes");

            migrationBuilder.DropTable(
                name: "ai_resolution_plans");
        }
    }
}
