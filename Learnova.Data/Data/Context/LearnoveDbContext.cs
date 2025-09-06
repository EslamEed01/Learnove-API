using Learnova.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Data.Context
{
    public class LearnoveDbContext : IdentityDbContext<AppUser, ApplicationRole, string>
    {

        public LearnoveDbContext(DbContextOptions<LearnoveDbContext> options)
            : base(options)
        {
        }


        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonVideo> LessonVideos { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<pdfContents> pdfContents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<PdfChunk> pdfChunks { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // course
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.CreatedCourses)
                 .HasForeignKey(c => c.CreatedBy);

            modelBuilder.Entity<Course>()
               .HasOne(c => c.Category)
               .WithMany(u => u.Courses)
                .HasForeignKey(c => c.CategoryId);


            // enrollment

            modelBuilder.Entity<Enrollment>()
                .HasOne(u => u.User)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<Enrollment>()
               .HasOne(u => u.Course)
               .WithMany(e => e.Enrollments)
               .HasForeignKey(u => u.CourseId);

            // lesson

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId);




            // order

            modelBuilder.Entity<Order>()
               .HasOne(o => o.User)
                 .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Order>(o => o.PaymentId);

            modelBuilder.Entity<OrderItem>()
    .HasOne(oi => oi.Order)
    .WithMany(o => o.OrderItems)
    .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Course)
                .WithMany()
                .HasForeignKey(oi => oi.CourseId);


            // Review relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure one review per user per course
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.CourseId })
                .IsUnique();


            modelBuilder.Entity<PdfChunk>()
               .Property(c => c.PageNumbers)
               .IsRequired();

            modelBuilder.Entity<PdfChunk>()
                .Property(c => c.TextContent)
                .IsRequired();

            modelBuilder.Entity<PdfChunk>()
                .Property(c => c.PineconeVectorId)
                .IsRequired(false);


            modelBuilder.Entity<pdfContents>()
                .Property(p => p.FileName)
                .IsRequired();

            modelBuilder.Entity<pdfContents>()
                .Property(p => p.FileUrl)
                .IsRequired();

            modelBuilder.Entity<pdfContents>()
                .Property(p => p.S3FileUrl)
                .IsRequired();

            modelBuilder.Entity<pdfContents>()
                .Property(p => p.UploadedById)
                .IsRequired();



            modelBuilder.Entity<IdentityRole>().HasData(
          new IdentityRole
          {
              Id = Guid.NewGuid().ToString(),
              Name = "instructor",
              NormalizedName = "instructor",
              ConcurrencyStamp = Guid.NewGuid().ToString()
          },
          new IdentityRole
          {
              Id = Guid.NewGuid().ToString(),
              Name = "Student",
              NormalizedName = "STUDENT",
              ConcurrencyStamp = Guid.NewGuid().ToString()
          },
          new IdentityRole
          {
              Id = Guid.NewGuid().ToString(),
              Name = "Admin",
              NormalizedName = "ADMIN",
              ConcurrencyStamp = Guid.NewGuid().ToString()
          }
      );
        }
    }
}
