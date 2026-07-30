import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  AddActionRequest,
  AddFieldRequest,
  UpdateFeatureRequest,
  ClassAction,
  ClassInstanceDetail,
  ClassInstanceSummary,
  ClassTypeDetail,
  ClassTypeSummary,
  CreateClassInstanceRequest,
  ExecuteActionRequest,
  UpdateClassInstanceValuesRequest,
  Contact,
  AllowedContact
} from './models';

const API_BASE = 'http://localhost:5000/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private readonly http: HttpClient) {}

  listClassTypes() {
    return this.http.get<ClassTypeSummary[]>(`${API_BASE}/class-types`);
  }

  getClassType(id: number) {
    return this.http.get<ClassTypeDetail>(`${API_BASE}/class-types/${id}`);
  }

  createClassType(payload: { name: string; description: string }) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types`, payload);
  }

  addField(classTypeId: number, payload: AddFieldRequest) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types/${classTypeId}/fields`, payload);
  }

  addAction(classTypeId: number, payload: AddActionRequest) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types/${classTypeId}/actions`, payload);
  }

  updateFeature(classTypeId: number, payload: UpdateFeatureRequest) {
    return this.http.put<ClassTypeDetail>(`${API_BASE}/class-types/${classTypeId}/feature`, payload);
  }

  listInstances() {
    return this.http.get<ClassInstanceSummary[]>(`${API_BASE}/class-instances`);
  }

  getInstance(id: number) {
    return this.http.get<ClassInstanceDetail>(`${API_BASE}/class-instances/${id}`);
  }

  createInstance(payload: CreateClassInstanceRequest) {
    return this.http.post<ClassInstanceDetail>(`${API_BASE}/class-instances`, payload);
  }

  updateInstanceValues(id: number, payload: UpdateClassInstanceValuesRequest) {
    return this.http.put<ClassInstanceDetail>(`${API_BASE}/class-instances/${id}/field-values`, payload);
  }

  getAvailableActions(id: number) {
    return this.http.get<ClassAction[]>(`${API_BASE}/class-instances/${id}/available-actions`);
  }

  executeAction(id: number, payload: ExecuteActionRequest) {
    return this.http.post<ClassInstanceDetail>(`${API_BASE}/class-instances/${id}/actions`, payload);
  }

  getInstanceContacts(instanceId: number) {
    return this.http.get<Contact[]>(`${API_BASE}/contacts/${instanceId}`);
  }

  deleteContact(id: number) {
    return this.http.delete(`${API_BASE}/contacts/${id}`);
  }

  getContactsConfig(typeId: number) {
    return this.http.get<AllowedContact[]>(`${API_BASE}/contacts/config/${typeId}`);
  }

  deleteAllowedContact(id: number) {
    return this.http.delete(`${API_BASE}/contacts/config/${id}`);
  }

  addAllowedContact(typeId: number, contact: AllowedContact) {
    return this.http.post(`${API_BASE}/contacts/config`, {
      classTypeId: typeId,
      ...contact
    });
  }
}
