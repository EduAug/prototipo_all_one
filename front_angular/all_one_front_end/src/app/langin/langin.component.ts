import { AuthService } from './../auth.service';
import { BrowserModule } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

import { Component, NgModule } from '@angular/core';

@Component({
  selector: 'app-langin',
  templateUrl: './langin.component.html',
  styleUrl: './langin.component.css'
})
export class LanginComponent {
  UserName : string = '';
  Password: string = '';
  errorMessage : string = "";
  loginFailed: Boolean = false;

  constructor(private http: HttpClient, private auths: AuthService, private router : Router){}

  login(){
    const loginData = {
      UserName: this.UserName,
      Password: this.Password
    };
    this.http.post<any>('http://redeallone.somee.com/users/login', loginData).subscribe({
      next: (data) =>{
        this.auths.login(data.token);
        this.router.navigateByUrl('/home');
      },
      error: (error) =>{
        this.errorMessage = "Username or Password is Incorrect";
        this.loginFailed = true;
        setTimeout(()=>{
          this.errorMessage = '';
          this.loginFailed = false;
        }, 5000);
      }
    });
  }
}
