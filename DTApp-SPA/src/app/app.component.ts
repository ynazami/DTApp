import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/Auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  ngOnInit(): void {
    this.authService.SetToken();
  }

  constructor(private authService: AuthService) {

  }
}
