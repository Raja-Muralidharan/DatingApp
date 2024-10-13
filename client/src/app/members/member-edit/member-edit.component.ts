import { Component, HostListener, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { Member } from '../../_modules/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormControl, FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule, PhotoEditorComponent],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit{

@ViewChild('editForm') editForm?: NgForm;
@HostListener('window:beforeunload',['$event']) notify($event: any){
  if(this.editForm?.dirty){
    $event.returnValue = true;
  }
}
  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toster = inject(ToastrService);
  

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    const user = this.accountService.currentUser();
    if(!user) return;
    this.memberService.getMember(user.username).subscribe({
         next: memnber => this.member = memnber
    })
  }

  updateMember(){
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _=> {
        this.toster.success('Profile updated Successfully');
        this.editForm?.reset(this.member);
      }
    })
    
  }

  onMemberChange(event: Member){
    this.member = event;
  }
}
