using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Global.Motorcycle.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class locationaddcolumnstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "STATUS",
                table: "TB_LOCATION",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STATUS",
                table: "TB_LOCATION");
        }
    }
}
