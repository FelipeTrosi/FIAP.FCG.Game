using FIAP.FCG.Game.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.FCG.Game.Infrastructure.Repository.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder.ToTable("Game");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(p => p.Code)
            .HasColumnType("bigint");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
