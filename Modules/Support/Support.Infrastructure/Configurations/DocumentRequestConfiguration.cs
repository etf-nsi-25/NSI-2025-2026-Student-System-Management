using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Support.Core.Entities;

namespace Support.Infrastructure.Configurations
{
	public class DocumentRequestConfiguration : IEntityTypeConfiguration<DocumentRequest>
	{
		public void Configure(EntityTypeBuilder<DocumentRequest> builder)
		{
			builder.ToTable("DocumentRequests");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.UserId).IsRequired();
			builder.Property(x => x.FacultyId).IsRequired();
			builder.Property(x => x.DocumentType).IsRequired();
			builder.Property(x => x.Status).IsRequired();

			builder.Property(x => x.CreatedAt).IsRequired();
		}
	}
}
