using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MiniLojaVirtual.Domain.Constants;
using MiniLojaVirtual.Domain.Models.Abstracts;

namespace MiniLojaVirtual.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
	public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
		where TEntity : Entity
	{
		_ = builder.HasKey(x => x.Id);

		_ = builder.Property(x => x.Id)
			.UseIdentityColumn(1);

		_ = builder.Property(x => x.Code)
			.HasColumnType(DomainConstants.CodeColumnTypeName)
			.HasDefaultValueSql(DomainConstants.UuidColumnDefaultValue)
			.ValueGeneratedOnAdd();

		_ = builder.HasIndex(x => x.Code).IsUnique();

		_ = builder.Property(x => x.CreatedAt)
			.HasColumnType(DomainConstants.DefaultDateTimeColumnTypeName)
			.IsRequired();

		_ = builder.Property(x => x.UpdatedAt)
			.HasColumnType(DomainConstants.DefaultDateTimeColumnTypeName);

		_ = builder.Property(x => x.IsDeleted)
			.HasColumnType(DomainConstants.DefaultBooleanColumnTypeName)
			.HasDefaultValue(false);

		_ = builder.HasQueryFilter(x => !x.IsDeleted);
	}
}