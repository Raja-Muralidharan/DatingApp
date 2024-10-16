import { Component, inject } from '@angular/core';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { NgIf } from '@angular/common';
import {BsDropdownModule} from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule,RouterLink,RouterLinkActive],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {

  accountService = inject(AccountService);
  private router = inject(Router);
  private toster = inject(ToastrService);
  model: any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members')
      },
      error: error => this.toster.error(error.error)
      
    })
  }

  routeToEditpage()
  {
    this.router.navigateByUrl('/edit');
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
