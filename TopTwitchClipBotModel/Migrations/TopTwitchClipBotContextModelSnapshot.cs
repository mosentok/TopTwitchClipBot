﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotModel.Migrations
{
    [DbContext(typeof(TopTwitchClipBotContext))]
    partial class TopTwitchClipBotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TopTwitchClipBotModel.BroadcasterConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Broadcaster")
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("NumberOfClipsPerDay");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("BroadcasterConfig");
                });

            modelBuilder.Entity("TopTwitchClipBotModel.BroadcasterHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BroadcasterConfigId");

                    b.Property<string>("ClipUrl")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("Stamp");

                    b.HasKey("Id");

                    b.HasIndex("BroadcasterConfigId");

                    b.ToTable("BroadcasterHistory");
                });

            modelBuilder.Entity("TopTwitchClipBotModel.ChannelConfig", b =>
                {
                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("MaxPostingHour");

                    b.Property<int?>("MinPostingHour");

                    b.Property<int?>("NumberOfClipsAtATime");

                    b.Property<string>("Prefix")
                        .HasColumnType("varchar(4)");

                    b.HasKey("ChannelId");

                    b.ToTable("ChannelConfig");
                });

            modelBuilder.Entity("TopTwitchClipBotModel.BroadcasterConfig", b =>
                {
                    b.HasOne("TopTwitchClipBotModel.ChannelConfig", "ChannelConfig")
                        .WithMany("BroadcasterConfigs")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TopTwitchClipBotModel.BroadcasterHistory", b =>
                {
                    b.HasOne("TopTwitchClipBotModel.BroadcasterConfig", "BroadcasterConfig")
                        .WithMany("BroadcasterHistories")
                        .HasForeignKey("BroadcasterConfigId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
