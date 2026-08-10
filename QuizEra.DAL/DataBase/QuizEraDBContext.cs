using Microsoft.EntityFrameworkCore;
using QuizEra.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.DataBase
{
    public class QuizEraDBContext:DbContext
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

            modelBuilder.Entity<Instructor>()
                .HasKey(i => i.InstructorID);

            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseID);

            modelBuilder.Entity<Topic>()
                .HasKey(t => t.TopicID);

            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedbackID);

            modelBuilder.Entity<Exam>()
                .HasKey(e => e.ExamID);

            modelBuilder.Entity<Question>()
                .HasKey(q => q.QuestionID);

            modelBuilder.Entity<ExamQuestions>()
                .HasKey(eq => eq.ExamQID);

            modelBuilder.Entity<StudentExamAttempt>()
                .HasKey(sea => sea.StudExamID); // <--- This fixes your current error!


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
