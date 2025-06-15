using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Topugema.Migrations
{
    /// <inheritdoc />
    public partial class attch3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Owner_ID",
                table: "Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner_ID",
                table: "Servers");
        }
    }
}
