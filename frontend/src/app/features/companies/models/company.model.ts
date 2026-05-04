import { Vacancy } from '../../vacancies/models/vacancy.model';

export interface Company {
  id: number;
  name: string;
  address: string;
  vacancies: Vacancy[];
}

export interface CompanyQuery {
  search?: string;
  hasActiveVacancies?: boolean;
  page?: number;
  pageSize?: number;
}
