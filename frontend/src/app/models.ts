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

export interface CreateCompanyVacancyRequest {
  title: string;
  description?: string | null;
  isActive: boolean;
}
