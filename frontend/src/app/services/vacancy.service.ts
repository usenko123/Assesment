import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateCompanyVacancyRequest, Vacancy } from '../models';

@Injectable({ providedIn: 'root' })
export class VacancyService {
  private readonly http = inject(HttpClient);
  private readonly companiesBaseUrl = '/api/companies';

  createForCompany(companyId: number, payload: CreateCompanyVacancyRequest): Observable<Vacancy> {
    return this.http.post<Vacancy>(`${this.companiesBaseUrl}/${companyId}/vacancies`, payload);
  }
}
