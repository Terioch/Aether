using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aether.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddAqiToReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name='locations' AND column_name='name'
                    ) THEN
                        ALTER TABLE locations ADD COLUMN name text NOT NULL DEFAULT '';
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<int>(
                name: "aqi",
                table: "air_quality_readings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aqi",
                table: "air_quality_readings");
        }
    }
}
