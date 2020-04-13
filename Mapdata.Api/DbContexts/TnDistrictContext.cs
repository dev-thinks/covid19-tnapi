using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Mapdata.Api.Entities;

namespace Mapdata.Api.DbContexts
{
    public partial class TnDistrictContext : DbContext
    {
        public TnDistrictContext()
        {
        }

        public TnDistrictContext(DbContextOptions<TnDistrictContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<DailyData> DailyData { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<StateCumulative> StateCumulative { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Feedback).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<DailyData>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.District)
                    .WithMany(p => p.DailyData)
                    .HasForeignKey(d => d.DistrictId);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<StateCumulative>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
