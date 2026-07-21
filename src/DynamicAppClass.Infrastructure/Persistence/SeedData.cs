using DynamicAppClass.Domain.Entities;
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

        var supportTicket = new ClassType
        {
            Name = "Support Ticket",
            Description = "Tracks internal support requests from intake through closure."
        };

        var titleField = new ClassField { ClassTypeId = supportTicket.Id, Name = "Title", FieldType = ClassFieldType.Text, IsRequired = true, SortOrder = 10 };
        var descriptionField = new ClassField { ClassTypeId = supportTicket.Id, Name = "Description", FieldType = ClassFieldType.LongText, IsRequired = true, SortOrder = 20 };
        var priorityField = new ClassField { ClassTypeId = supportTicket.Id, Name = "Priority", FieldType = ClassFieldType.Select, IsRequired = true, SortOrder = 30, OptionsCsv = "Low|Medium|High" };
        var requestedByField = new ClassField { ClassTypeId = supportTicket.Id, Name = "Requested By", FieldType = ClassFieldType.Text, IsRequired = true, SortOrder = 40 };
        supportTicket.Fields.AddRange([titleField, descriptionField, priorityField, requestedByField]);

        var newStatus = new ClassStatus { ClassTypeId = supportTicket.Id, Name = "New", SortOrder = 10 };
        var inProgressStatus = new ClassStatus { ClassTypeId = supportTicket.Id, Name = "In Progress", SortOrder = 20 };
        var resolvedStatus = new ClassStatus { ClassTypeId = supportTicket.Id, Name = "Resolved", SortOrder = 30 };
        var closedStatus = new ClassStatus { ClassTypeId = supportTicket.Id, Name = "Closed", SortOrder = 40 };
        supportTicket.Statuses.AddRange([newStatus, inProgressStatus, resolvedStatus, closedStatus]);

        supportTicket.Actions.AddRange(
        [
            new ClassAction { ClassTypeId = supportTicket.Id, Name = "Start Work", FromStatusId = newStatus.Id, ToStatusId = inProgressStatus.Id },
            new ClassAction { ClassTypeId = supportTicket.Id, Name = "Resolve", FromStatusId = inProgressStatus.Id, ToStatusId = resolvedStatus.Id },
            new ClassAction { ClassTypeId = supportTicket.Id, Name = "Close", FromStatusId = resolvedStatus.Id, ToStatusId = closedStatus.Id },
            new ClassAction { ClassTypeId = supportTicket.Id, Name = "Reopen", FromStatusId = resolvedStatus.Id, ToStatusId = inProgressStatus.Id }
        ]);

        var instance = new ClassInstance
        {
            ClassTypeId = supportTicket.Id,
            CurrentStatusId = newStatus.Id,
            Title = "Printer not working on Floor 3",
            FieldValues =
            [
                new ClassInstanceFieldValue { ClassFieldId = titleField.Id, Value = "Printer not working on Floor 3" },
                new ClassInstanceFieldValue { ClassFieldId = descriptionField.Id, Value = "The shared printer near reception is jammed and unavailable." },
                new ClassInstanceFieldValue { ClassFieldId = priorityField.Id, Value = "Medium" },
                new ClassInstanceFieldValue { ClassFieldId = requestedByField.Id, Value = "Alex Rivera" }
            ]
        };

        dbContext.ClassTypes.Add(supportTicket);
        dbContext.ClassInstances.Add(instance);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
