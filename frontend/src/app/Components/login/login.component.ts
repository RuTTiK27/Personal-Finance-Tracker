import { Component } from '@angular/core';
import {
  FormControl,
  FormGroupDirective,
  NgForm,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormGroup,
  FormBuilder
} from '@angular/forms';
import {ErrorStateMatcher} from '@angular/material/core';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import {MatGridListModule} from '@angular/material/grid-list';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../Services/auth.service'; // Create this service
import { HttpErrorResponse } from '@angular/common/http';

/** Error when invalid control is dirty, touched, or submitted. */
export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    MatGridListModule, 
    FormsModule, 
    MatFormFieldModule, 
    MatInputModule, 
    ReactiveFormsModule,
    MatButtonModule,
    RouterLink,
    CommonModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  isSmallScreen: boolean = false;

  matcher = new MyErrorStateMatcher();
  errorMessage: string = '';
  loginForm!: FormGroup;

  constructor(private breakpointObserver: BreakpointObserver,private fb: FormBuilder,private authService: AuthService,private router: Router) {
    this.breakpointObserver.observe([Breakpoints.Small, Breakpoints.XSmall])
      .subscribe(result => {
        this.isSmallScreen = result.matches;
      })
      // Properly initialize the FormGroup with validation
      this.loginForm = this.fb.group({
        Email: ['', [Validators.required, Validators.email]],
        Password: ['', Validators.required]
      });
  }


  onSubmit(){
    if (this.loginForm.valid) {
      const { Email, Password } = this.loginForm.value;  // Extract username and password
      this.authService.login(Email, Password).subscribe(
        (response: any) => {
          // Store the token
          this.router.navigate(['/dashboard']); // Navigate to protected route
        },
        (error: HttpErrorResponse) => {
          this.errorMessage = 'Invalid username or password';
        }
      );
    }
    
  }

}
