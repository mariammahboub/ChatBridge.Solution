using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repo.Config
{
    internal class GroupMembersConfig : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.HasOne(gm => gm.AppUser)
                   .WithMany(u => u.GroupMembers) // Assuming GroupMemberships is the correct property name
                   .HasForeignKey(gm => gm.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(gm => gm.ChatGroup)
                   .WithMany(g => g.GroupMember)
                   .HasForeignKey(gm => gm.ChatGroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
