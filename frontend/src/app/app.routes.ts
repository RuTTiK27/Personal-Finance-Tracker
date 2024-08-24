import { Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';

export const routes: Routes = [
    {path:'login', component:LoginComponent},
    {path: 'register', component:RegisterComponent},
    {path: '', redirectTo:'login', pathMatch:'full'} //Default route
];
