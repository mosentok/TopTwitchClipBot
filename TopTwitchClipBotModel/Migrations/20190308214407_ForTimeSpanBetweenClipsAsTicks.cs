using Microsoft.EntityFrameworkCore.Migrations;

namespace TopTwitchClipBotModel.Migrations
{
    public partial class ForTimeSpanBetweenClipsAsTicks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TimeSpanBetweenClipsAsTicks",
                table: "ChannelConfig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeSpanBetweenClipsAsTicks",
                table: "ChannelConfig");
        }
    }
}
