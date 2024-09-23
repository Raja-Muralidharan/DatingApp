import { Component,inject,input,  output, } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-regiester',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './regiester.component.html',
  styleUrl: './regiester.component.css'
})
export class RegiesterComponent {
  private accountservice = inject(AccountService);
  cancelRegister = output<boolean>();
  model: any = {}

  register(){
    this.accountservice.register(this.model).subscribe({
      next: response => {
        console.log(response);
      this.cancel();
      },
      error: error => console.log()
      
    })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
}
