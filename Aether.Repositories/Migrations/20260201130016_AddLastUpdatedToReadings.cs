using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aether.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedToReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_updated",
                table: "air_quality_readings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "air_quality_readings");
        }
    }
}
