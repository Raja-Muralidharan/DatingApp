import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_modules/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModelComponent } from '../../modals/roles-model/roles-model.component';
import { Title } from '@angular/platform-browser';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit{

  private adminservice = inject(AdminService);
  private modalserice = inject(BsModalService);
  users: User[] =[];
  bsModalRef: BsModalRef<RolesModelComponent> = new BsModalRef<RolesModelComponent>();

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  openRolesModal(user: User){
    const initialState: ModalOptions = {
      class: 'modal-lg',
      initialState: {
        title:'User roles',
        username: user.username,
        selectedRoles: [...user.roles],
        availableRoles: ['Admin', 'Moderator','Member'],
        users: this.users,
        rolesUpdated: false,
      }
    }
    this.bsModalRef =  this.modalserice.show(RolesModelComponent, initialState);
    this.bsModalRef.onHide?.subscribe({
      next: ()=>{
        if(this.bsModalRef.content && this.bsModalRef.content.rolesUpdated)
          {
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminservice.updateUserRoles(user.username, selectedRoles).subscribe({
            next: (roles: any) => {
              user.roles = roles;
            }
          })
        }
      }
    })
  }

  
  getUsersWithRoles(){
    this.adminservice.getUserwithRoles().subscribe({
      next: users => this.users = users
    })
  }


}
