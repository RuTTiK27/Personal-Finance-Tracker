import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError,catchError, switchMap } from 'rxjs';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.authService.getAccessToken();

    // Clone the request and add the Authorization header if accessToken exists
    let clonedRequest = req;
    if (accessToken) {
      clonedRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      });
    }
    // Proceed with the request and handle errors
    return next.handle(clonedRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        // If we get a 401 Unauthorized response, try to refresh the token
        if (error.status === 401) {
          return this.handle401Error(clonedRequest, next);
        }

        // If another error, just pass it along
        return throwError(() => error);
      })
    );
  }
  // Handle 401 errors by trying to refresh the token
  private handle401Error(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.authService.refreshToken().pipe(
      switchMap((newToken: any) => {
        // Clone the request again with the new access token
        const accessToken = this.authService.getAccessToken();
        if (accessToken) {
          const clonedRequest = req.clone({
            setHeaders: {
              Authorization: `Bearer ${accessToken}`
            }
          });
          return next.handle(clonedRequest); // Retry the request with the new token
        }

        // If no token available, log the user out
        this.authService.logout();
        return throwError(() => new Error('Unable to refresh token'));
      }),
      catchError((error) => {
        // If refreshing the token fails, log out the user
        this.authService.logout();
        return throwError(() => error);
      })
    );
  }
}
