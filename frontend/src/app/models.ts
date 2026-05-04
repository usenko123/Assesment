export interface Vacancy {
  id: number;
  title: string;
  description?: string | null;
  isActive: boolean;
  companyId: number;
}

export interface Company {
  id: number;
  name: string;
  address: string;
  vacancies: Vacancy[];
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface CompanyQuery {
  search?: string;
  hasActiveVacancies?: boolean;
  page?: number;
  pageSize?: number;
}

export interface VacancyQuery {
  search?: string;
  companyId?: number;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface CreateCompanyVacancyRequest {
  title: string;
  description?: string | null;
  isActive: boolean;
}
