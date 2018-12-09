import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ValueService {
  baseURL = 'http://localhost:5000/api/values/';
  constructor(private http: HttpClient) { }

  getvalue() {
    return this.http.get(this.baseURL);
  }


}
