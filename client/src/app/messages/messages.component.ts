import { Component } from '@angular/core';
import { Pagination } from '../_modules/pagination';
import { MessageService } from '../services/message.service';
import { Message } from '../_modules/message';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent {

  messages?: Message[];
  pagination?: Pagination;
  //container = 'Unread';
  container = 'Inbox';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(private messageService: MessageService) {

  }

  ngOnInit():void{
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe({
      next: response =>{
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
      }
    })
  }

  pageChanged(event: any)
  {
    if(this.pageNumber !== event.page)
    {
      this.pageNumber = event.page;
      this.loadMessages();
    }

  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe({
      next: () => this.messages?.slice(this.messages.findIndex(m => m.id === id), 1)
    });
  }

}
