using GameProject.Data.Models.Game;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameProject.Data.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.HasOne<Word>().WithMany(x => x.Matches).HasForeignKey(x => x.WordId);
            builder.HasOne(x => x.User).WithMany(x => x.Matches).HasForeignKey(x => x.UserId);
        }
    }
}
