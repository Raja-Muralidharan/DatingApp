import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_modules/user';
import { take } from 'rxjs';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);
  private router = inject(Router);
  onlinceUsers = signal<string[]>([]);

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(err => console.log(err));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlinceUsers.update(users => [...users, username]);
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.onlinceUsers.update(users => users.filter(x => x !== username));
    });

    this.hubConnection.on('GetOnlineUsers', username => {
      this.onlinceUsers.set(username)
    });

    this.hubConnection.on('NewMessageReceived', ({username, KnownAs}) => {
      this.toastr.info('New Message from ' + KnownAs + ' has been received')
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=Messages'))
    })
  }


  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(err => console.log(err))

    }
  }
}
