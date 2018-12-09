import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { emit } from 'cluster';
import { AuthService } from '../_services/Auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register()  {
    console.log(this.model);
    this.authService.register(this.model).subscribe(
      next => {
        console.log('Registration Successful');
    },
      error => {
        console.log('Registration Failed');
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('canceled');
  }
}
