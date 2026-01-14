using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journaway.Infrastructure.Persistence.Configurations;

public sealed class RoomAssignmentRecordConfiguration : IEntityTypeConfiguration<RoomAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<RoomAssignmentEntity> b)
    {
        b.ToTable("room_assignments");

        b.HasKey(x => x.Id);

        b.Property(x => x.HotelId).IsRequired();
        b.Property(x => x.Date).IsRequired();

        // Relationships
        b.HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.TravelGroup)
            .WithMany()
            .HasForeignKey(x => x.TravelGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Traveller)
            .WithMany()
            .HasForeignKey(x => x.TravellerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        b.HasIndex(x => new { x.HotelId, x.Date });
        
        b.HasIndex(x => new { x.HotelId, x.TravelGroupId, x.Date });
        
        b.HasIndex(x => new { x.HotelId, x.RoomId, x.Date });
        
        b.HasIndex(x => new { x.HotelId, x.Date, x.TravellerId })
            .IsUnique();
    }
}