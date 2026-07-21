import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  AddActionRequest,
  AddFieldRequest,
  AddStatusRequest,
  ClassAction,
  ClassInstanceDetail,
  ClassInstanceSummary,
  ClassTypeDetail,
  ClassTypeSummary,
  CreateClassInstanceRequest,
  ExecuteActionRequest,
  UpdateClassInstanceValuesRequest
} from './models';

const API_BASE = 'http://localhost:5000/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private readonly http: HttpClient) {}

  listClassTypes() {
    return this.http.get<ClassTypeSummary[]>(`${API_BASE}/class-types`);
  }

  getClassType(id: string) {
    return this.http.get<ClassTypeDetail>(`${API_BASE}/class-types/${id}`);
  }

  createClassType(payload: { name: string; description: string }) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types`, payload);
  }

  addField(classTypeId: string, payload: AddFieldRequest) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types/${classTypeId}/fields`, payload);
  }

  addStatus(classTypeId: string, payload: AddStatusRequest) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types/${classTypeId}/statuses`, payload);
  }

  addAction(classTypeId: string, payload: AddActionRequest) {
    return this.http.post<ClassTypeDetail>(`${API_BASE}/class-types/${classTypeId}/actions`, payload);
  }

  listInstances() {
    return this.http.get<ClassInstanceSummary[]>(`${API_BASE}/class-instances`);
  }

  getInstance(id: string) {
    return this.http.get<ClassInstanceDetail>(`${API_BASE}/class-instances/${id}`);
  }

  createInstance(payload: CreateClassInstanceRequest) {
    return this.http.post<ClassInstanceDetail>(`${API_BASE}/class-instances`, payload);
  }

  updateInstanceValues(id: string, payload: UpdateClassInstanceValuesRequest) {
    return this.http.put<ClassInstanceDetail>(`${API_BASE}/class-instances/${id}/field-values`, payload);
  }

  getAvailableActions(id: string) {
    return this.http.get<ClassAction[]>(`${API_BASE}/class-instances/${id}/available-actions`);
  }

  executeAction(id: string, payload: ExecuteActionRequest) {
    return this.http.post<ClassInstanceDetail>(`${API_BASE}/class-instances/${id}/actions`, payload);
  }
}
