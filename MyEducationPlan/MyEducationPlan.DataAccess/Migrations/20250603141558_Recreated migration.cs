using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEducationPlan.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Recreatedmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InternProjects_Name",
                table: "InternProjects");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InternProjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "InternProjectFeedbacks",
                keyColumn: "FeedbackId",
                keyValue: 3,
                columns: new[] { "Comment", "EmployeeName", "ProjectId", "Rating" },
                values: new object[] { ">>Негативний фітбек №1<<", "Hola Rega", 1, 2 });

            migrationBuilder.InsertData(
                table: "InternProjectFeedbacks",
                columns: new[] { "FeedbackId", "Comment", "EmployeeName", "ProjectId", "Rating" },
                values: new object[] { 4, "Тест фітбек волонтерка №1", "Olexander Borozuy", 2, 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InternProjectFeedbacks",
                keyColumn: "FeedbackId",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InternProjects",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "InternProjectFeedbacks",
                keyColumn: "FeedbackId",
                keyValue: 3,
                columns: new[] { "Comment", "EmployeeName", "ProjectId", "Rating" },
                values: new object[] { "Тест фітбек волонтерка №1", "Olexander Borozuy", 2, 10 });

            migrationBuilder.CreateIndex(
                name: "IX_InternProjects_Name",
                table: "InternProjects",
                column: "Name",
                unique: true);
        }
    }
}
