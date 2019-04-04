using Microsoft.EntityFrameworkCore.Migrations;

namespace TopTwitchClipBotModel.Migrations
{
    public partial class ForClipOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClipOrder",
                table: "ChannelConfig",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClipOrder",
                table: "ChannelConfig");
        }
    }
}
