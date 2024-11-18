import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-model',
  standalone: true,
  imports: [],
  templateUrl: './roles-model.component.html',
  styleUrl: './roles-model.component.css'
})
export class RolesModelComponent {
   bsModelRef = inject(BsModalRef);
   title = '';
   username = '';
   list: string[] = [];
   availableRoles: string[] = [];
   selectedRoles: string[] = [];
   rolesUpdated = false;

   updateChecked(checkedValue: string){
     if(this.selectedRoles.includes(checkedValue)) {
      this.selectedRoles = this.selectedRoles.filter(r => r !== checkedValue)
     } else {
      this.selectedRoles.push(checkedValue);
     }
   }


   onSelectRoles(){
    this.rolesUpdated = true;
    this.bsModelRef.hide();
   }
}
