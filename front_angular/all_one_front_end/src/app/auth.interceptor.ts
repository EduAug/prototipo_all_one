import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler ,HttpInterceptorFn, HttpEvent } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(){}

  intercept(req: HttpRequest<any>, next : HttpHandler): Observable<HttpEvent<any>>{
    const token = sessionStorage.getItem('token');
    if (token){
      const authRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      return next.handle(authRequest);
    }
    return next.handle(req);
  }
}
