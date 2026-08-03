using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class IndexSongsForPerf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Songs_Title",
                table: "Songs",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_Title",
                table: "Songs");
        }
    }
}
