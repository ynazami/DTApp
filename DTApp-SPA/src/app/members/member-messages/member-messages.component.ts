import { Component, OnInit, Input } from '@angular/core';
import { AuthService } from 'src/app/_services/Auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/User.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
@Input() recipientId: number;
messages: Message[];
newMessage: any = {};
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessagesThread(currentUserId, this.recipientId)
    .pipe(
      tap(messages => {
        for (let i = 0;  i < messages.length; i++) {
          if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
            this.userService.markAsRead(currentUserId, messages[i].id);
          }
        }
      })
    )
    .subscribe(
      (messages) => {
        this.messages = messages;
      }, error => {
        this.alertify.warning(error);
      }
    );
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, error => {this.alertify.warning(error);
    }     );
  }
}
