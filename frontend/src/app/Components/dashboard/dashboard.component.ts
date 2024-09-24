import { Component } from '@angular/core';
import { AuthService } from '../../Services/auth.service'; // Adjust the path as necessary
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  constructor(private authService: AuthService, private router: Router) {}
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']); // Redirect to the login page after logout
  }
}
