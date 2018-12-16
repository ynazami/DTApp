import { Component, OnInit } from '@angular/core';
import { UserService } from '../_services/User.service';
import { AlertifyService } from '../_services/alertify.service';
import { User } from '../_models/user';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { AuthService } from '../_services/Auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam = 'likers';

  constructor(private authService: AuthService, private userService: UserService,
    private route: ActivatedRoute, private alertify: AlertifyService) {
  }

  ngOnInit() {
    this.route.data.subscribe(data => {
        this.users = data['users'].results;
        this.pagination = data['users'].pagination;
    });
    this.likesParam = 'likers';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likesParam).subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.results;
      this.pagination = res.pagination;
    },
    error => {
      this.alertify.warning(error);
    });
  }
}
