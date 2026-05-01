import { Routes } from '@angular/router';
import { CompanyListPageComponent } from './pages/company-list-page.component';
import { CreateVacancyPageComponent } from './pages/create-vacancy-page.component';

export const routes: Routes = [
  { path: '', component: CompanyListPageComponent },
  { path: 'vacatures/nieuw', component: CreateVacancyPageComponent },
  { path: '**', redirectTo: '' }
];
