using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Aether.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class create_air_quality_readings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.CreateTable(
                name: "air_quality_readings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    location_id = table.Column<int>(type: "integer", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    sulfur_dioxide = table.Column<double>(type: "double precision", nullable: false),
                    nitrogen_oxide = table.Column<double>(type: "double precision", nullable: false),
                    nitrogen_dioxide = table.Column<double>(type: "double precision", nullable: false),
                    particulate_matter10 = table.Column<double>(type: "double precision", nullable: false),
                    particulate_matter2_5 = table.Column<double>(type: "double precision", nullable: false),
                    ozone = table.Column<double>(type: "double precision", nullable: false),
                    carbon_monoxide = table.Column<double>(type: "double precision", nullable: false),
                    ammonia = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_air_quality_readings", x => x.id);
                    table.ForeignKey(
                        name: "fk_air_quality_readings_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_air_quality_readings_location_id",
                table: "air_quality_readings",
                column: "location_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "air_quality_readings");
        }
    }
}
