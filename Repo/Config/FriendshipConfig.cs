using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repo.Config
{
    public class FriendshipConfig : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(fr => fr.Id);
            builder.HasOne<AppUser>(f => f.User1) 
                   .WithMany(u => u.Friendships) 
                   .HasForeignKey(f => f.User1Id) 
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne<AppUser>(f => f.User2) 
                   .WithMany() 
                   .HasForeignKey(f => f.User2Id) 
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
