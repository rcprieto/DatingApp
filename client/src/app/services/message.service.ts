import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { Message } from '../_modules/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from '../_modules/user';
import { Group } from '../_modules/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) {

   }

   createHubConnection(user: User, otherUsername: string){
    //chama o messageHub OnConnectedAsync() que pega o user na querystring
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl +  'message?user=' + otherUsername, {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build();
    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('ReceivedMessageThread', m => {
      this.messageThreadSource.next(m);
    });

    this.hubConnection.on("NewMessage", m => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messageThreadSource.next([...messages, m])
        }
      })

    });

    this.hubConnection.on("UpdateGroup", (group: Group) => {
      if(group.connections.some(x => x.username === otherUsername)){
        this.messageThread$.pipe(take(1)).subscribe({
          next: ms => {
            ms.forEach(m => {
              if(!m.dateRead){
                m.dateRead = new Date(Date.now());
              }
            })
            this.messageThreadSource.next([...ms]);
          }
        })
      }

    });
   }

   stopHubConnection()
   {
    if(this.hubConnection)
      this.hubConnection?.stop();
   }

   getMessages(pageNumber: number, pageSize: number, container: string){
     let params = getPaginationHeader(pageNumber, pageSize);
     params = params.append('Container', container);
     return getPaginatedResult<Message[]>(this.baseUrl + 'message', params, this.http);
   }

   getMessagesThread(username: string){
      return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + username);
   }

   async sendMessage(username:string, content: string){
    return this.hubConnection?.invoke("SendMessage", {recipientUsername: username, content})
    .catch(e => console.log(e));
   }

   deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'message/' + id);
 }
}
