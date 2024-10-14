import { Component,inject,  OnInit,  output, } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatePickerComponent } from "../_form/date-picker/date-picker.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-regiester',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, TextInputComponent, DatePickerComponent],
  templateUrl: './regiester.component.html',
  styleUrl: './regiester.component.css'
})
export class RegiesterComponent implements OnInit {
 
  private accountservice = inject(AccountService);
  private frombuilder = inject(FormBuilder);
  private router = inject(Router);
  cancelRegister = output<boolean>();
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();

  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.intializeform();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }

  intializeform(){
    this.registerForm = this.frombuilder.group({
      gender: ['male'],
      KnownAs: ['',Validators.required],
      dateofBirth: ['',Validators.required],
      city: ['',Validators.required],
      country: ['',Validators.required],
      username: ['',Validators.required],
      password: ['',[Validators.required,Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['',[Validators.required,this.matchValue('password')]]
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValue(matchTo: string):ValidatorFn{
    return(control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}
    }
  }

  register(){
    const dob = this.getDateOnly(this.registerForm.get('dateofBirth')?.value);
    this.registerForm.patchValue({dateofBirth: dob});
    this.accountservice.register(this.registerForm.value).subscribe({
      next: _ => this.router.navigateByUrl('/members'),
      error: error => this.validationErrors = error
      
    })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | string){
    if(!dob) return;
    return new Date(dob).toISOString().slice(0,10);
  }
}
