import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { AuthService } from '../_services/Auth.service';
import { UserService } from '../_services/User.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(private authService: AuthService, private userService: UserService,
              private route: ActivatedRoute, private alertify: AlertifyService  ) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].results;
      this.pagination = data['messages'].pagination;
    });
  }

  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid, this.pagination.currentPage,
      this.pagination.itemsPerPage, this.messageContainer).subscribe(
        (res: PaginatedResult<Message[]>) => {
          this.messages = res.results;
          this.pagination = res.pagination;
        }, error => {
          this.alertify.warning(error);
        }
      );
  }

  deleteMessage(id: number) {
    this.alertify.confirm('Are you sure you want to delete this message?', () => {
      this.userService.deleteMessage(this.authService.decodedToken.nameid, id).subscribe(
        () => {
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
          this.alertify.success('Message deleted');
        }, error => {
          this.alertify.warning('Failed to delete message.');
        }
      );
    });
  }

pageChanged(event: any): void {
  this.pagination.currentPage = event.page;
  this.loadMessages();
}

}
