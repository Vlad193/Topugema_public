using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Topugema.Migrations
{
    /// <inheritdoc />
    public partial class attachments2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.AddColumn<ulong>(
                name: "attachment",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attachment",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    ID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Target_ID = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_Target_ID",
                table: "Attachments",
                column: "Target_ID");
        }
    }
}
