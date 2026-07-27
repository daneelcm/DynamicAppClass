import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassFieldType, ClassTypeDetail } from '../../core/models';

@Component({
  selector: 'app-class-type-detail',
  imports: [ReactiveFormsModule],
  template: `
    @if (!classType && isLoading) {
      <div style="padding: 20px; background: #fff3cd; border: 1px solid #ffc107;">
        ⏳ Loading class type... id="{{ id }}"
      </div>
    }
    @if (!classType && !isLoading) {
      <div style="padding: 20px; background: #f8d7da; border: 1px solid #f5c6cb; color: #721c24;">
        ❌ Failed to load class type. Error: "{{ error || 'Unknown error' }}" id="{{ id }}"
      </div>
    }
    @if (classType) {
      <section class="page-heading">
        <div>
          <h1>{{ classType.name }}</h1>
          <p>{{ classType.description }}</p>
        </div>
      </section>

      @if (error) { <p class="error banner">{{ error }}</p> }

      <section class="config-grid">
        <form class="panel" [formGroup]="fieldForm" (ngSubmit)="addField()">
          <h2>Fields</h2>
          <label><div>Name<span class="required">*</span></div><input formControlName="name" /></label>
          <label><div>Type<span class="required">*</span></div>
            <select formControlName="fieldType">
              @for (type of fieldTypes; track type) { <option [value]="type">{{ type }}</option> }
            </select>
          </label>
          <label class="check"><input type="checkbox" formControlName="isRequired" /> Required</label>
          <label class="check"><input type="checkbox" formControlName="isHidden" (change)="hiddenClick()" /> Hidden Field</label>
          <label>Sort Order <input type="number" formControlName="sortOrder" /></label>
          <label><div>Default Value@if (fieldForm.get('isHidden')?.value) {<span class="required">*</span>}</div>
            <input formControlName="defaultValue" />
          </label>
          @if (fieldForm.get('fieldType')?.value === 'Select') {
            <label>Options <input formControlName="options" placeholder="Low, Medium, High" /></label>
          }
          <button type="submit" [disabled]="fieldForm.invalid">Add Field</button>
          <ul class="compact-list">
            @for (field of classType.fields; track field.id) {
              <li><strong>{{ field.name }}</strong><span>{{ field.fieldType }} @if (field.isRequired) { · required }</span></li>
            }
          </ul>
        </form>

        <form class="panel" [formGroup]="actionForm" (ngSubmit)="addAction()">
          <h2>Actions</h2>
          <label>Name <input formControlName="name" /></label>
          <label>Field
            <select formControlName="assignClassFieldId">
              @for (field of classType.fields; track field.id) { <option [value]="field.id">{{ field.name }}</option> }
            </select>
          </label>
          <label>New Value
            @if (classType.fields.find(f => f.id == actionForm.get('assignClassFieldId')?.value)?.fieldType === 'Select') {
              <select formControlName="valueToAssign">
                @for (opt of classType.fields.find(f => f.id == actionForm.get('assignClassFieldId')?.value)?.options; track opt) { <option [value]="opt.value">{{ opt.caption }}</option> }
              </select>
            } @else {
              <input formControlName="valueToAssign" />
            }
          </label>
          <label>Condition
            <select formControlName="conditionClassFieldId">
              @for (field of classType.fields; track field.id) { <option [value]="field.id">{{ field.name }}</option> }
            </select>
          </label>
          <label>Condition Value
            @if (classType.fields.find(f => f.id == (actionForm.get('conditionClassFieldId')?.value ?? 0))?.fieldType === 'Select') {
              <select formControlName="conditionValue">
                @for (opt of classType.fields.find(f => f.id == (actionForm.get('conditionClassFieldId')?.value ?? 0))?.options; track opt) { <option [value]="opt.value">{{ opt.caption }}</option> }
              </select>
            } @else {
              <input formControlName="conditionValue" />
            }
          </label>
          <button type="submit" [disabled]="actionForm.invalid">Add Action</button>
          <ul class="compact-list">
            @for (action of classType.actions; track action.id) {
              <li><strong>{{ action.name }}</strong>
              <span>{{ action.assignFieldName }} → {{ action.valueToAssign }} {{ (action.conditionClassFieldId ?? 0) > 0 ? '| WHEN: ' + action.conditionFieldName + ' = ' + action.conditionValue : '' }}</span></li>
            }
          </ul>
        </form>
      </section>
    }
  `
})
export class ClassTypeDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  classType?: ClassTypeDetail;
  error = '';
  isLoading = true;
  fieldTypes: ClassFieldType[] = ['Text', 'LongText', 'Select', 'Number', 'Date', 'Boolean'];
  id = 0;

  fieldForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    fieldType: ['Text' as ClassFieldType, Validators.required],
    isRequired: [false],
    isHidden: [false],
    defaultValue: [null],
    sortOrder: [10, Validators.required],
    options: ['']
  });

  actionForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    assignClassFieldId: [0, Validators.required],
    valueToAssign: ['', Validators.required],
    conditionClassFieldId: [null],
    conditionValue: [null]
  });

  ngOnInit() {
    console.log('ClassTypeDetailPage.ngOnInit called');
    this.id = Number.parseInt(this.route.snapshot.paramMap.get('id') ?? '0');
    console.log('Got id:', this.id);
    this.load();
  }

  hiddenClick(){
    if (this.fieldForm.get('isHidden')?.value) {
      this.fieldForm.get('defaultValue')?.setValidators([Validators.required]);
    } else {
      this.fieldForm.get('defaultValue')?.clearValidators();
    }
    this.fieldForm.get('defaultValue')?.updateValueAndValidity();
  }

  addField() {
    const raw = this.fieldForm.getRawValue();
    this.api.addField(this.id, {
      ...raw,
      options: raw.options.split(',').map(option => option.trim()).filter(Boolean),
      concurrencyToken: this.classType?.concurrencyToken
    }).subscribe(this.refreshObserver());
  }

  addAction() {
    this.api.addAction(this.id, {
      ...this.actionForm.getRawValue(),
      concurrencyToken: this.classType?.concurrencyToken
    }).subscribe(this.refreshObserver());
  }

  private load() {
    console.log('ClassTypeDetailPage.load called with id:', this.id);
    this.isLoading = true;
    this.api.getClassType(this.id).subscribe({
      next: type => {
        console.log('API response received:', type);
        this.classType = type;
        this.error = '';
        this.isLoading = false;
        this.cdr.detectChanges();
        // const firstStatus = type.statuses[0]?.id ?? '';
        // this.actionForm.patchValue({ assignClassFieldId: firstStatus, toStatusId: firstStatus });
      },
      error: err => {
        console.log('API error:', err);
        this.error = err.error?.title ?? 'Unable to load class type.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private refreshObserver() {
    return {
      next: () => {
        this.error = '';
        this.fieldForm.patchValue({ name: '', options: '' });
        this.actionForm.patchValue({ name: '', valueToAssign: '', conditionClassFieldId: null, conditionValue: null });
        this.load();
      },
      error: (err: { error?: { title?: string } }) => {
        this.error = err.error?.title ?? 'Unable to save change.';
        this.cdr.detectChanges();
      }
    };
  }
}
