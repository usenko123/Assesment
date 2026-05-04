export interface Vacancy {
  id: number;
  title: string;
  description?: string | null;
  isActive: boolean;
  companyId: number;
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
