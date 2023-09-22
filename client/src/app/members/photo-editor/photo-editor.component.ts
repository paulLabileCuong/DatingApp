import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environement';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member !: Member;
  uploader !: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user !: User;

  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader(); // to initialize the uploader
  }
  
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;// to check if the file is over the drop zone
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url; // to set the photo url to the user photo url
      this.accountService.setCurrentUser(this.user); // to set the current user
      this.member.photoUrl = photo.url; // to set the photo url to the member photo url
      this.member.photos.forEach(p => {
        if(p.isMain) p.isMain = false; // to set the isMain property to false
        if(p.id === photo.id) p.isMain = true; // to set the isMain property to true
      })
    });
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id !== photoId); // to filter the photos array
        
    });
  }

  initializeUploader() {
    // to initialize the uploader
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',//url to send the file to
      authToken: 'Bearer ' + this.user.token, //token to send with the file
      isHTML5: true,//to check if the browser supports HTML5
      allowedFileType: ['image'], // to check if the file is an image
      removeAfterUpload: true, // to remove the file from the queue after uploading
      autoUpload: false, // to upload the file automatically
      maxFileSize: 10 * 1024 * 1024 // to set the max file size to 10MB
    });

    this.uploader.onAfterAddingFile = (file) => {
  
      file.withCredentials = false; // to prevent CORS error
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response){
        const photo = JSON.parse(response); // to convert the response to JSON
        this.member.photos.push(photo); // to push the photo to the photos array
      }
    };
  }

}
