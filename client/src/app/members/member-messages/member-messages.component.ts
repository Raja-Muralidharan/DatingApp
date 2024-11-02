import { Component, inject, input, OnInit, output, ViewChild } from '@angular/core';
import { Message } from '../../_modules/message';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm?: NgForm;
  private messageservice = inject(MessageService);
  username = input.required<string>();
  messages = input.required<Message[]>();
  messagecontent = '';
  updatemessage = output<Message>();


  sendMessage(){
    this.messageservice.sendMessage(this.username(), this.messagecontent).subscribe({
      next: message => {
        this.updatemessage.emit(message);
        this.messageForm?.reset();
      }
    })
  }
 



}
