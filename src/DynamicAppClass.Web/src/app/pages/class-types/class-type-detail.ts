import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassFieldType, ClassTypeDetail } from '../../core/models';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-class-type-detail',
  imports: [ReactiveFormsModule, RouterLink],
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
          <label><div>Fields Group<span class="required">*</span></div>
            <select formControlName="featureCode">
              @for (feat of classType.features.filter(x => x.isFieldFeature); track feat.id) { <option [value]="feat.code">{{ feat.name }} (Step {{ $index + 1 }})</option> }
            </select>
          </label>
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
          <label>Depends On Field
            <select formControlName="dependsOnClassFieldId">
              @for (field of classType.fields; track field.id) { <option [value]="field.id">{{ field.name }}</option> }
            </select>
          </label>
          @if ((fieldForm.get('dependsOnClassFieldId')?.value ?? 0) > 0) {
            @let f = classType.fields.find(x => x.id == fieldForm.get('dependsOnClassFieldId')!.value)!;
            @if (f?.fieldType == 'Select'){
              <select formControlName="dependsOnClassFieldValue">
                @for (opt of f?.options; track opt) { <option [value]="opt.value">{{ opt.caption }}</option> }
              </select>
            }
            @else {
              <label>Depends On Value <input formControlName="dependsOnClassFieldValue" /></label>
            }
          }
          <button type="submit" [disabled]="fieldForm.invalid">Add Field</button>
          <ul class="compact-list">
            @for (field of classType.fields; track field.id) {
              <li><strong>{{ field.name }}</strong>
                <span>
                  {{ field.fieldType }}
                  @if (field.isRequired) { · required }
                  @if ((field.dependsOnClassFieldId ?? 0) > 0) {
                    @let f = classType.fields.find(x => x.id == field.dependsOnClassFieldId)!;
                    | WHEN: {{f?.name }} = 
                    {{ f?.fieldType == 'Select' ? f?.options.find(o => o.value == field.dependsOnClassFieldValue)?.caption : field.dependsOnClassFieldValue }}
                  }
                </span>
              </li>
            }
          </ul>
        </form>

        <form class="panel" [formGroup]="actionForm" (ngSubmit)="addAction()">
          <h2>Actions</h2>
          <label><div>Name<span class="required">*</span></div><input formControlName="name" /></label>
          <label><div>Field<span class="required">*</span></div>
            <select formControlName="assignClassFieldId" (change)="actionForm.get('valueToAssign')?.setValue('');">
              @for (field of classType.fields; track field.id) { <option [value]="field.id">{{ field.name }}</option> }
            </select>
          </label>
          <label><div>New Value<span class="required">*</span></div>
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

        <div class="panel">
          <h2>Features</h2>
          @for (feat of classType.features.filter(x => !x.isFieldFeature); track feat.id) {
            <label class="check" style="justify-content: space-between;">
              <span style="padding: 10px;"><input type="checkbox" (change)="featureEvent(feat.id, $event)" [checked]="feat.isEnabled" /> {{ feat.name }}</span>
              @if (feat.isEnabled){
                <a class="row-link" [routerLink]="['/class-types', classType.id, 'config-' + feat.code, feat.id]" style="padding: 9px;">Configure</a>
              }
            </label>
          }
        </div>
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
    featureCode: ['', Validators.required],
    name: ['', Validators.required],
    fieldType: ['Text' as ClassFieldType, Validators.required],
    isRequired: [false],
    isHidden: [false],
    defaultValue: [null],
    sortOrder: [10, Validators.required],
    dependsOnClassFieldId: [null],
    dependsOnClassFieldValue: [null],
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

  featureEvent(id: number, $event: Event) {
    const checkbox = $event.target as HTMLInputElement;
    this.isLoading = true;
    this.api.updateFeature(this.id, {
      id: id,
      isEnabled: checkbox.checked
    }).subscribe(this.refreshObserver());
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
      next: (type: ClassTypeDetail) => {
        this.error = '';
        this.fieldForm.reset();
        this.actionForm.reset();
        this.classType = type;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err: { error?: { title?: string } }) => {
        this.error = err.error?.title ?? 'Unable to save change.';
        this.cdr.detectChanges();
      }
    };
  }
}
