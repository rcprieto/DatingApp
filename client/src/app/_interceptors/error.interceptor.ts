import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((response: HttpErrorResponse) => {

        if(response){
          switch(response.status){
            case 400:

              if(response.error.errors){
                const modelStateErrors =[];
                for(const key in response.error.errors){
                  if(response.error.errors[key])
                    {
                      modelStateErrors.push(response.error.errors[key]);
                    }
                }

                this.toastr.error(modelStateErrors.join(', '), response.status.toString())
                throw modelStateErrors.flat();
              }
              else{

                this.toastr.error(response.error, response.status.toString())
              }
              break;

              case 401:
                this.toastr.error('Sem Autorização', response.status.toString());
                break;

              case 404:
                 this.router.navigateByUrl('/not-found');
                 break;
              case 405:
                  const navigationExtra3: NavigationExtras ={state: {error: response.error.error}};
                  this.router.navigateByUrl('/server-error', navigationExtra3);
                 break;
              case 500:
                 const navigationExtras: NavigationExtras ={state: {error: response.error.details}};
                 break;
              default:
                  this.toastr.error('Algo deu errado');
                  console.log(response);
                  break;

          }
        }
        throw response;
      })
    )
  }
}
