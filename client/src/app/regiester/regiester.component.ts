import { Component,inject,input,  output, } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-regiester',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './regiester.component.html',
  styleUrl: './regiester.component.css'
})
export class RegiesterComponent {
  private accountservice = inject(AccountService);
  private toster = inject(ToastrService);
  cancelRegister = output<boolean>();
  model: any = {}

  register(){
    this.accountservice.register(this.model).subscribe({
      next: response => {
        console.log(response);
      this.cancel();
      },
      error: error => this.toster.error(error.error)
      
    })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
}
