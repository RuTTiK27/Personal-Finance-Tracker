import { Component } from '@angular/core';
import {
  FormControl,
  FormGroupDirective,
  NgForm,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  ValidatorFn,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import {ErrorStateMatcher} from '@angular/material/core';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import { RouterLink } from '@angular/router';
import {MatGridListModule} from '@angular/material/grid-list';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { UserService } from '../../Services/user.service';

/** Error when invalid control is dirty, touched, or submitted. */

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    MatGridListModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule,
    FormsModule, 
    ReactiveFormsModule,
    RouterLink,
    CommonModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})

export class RegisterComponent {

  isSmallScreen: boolean = false;
  reservedUsernames: string[] = [];

  usernameFormControl = new FormControl('', [Validators.required,
    Validators.minLength(3),
    Validators.maxLength(20),
    Validators.pattern('^[a-zA-Z0-9_-]{3,20}$')
    ]);

  emailFormControl = new FormControl('', [Validators.required, 
      Validators.email,
      Validators.minLength(5),
      Validators.maxLength(254)
    ]);

  passwordFormControl = new FormControl('', [Validators.required,
      Validators.minLength(8),
      Validators.maxLength(128),
      Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$')
    ]);

  matcher = new MyErrorStateMatcher();

  constructor(private breakpointObserver: BreakpointObserver,private userService: UserService) {
    
    this.breakpointObserver.observe([Breakpoints.Small, Breakpoints.XSmall])
      .subscribe(result => {
        this.isSmallScreen = result.matches;
      });

    this.userService.getReservedUsernames().subscribe(
        usernames => {
          this.reservedUsernames = usernames;
          this.usernameFormControl.addValidators(this.reservedUsernameValidator(this.reservedUsernames));
          this.usernameFormControl.updateValueAndValidity(); // Make sure to revalidate after adding validators
        },
        error => {
          console.error('Error fetching reserved usernames', error);
        }
      );  
  }

  reservedUsernameValidator(reservedUsernames: string[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return reservedUsernames.includes(control.value) ? { reservedUsername: true } : null;
    };
  }

  onSubmit(){
    if (!this.emailFormControl.errors && !this.usernameFormControl.errors &&  !this.passwordFormControl.errors) {
      console.warn("submited");
    }else{
      console.warn("error there");
    } 
    
  }
}
