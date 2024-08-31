using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Global.Motorcycle.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class insertplandata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""TB_PLAN"" (""ID"", ""NAME"", ""DAYS"", ""DAILY"", ""FEE_BEFORE"", ""FEE_AFTER"") VALUES 
                ('3fa85f64-5717-4562-b3fc-2c963f66afa6', '7 dias', 7, 30, 20, 50),
                ('4fa85f64-5717-4562-b3fc-2c963f66afa7', '15 dias', 15, 28, 40, 50),
                ('5fa85f64-5717-4562-b3fc-2c963f66afa8', '30 dias', 30, 22, 40, 50),
                ('6fa85f64-5717-4562-b3fc-2c963f66afa9', '45 dias', 45, 20, 40, 50),
                ('7fa85f64-5717-4562-b3fc-2c963f66afa0', '50 dias', 50, 18, 40, 50);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""TB_PLAN"" WHERE ID IN (
                    '3fa85f64-5717-4562-b3fc-2c963f66afa6',
                    '4fa85f64-5717-4562-b3fc-2c963f66afa7',
                    '5fa85f64-5717-4562-b3fc-2c963f66afa8',
                    '6fa85f64-5717-4562-b3fc-2c963f66afa9',
                    '7fa85f64-5717-4562-b3fc-2c963f66afa0'
                );");
        }
    }
}
