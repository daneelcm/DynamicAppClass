using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicAppClass.Domain.Entities.Core
{
    public sealed class ClassFieldLookup : BaseEntity
    {
        public int ClassFieldId { get; set; }
        public int LookupId { get; set; }
        public string? OverrideValue { get; set; }
        public string? OverrideCaption { get; set; }
        public int? OverrideSortOrder { get; set; }

        public string Value => OverrideValue ?? Lookup.Value;
        public string Caption => OverrideCaption ?? Lookup.Caption;
        public int SortOrder => OverrideSortOrder ?? Lookup.SortOrder ?? 999;
        public Lookup Lookup { get; set; } = null!;
    }
}
