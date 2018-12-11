import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/User.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  constructor(private uService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  users: User[];
  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'];
    });
  }

  loadUsers() {
    this.uService.getUsers().subscribe((users: User[]) => {
      this.users = users;
    },
    error => {
      this.alertify.warning(error);
    });
  }

}
