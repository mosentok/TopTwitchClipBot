using Microsoft.EntityFrameworkCore.Migrations;

namespace TopTwitchClipBotModel.Migrations
{
    public partial class ForMinViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GlobalMinViews",
                table: "ChannelConfig",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinViews",
                table: "BroadcasterConfig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalMinViews",
                table: "ChannelConfig");

            migrationBuilder.DropColumn(
                name: "MinViews",
                table: "BroadcasterConfig");
        }
    }
}
