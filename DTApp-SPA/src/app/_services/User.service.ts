import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

const httpOptions = {
  headers: new HttpHeaders ({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
};

@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl = environment.apiUrl ;
constructor(private http:  HttpClient) { }

getMessages(id, page?, itemsPerPage?, messageContainer?) {
  const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  if (messageContainer != null) {
    params = params.append('messageContainer', messageContainer);
  }

  return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params: params}).pipe(
    map(response => {
      paginatedResult.results = response.body;
      if (response.headers.get('Pagination') != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}

getUsers(page?, itemsPerPage?, userParams?, likeParams?): Observable<PaginatedResult<User[]>> {

  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
  let params = new HttpParams();

  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  if (userParams != null) {
    params = params.append('minimumAge', userParams.minimumAge);
    params = params.append('maximumAge', userParams.maximumAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
  }

  if (likeParams === 'likers') {
    params = params.append('likers', 'true');
  }

  if (likeParams === 'likees') {
    params = params.append('likees', 'true');
  }


  return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params: params}).pipe(
    map(response => {
      paginatedResult.results = response.body;
      if (response.headers.get('Pagination') != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}

getMessagesThread(id: number, recipientId: number ) {
  return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
}

sendMessage(id: number, message: Message ) {
  return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
}

updateUser(id: number, user: User) {
  return this.http.put<User>(this.baseUrl + 'users/' + id, user);
}

setMainPhoto(userId: number, id: number) {
  return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
}

deletePhoto(userId: number, id: number) {
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id, {});
}

deleteMessage(userId: number, id: number) {
  return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
}

markAsRead(userId: number, id: number) {
  return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id + '/read', {}).subscribe();
}

sendLike(userId: number, recipientId: number) {
  return this.http.post(this.baseUrl + 'users/' + userId + '/like/' + recipientId, {});
}
}
