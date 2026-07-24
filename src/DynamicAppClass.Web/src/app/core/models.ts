import { KeyValue } from "@angular/common";

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
  statusCount: number;
  actionCount: number;
  concurrencyToken?: string;
}

export interface ClassTypeDetail {
  id: number;
  name: string;
  description: string;
  fields: ClassField[];
  statuses: ClassStatus[];
  actions: ClassAction[];
  concurrencyToken?: string;
}

export interface ClassField {
  id: number;
  name: string;
  fieldType: ClassFieldType;
  isRequired: boolean;
  sortOrder: number;
  options: FieldOptions[];
}

export interface ClassStatus {
  id: number;
  name: string;
  sortOrder: number;
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
  currentStatusId: number;
  currentStatusName: string;
  createdAt: string;
  updatedAt: string;
  concurrencyToken?: string;
}

export interface ClassInstanceDetail extends ClassInstanceSummary {
  currentStatus: ClassStatus;
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
  sortOrder: number;
  options?: string[];
  concurrencyToken?: string;
}

export interface AddStatusRequest {
  name: string;
  sortOrder: number;
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
