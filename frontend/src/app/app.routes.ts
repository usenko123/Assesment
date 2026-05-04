import { Routes } from '@angular/router';
import { CompanyListPageComponent } from './features/companies/pages/company-list-page/company-list-page.component';
import { CreateVacancyPageComponent } from './features/vacancies/pages/create-vacancy-page/create-vacancy-page.component';

export const routes: Routes = [
  { path: '', component: CompanyListPageComponent },
  { path: 'vacatures/nieuw', component: CreateVacancyPageComponent },
  { path: '**', redirectTo: '' }
];
