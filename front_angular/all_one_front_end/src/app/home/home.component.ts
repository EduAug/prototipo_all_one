import { AfterViewChecked, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../auth.service';
import { HttpClient } from '@angular/common/http';
import { ChatService } from '../chat.service';
import { Subscription, from } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit, AfterViewChecked {
  displayName : string = '';
  messageInput : string = '';
  friends : any[] = [];
  chatMessages : any[] = [];
  currentChattingId : number | null = null;
  currentChattingDName : string = "";
  private fetchFriendSubs: any;
  private messageSubs: Subscription | undefined;
  chats! : ChatService;

  @ViewChild('messagesCont') private messagesContainer?: ElementRef;

  constructor(
    private auths : AuthService,
    private http : HttpClient
  ){}

  ngAfterViewChecked(): void {
    if (this.messagesContainer) {
      this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
    }
  }

  ngOnInit(): void {
  const token = sessionStorage.getItem('auth_token');
  if (token) {
    this.auths.decodeToken(token).subscribe({
      next: (data) => {
        const userIdToQuery = data.nameid;
        this.chats = new ChatService(userIdToQuery, this.http);

        this.messageSubs = this.chats.receiveMessage().subscribe(msg => {
          this.chatMessages.push({ senderId: msg.fromUserId, messageText: msg.message});
        });

      },
        error: (err) => { console.error("Error decoding token:", err); }
      });
    }

    this.fetchFriends();

    this.fetchFriendSubs = setInterval(() => {
      this.fetchFriends().then(()=> {
        let flagFriend = this.friends.some(f=> this.currentChattingId == f.id);
        if(!flagFriend){
          this.currentChattingId = null;
        }
      }).catch(er=>{console.error("Error fetching friends /Checking: ",er )});
    }, 1 * 30 * 1000);
  }

  ngOnDestroy(): void{
    clearInterval(this.fetchFriendSubs);
    if(this.messageSubs){
      this.messageSubs.unsubscribe;
    }
  }

  fetchFriends(): Promise<void>{
    return new Promise((resolve, reject)=> {
      const token = sessionStorage.getItem('auth_token');
      if (token){
        this.auths.decodeToken(token).subscribe(
          (data) => {
            this.displayName = data.unique_name;
            this.http.get<any>(`http://redeallone.somee.com/users/friendsof/${data.nameid}`).subscribe({
              next: (friends) => {
                this.friends = friends;
                resolve();
              },
              error: (error)=> {console.error("There was an error fetching friends: ", error);reject(error);}
            });
          },
          (error)=> {console.error('Error decoding token:', error);reject(error);}
        );
      } else {
        reject(new Error("No auth token found."));
      }
    });
  }

  fetchMessages(groupName: string): void{
    this.chats.fetchGroupMessages(groupName).subscribe({
      next: (messages) => {
        console.log(messages);
        this.chatMessages = messages;
      },
      error: (err) => {
        console.error("Something went wrong fetching the messages: ",err);
      }
    });
  }




  openChatWithFriend(friend : any): void{
    const token = sessionStorage.getItem('auth_token');
    if(token){
      this.auths.decodeToken(token).subscribe({
        next: (data) => {
          this.chatMessages = [];
          const fromUserId = data.nameid;
          const toUserId = friend.id;
          this.currentChattingDName = friend.displayName;

          const group : string = `${Math.min(fromUserId,toUserId)} || ${Math.max(fromUserId,toUserId)}`;

          this.chats.JoinGroup(fromUserId,toUserId).then(()=>{
            this.currentChattingId = friend.id;
            this.fetchMessages(group);
            //console.log(this.chatMessages);
          }).catch(err =>{console.error("Unable to join group: ",err)});
        }
      });
    }
  }

  sendMessage(message: string): void{
    const token = sessionStorage.getItem('auth_token');
    if(this.currentChattingId && token){
      this.auths.decodeToken(token).subscribe({
        next: (data) => {
          const fromUserId = data.nameid;
          const toUserId = this.currentChattingId!;

          this.chats.SendMessage(fromUserId,toUserId,message)
          .catch(err => {console.error(err);});
        },
        error: (err) => {console.error("Something went wrong sending the message: ",err);}
      });
    }
    this.messageInput = '';
  }

  removeFriend(): void{
    const token = sessionStorage.getItem('auth_token');
    if(this.currentChattingId && token){
      this.auths.decodeToken(token).subscribe({
        next: (data)=> {
          const uId = data.nameid;
          const fId = this.currentChattingId;

          this.http.post(`http://redeallone.somee.com/users/removeFriend`, { user1Id: uId, user2Id: fId }, { responseType: 'text' })
            .subscribe({
              next: ()=> {
                console.log(`Friend removed! Bye ${this.currentChattingDName}`);
                this.currentChattingId= null;
                this.currentChattingDName= "";
                this.fetchFriends();
              },
              error: (err)=> {
                console.error("Something went wrong when removing friend: ", err);
              }
            });
        }
      });
    }
  }
}
