using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class preference_description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Preferences",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Preferences");
        }
    }
}
