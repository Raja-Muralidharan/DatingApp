import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_modules/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { Photo } from '../../_modules/Photo';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgIf, NgFor, NgStyle, NgClass, FileUploadModule, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',
})
export class PhotoEditorComponent implements OnInit {
  private accountSerice = inject(AccountService);
  member = input.required<Member>();
  uploader?: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  memberChange = output<Member>();
  private memberservice = inject(MembersService);

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(event: any) {
    this.hasBaseDropzoneOver = event;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.accountSerice.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = { ...this.member() };
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);

      if (photo.isMain) {
        const user = this.accountSerice.currentUser();
        if (user) {
          user.photoURL = photo.url;
          this.accountSerice.setCurrentUser(user);
        }
        updatedMember.photoURL = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      }

    };
  }

  setMainPhoto(Photo: Photo) {
    this.memberservice.setMainPhoto(Photo).subscribe({
      next: (_) => {
        const user = this.accountSerice.currentUser();
        if (user) {
          user.photoURL = Photo.url;
          this.accountSerice.setCurrentUser(user);
        }
        const updatedMember = { ...this.member() };
        updatedMember.photoURL = Photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === Photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      },
    });
  }

  deletePhoto(Photo: Photo) {
    this.memberservice.deletePhoto(Photo).subscribe({
      next: (_) => {
        const updatedMember = { ...this.member() };
        updatedMember.photos = updatedMember.photos.filter(
          (x) => x.id != Photo.id
        );
        this.memberChange.emit(updatedMember);
      },
    });
  }
}
