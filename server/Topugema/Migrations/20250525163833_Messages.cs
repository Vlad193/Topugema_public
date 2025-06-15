using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Topugema.Migrations
{
    /// <inheritdoc />
    public partial class Messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_Channel_ID_ID",
                table: "Messages",
                columns: new[] { "Channel_ID", "ID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_Channel_ID_ID",
                table: "Messages");
        }
    }
}
