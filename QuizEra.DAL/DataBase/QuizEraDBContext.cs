using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizEra.DAL.Entities;

namespace QuizEra.DAL.DataBase
{
    public class QuizEraDBContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<ExamQuestions> ExamQuestions { get; set; }
        public DbSet<StudentExamAttempt> StudentExamAttempts { get; set; }
        public DbSet<StudentExamQuestionAnswer> StudentExamQuestionAnswers { get; set; }

        public QuizEraDBContext(
            DbContextOptions<QuizEraDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================
            // Student
            // =========================================

            modelBuilder.Entity<Student>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.AppUser)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================
            // Instructor
            // =========================================

            modelBuilder.Entity<Instructor>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.AppUser)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================
            // Course
            // =========================================

            modelBuilder.Entity<Course>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Course>()
                .HasIndex(c => new
                {
                    c.CourseName,
                    c.InstructorID
                })
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorID)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================
            // Topic
            // =========================================

            modelBuilder.Entity<Topic>()
                .HasKey(t => t.Id);


            // =========================================
            // Feedback
            // =========================================

            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Student)
                .WithMany(s => s.Feedbacks)
                .HasForeignKey(f => f.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Course)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CourseID)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================
            // Exam
            // =========================================

            modelBuilder.Entity<Exam>()
                .HasKey(e => e.Id);


            // =========================================
            // Question
            // =========================================

            modelBuilder.Entity<Question>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Topic)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .Property(q => q.DifficultyLevel)
                .HasConversion<string>();

            modelBuilder.Entity<Question>()
              .Property(q => q.QuestionFormat)
              .HasConversion<string>();

            // =========================================
            // QuestionOption
            // =========================================

            modelBuilder.Entity<QuestionOption>()
                .HasKey(qo => qo.Id);

            modelBuilder.Entity<QuestionOption>()
                .HasOne(qo => qo.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(qo => qo.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);// as if question is deleted delete its options too

            // =========================================
            // ExamQuestions
            // =========================================

            modelBuilder.Entity<ExamQuestions>()
                .HasKey(eq => eq.Id);

            modelBuilder.Entity<ExamQuestions>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamQuestions>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================
            // StudentExamAttempt
            // =========================================

            modelBuilder.Entity<StudentExamAttempt>()
                .HasKey(sea => sea.Id);

            modelBuilder.Entity<StudentExamAttempt>()
                .HasOne(sea => sea.Exam)
                .WithMany(e => e.StudentExamAttempts)
                .HasForeignKey(sea => sea.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentExamAttempt>()
                .HasOne(sea => sea.Student)
                .WithMany(s => s.StudentExamAttempts)
                .HasForeignKey(sea => sea.StudentId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================
            // StudentCourse
            // Composite Primary Key
            // =========================================

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new
                {
                    sc.StudentId,
                    sc.CourseId
                });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================
            // StudentExamQuestionAnswer
            // Composite Primary Key
            // =========================================

            modelBuilder.Entity<StudentExamQuestionAnswer>()
                .HasKey(seqa => new
                {
                    seqa.ExamQuestionsId,
                    seqa.StudentExamAttemptId
                });

            modelBuilder.Entity<StudentExamQuestionAnswer>()
                .HasOne(seqa => seqa.ExamQuestions)
                .WithMany(eq => eq.StudentExamQuestionAnswers)
                .HasForeignKey(seqa => seqa.ExamQuestionsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentExamQuestionAnswer>()
                .HasOne(seqa => seqa.StudentExamAttempt)
                .WithMany(sea => sea.StudentExamQuestionAnswers)
                .HasForeignKey(seqa => seqa.StudentExamAttemptId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}