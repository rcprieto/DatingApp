import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { Message } from '../_modules/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {

   }

   getMessages(pageNumber: number, pageSize: number, container: string){
     let params = getPaginationHeader(pageNumber, pageSize);
     params = params.append('Container', container);
     return getPaginatedResult<Message[]>(this.baseUrl + 'message', params, this.http);
   }

   getMessagesThread(username: string){
      return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + username);
   }

   sendMessage(username:string, content: string){
    return this.http.post<Message>(this.baseUrl + 'message', {recipientUsername: username, content});
   }

   deleteMessage(id: number){
    return this.http.delete(this.baseUrl + 'message/' + id);
 }
}
