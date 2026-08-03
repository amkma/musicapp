using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MusicApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pop" },
                    { 2, "Rock" },
                    { 3, "Jazz" }
                });

            migrationBuilder.InsertData(
                table: "Singers",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://via.placeholder.com/150?text=Adele", "Adele" },
                    { 2, "https://via.placeholder.com/150?text=Ed+Sheeran", "Ed Sheeran" },
                    { 3, "https://via.placeholder.com/150?text=Aurora", "Aurora" },
                    { 4, "https://via.placeholder.com/150?text=Freddie+King", "Freddie King" }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "CategoryId", "SingerId", "Title" },
                values: new object[,]
                {
                    { 1, 1, 1, "Hello" },
                    { 2, 1, 1, "Rolling in Deep" },
                    { 3, 1, 2, "Shape of You" },
                    { 4, 1, 2, "Perfect" },
                    { 5, 2, 3, "Runaway" },
                    { 6, 2, 3, "Running to Sea" },
                    { 7, 3, 3, "Half Light" },
                    { 8, 2, 4, "Hide Away" },
                    { 9, 3, 4, "You Got What" },
                    { 10, 1, 1, "Someone Like You" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Singers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Singers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Singers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Singers",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
