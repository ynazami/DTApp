import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/Auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(public authService: AuthService, private alertService: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    console.log(this.model);
    this.authService.login(this.model).subscribe(
      next => {
        this.alertService.success('Welcome ' + this.model.username);
        console.log('Login Successful');
    },
      error => {
        this.alertService.warning('Login Failed ' + error);
    });
  }

  loggedIn() {
    // const token = localStorage.getItem('token');
    // return !!token;
    return this.authService.loggedIn();
  }

  logOut() {
    this.alertService.message('Logged Out');
    localStorage.removeItem('token');
  }

}
