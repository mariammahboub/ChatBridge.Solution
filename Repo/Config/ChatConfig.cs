using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repo.Config
{
    internal class ChatConfig : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasOne(p => p.AppUser)
                   .WithMany(a => a.ChatMessages)
                   .HasForeignKey(p => p.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ChatGroup)
                   .WithMany(g => g.ChatMessage)
                   .HasForeignKey(p => p.ChatGroupId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
