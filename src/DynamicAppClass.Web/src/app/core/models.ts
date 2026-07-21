export type ClassFieldType = 'Text' | 'LongText' | 'Select' | 'Number' | 'Date' | 'Boolean';

export interface ClassTypeSummary {
  id: string;
  name: string;
  description: string;
  fieldCount: number;
  statusCount: number;
  actionCount: number;
  concurrencyToken?: string;
}

export interface ClassTypeDetail {
  id: string;
  name: string;
  description: string;
  fields: ClassField[];
  statuses: ClassStatus[];
  actions: ClassAction[];
  concurrencyToken?: string;
}

export interface ClassField {
  id: string;
  name: string;
  fieldType: ClassFieldType;
  isRequired: boolean;
  sortOrder: number;
  options: string[];
}

export interface ClassStatus {
  id: string;
  name: string;
  sortOrder: number;
}

export interface ClassAction {
  id: string;
  name: string;
  fromStatusId: string;
  fromStatusName: string;
  toStatusId: string;
  toStatusName: string;
}

export interface ClassInstanceSummary {
  id: string;
  classTypeId: string;
  classTypeName: string;
  title: string;
  currentStatusId: string;
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
  fieldId: string;
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
  fromStatusId: string;
  toStatusId: string;
  concurrencyToken?: string;
}

export interface CreateClassInstanceRequest {
  classTypeId: string;
  title?: string;
  fieldValues: Record<string, string | null>;
  concurrencyToken?: string;
}

export interface UpdateClassInstanceValuesRequest {
  fieldValues: Record<string, string | null>;
  concurrencyToken?: string;
}

export interface ExecuteActionRequest {
  actionId: string;
  concurrencyToken?: string;
}
