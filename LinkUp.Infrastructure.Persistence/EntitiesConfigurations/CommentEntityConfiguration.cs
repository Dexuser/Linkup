using LinkUp.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PostId).IsRequired();
        builder.Property(x => x.Text).IsRequired();
        builder.Property(x => x.AuthorId).IsRequired().HasMaxLength(450); // Identity usa este limite, lo copiare
        builder.Property(x => x.ParentCommentId).HasMaxLength(450); // comentario al que responde
        //builder.Property(x => x.ReplyToUserId); nullable
        
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
}