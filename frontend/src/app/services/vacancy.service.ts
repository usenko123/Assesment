import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateCompanyVacancyRequest, PagedResult, Vacancy, VacancyQuery } from '../models';

@Injectable({ providedIn: 'root' })
export class VacancyService {
  private readonly http = inject(HttpClient);
  private readonly companiesBaseUrl = '/api/companies';
  private readonly vacanciesBaseUrl = '/api/vacancies';

  getVacancies(query: VacancyQuery = {}): Observable<PagedResult<Vacancy>> {
    let params = new HttpParams();
    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.companyId !== undefined) {
      params = params.set('companyId', String(query.companyId));
    }
    if (query.isActive !== undefined) {
      params = params.set('isActive', String(query.isActive));
    }
    if (query.page !== undefined) {
      params = params.set('page', String(query.page));
    }
    if (query.pageSize !== undefined) {
      params = params.set('pageSize', String(query.pageSize));
    }
    return this.http.get<PagedResult<Vacancy>>(this.vacanciesBaseUrl, { params });
  }

  createForCompany(companyId: number, payload: CreateCompanyVacancyRequest): Observable<Vacancy> {
    return this.http.post<Vacancy>(`${this.companiesBaseUrl}/${companyId}/vacancies`, payload);
  }
}
