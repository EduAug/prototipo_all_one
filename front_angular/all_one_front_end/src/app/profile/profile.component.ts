import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { GeocodingService } from '../geocoding.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit{
    constructor(
      private http : HttpClient,
      private auths : AuthService,
      private route : Router,
      private datePipe : DatePipe,
      private geocodingService : GeocodingService,
      public dialog : MatDialog
    ){}

    profileData : any = { displayName: '', email: '', password: '', birthday: '', latitude: '', longitude: '' };
    topics : any[] = [];
    selectedTopics : number[] = [];
    birthDate : String | null = '';
    isEditable : boolean = false;
    fullAddress : String = '';

    ngOnInit(): void {
      const token = sessionStorage.getItem('auth_token');
      if (token) {
        const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

        this.http.get<any>(`http://redeallone.somee.com/users/profile`, { headers }).subscribe(profile => {
          this.profileData = profile;
          this.birthDate = this.datePipe.transform(this.profileData.birthday, 'yyyy-MM-dd');
          this.alreadySavedAddress();
        });
        this.auths.decodeToken(token).subscribe(userId => {

          this.http.get<any>(`http://redeallone.somee.com/deeper/topics/${userId.nameid}/topics`).subscribe(topics => {
            this.topics = topics;
          });

        });
      }

    }

    alreadySavedAddress() {
      if (this.profileData.latitude && this.profileData.longitude) {
        this.geocodingService.getLocal(this.profileData.latitude, this.profileData.longitude).subscribe(
          (response) => {
            const city = response.address.city || response.address.town || response.address.village;
            const state = response.address.state;
            const country = response.address.country;
            this.fullAddress = `${city}, ${state}, ${country}`;
          },
          (error) => {
            console.error("Error getting location details:", error);
          }
        );
      }
    }

    confirmPassword(): void {
      const password = prompt("Please enter your password to confirm your identity");
      if(password == this.profileData.password){
        this.isEditable = true;
      }else{
        alert('Incorrect password. Try again.');
      }
    }

    getUserLocation(event: Event) {
      event.preventDefault();
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          (position) => {
            this.profileData.latitude = position.coords.latitude;
            this.profileData.longitude = position.coords.longitude;

            this.geocodingService.getLocal(this.profileData.latitude, this.profileData.longitude).subscribe(
              (response) => {
                const city = response.address.city || response.address.town || response.address.village;
                const state = response.address.state;
                const country = response.address.country;
                this.fullAddress = `${city}, ${state}, ${country}`;
              },
              (error) => {
                console.error("Error getting location details:", error);
              }
            );
          },
          (error) => {
            console.error("Error getting user's location:", error);
          },
          { enableHighAccuracy: true }
        );
      } else {
        console.error("Geolocation is not supported by this browser.");
      }
    }

    removeTopic(topicId: number){
      const i = this.topics.findIndex(topic => topic.id == topicId);
      if(i !== -1){
        this.topics.splice(i,1);
      }
    }

    cancelChanges(){
      this.route.navigate(['/home']);
    }

    confirmChanges() {
      const finalUserObject = {
        DisplayName: this.profileData.displayName,
        Email: this.profileData.email,
        Password: this.profileData.password,
        Birthday: this.birthDate,
        Latitude: this.profileData.latitude,
        Longitude: this.profileData.longitude,
        TopicIds: this.topics.map(t => t.id)
      };

      const token = sessionStorage.getItem('auth_token');
      if (token) {
        this.auths.decodeToken(token).subscribe({
          next: (userId) => {
            this.http.put(`http://redeallone.somee.com/users/update?userId=${userId.nameid}`, finalUserObject, { responseType: 'text' }).subscribe({
              next: (response) => {
                console.log(response);
                sessionStorage.removeItem('auth_token');
                this.route.navigate(['/signin']);
              },
              error: (error) => {console.error('Error updating user:', error);}
            });
          },
          error: (error) => {console.error('Error decoding token:', error);}
        });
      }
    }

    openDeleteConfirm() : void {
      if(confirm("Are you sure about deleting your account?")){
        this.deleteAccount();
      }
    }

    deleteAccount() {
      const token = sessionStorage.getItem('auth_token');
      if (token){
        this.auths.decodeToken(token).subscribe({
          next: (userId) => {
            this.http.delete(`http://redeallone.somee.com/users/final/delete/${userId.nameid}`, { responseType: 'text' }).subscribe({
              next: (response) => {
                console.log(response);
                sessionStorage.removeItem('auth_token');
                this.route.navigate(['/signin']);
              },
              error: (error) => {console.error('Error deleting user:', error);}
            })
          },
          error: (error) => {console.error('Error decoding token: ',error);}
        });
      }
    }
}
