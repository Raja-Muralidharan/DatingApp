import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_modules/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);
  private presenceservice =inject(PresenceService);
  
  private likeservice = inject(LikesService);
  baseUrl = environment.apiUrl;
  
  currentUser = signal<User | null>(null);

  roles = computed(() => {
    const user = this.currentUser();
    if(user && user.token){
      const role = JSON.parse(atob(user.token.split('.')[1])).role
      return Array.isArray(role) ? role : [role]
    }
    return [];
  })

  login(model: any)
  {
    
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
         if(user){
           this.setCurrentUser(user);
         }
      })
    )

  }

  register(model: any)
  {
    
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
         if(user){
          this.setCurrentUser(user);
         }
         return user;
      })
    )

  }

  setCurrentUser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likeservice.getLikeIds();
    this.presenceservice.createHubConnection(user)
  }

  logout()
  {
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.presenceservice.stopHubConnection();
  }
  
}
