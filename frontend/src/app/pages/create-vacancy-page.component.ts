import { Component, DestroyRef, computed, effect, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { CompanyService } from '../services/company.service';
import { VacancyService } from '../services/vacancy.service';
import { Company } from '../models';

interface CompaniesState {
  companies: Company[];
  error: string | null;
}

@Component({
  selector: 'app-create-vacancy-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-vacancy-page.component.html',
  styleUrl: './create-vacancy-page.component.scss'
})
export class CreateVacancyPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly companyService = inject(CompanyService);
  private readonly vacancyService = inject(VacancyService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly state = toSignal<CompaniesState | null>(
    this.companyService.getAll().pipe(
      map(companies => ({ companies, error: null })),
      catchError(() =>
        of<CompaniesState>({
          companies: [],
          error: 'Kon bedrijven niet laden voor de vacature.'
        })
      )
    ),
    { initialValue: null }
  );

  protected readonly companies = computed(() => this.state()?.companies ?? []);
  protected readonly loading = computed(() => this.state() === null);
  protected readonly submitError = signal<string | null>(null);
  protected readonly submitSuccess = signal<string | null>(null);

  protected readonly form = this.fb.group({
    companyId: this.fb.control<number | null>(null, [Validators.required]),
    title: this.fb.control('', [Validators.required]),
    description: this.fb.control(''),
    isActive: this.fb.control(true, [Validators.required])
  });

  constructor() {
    effect(() => {
      const loadError = this.state()?.error ?? null;
      if (loadError) {
        this.submitError.set(loadError);
      }
    });

    effect(() => {
      const companies = this.companies();
      if (companies.length > 0 && this.form.controls.companyId.value === null) {
        this.form.patchValue({ companyId: companies[0].id });
      }
    });
  }

  protected submit(): void {
    const companyId = this.form.controls.companyId.value;
    if (this.form.invalid || companyId === null) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitError.set(null);
    this.submitSuccess.set(null);

    this.vacancyService.createForCompany(companyId, {
      title: (this.form.value.title ?? '').trim(),
      description: this.form.value.description?.trim() || null,
      isActive: this.form.value.isActive ?? true
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.submitSuccess.set('Vacature succesvol aangemaakt.');
          const isActive = this.form.controls.isActive.value ?? true;
          this.form.patchValue({ title: '', description: '', isActive });
          this.form.markAsPristine();
          this.form.markAsUntouched();
        },
        error: () => {
          this.submitError.set('Vacature kon niet worden opgeslagen.');
        }
      });
  }
}
