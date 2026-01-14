using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journaway.Infrastructure.Persistence.Configurations;

public sealed class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    public void Configure(EntityTypeBuilder<RoomEntity> b)
    {
        b.ToTable("rooms");
        b.HasKey(x => x.Id);

        b.Property(x => x.HotelId).IsRequired();
        b.Property(x => x.RoomCode)
            .HasMaxLength(4)
            .IsRequired();
        b.Property(x => x.BedCount).IsRequired();

        // Uniqueness per hotel
        b.HasIndex(x => new { x.HotelId, x.RoomCode }).IsUnique();
    }
}