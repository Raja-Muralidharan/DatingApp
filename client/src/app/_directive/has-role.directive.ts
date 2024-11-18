import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]',
  standalone: true
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[] = [];

  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templeteref = inject(TemplateRef);
  
  ngOnInit(): void {
    if(this.accountService.roles()?.some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templeteref)
    }else{
      this.viewContainerRef.clear();
    }
  }

}
