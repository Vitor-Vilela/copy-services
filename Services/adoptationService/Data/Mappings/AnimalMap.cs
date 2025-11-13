using AnimalCatalog.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalCatalog.API.Data.Mappings
{
    public class AnimalMap : IEntityTypeConfiguration<Animal>
    {
        public void Configure(EntityTypeBuilder<Animal> builder)
        {
            builder.ToTable("Animal");

            builder.HasKey(a => a.Id).HasName("pk_animal");

            builder.Property(a => a.InstitutionId).IsRequired();
            builder.Property(a => a.Name).HasColumnType("varchar(100)").IsRequired();
            builder.Property(a => a.Sex).IsRequired();
            builder.Property(a => a.PetSize).IsRequired();
            builder.Property(a => a.BirthDate).HasColumnType("date").IsRequired();
            builder.Property(a => a.RescueDate).HasColumnType("date").IsRequired();
            builder.Property(a => a.Breed).HasColumnType("varchar(50)").IsRequired();
            builder.Property(a => a.Species).IsRequired();
            builder.Property(a => a.Description).HasColumnType("varchar(255)");
            builder.Property(a => a.AnimalPic).HasColumnType("longblob");
            builder.Property(a => a.Status);
        }
    }
}