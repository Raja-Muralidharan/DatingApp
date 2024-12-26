import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginatedResult } from '../_modules/pagination';
import { Message } from '../_modules/message';
import { setPaginatedResponse, setPaginationHeaders } from './PaginationHelper';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_modules/user';
import { Group } from '../_modules/groups';
import { BusyService } from './busy.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;
  hubConnection?: HubConnection;
  private busyService = inject(BusyService);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);
  messageThread = signal<Message[]>([]);

  constructor(private http: HttpClient) { }

  createHubConnection(user:User, otherusername: string){
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherusername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();


    this.hubConnection.start()
      .catch(error => console.log(error))
      .finally(() => this.busyService.idle());

    this.hubConnection.on('ReciveMessageThread', messages => {
        this.messageThread.set(messages)
    });

    this.hubConnection.on('NewMessage', message => {
        this.messageThread.update(messages => [...messages, message])
    });

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if(group.connections.some(x => x.username === otherusername)){
        this.messageThread.update(messages => {
          messages.forEach(message => {
            if(!message.dateRead){
              message.dateRead = new Date(Date.now())
            }
          })
          return messages;
        })
      }
    });

  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log(error))
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string){
    let params = setPaginationHeaders(pageNumber,pageSize);

    params = params.append('Container', container);

    return this.http.get<Message[]>(this.baseUrl + 'messages', {observe: 'response', params})
       .subscribe({
        next: response => setPaginatedResponse(response, this.paginatedResult)
       })
  }

  getMessageThread(username: string){
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }


  async sendMessage(username: string, content: string) {
    if (this.hubConnection?.state === HubConnectionState.Disconnected) {
      this.hubConnection.start().catch(error => console.log(error))
    }
    return this.hubConnection?.invoke('SendMessage', {recipientUsername: username, content})
  }

  deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
