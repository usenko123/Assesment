import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, forkJoin, map, of, switchMap } from 'rxjs';
import { PagedResult } from '../../../shared/models/paged-result.model';
import { Company, CompanyQuery } from '../models/company.model';

/** Matches backend `PageQuery.MaxPageSize` — use full pages to minimize requests when aggregating. */
const FETCH_PAGE_SIZE = 100;

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

  /** Loads every page until all matching companies are in memory (no silent cap). */
  getWithActiveVacancies(): Observable<PagedResult<Company>> {
    return this.fetchAllCompanies({ hasActiveVacancies: true });
  }

  /** Loads every page until all companies are in memory (no silent cap). */
  getAll(): Observable<PagedResult<Company>> {
    return this.fetchAllCompanies({});
  }

  private fetchAllCompanies(query: Omit<CompanyQuery, 'page' | 'pageSize'>): Observable<PagedResult<Company>> {
    return this.getCompanies({ ...query, page: 1, pageSize: FETCH_PAGE_SIZE }).pipe(
      switchMap((first) => this.mergeRemainingPages(first, query)),
    );
  }

  private mergeRemainingPages(
    first: PagedResult<Company>,
    query: Omit<CompanyQuery, 'page' | 'pageSize'>
  ): Observable<PagedResult<Company>> {
    const pageSize = first.pageSize;
    const total = first.total;
    const pageCount = Math.max(1, Math.ceil(total / pageSize));

    if (pageCount <= 1) {
      return of(this.singleAggregated(first));
    }

    const extraRequests: Observable<PagedResult<Company>>[] = [];
    for (let page = 2; page <= pageCount; page++) {
      extraRequests.push(this.getCompanies({ ...query, page, pageSize }));
    }

    return forkJoin(extraRequests).pipe(
      map((rest) => ({
        items: [...first.items, ...rest.flatMap((p) => p.items)],
        total,
        page: 1,
        pageSize
      }))
    );
  }

  /** One API page covers the full dataset. */
  private singleAggregated(page: PagedResult<Company>): PagedResult<Company> {
    return {
      items: page.items,
      total: page.total,
      page: 1,
      pageSize: page.pageSize
    };
  }
}
