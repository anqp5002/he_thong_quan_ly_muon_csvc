namespace CSVC_PTIT.Data.Configurations;

using CSVC_PTIT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Fluent API cho bảng users
/// Bảng phức tạp nhất nhóm 1: có 2 FK (role, department), soft delete filter, nhiều index
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("user_id");

        builder.Property(u => u.Username)
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("is_email_verified")
            .HasDefaultValue(false);

        builder.Property(u => u.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(u => u.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.BorrowerType)
            .HasColumnName("borrower_type")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.StudentCode)
            .HasColumnName("student_code")
            .HasMaxLength(20);

        builder.Property(u => u.EmployeeCode)
            .HasColumnName("employee_code")
            .HasMaxLength(20);

        builder.Property(u => u.ClassName)
            .HasColumnName("class_name")
            .HasMaxLength(30);

        builder.Property(u => u.IdentityNo)
            .HasColumnName("identity_no")
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(u => u.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        // FK: Role
        builder.Property(u => u.RoleId)
            .HasColumnName("role_id");

        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: Department (nullable)
        builder.Property(u => u.DepartmentId)
            .HasColumnName("department_id");

        builder.HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // ===== Indexes =====
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.StudentCode).HasFilter("[student_code] IS NOT NULL");
        builder.HasIndex(u => u.EmployeeCode).HasFilter("[employee_code] IS NOT NULL");

        // ===== Soft delete global filter =====
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
