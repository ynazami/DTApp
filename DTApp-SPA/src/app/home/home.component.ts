import { Component, OnInit } from '@angular/core';
import { ValueService } from '../_services/Value.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;
  constructor(private valService: ValueService) { }

  values: any;
  ngOnInit() {
    // this.getValues();
  }

  registerToggle() {
    this.registerMode = true;
  }

  getValues() {
    this.valService.getvalue().subscribe(response => {
      this.values = response;
    },
    error => {
      console.log(error);
    }
  );
  }
  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }


}
