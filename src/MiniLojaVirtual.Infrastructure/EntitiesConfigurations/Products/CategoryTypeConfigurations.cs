using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MiniLojaVirtual.Domain.Models.Products.Constants;
using MiniLojaVirtual.Infrastructure.Entities.Products;
using MiniLojaVirtual.Infrastructure.Extensions;

namespace MiniLojaVirtual.Infrastructure.EntitiesConfigurations.Products;

public class CategoryTypeConfigurations : IEntityTypeConfiguration<CategoryEntity>
{
	public void Configure(EntityTypeBuilder<CategoryEntity> builder)
	{
		builder.ToTable(CategoryConstants.CategoryTableName);

		builder.ConfigureBaseEntity();

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(CategoryConstants.NameMaxLength);

		builder.Property(x => x.Description)
			.HasMaxLength(CategoryConstants.DescriptionMaxLength);

		builder.HasMany(x => x.Products)
			.WithOne(x => x.Category)
			.HasForeignKey(x => x.CategoryId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(x => x.User)
			.WithMany(x => x.Categories)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}