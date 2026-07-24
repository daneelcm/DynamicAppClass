import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassField, ClassInstanceDetail, ClassTypeDetail } from '../../core/models';

@Component({
  selector: 'app-class-instance-detail',
  imports: [ReactiveFormsModule],
  template: `
    @if (instance && classType) {
      <section class="page-heading">
        <div>
          <h1>{{ instance.title }}</h1>
          <p>{{ instance.classTypeName }}</p>
        </div>
        <span class="status-pill large">{{ instance.currentStatus.name }}</span>
      </section>

      @if (error) { <p class="error banner">{{ error }}</p> }

      <section class="two-column">
        <form class="panel" [formGroup]="form" (ngSubmit)="save()">
          <h2>Field Values</h2>
          @for (field of classType.fields; track field.id) {
            <label><div>{{ field.name }}@if (field.isRequired) { <span class="required">*</span> }</div>
              @if (field.fieldType === 'LongText') {
                <textarea [formControlName]="field.id"></textarea>
              } @else if (field.fieldType === 'Select') {
                <select [formControlName]="field.id">
                  <option value="">Choose</option>
                  @for (option of field.options; track option) { <option [value]="option.value">{{ option.caption }}</option> }
                </select>
              } @else if (field.fieldType === 'Boolean') {
                <select [formControlName]="field.id"><option value="false">No</option><option value="true">Yes</option></select>
              } @else {
                <input [type]="inputType(field)" [formControlName]="field.id" />
              }
            </label>
          }
          <button type="submit" [disabled]="form.invalid">Save Values</button>
        </form>

        <div class="panel">
          <h2>Workflow</h2>
          <p class="muted">Available actions are based on the current status.</p>
          <div class="action-bar">
            @for (action of instance.availableActions; track action.id) {
              <button type="button" (click)="execute(action.id)">{{ action.name }}</button>
            } @empty {
              <p>No actions are available from {{ instance.currentStatus.name }}.</p>
            }
          </div>
          <h3>Configured Path</h3>
          <ul class="compact-list">
            @for (action of classType.actions; track action.id) {
              <li><strong>{{ action.name }}</strong><span>{{ action.assignFieldName }} -> {{ action.valueToAssign }}</span></li>
            }
          </ul>
        </div>
      </section>
    }
  `
})
export class ClassInstanceDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  instance?: ClassInstanceDetail;
  classType?: ClassTypeDetail;
  error = '';
  private id = 0;
  form = this.fb.group({});

  ngOnInit() {
    this.id = Number.parseInt(this.route.snapshot.paramMap.get('id') ?? '0');
    this.load();
  }

  save() {
    this.api.updateInstanceValues(this.id, {
      fieldValues: this.form.getRawValue() as Record<string, string | null>,
      concurrencyToken: this.instance?.concurrencyToken
    }).subscribe({
      next: instance => {
        this.instance = instance;
        this.error = '';
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to save field values.';
        this.cdr.detectChanges();
      }
    });
  }

  execute(actionId: number) {
    this.api.executeAction(this.id, {
      actionId,
      concurrencyToken: this.instance?.concurrencyToken
    }).subscribe({
      next: instance => {
        this.instance = instance;
        this.error = '';
        this.refreshFields();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to execute action.';
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

  private load() {
    this.api.getInstance(this.id).subscribe({
      next: instance => {
        this.instance = instance;
        this.api.getClassType(instance.classTypeId).subscribe(type => {
          this.classType = type;
          this.refreshFields();
        });
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load instance.';
        this.cdr.detectChanges();
      }
    });
  }

  private refreshFields(){
    const controls: Record<string, FormControl<string | null>> = {};
    for (const field of this.classType?.fields ?? []) {
      const value = this.instance?.fieldValues.find(candidate => candidate.fieldId === field.id)?.value ?? '';
      controls[field.id] = new FormControl<string | null>(value, field.isRequired ? Validators.required : []);
    }
    this.form = new FormGroup(controls);
    this.cdr.detectChanges();
  }
}
