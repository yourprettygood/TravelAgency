﻿using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<CourseProgram> Programs { get; set; } = null!;
        public DbSet<ProgramTeacher> ProgramTeachers { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProgramTeacher>()
                .HasKey(pt => new { pt.ProgramId, pt.TeacherId });

            modelBuilder.Entity<ProgramTeacher>()
                .HasOne(pt => pt.Program)
                .WithMany(p => p.ProgramTeachers)
                .HasForeignKey(pt => pt.ProgramId);

            modelBuilder.Entity<ProgramTeacher>()
                .HasOne(pt => pt.Teacher)
                .WithMany(t => t.ProgramTeachers)
                .HasForeignKey(pt => pt.TeacherId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Program)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(e => e.ProgramId);
        }
    }
}