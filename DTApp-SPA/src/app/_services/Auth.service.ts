import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { registerContentQuery } from '@angular/core/src/render3/instructions';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseURL = 'http://localhost:5000/api/auth/';
  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseURL + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        localStorage.setItem('token', user.token);
      })
    );
    }

  register(model: any) {
    return this.http.post(this.baseURL + 'register', model);
  }
}
