import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassFeature, ClassTypeDetail, ClassTypeSummary } from '../../core/models';

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
  `
})
export class ConfigContactsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  classType?: ClassTypeDetail;
  feature?: ClassFeature;
  error = '';
  id = 0;
  typeId = 0;
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['']
  });


  ngOnInit() {
    this.id = Number.parseInt(this.route.snapshot.paramMap.get('id') ?? '0');
    this.typeId = Number.parseInt(this.route.snapshot.paramMap.get('typeId') ?? '0');

    this.load();
  }

  private load() {
    this.api.getClassType(this.typeId).subscribe({
      next: type => {
        this.classType = type;
        this.error = '';
        this.cdr.detectChanges();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to load class types. Confirm the API is running at http://localhost:5000.';
        this.cdr.detectChanges();
      }
    });
  }
}
