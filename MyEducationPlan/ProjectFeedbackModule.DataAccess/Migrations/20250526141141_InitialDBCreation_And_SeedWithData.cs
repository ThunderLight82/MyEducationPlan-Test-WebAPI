using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectFeedbackModule.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBCreation_And_SeedWithData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternProjects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedBudget = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternProjects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "InternProjectFeedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternProjectFeedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_InternProjectFeedbacks_InternProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "InternProjects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InternProjects",
                columns: new[] { "ProjectId", "EstimatedBudget", "Name", "Owner" },
                values: new object[,]
                {
                    { 1, 1800, "Online Store", "Vitaliy Kovalchuk" },
                    { 2, 1000, "Find Missing People - Volunteer project", "Olexander Borozuy" }
                });

            migrationBuilder.InsertData(
                table: "InternProjectFeedbacks",
                columns: new[] { "FeedbackId", "Comment", "EmployeeName", "ProjectId", "Rating" },
                values: new object[,]
                {
                    { 1, "Тест фітбек №1", "Daniela Kilova", 1, 8 },
                    { 2, "Тест фітбек №2", "Ethan Douglass", 1, 6 },
                    { 3, "Тест фітбек волонтерка №1", "Olexander Borozuy", 2, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternProjectFeedbacks_ProjectId",
                table: "InternProjectFeedbacks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InternProjects_Name",
                table: "InternProjects",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternProjectFeedbacks");

            migrationBuilder.DropTable(
                name: "InternProjects");
        }
    }
}
