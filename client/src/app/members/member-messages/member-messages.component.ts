import { AfterViewChecked, Component, inject, Input, ViewChild } from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})


export class MemberMessagesComponent implements AfterViewChecked {
  @ViewChild('messageForm') messageForm?: NgForm;
  @ViewChild('scrollMe') scroll?: any;
  messageservice = inject(MessageService);
  @Input() username: string = '';
  messageContent: string = '';

  constructor(public messageService: MessageService) { }


  sendMessage() {
    
    if (this.username && this.messageContent) {
      this.messageservice.sendMessage(this.username, this.messageContent).then(() => {
        this.messageForm?.reset();
        this.scrollToBottom();
      });
    }
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  private scrollToBottom() {
    if(this.scroll){
      this.scroll.nativeElement.scrollTop = this.scroll.nativeElement.scrollHeight;
    }
  
  }
 



}
