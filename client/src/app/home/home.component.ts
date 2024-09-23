import { Component, inject, OnInit } from '@angular/core';
import { RegiesterComponent } from "../regiester/regiester.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegiesterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{



  registerMode = false;
  http = inject(HttpClient);
  users: any;

  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle(){
    this.registerMode = !this.registerMode
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
    }

  getUsers() {
    this.http.get('http://localhost:5164/api/users').subscribe({
      next: Response => this.users = Response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    })
  }

}
