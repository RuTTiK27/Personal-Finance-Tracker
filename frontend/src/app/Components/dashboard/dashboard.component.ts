import { Component } from '@angular/core';
import { AuthService } from '../../Services/auth.service'; // Adjust the path as necessary
import { DashboardService } from '../../Services/dashboard.service'; // Adjust the path as necessary
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  dashboardData: any;
  constructor(private authService: AuthService, private router: Router,private dashboardService: DashboardService) {
    this.loadDashboardData();
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']); // Redirect to the login page after logout
  }
  loadDashboardData(): void {
    this.dashboardService.getDashboardData().subscribe({
      next: (data) => {
        this.dashboardData = data;
      },
      error: (err) => {
        console.error('Error loading dashboard data:', err);
      }
    });
  }
}
