// File: WebApplication1/WebApplication1/Models/Entities.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainTravelAgency.Models;

    // === USERS ===
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        // В БД поле называется "password" и хранит пароль в открытом виде
        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

    // === MESSAGES ===
    [Table("messages")]
    public class Message
    {
        [Key]
        [Column("message_id")]
        public long MessageId { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column("body")]
        public string Body { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Навигация к пользователю
        public User? User { get; set; }
    }

    // === TEACHERS ===
    [Table("teachers")]
    public class Teacher
    {
        [Key]
        [Column("teacher_id")]
        public long TeacherId { get; set; }

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Column("email")]
        public string? Email { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public ICollection<ProgramTeacher> ProgramTeachers { get; set; } = new List<ProgramTeacher>();
    }

    // === PROGRAMS (курсы) ===
    // Название класса НЕ "Program", чтобы не конфликтовать с Program.cs
    [Table("programs")]
    public class CourseProgram
    {
        [Key]
        [Column("program_id")]
        public long ProgramId { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("short_desc")]
        public string? ShortDescription { get; set; }

        [Required]
        [Column("duration_hours")]
        public int DurationHours { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        // В БД это enum difficulty_level (beginner/intermediate/advanced)
        // Тут храним как string с этими значениями
        [Required]
        [Column("difficulty")]
        public string Difficulty { get; set; } = "beginner";

        [Required]
        [Column("is_featured")]
        public bool IsFeatured { get; set; }

        [Required]
        [Column("popularity_score")]
        public int PopularityScore { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public ICollection<ProgramTeacher> ProgramTeachers { get; set; } = new List<ProgramTeacher>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

    // === PROGRAM_TEACHERS (M:N курсы ↔ учителя) ===
    [Table("program_teachers")]
    public class ProgramTeacher
    {
        [Column("program_id")]
        public long ProgramId { get; set; }

        [Column("teacher_id")]
        public long TeacherId { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        public CourseProgram? Program { get; set; }
        public Teacher? Teacher { get; set; }
    }

    // === ENROLLMENTS (запись пользователя на курс) ===
    [Table("enrollments")]
    public class Enrollment
    {
        [Key]
        [Column("enrollment_id")]
        public long EnrollmentId { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Required]
        [Column("program_id")]
        public long ProgramId { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "active";

        [Required]
        [Column("enrolled_at")]
        public DateTime EnrolledAt { get; set; }

        public User? User { get; set; }
        public CourseProgram? Program { get; set; }
    }