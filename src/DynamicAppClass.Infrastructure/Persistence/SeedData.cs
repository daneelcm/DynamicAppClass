using DynamicAppClass.Domain.Entities.Contacts;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.ClassTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        var clx = new ClassType
        {
            Name = "CLX",
            Description = "Class X Application Process example."
        };

        var lookupPending = new Lookup { Value = "1", Caption = "Pending", SortOrder = 10 };
        var lookupIssued = new Lookup { Value = "2", Caption = "Issued", SortOrder = 20 };
        var lookupRejected = new Lookup { Value = "3", Caption = "Rejected", SortOrder = 30 };
        var lookupRevision = new Lookup { Value = "4", Caption = "Revision", SortOrder = 40 };

        var materialWood = new Lookup { Value = "1", Caption = "Wood", SortOrder = 10 };
        var materialBricks = new Lookup { Value = "2", Caption = "Bricks", SortOrder = 20 };
        var materialMetal = new Lookup { Value = "3", Caption = "Metal", SortOrder = 30 };

        var brickTypeConcrete = new Lookup { Value = "1", Caption = "Concrete", SortOrder = 10 };
        var brickTypeClay = new Lookup { Value = "2", Caption = "Clay", SortOrder = 20 };

        //var ticketField = new ClassField { Field = new Field { Name = "Title", FieldType = ClassFieldType.Sequence }, IsRequired = true, SortOrder = 0 };
        var titleField = new ClassField { Field = new Field { Name = "Title", FieldType = ClassFieldType.Text }, IsRequired = true, SortOrder = 10 };
        var descriptionField = new ClassField { OverrideName = "Work Description", Field = new Field { Name = "Description", FieldType = ClassFieldType.LongText }, IsRequired = true, SortOrder = 20 };
        var statusField = new ClassField { 
            Field = new Field { Name = "Status", FieldType = ClassFieldType.Select, Options = [ lookupPending, lookupIssued, lookupRejected, lookupRevision ] },
            IsRequired = true, IsHidden = true, DefaultValue = "1", SortOrder = 0, Options = [ 
                new() { Lookup = lookupPending },
                new() { Lookup = lookupIssued },
                new() { Lookup = lookupRejected }
            ]
        };
        var materialField = new ClassField {
            Field = new Field { Name = "Materials", FieldType = ClassFieldType.Select, Options = [ materialWood, materialBricks, materialMetal ] },
            IsRequired = false, SortOrder = 30, Options = [ 
                new() { Lookup = materialWood },
                new() { Lookup = materialBricks },
                new() { Lookup = materialMetal }
            ]
        };
        var brickTypeField = new ClassField {
            Field = new Field { Name = "Brick Type", FieldType = ClassFieldType.Select, Options = [ brickTypeConcrete, brickTypeClay ] },
            IsRequired = true, SortOrder = 40, DependsOnClassField = materialField, DependsOnClassFieldValue = "2", Options = [ 
                new() { Lookup = brickTypeConcrete },
                new() { Lookup = brickTypeClay }
            ]
        };
        clx.Fields.AddRange([statusField, titleField, descriptionField, materialField, brickTypeField]);
        clx.Actions.AddRange(
        [
            new ClassAction { Name = "Issue", AssignClassField = statusField, ValueToAssign = "2", ConditionClassField = statusField, ConditionValue = "1" },
            new ClassAction { Name = "Reject", AssignClassField = statusField, ValueToAssign = "3", ConditionClassField = statusField, ConditionValue = "1" },
            new ClassAction { Name = "Reopen", AssignClassField = statusField, ValueToAssign = "1", ConditionClassField = statusField, ConditionValue = "3" }
        ]);
        clx.Features.AddRange(
        [
            new ClassTypeFeature { Feature = new Feature { Name = "Address", Code = "address" }, Active = false },
            new ClassTypeFeature { Feature = new Feature { Name = "Contacts", Code = "contacts" }, Active = true },
            new ClassTypeFeature { Feature = new Feature { Name = "Documents", Code = "documents" }, Active = false }
        ]);

        var instance = new ClassInstance
        {
            ClassType = clx,
            FieldValues =
            [
                new ClassInstanceFieldValue { ClassField = titleField, Value = "Fence Fix" },
                new ClassInstanceFieldValue { ClassField = descriptionField, Value = "We are going to fix the fence." },
                new ClassInstanceFieldValue { ClassField = statusField, Value = "1" },
                new ClassInstanceFieldValue { ClassField = materialField, Value = "1" }
            ]
        };

        dbContext.ClassInstanceContacts.AddRange([
            new ClassInstanceContact { ClassInstance = instance, ContactType = "1", FirstName = "John", LastName = "Doe", Phone = "123-456-7890", Address1 = "123 Main St" },
            new ClassInstanceContact { ClassInstance = instance, ContactType = "2", FirstName = "Jane", LastName = "Smith", Address1 = "456 Elm St" }
        ]);
        dbContext.AllowedContacts.AddRange([
            new AllowedContact { ClassType = clx, ContactTypeValue = "1", ContactTypeCaption = "Aplicant", QuantityAllowed = 1, Required = true, QuantityRequired = 1, CanBeEntity = true, RequirePhone = true, RequireEmail = true, RequireAddress = true, RequireLicense = false },
            new AllowedContact { ClassType = clx, ContactTypeValue = "2", ContactTypeCaption = "Property Owner", QuantityAllowed = 3, Required = true, QuantityRequired = 1, CanBeEntity = true, RequirePhone = false, RequireEmail = false, RequireAddress = true, RequireLicense = false },
            new AllowedContact { ClassType = clx, ContactTypeValue = "3", ContactTypeCaption = "Contractor", QuantityAllowed = 1, Required = false, QuantityRequired = 1, CanBeEntity = true, RequirePhone = true, RequireEmail = false, RequireAddress = true, RequireLicense = true }
        ]);
        dbContext.ClassTypes.Add(clx);
        dbContext.ClassInstances.Add(instance);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
