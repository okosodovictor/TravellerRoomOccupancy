using Journaway.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journaway.Infrastructure.Persistence.Configurations;

public sealed class TravellerEntityConfiguration : IEntityTypeConfiguration<TravellerEntity>
{
    public void Configure(EntityTypeBuilder<TravellerEntity> b)
    {
        b.ToTable("travellers");

        b.HasKey(x => x.Id);

        b.Property(x => x.HotelId).IsRequired();
        b.Property(x => x.Surname).HasMaxLength(200).IsRequired();
        b.Property(x => x.FirstName).HasMaxLength(200).IsRequired();
        b.Property(x => x.DateOfBirth).IsRequired();

        // Prevent duplicates within a group by traveller natural identity
        b.HasIndex(x => new { x.TravelGroupId, x.Surname, x.FirstName, x.DateOfBirth }).IsUnique();
    }
}
