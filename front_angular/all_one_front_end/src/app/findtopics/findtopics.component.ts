import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-findtopics',
  templateUrl: './findtopics.component.html',
  styleUrl: './findtopics.component.css'
})
export class FindtopicsComponent {
  topicName = '';
  matchingTopics: any[] = [];

  constructor(private http : HttpClient, private snackbar: MatSnackBar, private auths : AuthService){}

  onInputChange(): void{
    if(this.topicName.length > 0){
      setTimeout(() => {
        this.fetchMatchingTopics();
      }, 1000);
    } else {
      this.matchingTopics = [];
    }
  }

  fetchMatchingTopics(): void {
    const token = sessionStorage.getItem('auth_token');
    if (token) {
      this.auths.decodeToken(token).subscribe(
        (data: any) => {
          const userId = data.nameid;
          this.http.get<any[]>(`http://redeallone.somee.com/deeper/topics/byName?thisTopicName=${this.topicName}&userId=${userId}`)
            .subscribe(data => {
              this.matchingTopics = data.map(topic => ({ id: topic.id, name: topic.name }));
            });
        },
        (error) => {
          console.error('Error decoding token:', error);
        }
      );
    }
  }

  selectTopic(topic: any): void {
    const topicId = topic.id;
    const token = sessionStorage.getItem('auth_token');

    if (token) {
      this.auths.decodeToken(token).subscribe(
        (data: any) => {
          const userId = data.nameid;
          this.subscribeToTopic(userId, topicId);
          this.snackbar.open(`Subscribed to ${topic.name}`, 'Close', {
            duration: 3000,
            verticalPosition: 'top',
            horizontalPosition: 'end'
          });
          this.topicName = '';
          this.matchingTopics = [];
        },
        (error) => {
          console.error('Error decoding token:', error);
        }
      );
    }
  }

  subscribeToTopic(userId: number, topicId: number): void {
    const url = `http://redeallone.somee.com/deeper/topics/subscribe?userId=${userId}&topicId=${topicId}`;
    this.http.post(url, {}, { responseType: 'text' }).subscribe(
      (response) => {
        console.log('Subscription successful:', response);
        if (response.includes('already subscribed')) {
          this.snackbar.open('You are already subscribed to this topic', 'Close', {
            duration: 3000,
            verticalPosition: 'top',
            horizontalPosition: 'end'
          });
        } else {
          this.snackbar.open('Subscribed successfully', 'Close', {
            duration: 3000,
            verticalPosition: 'top',
            horizontalPosition: 'end'
          });
        }
      },
      (error) => {
        console.error('Error subscribing to topic:', error);
      }
    );
  }
}
