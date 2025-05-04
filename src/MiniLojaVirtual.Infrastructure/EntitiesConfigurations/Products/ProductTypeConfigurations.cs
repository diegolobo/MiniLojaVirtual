using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MiniLojaVirtual.Domain.Models.Products.Constants;
using MiniLojaVirtual.Infrastructure.Entities.Products;
using MiniLojaVirtual.Infrastructure.Extensions;

namespace MiniLojaVirtual.Infrastructure.EntitiesConfigurations.Products;

public class ProductTypeConfigurations : IEntityTypeConfiguration<ProductEntity>
{
	public void Configure(EntityTypeBuilder<ProductEntity> builder)
	{
		builder.ToTable(ProductConstants.ProductTableName);

		builder.ConfigureBaseEntity();

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(ProductConstants.NameMaxLength);

		builder.Property(x => x.Description)
			.HasMaxLength(ProductConstants.DescriptionMaxLength);

		builder.Property(x => x.Price)
			.IsRequired()
			.HasPrecision(18, 2);

		builder.Property(x => x.Stock)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(x => x.ImageUrl)
			.HasMaxLength(ProductConstants.ImagemUrlMaxLength);

		builder.HasOne(x => x.Category)
			.WithMany(x => x.Products)
			.HasForeignKey(x => x.CategoryId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(x => x.User)
			.WithMany(x => x.Products)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}