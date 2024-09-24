import { NgModule } from '@angular/core';
import { RouterModule,Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { DashboardComponent } from './Components/dashboard/dashboard.component';
import { AuthGuard } from './Guards/auth.guard'; // Import Auth Guard

export const routes: Routes = [
    {path:'login', component:LoginComponent},
    {path: 'register', component:RegisterComponent},
    {path: '', redirectTo:'login', pathMatch:'full'}, //Default route
    {path: 'dashboard',component:DashboardComponent,canActivate:[AuthGuard]}
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
  })
  export class AppRoutingModule { }