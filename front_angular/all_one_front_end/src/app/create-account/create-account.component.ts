import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GeocodingService } from '../geocoding.service';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrl: './create-account.component.css'
})
export class CreateAccountComponent {
  display: string = "";
  email: string = "";
  password: string = "";
  birthday: string = "";
  latitude: number = 0;
  longitude: number = 0;
  topicsId: Set<number> = new Set();

  matchingTopics: any[] = [];
  topicName: string = '';
  isLoading : boolean = false;

  fullAddress: string = "";

  constructor(private http : HttpClient, private geocodingService : GeocodingService, private auths : AuthService, private route: Router){}

  ngOnInit(): void{
    if(this.auths.isAuthenticated()){
      this.route.navigateByUrl('/home');
    }
  }

  getUserLocation(event: Event) {
    event.preventDefault();
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.latitude = position.coords.latitude;
          this.longitude = position.coords.longitude;
          // console.log("Latitude:", this.latitude);
          // console.log("Longitude:", this.longitude);

          this.geocodingService.getLocal(this.latitude, this.longitude).subscribe(
            (response) => {
              console.log(response);
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

  fetchMatchingTopics(): void {
    setTimeout(() => {
      if (this.topicName.length > 0) {
        this.http.get<any[]>('http://localhost:5164/deeper/topics/byName?thisTopicName=' + this.topicName)
          .subscribe(data => {
            this.matchingTopics = data.map(topic => ({ id: topic.id, name: topic.name }));
            console.log('Matching topics:', this.matchingTopics);
          });
      } else {
        this.matchingTopics = [];
      }
    }, 500);
  }

  selectTopic(event : Event): void {
    const target = event.target as HTMLSelectElement;
    const selectedValue = parseInt(target.value);
    if (selectedValue) {
      console.log('Selected topic ID:', selectedValue);
      this.topicsId.add(selectedValue);
      this.topicName = '';
      this.matchingTopics = [];
    }
  }

  create() {
    const topicsIdsArray = Array.from(this.topicsId);
    const user = {
      "DisplayName":this.display,
      "Email":this.email,
      "Password":this.password,
      "Birthday":this.birthday,
      "Latitude":this.latitude,
      "Longitude":this.longitude,
      "TopicIds":topicsIdsArray
    };

    console.log(
      user
    );

    this.http.post('http://localhost:5164/users/signup', user, { responseType: 'text' })
    .subscribe({
      next: (response) =>{
      console.log(response);

      this.http.post<any>('http://localhost:5164/users/login', {
        UserName: user.DisplayName,
        Password: user.Password
      }).subscribe({
      next: (data) =>{
        this.auths.login(data.token);
        this.route.navigateByUrl('/home');
        console.clear();
      }
    });
    },
    error: (error) => {
      console.error('Error creating user:',error);
    }
  });
  }
}
