using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GadgetHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDistributorNameToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistributorName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "GadgetHub");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributorName",
                table: "Products");
        }
    }
}
