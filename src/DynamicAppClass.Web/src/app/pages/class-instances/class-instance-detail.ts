import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassField, ClassInstanceDetail, ClassTypeDetail } from '../../core/models';
import { ManageContactPartial } from '../partial-components/manage-contact';

@Component({
  selector: 'app-class-instance-detail',
  imports: [ReactiveFormsModule, ManageContactPartial],
  template: `
    @if (instance && classType) {
      <section class="page-heading">
        <div>
          <h1>{{ instance.title }}</h1>
          <p>{{ instance.classTypeName }}</p>
        </div>
        <span class="status-pill large">{{ classType.fields.find(fv => fv.name === 'Status')?.options?.find(o => o.value === instance?.fieldValues?.find(fv => fv.fieldName === 'Status')?.value)?.caption }}</span>
      </section>

      @if (error) { <p class="error banner">{{ error }}</p> }

      <section class="two-column">
        <form class="panel" [formGroup]="form" (ngSubmit)="save()">
          <h2>Application Summary</h2>
          @for (field of classType.fields; track field.id) {
            @if (field.isHidden) {
              <input type="hidden" [formControlName]="field.id" />
            }
            @else if ((field.dependsOnClassFieldId ?? 0) === 0 || form.get(field.dependsOnClassFieldId!.toString())?.value == field.dependsOnClassFieldValue) {
              <label><div>{{ field.name }}@if (field.isRequired) { <span class="required">*</span> }</div>
                @if (field.fieldType === 'LongText') {
                  <textarea [formControlName]="field.id"></textarea>
                } @else if (field.fieldType === 'Select') {
                  <select [formControlName]="field.id">
                    <option [ngValue]="null">Choose</option>
                    @for (option of field.options; track option) { <option [value]="option.value">{{ option.caption }}</option> }
                  </select>
                } @else if (field.fieldType === 'Boolean') {
                  <select [formControlName]="field.id"><option value="false">No</option><option value="true">Yes</option></select>
                } @else {
                  <input [type]="inputType(field)" [formControlName]="field.id" />
                }
              </label>
            }
          }
          <button type="submit" [disabled]="form.invalid">Save Values</button>
        </form>

        <div style="display: flex; flex-direction: column; gap: 10px;">
          @for (feat of classType.features.filter(f => f.isEnabled && !f.isFieldFeature); track feat.id) {
            @if (feat.code === 'contacts') {
              <app-instance-contact [classTypeId]="instance.classTypeId" [instanceId]="instance.id"></app-instance-contact>
            }
            @else {
              <div class="panel">
                <div style="display: flex; justify-content: space-between; align-items: center;">
                  <h2>{{feat.name}}</h2>
                  <button type="button">Add {{ feat.name }}</button>
                </div>
            </div>
            }
          }
          <div class="panel" style="display: flex; justify-content: end;">
            @for (action of instance.availableActions; track action.id) {
              <button type="button" (click)="execute(action.id)">{{ action.name }}</button>
            } @empty {
              <p>No actions are available.</p>
            }
          </div>
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
      const value = this.instance?.fieldValues.find(candidate => candidate.fieldId === field.id)?.value ?? null;
      controls[field.id] = new FormControl<string | null>(value, (field.isRequired && (field.dependsOnClassFieldId ?? 0) == 0) ? Validators.required : []);

      //if somebody depends on this field will suscribe to the change event to evaluate required or not for the dependant fields
      if (this.classType?.fields.some(f => f.dependsOnClassFieldId === field.id)) {
        controls[field.id].valueChanges.subscribe((value) => {
          this.classType?.fields.filter(f => f.dependsOnClassFieldId === field.id).forEach(x => {
            if (x.dependsOnClassFieldValue == value){
              if (x.isRequired)
                controls[x.id].setValidators(Validators.required);
            }
            else{
              controls[x.id].clearValidators();
              controls[x.id].setValue(null);
            }
          });
        });
      }
    }
    this.form = new FormGroup(controls);
    this.cdr.detectChanges();
  }
}
