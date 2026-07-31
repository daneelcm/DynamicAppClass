import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassField, ClassInstanceSummary, ClassTypeDetail, ClassTypeSummary } from '../../core/models';
import { ManageContactPartial } from "../partial-components/manage-contact";
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-class-instances-list',
  imports: [ReactiveFormsModule, RouterLink, ManageContactPartial, JsonPipe],
  template: `
    <section class="page-heading">
      <div>
        <h1>Applications</h1>
        <p>Create and process individual applications from configured class types.</p>
      </div>
    </section>

    <form [formGroup]="form" (ngSubmit)="create()">
      <section class="two-column">
        <div class="panel">
          <h2>New Application</h2>
          <label>Class Type
            <select formControlName="classTypeId" (change)="selectType()">
              <option value="">Choose a type</option>
              @for (type of classTypes; track type.id) { <option [value]="type.id">{{ type.name }}</option> }
            </select>
          </label>
          @if (selectedType) {
            <ul class="compact-list">
              @if(currentStep == 'appData') { <li><strong>Application Data</strong></li> }
              @else { <li>Application Data</li> }
              @for (feat of selectedType.features?.filter(x => x.isEnabled); track $index) {
                @if(currentStep == feat.code) { <li><strong>{{ feat.name }}</strong></li> }
                @else { <li>{{ feat.name }}</li> }
              }
            </ul>
          }
          @if (error) { <p class="error">{{ error }}</p> }
        </div>

        @if (selectedType) {
          <div style="display: grid; gap: 10px">
            @if(currentStep == 'appData') {
              <div formGroupName="fieldValues" class="panel dynamic-fields">
                @for (field of selectedType.fields; track field.id) {
                  @if (field.isHidden) {
                    <input type="hidden" [formControlName]="field.id" />
                  }
                @else if ((field.dependsOnClassFieldId ?? 0) === 0 || form.get('fieldValues.' + field.dependsOnClassFieldId)?.value == field.dependsOnClassFieldValue) {
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
              </div>
            }
            @else {
              @if (currentStep === 'contacts') {
                <app-instance-contact [classTypeId]="selectedType.id" [(contacts)]="featuresData['contacts']" style="display: grid;"></app-instance-contact>
              }
              @else {
                <div class="panel">
                  <div style="display: flex; justify-content: space-between; align-items: center;">
                    <h2>{{selectedType.features.find(x => x.code == currentStep)?.name}}</h2>
                    <button type="button">Add {{ selectedType.features.find(x => x.code == currentStep)?.name }}</button>
                  </div>
              </div>
              }
            }

            <div class="panel" style="display: flex; justify-content: end; gap:10px; align-items: flex-end;">
              <button type="button" [disabled]="currentStep == 'appData'" (click)="currentStep = featureCodes[0] == currentStep ? 'appData' : featureCodes[featureCodes.indexOf(currentStep) - 1]"><- Back</button>
              @if (featureCodes[featureCodes.length - 1] == currentStep) {
                <button type="submit" [disabled]="form.invalid || !selectedType">Submit</button>
              }
              @else {
                <button type="button" [disabled]="form.invalid" (click)="currentStep = featureCodes[featureCodes.indexOf(currentStep) + 1]">Next -></button>
              }
            </div>
          </div>
        }
        @else {
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
        }
      </section>
    </form>
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
  featureCodes: string[] = [];
  featuresData: Record<string, []> = {};
  currentStep = 'appData';
  error = '';

  form = this.fb.group({
    classTypeId: [0, Validators.required],
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
          controls[field.id] = new FormControl<string | null>(field.defaultValue, (field.isRequired && (field.dependsOnClassFieldId ?? 0) == 0) ? Validators.required : []);

          //if somebody depends on this field will suscribe to the change event to evaluate required or not for the dependant fields
          if (type.fields.some(f => f.dependsOnClassFieldId === field.id)) {
            controls[field.id].valueChanges.subscribe((value) => {
              type.fields.filter(f => f.dependsOnClassFieldId === field.id).forEach(x => {
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
        this.form.setControl('fieldValues', new FormGroup(controls));
        
        for (const feat of type.features.filter(x => x.isEnabled)){
          this.featureCodes.push(feat.code);
          this.featuresData[feat.code] = [];
        }
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
      classTypeId: raw.classTypeId ?? 0,
      fieldValues: raw.fieldValues as Record<string, string | null>,
      featuresData: this.featuresData,
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
