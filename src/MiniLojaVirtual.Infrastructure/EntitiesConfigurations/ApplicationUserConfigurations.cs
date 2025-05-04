using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MiniLojaVirtual.Domain.Constants;
using MiniLojaVirtual.Domain.Models.Users.Constants;
using MiniLojaVirtual.Infrastructure.Entities;

namespace MiniLojaVirtual.Infrastructure.EntitiesConfigurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<UserEntity>
{
	public void Configure(EntityTypeBuilder<UserEntity> builder)
	{
		builder.ToTable(UserConstants.UserTableName);
		builder.HasKey(u => u.Id);

		builder.Property(u => u.Id)
			.IsRequired();

		builder.Property(u => u.Code)
			.HasColumnType(DomainConstants.CodeColumnTypeName)
			.IsRequired();

		builder.Property(u => u.UserName)
			.IsRequired()
			.HasMaxLength(UserConstants.UserNameMaxLength);

		builder.Property(u => u.NormalizedUserName)
			.IsRequired()
			.HasMaxLength(UserConstants.UserNameMaxLength);

		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(UserConstants.EmailMaxLength);

		builder.Property(u => u.NormalizedEmail)
			.IsRequired()
			.HasMaxLength(UserConstants.EmailMaxLength);

		builder.Property(u => u.PasswordHash)
			.HasMaxLength(UserConstants.PasswordHashMaxLength)
			.IsRequired();

		builder.Property(u => u.SecurityStamp)
			.IsRequired();

		builder.Property(u => u.Name)
			.IsRequired()
			.HasColumnType(DomainConstants.DefaultNVarcharColumnTypeName);

		builder.Property(u => u.CreatedAt)
			.HasColumnType(DomainConstants.DefaultDateTimeColumnTypeName)
			.IsRequired();

		builder.Property(u => u.UpdatedAt)
			.HasColumnType(DomainConstants.DefaultDateTimeColumnTypeName)
			.IsRequired(false);

		builder.Property(u => u.IsDeleted)
			.HasColumnType(DomainConstants.DefaultBooleanColumnTypeName)
			.IsRequired();

		builder.HasMany(x => x.Products)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(x => x.Categories)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}