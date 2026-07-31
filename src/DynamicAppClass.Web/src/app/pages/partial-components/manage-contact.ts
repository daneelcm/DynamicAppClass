import { ChangeDetectorRef, Component, inject, Input, model, OnInit } from '@angular/core';
import { AllowedContact, Contact } from '../../core/models';
import { ApiService } from '../../core/api.service';
import { ɵInternalFormsSharedModule, ReactiveFormsModule, FormBuilder, Validators } from "@angular/forms";

@Component({
  selector: 'app-instance-contact',
  imports: [ɵInternalFormsSharedModule, ReactiveFormsModule],
  template: `
    <div class="panel">
      <div style="display: flex; justify-content: space-between; align-items: center;">
        <h2>Contacts</h2>
        <button type="button" (click)="switchForm()">{{!addInProgress ? 'Add Contact' : 'Cancel'}}</button>
      </div>

      @if (error) { <p class="error banner">{{ error }}</p> }

      @if(addInProgress){
        <form [formGroup]="form" (ngSubmit)="addContact()" style="display: grid; gap: 10px;">
          <label><div>Contact Type<span class="required">*</span></div>
            <select formControlName="contactType" (change)="setValidators()">
            @for (contactType of _allowedContacts; track $index) {
              <option [value]="contactType.contactTypeValue" 
                [disabled]="_contacts?.filter(x => x.contactType == contactType.contactTypeValue)?.length == contactType.quantityAllowed"
              >{{contactType.contactTypeCaption}}
              @if (_contacts.filter(x => x.contactType == contactType.contactTypeValue).length < contactType.quantityRequired) {<span class="required">*</span>}
              </option>
            }
          </select></label>
          @if(form.get('contactType')?.value){
            @let ctSelected = _allowedContacts?.find(x => x.contactTypeValue == form.get('contactType')?.value);
            @if (ctSelected?.canBeEntity){
              <label class="check"><input type="checkbox" formControlName="isEntity" (change)="setValidators();" /> {{ ctSelected.contactTypeCaption }} is an Entity</label>
              @if (form.get('isEntity')?.value) {
                <label><div>Entity Name<span class="required">*</span></div><input formControlName="entityName" /></label>
              }
            }
            @if (!ctSelected?.canBeEntity || !form.get('isEntity')?.value) {
              <label><div>First Name<span class="required">*</span></div><input formControlName="firstName" /></label>
              <label><div>Last Name<span class="required">*</span></div><input formControlName="lastName" /></label>
            }
            <label><div>Phone@if(ctSelected?.requirePhone) { <span class="required">*</span> }</div><input formControlName="phone" /></label>
            <label><div>Email@if(ctSelected?.requireEmail) { <span class="required">*</span> }</div><input formControlName="email" /></label>
            <label><div>License Number@if(ctSelected?.requireLicense) { <span class="required">*</span> }</div><input formControlName="licenseNumber" /></label>

            <label><div>Address 1@if(ctSelected?.requireAddress) { <span class="required">*</span> }</div><input formControlName="address1" /></label>
            <label>Address 2<input formControlName="address2" /></label>
            <div style="display: flex; gap=10px; justify-content: space-between;">
              <label><div>City@if(ctSelected?.requireAddress) { <span class="required">*</span> }</div><input formControlName="city" /></label>
              <label><div>State@if(ctSelected?.requireAddress) { <span class="required">*</span> }</div><input formControlName="state" /></label>
              <label><div>Zip Code@if(ctSelected?.requireAddress) { <span class="required">*</span> }</div><input formControlName="zipCode" /></label>
            </div>
            <label><div>Country@if(ctSelected?.requireAddress) { <span class="required">*</span> }</div><input formControlName="country" /></label>
          }

          <button type="submit" [disabled]="form.invalid">Add Contact</button>
        </form>
      }
      @else {
        <ul class="compact-list">
          @for (contact of _contacts; track $index) {
            <li style="display: flex; justify-content: space-between;">
              <span style="display: grid;">
              @if (contact.isEntity) { <strong>{{ contact.entityName }}</strong> } @else { <strong>{{ contact.firstName }} {{ contact.lastName }}</strong> }
              @if (contact.contactType) { {{ _allowedContacts?.find(c => c.contactTypeValue === contact.contactType)?.contactTypeCaption }} }
              @if (contact.phone) { | {{ contact.phone }} }
              @if (contact.email) { | {{ contact.email }} }
              @if (contact.address1 || contact.address2 || contact.city || contact.zipCode || contact.country) {
              | {{ [contact.address1, contact.address2, contact.city, contact.zipCode, contact.country].filter(x => x).join(', ') }}
              }
              @if (contact.licenseNumber) { | License: {{ contact.licenseNumber }} }
              </span>
              <a class="row-link" style="padding: 10px; cursor: pointer; margin: auto 0;" (click)="remove(contact.id ?? $index)">❌</a>
            </li>
          }
        </ul>
      }
    </div>
  `
})
export class ManageContactPartial implements OnInit {
  private readonly api = inject(ApiService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly fb = inject(FormBuilder);
  
  @Input()
  classTypeId!: number;
  @Input()
  instanceId?: number;

  contacts = model<Contact[]>();
  isValid = model<boolean>(false);

  protected _allowedContacts?: AllowedContact[];
  protected _contacts: Contact[] = [];
  addInProgress = false;
  error = ''
  form = this.fb.nonNullable.group({
    contactType: ['', Validators.required],
    isEntity: [false],
    firstName: [''],
    lastName: [''],
    entityName: [''],
    phone: [''],
    email: [''],
    address1: [''],
    address2: [''],
    city: [''],
    state: [''],
    zipCode: [''],
    country: [''],
    licenseNumber: ['']
  });

  ngOnInit(): void {
    this.load();
  }

  setValidators() {
    var type = this._allowedContacts?.find(x => x.contactTypeValue == this.form.get('contactType')?.value);

    if (type?.canBeEntity) {
      if (this.form.get('isEntity')?.value) {
        this.form.get('entityName')?.setValidators([Validators.required]);
        this.form.get('firstName')?.setValue('');
        this.form.get('firstName')?.clearValidators();
        this.form.get('lastName')?.setValue('');
        this.form.get('lastName')?.clearValidators();
      }
    }
    if (!(type?.canBeEntity) || !(this.form.get('isEntity')?.value)) {
      this.form.get('entityName')?.setValue('');
      this.form.get('entityName')?.clearValidators();
      this.form.get('firstName')?.setValidators([Validators.required]);
      this.form.get('lastName')?.setValidators([Validators.required]);
    }

    if (type?.requirePhone)
      this.form.get('phone')?.setValidators([Validators.required]);
    else
      this.form.get('phone')?.clearValidators();
    
    if (type?.requireEmail)
      this.form.get('email')?.setValidators([Validators.required]);
    else
      this.form.get('email')?.clearValidators();
    
    if (type?.requireLicense)
      this.form.get('licenseNumber')?.setValidators([Validators.required]);
    else
      this.form.get('licenseNumber')?.clearValidators();
    
    if (type?.requireAddress){
      this.form.get('address1')?.setValidators([Validators.required]);
      this.form.get('city')?.setValidators([Validators.required]);
      this.form.get('state')?.setValidators([Validators.required]);
      this.form.get('zipCode')?.setValidators([Validators.required]);
      this.form.get('country')?.setValidators([Validators.required]);
    }
    else {
      this.form.get('address1')?.clearValidators();
      this.form.get('city')?.clearValidators();
      this.form.get('state')?.clearValidators();
      this.form.get('zipCode')?.clearValidators();
      this.form.get('country')?.clearValidators();
    }
    
    this.form.get('entityName')?.updateValueAndValidity();
    this.form.get('firstName')?.updateValueAndValidity();
    this.form.get('lastName')?.updateValueAndValidity();
    this.form.get('email')?.updateValueAndValidity();
    this.form.get('licenseNumber')?.updateValueAndValidity();
    this.form.get('phone')?.updateValueAndValidity();
    this.form.get('address1')?.updateValueAndValidity();
    this.form.get('city')?.updateValueAndValidity();
    this.form.get('state')?.updateValueAndValidity();
    this.form.get('zipCode')?.updateValueAndValidity();
    this.form.get('country')?.updateValueAndValidity();
  }

  checkFeatureValidation(){
    this.isValid.set(!this.addInProgress &&
      (this._allowedContacts?.every(x => x.quantityRequired <= (this._contacts.filter(y => y.contactType == x.contactTypeValue)?.length ?? 0)) ?? false)
    );
  }

  switchForm(){
    this.addInProgress = !this.addInProgress;
    this.checkFeatureValidation();
    if (!this.addInProgress){
      this.form.reset();
    }
  }

  addContact(){
    var values = this.form.getRawValue();
    if (this.instanceId){
      this.api.addContact(this.instanceId, values).subscribe({
        next: () =>{
          this.switchForm();
          this.load();
        },
        error: err => {
          this.error = err.error?.title ?? 'Unable to delete the Contact.';
          this.cdr.detectChanges();
        }
      });
    }
    else{
      this._contacts?.push(values);
      this.contacts.set(this._contacts);
      this.switchForm();
      this.cdr.detectChanges();
    }
  }

  remove(id: number){
    if (this.instanceId) {
      this.api.deleteContact(id).subscribe({
        next: () => {
          this.load();
          this.checkFeatureValidation();
          this.cdr.detectChanges();
        },
        error: err => {
          this.error = err.error?.title ?? 'Unable to delete the Contact.';
          this.cdr.detectChanges();
        }
      });
    }
    else{
      this._contacts.splice(id, 1);
      this.contacts.set(this._contacts);
      this.checkFeatureValidation();
    }
  }

  private load() {
    if(this.instanceId){
      this.api.getInstanceContacts(this.instanceId).subscribe({
        next: contacts => {
          this._contacts = contacts;
          this.cdr.detectChanges();
        },
        error: err => {
          this.error = err.error?.title ?? 'Unable to load instance.';
          this.cdr.detectChanges();
        }
      });
    }
    else{
      this._contacts = this.contacts() ?? [];
    }
    this.api.getContactsConfig(this.classTypeId).subscribe({
      next: contacts => {
        this._allowedContacts = contacts;
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load allowed contacts.';
        this.cdr.detectChanges();
      }
    });
  }
}