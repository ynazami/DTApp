import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/User.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';
import { AuthService } from '../_services/Auth.service';

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
 pageNumber = '1';
 pageSize = '5';
 messageContainer = 'Unread';
constructor(private authSerice: AuthService, private userService: UserService, private router: Router, private alertify: AlertifyService) {
}

resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.userService.getMessages(this.authSerice.decodedToken.nameid, this.pageNumber, this.pageSize, this.messageContainer).pipe(
        catchError( error => {
            this.alertify.warning('Error Retriving Messages');
            this.router.navigate(['/home']);
            return of(null);
        }
        )
    );
}
}
