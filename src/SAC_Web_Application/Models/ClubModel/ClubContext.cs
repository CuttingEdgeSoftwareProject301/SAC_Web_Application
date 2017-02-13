using Microsoft.EntityFrameworkCore;
using SAC_Web_Application.Models.ClubModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Models.ClubModel
{
    public class ClubContext : DbContext
    {
        public ClubContext(DbContextOptions<ClubContext> options)
            : base(options)
        { }

        public DbSet<Members> Members { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MemberPayment> MemberPayments { get; set; }

        public DbSet<Coaches> Coaches { get; set; }
        public DbSet<Qualifications> Qualifications { get; set; }
        public DbSet<CoachQualification> CoachQualifications { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<Gender> Genders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region MemberPayemnts link table

            modelBuilder.Entity<MemberPayment>()
                .HasKey(p => new { p.MemberID, p.PaymentID });

            modelBuilder.Entity<MemberPayment>()
            .HasOne(mp => mp.Member)
            .WithMany(m => m.MemberPayments)
            .HasForeignKey(mp => mp.MemberID);

            modelBuilder.Entity<MemberPayment>()
                .HasOne(mp => mp.Payment)
                .WithMany(p => p.MemberPayments)
                .HasForeignKey(mp => mp.PaymentID);

            #endregion
            #region Coach Qualifications link table

            modelBuilder.Entity<CoachQualification>()
                .HasKey(c => new { c.CoachID, c.QualID});

            modelBuilder.Entity<CoachQualification>()
                .HasOne(cq => cq.coaches)
                .WithMany(c => c.coachQualifications)
                .HasForeignKey(cq => cq.CoachID);

            modelBuilder.Entity<CoachQualification>()
                .HasOne(cq => cq.qualifications)
                .WithMany(q => q.coachQualifications)
                .HasForeignKey(cq => cq.QualID);

            #endregion
        }

    }

}
