import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'http://localhost:5021/api/users'; 

  constructor(private http: HttpClient, private authService: AuthService) { }

  private makeApiCall(apiCall: () => Observable<any>): Observable<any> {
    if (!this.authService.isLoggedIn()) {
      return this.authService.refreshToken().pipe(
        switchMap(() => apiCall()), // Proceed with the original API call after refresh
        catchError(err => {
          console.error('Could not refresh token', err);
          this.authService.logout(); // Log out if refresh fails
          return throwError(err); // Rethrow the error
        })
      );
    } else {
      return apiCall(); // Proceed with the original API call
    }
  }
  
  getReservedUsernames(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/reservedusernames`);
    //return this.makeApiCall(() => this.http.get<string[]>(`${this.apiUrl}/reservedusernames`));
  }

  getReservedEmails():Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/reservedemails`);
  }

  // Method to register user
  registerUser(user: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/`, user);
  }
  
  validateUser(user: any): Observable<any> {                                                  
    //return this.http.post<any>(`${this.apiUrl}/validate-user/`, user);
    return this.makeApiCall(() => this.http.post<any>(`${this.apiUrl}/validate-user/`, user));
  }
}
