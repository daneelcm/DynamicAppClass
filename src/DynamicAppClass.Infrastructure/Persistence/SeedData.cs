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
            return;

        var featGeneral = new Feature { Name = "General Application Questionnaire", Code = "general", IsFieldFeature = true };
        var featDetails = new Feature { Name = "Application Details", Code = "details", IsFieldFeature = true };
        var featAddress = new Feature { Name = "Address", Code = "address" };
        var featContacts = new Feature { Name = "Contacts", Code = "contacts" };
        var featDocuments = new Feature { Name = "Documents", Code = "documents" };

        //dbContext.Features.AddRange([featGeneral, featDetails, featAddress, featContacts, featDocuments]);
        //dbContext.SaveChanges();

        var clx = new ClassType { Name = "CLIV", Description = "Class IV Application Process example." };

        var lookupPending = new Lookup { Value = "1", Caption = "Pending", SortOrder = 10 };
        var lookupIssued = new Lookup { Value = "2", Caption = "Issued", SortOrder = 20 };
        var lookupRejected = new Lookup { Value = "3", Caption = "Rejected", SortOrder = 30 };
        var lookupRevision = new Lookup { Value = "4", Caption = "Revision", SortOrder = 40 };
        var statusField = new ClassField {
            Field = new Field { Name = "Status", FieldType = ClassFieldType.Select, Options = [lookupPending, lookupIssued, lookupRejected, lookupRevision] },
            IsRequired = true, IsHidden = true, DefaultValue = "1", SortOrder = 0, Feature = featGeneral, Options = [
                new() { Lookup = lookupPending },
                new() { Lookup = lookupIssued },
                new() { Lookup = lookupRejected }
            ]
        };
        
        var titleField = new ClassField { Field = new Field { Name = "Title", FieldType = ClassFieldType.Text },
            IsRequired = true, SortOrder = 1, Feature = featGeneral };
        var govProjField = new ClassField { Field = new Field { Name = "Is this a Government Project?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 2, Feature = featGeneral };

        var countyLookup = new Lookup { Value = "1", Caption = "County", SortOrder = 10 };
        var federalLookup = new Lookup { Value = "2", Caption = "Federal", SortOrder = 20 };
        var municipalityLookup = new Lookup { Value = "3", Caption = "Municipality", SortOrder = 30 };
        var stateLookup = new Lookup { Value = "4", Caption = "State", SortOrder = 40 };
        var projectTypeField = new ClassField {
            Field = new Field { Name = "Project Type", FieldType = ClassFieldType.Select, Options = [countyLookup, federalLookup, municipalityLookup, stateLookup] },
            IsRequired = true, DependsOnClassField = govProjField, DependsOnClassFieldValue = "true", SortOrder = 3, Feature = featGeneral, Options = [
                new() { Lookup = countyLookup },
                new() { Lookup = federalLookup },
                new() { Lookup = municipalityLookup },
                new() { Lookup = stateLookup }
            ] };

        var animalServicesLookup = new Lookup { Value = "1", Caption = "Animal Services", SortOrder = 10 };
        var rerLookup = new Lookup { Value = "2", Caption = "Regulatory and Economic Resources", SortOrder = 20 };
        var countyDepField = new ClassField {
            Field = new Field { Name = "County Department", FieldType = ClassFieldType.Select, Options = [animalServicesLookup, rerLookup] },
            IsRequired = true, DependsOnClassField = projectTypeField, DependsOnClassFieldValue = "1", SortOrder = 4, Feature = featGeneral, Options = [
                new() { Lookup = animalServicesLookup },
                new() { Lookup = rerLookup }
            ] };

        var aventuraLookup = new Lookup { Value = "1", Caption = "Aventura", SortOrder = 10 };
        var doralLookup = new Lookup { Value = "2", Caption = "Doral", SortOrder = 20 };
        var municipalityField = new ClassField {
            Field = new Field { Name = "Municipality", FieldType = ClassFieldType.Select, Options = [aventuraLookup, doralLookup] },
            IsRequired = true, DependsOnClassField = projectTypeField, DependsOnClassFieldValue = "3", SortOrder = 4, Feature = featGeneral, Options = [
                new() { Lookup = aventuraLookup },
                new() { Lookup = doralLookup }
            ] };

        var affordableField = new ClassField {
            Field = new Field { Name = "Is this project for Affordable Housing?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 5, Feature = featGeneral };

        var workforceField = new ClassField {
            Field = new Field { Name = "Is this project for Workforce Housing?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 6, Feature = featGeneral };

        var afterTheFactApprovalField = new ClassField {
            Field = new Field { Name = "Are you seeking an after-the-fact approval?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 7, Feature = featDetails };

        var totalPropertySizeField = new ClassField {
            Field = new Field { Name = "Total Property size (in acres)", FieldType = ClassFieldType.Number },
            IsRequired = true, SortOrder = 8, Feature = featDetails };

        var privateLookup = new Lookup { Value = "1", Caption = "Private", SortOrder = 10 };
        var publicLookup = new Lookup { Value = "2", Caption = "Public", SortOrder = 20 };
        var publicOrPrivateLandField = new ClassField {
            Field = new Field { Name = "Is this Public or Private land?", FieldType = ClassFieldType.Select, Options = [privateLookup, publicLookup] },
            IsRequired = true, SortOrder = 9, Feature = featDetails, Options = [
                new() { Lookup = privateLookup },
                new() { Lookup = publicLookup }
            ] };

        var bioAssesField = new ClassField {
            Field = new Field { Name = "Has the Wetlands Program performed a Biological Assessment in the last two years?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 10, Feature = featDetails };

        var eaaField = new ClassField {
            Field = new Field { Name = "Has the Wetlands Program issued an Expedited Administrative Authorization (EAA) in the last two years?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 11, Feature = featDetails };

        var waiverOfLimitationField = new ClassField {
            Field = new Field { Name = "Is Waiver of Limitation Waived?", FieldType = ClassFieldType.Boolean },
            IsRequired = true, SortOrder = 12, Feature = featDetails };

        clx.Fields.AddRange([statusField, titleField, govProjField, projectTypeField, countyDepField, municipalityField, affordableField, workforceField,
            afterTheFactApprovalField, totalPropertySizeField, publicOrPrivateLandField, bioAssesField, eaaField, waiverOfLimitationField]);
        clx.Actions.AddRange(
        [
            new ClassAction { Name = "Issue", AssignClassField = statusField, ValueToAssign = "2", ConditionClassField = statusField, ConditionValue = "1" },
            new ClassAction { Name = "Reject", AssignClassField = statusField, ValueToAssign = "3", ConditionClassField = statusField, ConditionValue = "1" },
            new ClassAction { Name = "Reopen", AssignClassField = statusField, ValueToAssign = "1", ConditionClassField = statusField, ConditionValue = "3" }
        ]);
        
        clx.Features.AddRange(
        [
            new ClassTypeFeature { Feature = featGeneral, Active = true },
            new ClassTypeFeature { Feature = featDetails, Active = true },
            new ClassTypeFeature { Feature = featAddress, Active = false },
            new ClassTypeFeature { Feature = featContacts, Active = true },
            new ClassTypeFeature { Feature = featDocuments, Active = false }
        ]);

        dbContext.AllowedContacts.AddRange([
            new AllowedContact { ClassType = clx, ContactTypeValue = "1", ContactTypeCaption = "Aplicant", QuantityAllowed = 1, Required = true, QuantityRequired = 1, CanBeEntity = true, RequirePhone = true, RequireEmail = true, RequireAddress = true, RequireLicense = false },
            new AllowedContact { ClassType = clx, ContactTypeValue = "2", ContactTypeCaption = "Property Owner", QuantityAllowed = 3, Required = true, QuantityRequired = 1, CanBeEntity = true, RequirePhone = false, RequireEmail = false, RequireAddress = true, RequireLicense = false },
            new AllowedContact { ClassType = clx, ContactTypeValue = "3", ContactTypeCaption = "Contractor", QuantityAllowed = 1, Required = false, QuantityRequired = 0, CanBeEntity = true, RequirePhone = true, RequireEmail = false, RequireAddress = true, RequireLicense = true }
        ]);
        dbContext.ClassTypes.Add(clx);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
