import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AllowedContact, ClassTypeDetail } from '../../core/models';

@Component({
  selector: 'app-class-types-list',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="page-heading">
      <span>
        <h1>Contacts Configuration</h1>
        <p>Configure Feature "Contacts" for Class "{{ classType?.name }}".</p>
      </span>
      <span>
        <a class="row-link" [routerLink]="['/class-types', typeId]"><- Back</a>
      </span>
    </section>

    <section class="two-column">
      <form class="panel" [formGroup]="form" (ngSubmit)="addAllowedContact()">
        <h2>New Allowed Contact Type</h2>
        <label>Contact Type Value <input formControlName="contactTypeValue" /></label>
        <label>Contact Type Caption <input formControlName="contactTypeCaption" /></label>
        <label>Quantity Allowed <input formControlName="quantityAllowed" type="number" min="1" max="10" /></label>
        <label class="check"><input type="checkbox" formControlName="required" /> Required
          @if(form.get('required')?.value) { 
            <input formControlName="quantityRequired" type="number" min="1" [max]="form.get('quantityAllowed')?.value ?? 10" />
          }
        </label>
        <label class="check"><input type="checkbox" formControlName="canBeEntity" /> Can Be Entity</label>
        <label class="check"><input type="checkbox" formControlName="requirePhone" /> Require Phone</label>
        <label class="check"><input type="checkbox" formControlName="requireEmail" /> Require Email</label>
        <label class="check"><input type="checkbox" formControlName="requireAddress" /> Require Address</label>
        <label class="check"><input type="checkbox" formControlName="requireLicense" /> Require License</label>

        <button type="submit" [disabled]="form.invalid">Add Contact Type</button>
        @if (error) { <p class="error">{{ error }}</p> }
      </form>

      <div class="panel">
        <h2>Allowed Contact Types</h2>
          <ul class="compact-list">
            @for (contactType of allowedContacts.sort((a, b) => a.contactTypeValue.localeCompare(b.contactTypeValue)); track contactType.id) {
              <li style="display: flex; justify-content: space-between;">
                <span style="display: grid;">
                  <strong>{{ contactType.contactTypeValue }} - {{ contactType.contactTypeCaption }}</strong>
                  <span>
                    {{ contactType.quantityAllowed }} Allowed
                    @if (contactType.required) { | {{ contactType.quantityRequired }} Required }
                    @if (contactType.canBeEntity) { | Can be Entity }
                    @if (contactType.requirePhone) { | Require Phone }
                    @if (contactType.requireEmail) { | Require Email }
                    @if (contactType.requireAddress) { | Require Address }
                    @if (contactType.requireLicense) { | Require License }
                  </span>
                </span>
                <a class="row-link" style="padding: 10px; cursor: pointer;" (click)="remove(contactType.id!)">❌</a>
              </li>
            }
          </ul>
      </div>
    </section>
  `
})
export class ConfigContactsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  classType?: ClassTypeDetail;
  allowedContacts: AllowedContact[] = [];
  error = '';
  typeId = 0;

  form = this.fb.nonNullable.group({
    contactTypeValue: ['', Validators.required],
    contactTypeCaption: ['', Validators.required],
    quantityAllowed: [1, [Validators.min(1), Validators.max(10)]],
    required: [false],
    quantityRequired: [1, [Validators.min(1), Validators.max(10)]],
    canBeEntity: [false],
    requirePhone: [false],
    requireEmail: [false],
    requireAddress: [false],
    requireLicense: [false]
  });

  ngOnInit() {
    this.typeId = Number.parseInt(this.route.snapshot.paramMap.get('typeId') ?? '0');

    this.load();
  }

  addAllowedContact(){
    var values = this.form.getRawValue();
    var existing = this.allowedContacts.find(c => c.contactTypeValue === values.contactTypeValue);
    if(existing){
      this.error = `Contact Type Value '${values.contactTypeValue}' already exists.`;
      return;
    }
    this.api.addAllowedContact(this.typeId, values).subscribe({
      next: () => {
        this.form.reset();
        this.load();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load contacts configuration. Confirm the API is running at http://localhost:5000.';
        this.cdr.detectChanges();
      }
    });
  }

  remove(id: number){
    this.api.deleteAllowedContact(id).subscribe({
      next: () => this.load(),
      error: err => {
        this.error = err.error?.title ?? 'Unable to load contacts configuration. Confirm the API is running at http://localhost:5000.';
        this.cdr.detectChanges();
      }
    });
  }

  private load() {
    this.api.getContactsConfig(this.typeId).subscribe({
      next: allowedContacts => {
        this.allowedContacts = allowedContacts;
        this.error = '';
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load contacts configuration. Confirm the API is running at http://localhost:5000.';
        this.cdr.detectChanges();
      }
    });
  }
}
