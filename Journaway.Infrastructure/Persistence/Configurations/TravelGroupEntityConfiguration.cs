using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journaway.Infrastructure.Persistence.Configurations;

public sealed class TravelGroupEntityConfiguration : IEntityTypeConfiguration<TravelGroupEntity>
{
    public void Configure(EntityTypeBuilder<TravelGroupEntity> b)
    {
        b.ToTable("travel_groups");
        b.HasKey(x => x.Id);

        b.Property(x => x.HotelId).IsRequired();
        b.Property(x => x.GroupId).HasMaxLength(6).IsRequired();
        b.Property(x => x.ArrivalDate).IsRequired();
        b.Property(x => x.ExpectedTravellerCount).IsRequired();

        b.HasIndex(x => new { x.HotelId, x.GroupId }).IsUnique();
        
        b.HasMany(x => x.Travellers)
            .WithOne(t => t.TravelGroup)
            .HasForeignKey(t => t.TravelGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}