import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-voting',
  templateUrl: './voting.component.html',
  styleUrl: './voting.component.css'
})
export class VotingComponent implements OnInit {
  topApprovedTopics: any = [];
  unapprovedTopics: any = [];
  informedTopic: string = '';
  searchQuery: string = '';
  msg: string = '';

  constructor(private http: HttpClient, private auths: AuthService, private snackbar: MatSnackBar){}
  ngOnInit(): void {
    this.loadTopApproved();
    this.loadUnapproved();
  }

  loadTopApproved(){
    this.http.get("http://localhost:5164/deeper/topics/topApproved").subscribe({
      next: (topics)=>{
        this.topApprovedTopics= topics;
      }
    })
  }

  loadUnapproved(){
    const token = sessionStorage.getItem('auth_token');
    if(token){
      this.auths.decodeToken(token).subscribe({
        next:(data)=>{
          this.http.get(`http://localhost:5164/deeper/topics/getUnapproved?userId=${data.nameid}`).subscribe({
            next:(unap)=>{
              this.unapprovedTopics = unap;
            },
            error: (err) =>{console.error("Something went wrong loading unapproved: ",err);}
          });
        }
      });
    }
  }

  searchUnapprovedTopics(){
    const token = sessionStorage.getItem('auth_token');
    if(token){
      this.auths.decodeToken(token).subscribe({
        next: (data)=> {
          const id = data.nameid
          this.http.get(`http://localhost:5164/deeper/topics/getUnapproved?userId=${id}&queriedTopic=${this.searchQuery}`).subscribe({
            next: (unapQueried)=> {
              this.unapprovedTopics = unapQueried;
            },
            error: (err)=> {console.error("Something went wrong querying: ",err);}
          });
        }
      });
    }
  }

  suggestTopic(){
    const token = sessionStorage.getItem('auth_token');
    if(token){
      this.auths.decodeToken(token).subscribe({
        next: (data)=> {
          const requestBody = {
            informedTopic: this.informedTopic,
            userId: data.nameid
          };
          this.http.post(`http://localhost:5164/deeper/topics/suggest`, requestBody).subscribe({
            next: (retSuggest)=> {
              console.log(retSuggest);
              this.msg = 'Suggested Successfully';
              this.informedTopic = '';
              this.loadUnapproved();
            },
            error: (err)=> {
              if(err.error.includes("Similar")){
                this.msg = err.error;
                this.informedTopic = '';
                this.loadUnapproved();
              }else{
                console.error("Something went wrong suggesting: ", err);
              }
            }
          });
        }
      });
    }
  }

  voteForTopic(topic: any){
    const topicId = topic.id;
    const token = sessionStorage.getItem('auth_token');
    if(token){
      this.auths.decodeToken(token).subscribe({
        next: (data)=> {
          const requestBody = {
            userId: data.nameid,
            topicId: topicId
          };
          console.log(requestBody);
          this.http.post(`http://localhost:5164/deeper/topics/voteForTopic`, requestBody, {responseType: 'text'}).subscribe({
            next: (voted)=> {
              console.log(voted);
              this.snackbar.open(voted, 'Close', {
                duration: 3000
              });
              this.loadUnapproved();
            },
            error: (err)=> {console.error("Something went wrong voting: ", err);}
          });
        }
      });
    }
  }
}
