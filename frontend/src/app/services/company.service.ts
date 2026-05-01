import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Company } from '../models';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/companies';

  getWithActiveVacancies(): Observable<Company[]> {
    return this.http.get<Company[]>(`${this.baseUrl}/with-active-vacancies`);
  }

  getAll(): Observable<Company[]> {
    return this.http.get<Company[]>(this.baseUrl);
  }
}
