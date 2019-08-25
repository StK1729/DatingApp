import {Injectable} from '@angular/core';
declare let alertify: any;

@Injectable({
    providedIn: "root"
})

export class AlertifyService{
    
    confirm(message: string, okCallback: () => any){
        alertify.confirm(message, function(e){
            if(e) {
                okCallback();
            } else {

            }
        });
    }

    success(message: string){
        alertify.success(message, 1);
    }
    error(message: string){
        alertify.error(message, 1);
    }
    warning(message: string){
        alertify.warning(message, 1);
    }
    notify(message: string){
        alertify.notify(message,'custom',1);
    }
}