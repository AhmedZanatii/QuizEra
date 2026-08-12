using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizEra.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.DataBase
{
    public class QuizEraDBContext: IdentityDbContext<ApplicationUser>
    {

        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamQuestions> ExamQuestions { get; set; }
        public DbSet<StudentExamAttempt> StudentExamAttempts { get; set; }
        public DbSet<StudentExamQuestionAnswer> StudentExamQuestionAnswers { get; set; }
        public QuizEraDBContext(DbContextOptions<QuizEraDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Primary Keys for entities where the ID name doesn't match standard EF Core naming (like Id)
            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentID);

            modelBuilder.Entity<Student>()
            .HasOne(s => s.AppUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Instructor>()
                .HasKey(i => i.InstructorID);

            modelBuilder.Entity<Instructor>()
            .HasOne(i => i.AppUser)
            .WithOne()
            .HasForeignKey<Instructor>(i => i.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseID);

            modelBuilder.Entity<Course>()
                .HasIndex(c => new { c.CourseName, c.InstructorID })
                 .IsUnique();

            modelBuilder.Entity<Topic>()
                .HasKey(t => t.TopicID);

            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedbackID);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Student)
                .WithMany(s => s.Feedbacks)
                .HasForeignKey(f => f.StudentID)
                .OnDelete(DeleteBehavior.Restrict); // يفضل Restrict عشان ميعملش Multiple Cascade Paths

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Course)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exam>()
                .HasKey(e => e.ExamID);

            modelBuilder.Entity<Question>()
                .HasKey(q => q.QuestionID);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Topic)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamQuestions>()
                .HasKey(eq => eq.ExamQID);

            modelBuilder.Entity<ExamQuestions>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionID);

            modelBuilder.Entity<ExamQuestions>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentExamAttempt>()
                .HasKey(sea => sea.StudExamID); 

            modelBuilder.Entity<StudentExamAttempt>()
                .HasOne(sea => sea.Exam)
                .WithMany(e => e.StudentExamAttempts)
                .HasForeignKey(sea => sea.ExamID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentExamAttempt>()
                .HasOne(sea => sea.Student)
                .WithMany(s => s.StudentExamAttempts)
                .HasForeignKey(sea => sea.StudentID)
                .OnDelete(DeleteBehavior.Restrict);


            // 2. Composite Primary Keys for Junction/Relationship tables
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentID, sc.CourseID });

            modelBuilder.Entity<StudentExamQuestionAnswer>()
                .HasKey(seqa => new { seqa.ExamQID, seqa.StudExamID });


            // 3. Relationships & Foreign Keys Configuration

            // StudentCourse relationships
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentID);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseID);

            // StudentExamQuestionAnswer relationships (Restricted to avoid multiple cascade paths)
            modelBuilder.Entity<StudentExamQuestionAnswer>()
                .HasOne(seqa => seqa.ExamQuestions)
                .WithMany(eq => eq.StudentExamQuestionAnswers)
                .HasForeignKey(seqa => seqa.ExamQID)
                .OnDelete(DeleteBehavior.Restrict); // <--- Prevents cascade conflict

            modelBuilder.Entity<StudentExamQuestionAnswer>()
                .HasOne(seqa => seqa.StudentExamAttempt)
                .WithMany(sea => sea.StudentExamQuestionAnswers)
                .HasForeignKey(seqa => seqa.StudExamID)
                .OnDelete(DeleteBehavior.Restrict); // <--- Prevents cascade conflict

            // Course -> Instructor relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorID)
                .OnDelete(DeleteBehavior.Restrict);


            // 4. Configure Enum mapping for DifficultyLevel to store as string
            modelBuilder.Entity<Question>()
                .Property(q => q.DifficultyLevel)
                .HasConversion<string>();
        }
    }
}
