using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Global.Motorcycle.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class fixfklocationmotorcycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_LOCATION_TB_MOTORCYCLE_PLAN_ID",
                table: "TB_LOCATION");

            migrationBuilder.CreateIndex(
                name: "IX_TB_LOCATION_MOTORCYCLE_ID",
                table: "TB_LOCATION",
                column: "MOTORCYCLE_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_LOCATION_TB_MOTORCYCLE_MOTORCYCLE_ID",
                table: "TB_LOCATION",
                column: "MOTORCYCLE_ID",
                principalTable: "TB_MOTORCYCLE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_LOCATION_TB_MOTORCYCLE_MOTORCYCLE_ID",
                table: "TB_LOCATION");

            migrationBuilder.DropIndex(
                name: "IX_TB_LOCATION_MOTORCYCLE_ID",
                table: "TB_LOCATION");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_LOCATION_TB_MOTORCYCLE_PLAN_ID",
                table: "TB_LOCATION",
                column: "PLAN_ID",
                principalTable: "TB_MOTORCYCLE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
