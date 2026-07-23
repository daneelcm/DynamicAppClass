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

        var titleField = new ClassField { OverrideName = "Subject", Field = new Field { Name = "Title", FieldType = ClassFieldType.Text }, IsRequired = true, SortOrder = 10 };
        var descriptionField = new ClassField { Field = new Field { Name = "Description", FieldType = ClassFieldType.LongText }, IsRequired = true, SortOrder = 20 };
        var priorityField = new ClassField { Field = new Field { Name = "Priority", FieldType = ClassFieldType.Select, Options = [ new Lookup { Caption = "Low", Value = "Low" }, new Lookup { Caption = "Medium", Value = "Medium" }, new Lookup { Caption = "High", Value = "High" } ] }, IsRequired = true, SortOrder = 30 };
        var requestedByField = new ClassField { Field = new Field { Name = "Requested By", FieldType = ClassFieldType.Text }, IsRequired = true, SortOrder = 40 };
        supportTicket.Fields.AddRange([titleField, descriptionField, priorityField, requestedByField]);

        var newStatus = new ClassStatus { Name = "New", SortOrder = 10 };
        var inProgressStatus = new ClassStatus { Name = "In Progress", SortOrder = 20 };
        var resolvedStatus = new ClassStatus { Name = "Resolved", SortOrder = 30 };
        var closedStatus = new ClassStatus { Name = "Closed", SortOrder = 40 };
        supportTicket.Statuses.AddRange([newStatus, inProgressStatus, resolvedStatus, closedStatus]);

        supportTicket.Actions.AddRange(
        [
            new ClassAction { Name = "Start Work", FromStatus = newStatus, ToStatus = inProgressStatus },
            new ClassAction { Name = "Resolve", FromStatus = inProgressStatus, ToStatus = resolvedStatus },
            new ClassAction { Name = "Close", FromStatus = resolvedStatus, ToStatus = closedStatus },
            new ClassAction { Name = "Reopen", FromStatus = resolvedStatus, ToStatus = inProgressStatus }
        ]);

        var instance = new ClassInstance
        {
            ClassType = supportTicket,
            CurrentStatus = newStatus,
            Title = "Printer not working on Floor 3",
            FieldValues =
            [
                new ClassInstanceFieldValue { ClassField = titleField, Value = "Printer not working on Floor 3" },
                new ClassInstanceFieldValue { ClassField = descriptionField, Value = "The shared printer near reception is jammed and unavailable." },
                new ClassInstanceFieldValue { ClassField = priorityField, Value = "Medium" },
                new ClassInstanceFieldValue { ClassField = requestedByField, Value = "Alex Rivera" }
            ]
        };

        dbContext.ClassTypes.Add(supportTicket);
        dbContext.ClassInstances.Add(instance);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
