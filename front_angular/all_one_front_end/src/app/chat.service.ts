import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Observable, Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConn: HubConnection;
  private receiverSubj: Subject<any> = new Subject<any>();

  constructor(@Inject(Number) private userIdParam:number, private http: HttpClient) {
    const userId = userIdParam;
    this.hubConn = new HubConnectionBuilder()
      .withUrl(`http://localhost:5164/chatHub?userId=${userId}`)
      .build();

    this.hubConn.start().catch(err=>console.error("Something went wrong booting up the chat: ",err));

    this.hubConn.on(`ReceiveMessage`,(fromUserId : number, message : string) => {
      this.receiverSubj.next({ fromUserId, message });
    });
   }

  async SendMessage(fromUserId : number, toUserId: number, message: string): Promise<void>{
    try{
      const groupName: string = `${Math.min(fromUserId,toUserId)} || ${Math.max(fromUserId,toUserId)}`;
      console.log(`From: ${fromUserId} | To: ${toUserId} | As: ${message} | In: ${groupName}`);
      await this.hubConn.invoke('SendMessage',groupName,fromUserId.toString(),toUserId.toString(),message);
      console.log("Message sent successfully.");
    }catch(exc){
      console.error("Failed to send message: ",exc);
    }
  }

  receiveMessage(): Observable<any>{
    return this.receiverSubj.asObservable();
  }

  async JoinGroup(userId1: number, userId2: number): Promise<void>{
    try{
      const group : string = `${Math.min(userId1,userId2)} || ${Math.max(userId1,userId2)}`;
      console.log(group);
      await this.hubConn.invoke('JoinGroup',group).catch(err =>{
        console.error("Failed to join group, inner: ",err);
      })
    } catch (exc){console.error("Failed to join group, in service: ",exc);}
  }

  async CreateGroup(userId1 : number, userId2: number): Promise<void> {
    try {
      await this.hubConn.invoke(`CreateGroup`, userId1, userId2).catch(err=>console.error("Something went wrong creating the group: ",err));
      console.log("Group created successfully.");
    } catch (exc) {
      console.error("Failed to create group: ",exc);
    }
  }

  // -------------------------------------

  fetchGroupMessages(groupName: string): Observable<any[]>{
    return this.http.get<any[]>(`http://localhost:5164/secrecy/messages/${groupName}`);
  }
}
