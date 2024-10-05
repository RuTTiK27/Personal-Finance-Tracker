import { Injectable } from '@angular/core';
import { HttpClient,HttpHeaders  } from '@angular/common/http';
import { catchError, map, Observable,tap, throwError } from 'rxjs';
import { jwtDecode } from "jwt-decode";
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5021/api/users'; 

  private accessTokenKey = 'accessToken';
  private refreshTokenKey = 'refreshToken';
  
  constructor(
    private http: HttpClient,
    private router: Router,
    private cookieService: CookieService
  ) {  }
  // Log in the user and set the tokens in cookies
  login(Email:string, Password:string): Observable<any> {
    
    return this.http.post(`${this.apiUrl}/login`, { Email, Password })
      .pipe(
        tap((response: any) => {
          this.setAccessToken(response.accessToken);
          this.setRefreshToken(response.refreshToken);
        })
      );
  }
// Method to refresh the access token
refreshAccessToken(): Observable<any> {
  const refreshToken = this.cookieService.get('refreshToken'); // Get refresh token from cookie

  if (!refreshToken) {
    return throwError('No refresh token available');
  }

  return this.http.post(`${this.apiUrl}/refresh-token`, { refreshToken: refreshToken }).pipe(
    map((response: any) => {
      this.setAccessToken(response.accessToken);
      this.setRefreshToken(response.refreshToken);
      return response.accessToken;
    }),
    catchError(error => {
      this.logout(); // If refresh token is invalid, log out the user
      return throwError(error);
    })
  );
}
  // Set the access token in a cookie
  setAccessToken(token: string) {
    const decoded: any = jwtDecode(token); // Decode the token to get expiration details
    const expiresIn = decoded.exp; // Get the expiration time (in seconds)

    // Create a Date object for the expiration date
    const expiryDate = new Date(expiresIn * 1000); // Convert to milliseconds

    this.cookieService.set('accessToken', token, expiryDate); // Set the access token cookie with expiration
  }

  // Set the refresh token in a cookie
  setRefreshToken(token: string) {
    const expiryDate = new Date(Date.now() + 2 * 24 * 60 * 60 * 1000);
    this.cookieService.set('refreshToken', token, expiryDate);
  }

  // Get the access token from cookies
  getAccessToken(): string | null {
    return this.cookieService.get(this.accessTokenKey);
  }

  // Get the refresh token from cookies
  getRefreshToken(): string | null {
    return this.cookieService.get(this.refreshTokenKey);
  }

  // Remove tokens from cookies (logout)
  clearTokens() {
    this.cookieService.delete(this.accessTokenKey, '/');
    this.cookieService.delete(this.refreshTokenKey, '/');
  }

  // Refresh the access token using the refresh token
  refreshToken(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      return this.http.post(`${this.apiUrl}/refresh-token`, { refreshToken })
        .pipe(
          tap((response: any) => {
            this.setAccessToken(response.accessToken);
          })
        );
    } else {
      // If no refresh token, log out or handle the situation
      this.logout();
      return new Observable(observer => observer.error('No refresh token available'));
    }
  }

  // Log out the user
  logout() {
    this.clearTokens();
    this.router.navigate(['/login']);
  }

  // Check if the user is logged in (access token exists and is valid)
  isLoggedIn(): boolean {
    const token = this.cookieService.get('accessToken');

    if (!token) {
      return false;
    }

    try {
      return !this.isTokenExpired(token);
    } catch {
      return false;
    }
  }
  private isTokenExpired(token: string): boolean {
    const decoded: any = jwtDecode(token); // Decode the token
    const currentTime = Date.now() / 1000; // Get current time in seconds
    return decoded.exp < currentTime; // Check if the token is expired
  }
}
