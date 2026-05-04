import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult } from '../../../shared/models/paged-result.model';
import { Company, CompanyQuery } from '../models/company.model';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/companies';

  getCompanies(query: CompanyQuery = {}): Observable<PagedResult<Company>> {
    let params = new HttpParams();
    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.hasActiveVacancies !== undefined) {
      params = params.set('hasActiveVacancies', String(query.hasActiveVacancies));
    }
    if (query.page !== undefined) {
      params = params.set('page', String(query.page));
    }
    if (query.pageSize !== undefined) {
      params = params.set('pageSize', String(query.pageSize));
    }
    return this.http.get<PagedResult<Company>>(this.baseUrl, { params });
  }

  getWithActiveVacancies(): Observable<PagedResult<Company>> {
    return this.getCompanies({ hasActiveVacancies: true, pageSize: 100 });
  }

  getAll(): Observable<PagedResult<Company>> {
    return this.getCompanies({ pageSize: 100 });
  }
}
