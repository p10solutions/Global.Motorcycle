using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Global.Motorcycle.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class initialstructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_MOTORCYCLE",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    MODEL = table.Column<string>(type: "varchar(200)", nullable: false),
                    PLATE = table.Column<string>(type: "varchar(10)", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DT_CREATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_UPDATE = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    STATUS = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_MOTORCYCLE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_PLAN",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    NAME = table.Column<string>(type: "varchar(200)", nullable: false),
                    DAYS = table.Column<int>(type: "integer", nullable: false),
                    DAILY = table.Column<double>(type: "double precision", nullable: false),
                    FEE_BEFORE = table.Column<double>(type: "numeric(10,2)", nullable: true),
                    FEE_AFTER = table.Column<double>(type: "numeric(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PLAN", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_LOCATION",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    DELIVERYMAN_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    PLAN_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    MOTORCYCLE_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AMOUNT = table.Column<double>(type: "numeric(10,2)", nullable: true),
                    DT_INITIAL = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_END = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PAID = table.Column<bool>(type: "boolean", nullable: true),
                    FEE = table.Column<double>(type: "numeric(10,2)", nullable: true),
                    DAYS_USE = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LOCATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TB_LOCATION_TB_MOTORCYCLE_PLAN_ID",
                        column: x => x.PLAN_ID,
                        principalTable: "TB_MOTORCYCLE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_LOCATION_TB_Plan_PLAN_ID",
                        column: x => x.PLAN_ID,
                        principalTable: "TB_PLAN",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_LOCATION_PLAN_ID",
                table: "TB_LOCATION",
                column: "PLAN_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_LOCATION");

            migrationBuilder.DropTable(
                name: "TB_MOTORCYCLE");

            migrationBuilder.DropTable(
                name: "TB_PLAN");
        }
    }
}
