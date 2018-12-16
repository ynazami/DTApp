import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/User.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/Pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  constructor(private uService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }
  pagination: Pagination;
  users: User[];
  currentUser: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: 'Males'},
               {value: 'female', display: 'Females'}];
  userParams: any = { };
  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].results;
      this.pagination = data['users'].pagination;
    });
    this.userParams.gender = this.currentUser.gender === 'female'  ? 'male' : 'female';
    this.userParams.minimumAge = 18;
    this.userParams.maximumAge = 99;
    this.userParams.orderBy = 'lastActive';
  }

  resetFilters() {
    this.userParams.gender = this.currentUser.gender === 'female'  ? 'male' : 'female';
    this.userParams.minimumAge = 18;
    this.userParams.maximumAge = 99;
    this.userParams.orderBy = 'lastActive';
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.uService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams).subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.results;
      this.pagination = res.pagination;
    },
    error => {
      this.alertify.warning(error);
    });
  }

}
