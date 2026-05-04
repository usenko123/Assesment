import { Component, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { CompanyService } from '../../services/company.service';
import { Company } from '../../models/company.model';

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

  private readonly companiesResource = rxResource({
    stream: () =>
      this.companyService.getWithActiveVacancies().pipe(
        map(
          (result): CompanyListState => ({
            companies: result.items,
            error: null
          })
        ),
        catchError(() =>
          of<CompanyListState>({
            companies: [],
            error: 'Kon bedrijven met actieve vacatures niet ophalen.'
          })
        )
      ),
    defaultValue: { companies: [], error: null } satisfies CompanyListState
  });

  /** Distinct from "no rows": stays true across refetches (`reload()`). */
  protected readonly loading = this.companiesResource.isLoading;
  protected readonly companies = computed(() => this.companiesResource.value().companies);
  protected readonly error = computed(() => this.companiesResource.value().error);
}
