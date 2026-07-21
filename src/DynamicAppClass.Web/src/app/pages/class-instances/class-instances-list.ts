import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassField, ClassInstanceSummary, ClassTypeDetail, ClassTypeSummary } from '../../core/models';

@Component({
  selector: 'app-class-instances-list',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="page-heading">
      <div>
        <h1>Class Instances</h1>
        <p>Create and process individual items from configured class types.</p>
      </div>
    </section>

    <section class="two-column">
      <form class="panel" [formGroup]="form" (ngSubmit)="create()">
        <h2>New Instance</h2>
        <label>Class Type
          <select formControlName="classTypeId" (change)="selectType()">
            <option value="">Choose a type</option>
            @for (type of classTypes; track type.id) { <option [value]="type.id">{{ type.name }}</option> }
          </select>
        </label>

        @if (selectedType) {
          <div formGroupName="fieldValues" class="dynamic-fields">
            @for (field of selectedType.fields; track field.id) {
              <label>{{ field.name }} @if (field.isRequired) { <span class="required">*</span> }
                @if (field.fieldType === 'LongText') {
                  <textarea [formControlName]="field.id"></textarea>
                } @else if (field.fieldType === 'Select') {
                  <select [formControlName]="field.id">
                    <option value="">Choose</option>
                    @for (option of field.options; track option) { <option [value]="option">{{ option }}</option> }
                  </select>
                } @else if (field.fieldType === 'Boolean') {
                  <select [formControlName]="field.id"><option value="false">No</option><option value="true">Yes</option></select>
                } @else {
                  <input [type]="inputType(field)" [formControlName]="field.id" />
                }
              </label>
            }
          </div>
        }

        <button type="submit" [disabled]="form.invalid || !selectedType">Create Instance</button>
        @if (error) { <p class="error">{{ error }}</p> }
      </form>

      <div class="panel">
        <h2>Open Items</h2>
        <div class="table-list">
          @for (instance of instances; track instance.id) {
            <a class="row-link" [routerLink]="['/instances', instance.id]">
              <span>
                <strong>{{ instance.title }}</strong>
                <small>{{ instance.classTypeName }}</small>
              </span>
              <span class="status-pill">{{ instance.currentStatusName }}</span>
            </a>
          } @empty {
            <p>{{ error || 'No instances yet.' }}</p>
          }
        </div>
      </div>
    </section>
  `
})
export class ClassInstancesList implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  classTypes: ClassTypeSummary[] = [];
  instances: ClassInstanceSummary[] = [];
  selectedType?: ClassTypeDetail;
  error = '';

  form = this.fb.group({
    classTypeId: ['', Validators.required],
    fieldValues: this.fb.group({})
  });

  ngOnInit() {
    this.api.listClassTypes().subscribe({
      next: types => {
        this.classTypes = types;
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load class types. Confirm the API is running at http://localhost:5000.';
        this.cdr.detectChanges();
      }
    });
    this.loadInstances();
  }

  selectType() {
    const id = this.form.controls.classTypeId.value;
    if (!id) {
      this.selectedType = undefined;
      this.form.setControl('fieldValues', this.fb.group({}));
      return;
    }

    this.api.getClassType(id).subscribe({
      next: type => {
        this.selectedType = type;
        this.error = '';
        const controls: Record<string, FormControl<string | null>> = {};
        for (const field of type.fields) {
          controls[field.id] = new FormControl<string | null>('', field.isRequired ? Validators.required : []);
        }
        this.form.setControl('fieldValues', new FormGroup(controls));
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load the selected class type.';
        this.cdr.detectChanges();
      }
    });
  }

  create() {
    if (this.form.invalid || !this.selectedType) {
      return;
    }

    const raw = this.form.getRawValue();
    this.api.createInstance({
      classTypeId: raw.classTypeId ?? '',
      fieldValues: raw.fieldValues as Record<string, string | null>,
      concurrencyToken: this.selectedType.concurrencyToken
    }).subscribe({
      next: instance => this.router.navigate(['/instances', instance.id]),
      error: err => {
        this.error = err.error?.title ?? 'Unable to create instance.';
        this.cdr.detectChanges();
      }
    });
  }

  inputType(field: ClassField) {
    if (field.fieldType === 'Number') {
      return 'number';
    }
    if (field.fieldType === 'Date') {
      return 'date';
    }
    return 'text';
  }

  private loadInstances() {
    this.api.listInstances().subscribe({
      next: instances => {
        this.instances = instances;
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load class instances. Confirm the API is running at http://localhost:5000.';
        this.cdr.detectChanges();
      }
    });
  }
}
