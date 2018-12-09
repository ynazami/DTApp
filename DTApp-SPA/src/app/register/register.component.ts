import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/Auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private authService: AuthService, private alertService: AlertifyService) { }

  ngOnInit() {
  }

  register()  {
    console.log(this.model);
    this.authService.register(this.model).subscribe(
      next => {
        this.alertService.success('Registration Successful');
    },
      error => {
        this.alertService.warning('Registration Failed');
        console.error(error);
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
    this.alertService.message('canceled');
  }
}
