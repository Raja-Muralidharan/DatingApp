import { Component, inject, OnInit } from '@angular/core';
import { RegiesterComponent } from "../regiester/regiester.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegiesterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent{

  registerMode = false;

  ngOnInit(): void {  
  }

  registerToggle(){
    this.registerMode = !this.registerMode
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
    }


}
