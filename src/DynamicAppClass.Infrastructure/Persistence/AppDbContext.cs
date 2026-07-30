using DynamicAppClass.Domain.Entities.Contacts;
using DynamicAppClass.Domain.Entities.Core;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region Core
    public DbSet<Field> Fields => Set<Field>();
    public DbSet<Lookup> Lookups => Set<Lookup>();
    public DbSet<ClassType> ClassTypes => Set<ClassType>();
    public DbSet<ClassField> ClassFields => Set<ClassField>();
    public DbSet<ClassFieldLookup> ClassFieldLookups => Set<ClassFieldLookup>();
    public DbSet<ClassAction> ClassActions => Set<ClassAction>();
    public DbSet<ClassInstance> ClassInstances => Set<ClassInstance>();
    public DbSet<ClassInstanceFieldValue> ClassInstanceFieldValues => Set<ClassInstanceFieldValue>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<ClassTypeFeature> ClassTypeFeatures => Set<ClassTypeFeature>();
    #endregion

    #region Contacts
    public DbSet<ClassInstanceContact> ClassInstanceContacts => Set<ClassInstanceContact>();
    public DbSet<AllowedContact> AllowedContacts => Set<AllowedContact>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Core
        modelBuilder.Entity<ClassType>(entity =>
        {
            entity.HasKey(classType => classType.Id);
            entity.Property(classType => classType.Name).HasMaxLength(120).IsRequired();
            entity.Property(classType => classType.Description).HasMaxLength(1000);
            entity.HasMany(classType => classType.Fields).WithOne().HasForeignKey(field => field.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(classType => classType.Actions).WithOne().HasForeignKey(action => action.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(classType => classType.Features).WithOne().HasForeignKey(feature => feature.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClassField>(entity =>
        {
            entity.HasKey(field => field.Id);
            entity.Property(field => field.OverrideName).HasMaxLength(120);
            entity.HasOne(field => field.Field).WithMany().HasForeignKey(field => field.FieldId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(field => field.DependsOnClassField).WithMany().HasForeignKey(field => field.DependsOnClassFieldId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(field => field.Options).WithOne().HasForeignKey(option => option.ClassFieldId).OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(field => field.Label);
        });

        modelBuilder.Entity<ClassAction>(entity =>
        {
            entity.HasKey(action => action.Id);
            entity.Property(action => action.Name).HasMaxLength(80).IsRequired();
            entity.HasOne(action => action.AssignClassField).WithMany().HasForeignKey(action => action.AssignClassFieldId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(action => action.ConditionClassField).WithMany().HasForeignKey(action => action.ConditionClassFieldId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClassInstance>(entity =>
        {
            entity.HasKey(instance => instance.Id);
            entity.HasOne(instance => instance.ClassType).WithMany().HasForeignKey(instance => instance.ClassTypeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(instance => instance.FieldValues).WithOne().HasForeignKey(value => value.ClassInstanceId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClassInstanceFieldValue>(entity =>
        {
            entity.HasKey(value => value.Id);
            entity.Property(value => value.Value).HasMaxLength(4000);
            entity.HasOne(value => value.ClassField).WithMany().HasForeignKey(value => value.ClassFieldId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(field => field.Id);
            entity.Property(field => field.Name).HasMaxLength(120).IsRequired();
            entity.Property(field => field.FieldType).HasConversion<string>().HasMaxLength(40);
            entity.HasMany(field => field.Options).WithOne().HasForeignKey(option => option.FieldId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(lookup => lookup.Id);
            entity.Property(lookup => lookup.Value).HasMaxLength(120).IsRequired();
            entity.Property(lookup => lookup.Caption).HasMaxLength(400).IsRequired();
        });

        modelBuilder.Entity<ClassFieldLookup>(entity =>
        {
            entity.HasKey(option => option.Id);
            entity.Property(option => option.OverrideValue).HasMaxLength(120);
            entity.Property(option => option.OverrideCaption).HasMaxLength(400);
            entity.HasOne(option => option.Lookup).WithMany().HasForeignKey(option => option.LookupId).OnDelete(DeleteBehavior.Restrict);
            entity.Ignore(option => option.Value);
            entity.Ignore(option => option.Caption);
            entity.Ignore(option => option.SortOrder);
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(feature => feature.Id);
            entity.Property(feature => feature.Code).HasMaxLength(20).IsRequired();
            entity.Property(feature => feature.Name).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<ClassTypeFeature>(entity =>
        {
            entity.HasKey(classFeature => classFeature.Id);
            entity.HasOne(classFeature => classFeature.Feature).WithMany().HasForeignKey(classFeature => classFeature.FeatureId).OnDelete(DeleteBehavior.Restrict);
        });
        #endregion

        #region Contacts
        modelBuilder.Entity<ClassInstanceContact>(entity =>
{
    entity.HasKey(c => c.Id);
    entity.Property(c => c.ContactType).HasMaxLength(80).IsRequired();
    entity.Property(c => c.FirstName).HasMaxLength(80);
    entity.Property(c => c.LastName).HasMaxLength(80);
    entity.Property(c => c.EntityName).HasMaxLength(120);
    entity.Property(c => c.Phone).HasMaxLength(40);
    entity.Property(c => c.Email).HasMaxLength(120);
    entity.Property(c => c.Address1).HasMaxLength(120);
    entity.Property(c => c.Address2).HasMaxLength(120);
    entity.Property(c => c.City).HasMaxLength(80);
    entity.Property(c => c.ZipCode).HasMaxLength(20);
    entity.Property(c => c.Country).HasMaxLength(80);
    entity.Property(c => c.LicenseNumber).HasMaxLength(40);
    entity.HasOne(c => c.ClassInstance).WithMany().HasForeignKey(c => c.ClassInstanceId).OnDelete(DeleteBehavior.Cascade);
});

        modelBuilder.Entity<AllowedContact>(entity =>
        {
            entity.HasKey(ac => ac.Id);
            entity.Property(ac => ac.ContactTypeValue).HasMaxLength(40).IsRequired();
            entity.Property(ac => ac.ContactTypeCaption).HasMaxLength(120).IsRequired();
            entity.HasOne(ac => ac.ClassType).WithMany().HasForeignKey(ac => ac.ClassTypeId).OnDelete(DeleteBehavior.Cascade);
        });
        #endregion    

        }
    }
