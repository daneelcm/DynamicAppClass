export type ClassFieldType = 'Text' | 'LongText' | 'Select' | 'Number' | 'Date' | 'Boolean';

export interface FieldOptions {
  value: string;
  caption: string;
}

export interface ClassTypeSummary {
  id: number;
  name: string;
  description: string;
  fieldCount: number;
  actionCount: number;
  concurrencyToken?: string;
}

export interface ClassTypeDetail {
  id: number;
  name: string;
  description: string;
  fields: ClassField[];
  actions: ClassAction[];
  concurrencyToken?: string;
}

export interface ClassField {
  id: number;
  name: string;
  fieldType: ClassFieldType;
  isRequired: boolean;
  isHidden: boolean;
  defaultValue: string | null;
  sortOrder: number;
  dependsOnClassFieldId: number | null;
  dependsOnClassFieldValue: string | null;
  options: FieldOptions[];
}

export interface ClassAction {
  id: number;
  name: string;
  assignClassFieldId: number;
  assignFieldName: string;
  valueToAssign: string;
  conditionClassFieldId: number | null;
  conditionFieldName: string | null;
  conditionValue: string | null;
}

export interface ClassInstanceSummary {
  id: number;
  classTypeId: number;
  classTypeName: string;
  title: string;
  currentStatusName: string;
  createdAt: string;
  updatedAt: string;
  concurrencyToken?: string;
}

export interface ClassInstanceDetail extends ClassInstanceSummary {
  fieldValues: ClassInstanceFieldValue[];
  availableActions: ClassAction[];
}

export interface ClassInstanceFieldValue {
  fieldId: number;
  fieldName: string;
  value: string | null;
}

// Request DTOs
export interface AddFieldRequest {
  name: string;
  fieldType: ClassFieldType;
  isRequired: boolean;
  isHidden: boolean;
  sortOrder: number;
  options?: string[];
  concurrencyToken?: string;
}

export interface AddActionRequest {
  name: string;
  assignClassFieldId: number;
  valueToAssign: string;
  conditionClassFieldId: number | null;
  conditionValue: string | null;
  concurrencyToken?: string;
}

export interface CreateClassInstanceRequest {
  classTypeId: number;
  title?: string;
  fieldValues: Record<string, string | null>;
  concurrencyToken?: string;
}

export interface UpdateClassInstanceValuesRequest {
  fieldValues: Record<string, string | null>;
  concurrencyToken?: string;
}

export interface ExecuteActionRequest {
  actionId: number;
  concurrencyToken?: string;
}
