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
                    ChannelId = table.Column<decimal>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    MinPostingHour = table.Column<int>(nullable: true),
                    MaxPostingHour = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelConfig", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "ChannelTopClipConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<decimal>(nullable: false),
                    Broadcaster = table.Column<string>(nullable: true),
                    NumberOfClipsPerDay = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelTopClipConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelTopClipConfig_ChannelConfig_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "ChannelConfig",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopClipHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<decimal>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    ClipUrl = table.Column<string>(nullable: true),
                    Stamp = table.Column<DateTime>(nullable: false),
                    ChannelTopClipConfigId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopClipHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopClipHistory_ChannelTopClipConfig_ChannelTopClipConfigId",
                        column: x => x.ChannelTopClipConfigId,
                        principalTable: "ChannelTopClipConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelTopClipConfig_ChannelId",
                table: "ChannelTopClipConfig",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_TopClipHistory_ChannelTopClipConfigId",
                table: "TopClipHistory",
                column: "ChannelTopClipConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopClipHistory");

            migrationBuilder.DropTable(
                name: "ChannelTopClipConfig");

            migrationBuilder.DropTable(
                name: "ChannelConfig");
        }
    }
}
