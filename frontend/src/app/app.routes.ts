import { NgModule } from '@angular/core';
import { RouterModule,Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { AuthGuard } from './Guards/auth.guard'; // Import Auth Guard

export const routes: Routes = [
    {path: 'register', component:RegisterComponent},
    { path: 'login', component: LoginComponent }, // No guard on login
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] }, // Protected route
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: '**', redirectTo: '/login' } // Redirect any unknown routes to login
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
  export class AppRoutingModule { }