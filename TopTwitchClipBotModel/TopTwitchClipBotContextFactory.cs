using Microsoft.EntityFrameworkCore;

namespace TopTwitchClipBotModel
{
    public class TopTwitchClipBotContextFactory : DesignTimeDbContextFactoryBase<TopTwitchClipBotContext>
    {
        protected override TopTwitchClipBotContext CreateNewInstance(DbContextOptions<TopTwitchClipBotContext> options)
        {
            return new TopTwitchClipBotContext(options);
        }
    }
}
