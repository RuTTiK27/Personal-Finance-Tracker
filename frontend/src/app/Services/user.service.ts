import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'http://localhost:5021/api/users'; 

  constructor(private http: HttpClient) { }

  getReservedUsernames(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/reservedusernames`);
  }

  getReservedEmails():Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/reservedemails`);
  }

  // Method to register user
  registerUser(user: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/`, user);
  }
  
  validateUser(user: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/validate-user/`, user);
  }
}
