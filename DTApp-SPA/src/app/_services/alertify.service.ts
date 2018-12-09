import { Injectable } from '@angular/core';
declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }

confirm(message: string, okcalback: () => any) {
  alertify.confirm(message, function(e) {
    if (e) {
      okcalback();
    } else {}
  });
}

success(message: string) {
  alertify.success(message);
}

warning(message: string) {
  alertify.warning(message);
}

message(message: string) {
  alertify.message(message);
}

}
