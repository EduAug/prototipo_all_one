import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-findusers',
  templateUrl: './findusers.component.html',
  styleUrl: './findusers.component.css'
})
export class FindusersComponent {
  selectedDistance: number = 0;
  users: any[] = [];

  constructor(private http : HttpClient, private auths : AuthService, private route : Router, private snackbar : MatSnackBar){}

  onFindUsers(): void{
    const token = sessionStorage.getItem('auth_token');
    if(!this.selectedDistance){
      console.error("Missing maximum distance.");
      return;
    }

    if (token) {
      this.auths.decodeToken(token).subscribe(
        (data: any) => {
          const userId = data.nameid;
          this.http.get<any[]>(`http://redeallone.somee.com/users/findUsers?userId=${userId}&maxDistance=${this.selectedDistance}`)
            .subscribe(
              (users) => {
                this.users = users;
                console.log(users);
              }, (error) => {
                console.error("Error finding users:", error);
              })
        },
        (error) => {console.error("Error decoding token:",error);}
      );
    }
  }

  onUserCardClick(clcikedUserId: number) : void{
    const token = sessionStorage.getItem('auth_token');
    console.log(clcikedUserId);
    if(token){
      this.auths.decodeToken(token).subscribe(
        (data: any) =>{
          const userId = parseInt(data.nameid);
          const uIds = { user1Id: userId, user2Id: clcikedUserId};
          this.http.post(`http://redeallone.somee.com/users/addFriend`,uIds,{ responseType:'text' })
            .subscribe(
              (response)=>{
                console.log(response);
                console.log("redirecting...");
                this.snackbar.open(`Friend added successfully!`, 'Close',{
                  duration: 1500,
                  verticalPosition: 'top',
                  horizontalPosition: 'end'
                });
                this.route.navigateByUrl('/home');
              },
              (error) => {
                console.error(error);
              }
            );
        },
        (error) => {console.error("Error decoding token:",error);}
      );
    }
  }
}
