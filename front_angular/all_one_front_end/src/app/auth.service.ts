import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'auth_token';

  constructor(private http: HttpClient) { }

  login(token: string): void {
    sessionStorage.setItem(this.tokenKey, token);
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!sessionStorage.getItem(this.tokenKey);
  }

  decodeToken(token : String): Observable<any>{
    return this.http.get<any>('http://redeallone.somee.com/crypt/decodeToken_generatedAsJWT',{
      headers: {Authorization: `Bearer ${token}`}
    });
  }
}
