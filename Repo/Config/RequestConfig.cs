using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repo.Config
{
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {        
                builder.HasKey(fr => fr.Id);
                builder.Property(fr => fr.SenderUserId).IsRequired();
                builder.Property(fr => fr.ReceiverUserId).IsRequired();
                builder.Property(fr => fr.Status).IsRequired();
                builder.Property(fr => fr.AppUserId).IsRequired(); // Ensure AppUserId is required

                builder.HasOne<AppUser>(fr => fr.AppUser)
                       .WithMany(u => u.FriendRequests)
                       .HasForeignKey(fr => fr.AppUserId)
                       .IsRequired();
            }
        }

    
}
