import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { jwtDecode } from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5021/api/users'; 

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: any) {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, credentials,{ withCredentials: true }).pipe(
      tap((response: { token: string; }) => {
        // Set the cookie here
        this.setCookie('token', response.token, 1/1440); // Set for 1 day
      })
    );
  }

  private setCookie(name: string, value: string, days: number) {
    const expires = new Date();
    expires.setTime(expires.getTime() + (days * 24 * 60 * 60 * 1000));
    const expiresString = `expires=${expires.toUTCString()}`;
    document.cookie = `${name}=${value}; ${expiresString}; path=/; Secure; SameSite=Strict`; // Ensure cookies are secure and scoped to your site
  }
  
  isAuthenticated(): boolean {
      const token = localStorage.getItem('token');
      console.log("token : "+token);
    if (token) {
      const decoded: any = jwtDecode(token);
      const expirationTime = decoded.exp * 1000; // Convert to milliseconds
      return Date.now() < expirationTime; // Check if the current time is less than the expiration time
    }
    return false; // No token found
  }

  logout() {
    localStorage.removeItem('token');
    document.cookie = 'yourCookieName=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
  }

}
