import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClassTypeSummary } from '../../core/models';

@Component({
  selector: 'app-class-types-list',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="page-heading">
      <div>
        <h1>Class Types</h1>
        <p>Define configurable business process templates, fields, statuses, and workflow actions.</p>
      </div>
    </section>

    <section class="two-column">
      <form class="panel" [formGroup]="form" (ngSubmit)="create()">
        <h2>Create Class Type</h2>
        <label>Name <input formControlName="name" /></label>
        <label>Description <textarea formControlName="description"></textarea></label>
        <button type="submit" [disabled]="form.invalid">Create</button>
        @if (error) { <p class="error">{{ error }}</p> }
      </form>

      <div class="panel">
        <h2>Configured Types</h2>
        <div class="table-list">
          @for (type of classTypes; track type.id) {
            <a class="row-link" [routerLink]="['/class-types', type.id]">
              <span>
                <strong>{{ type.name }}</strong>
                <small>{{ type.description }}</small>
              </span>
              <span class="metrics">{{ type.fieldCount }} fields · {{ type.actionCount }} actions</span>
            </a>
          } @empty {
            <p>{{ error || 'No class types yet.' }}</p>
          }
        </div>
      </div>
    </section>
  `
})
export class ClassTypesList implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  classTypes: ClassTypeSummary[] = [];
  error = '';
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['']
  });

  ngOnInit() {
    this.load();
  }

  create() {
    if (this.form.invalid) {
      return;
    }

    this.api.createClassType(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.reset({ name: '', description: '' });
        this.error = '';
        this.load();
      },
      error: err => {
        this.error = err.error?.title ?? 'Unable to create class type.';
        this.cdr.detectChanges();
      }
    });
  }

  private load() {
    this.api.listClassTypes().subscribe({
      next: types => {
        this.classTypes = types;
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
