import { CommonModule } from '@angular/common';
import { Component, Input, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_modules/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  standalone: true,
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;
  messageContent = '';

  constructor(public messageService: MessageService) {

  }

  ngOnInit():void{

  }

  sendMessage(){
    if(!this.username) return;

    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    })
  }



}
