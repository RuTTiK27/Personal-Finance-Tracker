import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../Services/auth.service'; //../services/
import { catchError, map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}
  canActivate(next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | boolean {
      if (this.authService.isLoggedIn()) {
        return true; // Access token is valid
      }
  
      // If the access token is expired, attempt to refresh it using the refresh token
      return this.authService.refreshAccessToken().pipe(
        map(() => true), // If the refresh is successful, allow navigation
        catchError(() => {
          this.router.navigate(['/login']); // If refresh fails, redirect to login
          return of(false); // Prevent navigation to the protected route
        })
      );
    
  }
}
