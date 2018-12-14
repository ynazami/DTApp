import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/Auth.service';
import { UserService } from 'src/app/_services/User.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;
  constructor(private authSerice: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authSerice.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 50 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = file => {file.withCredentials = false; } ;


    this.uploader.onSuccessItem = (item, response, status, headers) => {
        if (response) {
            const res: Photo = JSON.parse(response);
            const photo = {
              id: res.id,
              url: res.url,
              dateAdded: res.dateAdded,
              description: res.description,
              isMain: res.isMain
            };
            this.photos.push(photo);

        }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authSerice.decodedToken.nameid, photo.id).subscribe( next => {
        this.alertify.success('Main Photo is updated');
        this.currentMain = this.photos.filter( p => p.isMain === true)[0];
        this.currentMain.isMain = false;
        photo.isMain = true;
        this.authSerice.changeMemberPhoto(photo.url);
        this.authSerice.currentUser.photoUrl = photo.url;
        localStorage.setItem('user', JSON.stringify(this.authSerice.currentUser));
    }, error => {
        this.alertify.warning(error);
    });
  }

  deletePhoto(photo: Photo) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
    this.userService.deletePhoto(this.authSerice.decodedToken.nameid, photo.id).subscribe(
      next => {
        this.photos.splice(this.photos.findIndex(p => p.id === photo.id), 1);
        this.alertify.success('Photo has been removed successfully');
      }, error => {
        this.alertify.warning(error);
    }); });
  }


}
