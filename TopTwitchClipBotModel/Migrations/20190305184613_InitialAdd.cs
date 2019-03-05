using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TopTwitchClipBotModel.Migrations
{
    public partial class InitialAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelConfig",
                columns: table => new
                {
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(4)", nullable: true),
                    MinPostingHour = table.Column<int>(nullable: true),
                    MaxPostingHour = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelConfig", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "BroadcasterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Broadcaster = table.Column<string>(type: "varchar(50)", nullable: true),
                    NumberOfClipsPerDay = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BroadcasterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BroadcasterConfig_ChannelConfig_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "ChannelConfig",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BroadcasterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BroadcasterConfigId = table.Column<int>(nullable: false),
                    Slug = table.Column<string>(type: "varchar(100)", nullable: true),
                    ClipUrl = table.Column<string>(type: "varchar(255)", nullable: true),
                    Stamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BroadcasterHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BroadcasterHistory_BroadcasterConfig_BroadcasterConfigId",
                        column: x => x.BroadcasterConfigId,
                        principalTable: "BroadcasterConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BroadcasterConfig_ChannelId",
                table: "BroadcasterConfig",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_BroadcasterHistory_BroadcasterConfigId",
                table: "BroadcasterHistory",
                column: "BroadcasterConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BroadcasterHistory");

            migrationBuilder.DropTable(
                name: "BroadcasterConfig");

            migrationBuilder.DropTable(
                name: "ChannelConfig");
        }
    }
}
