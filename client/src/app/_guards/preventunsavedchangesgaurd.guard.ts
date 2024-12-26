import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';
import { inject } from '@angular/core';

export const preventunsavedchangesgaurdGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
 
  const confrimService = inject(ConfirmService);
 
 
  if(component.editForm?.dirty){
    return confrimService.confirm() ?? false;
  }
  return true;
};
