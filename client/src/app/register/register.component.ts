import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{

  @Output() cancelRegister = new EventEmitter();

  registerForm : FormGroup;

  maxDate: Date;

  validationErrors: string[] = [];

  constructor(private accountService : AccountService, private toastr : ToastrService,
              private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm(); // Initialize the form
    // Set the max date to 18 years ago
    this.maxDate = new Date(); // Today's date 
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18); // Subtract 18 years from today's date
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender : ['male'],
      username : ['',Validators.required],
      knownAs : ['',Validators.required],
      dateOfBirth : ['',Validators.required],
      city : ['',Validators.required],
      country : ['',Validators.required],
      password: [
        '',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
      
        
    });
  } 

  private matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      if (control.parent && control.parent.controls) {
        return control.value === (control.parent.controls as { [key: string]: AbstractControl })[matchTo].value
          ? null : { isMatching: true };
      }
      return null;
    }
  } 

  
  register() {
    this.accountService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
      console.log(error);
      // this.toastr.error(error.error);
      this.validationErrors = error;
    })
  }
  
  cancel(){
    this.cancelRegister.emit(false);
  }

}
