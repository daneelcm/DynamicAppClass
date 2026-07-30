import { ChangeDetectorRef, Component, inject, Input, OnInit } from '@angular/core';
import { AllowedContact, Contact } from '../../core/models';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-instance-contact',
  imports: [],
  template: `
    <div class="panel">
      <div style="display: flex; justify-content: space-between; align-items: center;">
        <h2>Contacts</h2>
        <button type="button">Add Contact</button>
      </div>

      @if (error) { <p class="error banner">{{ error }}</p> }

      <ul class="compact-list">
        @for (contact of contacts; track contact.id) {
        <li style="display: flex; justify-content: space-between;">
          <span style="display: grid;">
          @if (contact.isEntity) { <strong>{{ contact.entityName }}</strong> } @else { <strong>{{ contact.firstName }} {{ contact.lastName }}</strong> }
          @if (contact.contactType) { {{ allowedContacts?.find(c => c.contactTypeValue === contact.contactType)?.contactTypeCaption }} }
          @if (contact.phone) { | {{ contact.phone }} }
          @if (contact.email) { | {{ contact.email }} }
          @if (contact.address1 || contact.address2 || contact.city || contact.zipCode || contact.country) {
          | {{ [contact.address1, contact.address2, contact.city, contact.zipCode, contact.country].filter(x => x).join(', ') }}
          }
          @if (contact.licenseNumber) { | License: {{ contact.licenseNumber }} }
          </span>
          <a class="row-link" style="padding: 10px; cursor: pointer;" (click)="remove(contact.id)">❌</a>
        </li>
        }
      </ul>
    </div>
  `
})
export class ManageContactPartial implements OnInit {
  private readonly api = inject(ApiService);
  private readonly cdr = inject(ChangeDetectorRef);
  
  allowedContacts?: AllowedContact[];
  contacts?: Contact[];
  error = ''
  
  @Input()
  classTypeId!: number;
  @Input()
  instanceId!: number;

  ngOnInit(): void {
    this.load();
  }

  remove(id: number){
    this.api.deleteContact(id).subscribe({
      next: () => {
        this.load();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to delete the Contact.';
        this.cdr.detectChanges();
      }
    });
  }

  private load() {
    this.api.getInstanceContacts(this.instanceId).subscribe({
      next: contacts => {
        this.contacts = contacts;
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load instance.';
        this.cdr.detectChanges();
      }
    });
    this.api.getContactsConfig(this.classTypeId).subscribe({
      next: contacts => {
        this.allowedContacts = contacts;
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load allowed contacts.';
        this.cdr.detectChanges();
      }
    });
  }
}