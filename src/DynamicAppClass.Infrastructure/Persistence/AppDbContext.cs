using DynamicAppClass.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ClassType> ClassTypes => Set<ClassType>();
    public DbSet<ClassField> ClassFields => Set<ClassField>();
    public DbSet<ClassStatus> ClassStatuses => Set<ClassStatus>();
    public DbSet<ClassAction> ClassActions => Set<ClassAction>();
    public DbSet<ClassInstance> ClassInstances => Set<ClassInstance>();
    public DbSet<ClassInstanceFieldValue> ClassInstanceFieldValues => Set<ClassInstanceFieldValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClassType>(entity =>
        {
            entity.HasKey(classType => classType.Id);
            entity.Property(classType => classType.Name).HasMaxLength(120).IsRequired();
            entity.Property(classType => classType.Description).HasMaxLength(1000);
            entity.HasMany(classType => classType.Fields).WithOne().HasForeignKey(field => field.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(classType => classType.Statuses).WithOne().HasForeignKey(status => status.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(classType => classType.Actions).WithOne().HasForeignKey(action => action.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClassField>(entity =>
        {
            entity.HasKey(field => field.Id);
            entity.Property(field => field.Name).HasMaxLength(120).IsRequired();
            entity.Property(field => field.FieldType).HasConversion<string>().HasMaxLength(40);
            entity.Property(field => field.OptionsCsv).HasMaxLength(1000);
        });

        modelBuilder.Entity<ClassStatus>(entity =>
        {
            entity.HasKey(status => status.Id);
            entity.Property(status => status.Name).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<ClassAction>(entity =>
        {
            entity.HasKey(action => action.Id);
            entity.Property(action => action.Name).HasMaxLength(80).IsRequired();
            entity.HasOne(action => action.FromStatus).WithMany().HasForeignKey(action => action.FromStatusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(action => action.ToStatus).WithMany().HasForeignKey(action => action.ToStatusId).OnDelete(DeleteBehavior.Restrict);
            entity.Ignore(action => action.Transition);
        });

        modelBuilder.Entity<ClassInstance>(entity =>
        {
            entity.HasKey(instance => instance.Id);
            entity.Property(instance => instance.Title).HasMaxLength(200).IsRequired();
            entity.HasOne(instance => instance.ClassType).WithMany().HasForeignKey(instance => instance.ClassTypeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(instance => instance.CurrentStatus).WithMany().HasForeignKey(instance => instance.CurrentStatusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(instance => instance.FieldValues).WithOne().HasForeignKey(value => value.ClassInstanceId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClassInstanceFieldValue>(entity =>
        {
            entity.HasKey(value => value.Id);
            entity.Property(value => value.Value).HasMaxLength(4000);
            entity.HasOne(value => value.ClassField).WithMany().HasForeignKey(value => value.ClassFieldId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
