import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { CompanyService } from '../services/company.service';
import { Company } from '../models';

interface CompanyListState {
  companies: Company[];
  error: string | null;
}

@Component({
  selector: 'app-company-list-page',
  imports: [RouterLink],
  templateUrl: './company-list-page.component.html',
  styleUrl: './company-list-page.component.scss'
})
export class CompanyListPageComponent {
  private readonly companyService = inject(CompanyService);

  private readonly state = toSignal<CompanyListState | null>(
    this.companyService.getWithActiveVacancies().pipe(
      map(result => ({ companies: result.items, error: null })),
      catchError(() =>
        of<CompanyListState>({
          companies: [],
          error: 'Kon bedrijven met actieve vacatures niet ophalen.'
        })
      )
    ),
    { initialValue: null }
  );

  protected readonly loading = computed(() => this.state() === null);
  protected readonly companies = computed(() => this.state()?.companies ?? []);
  protected readonly error = computed(() => this.state()?.error ?? null);
}
