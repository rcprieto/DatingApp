import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/services/message.service';
import { Message } from 'src/app/_modules/message';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    MemberMessagesComponent
  ]
})
export class MemberDetailComponent implements OnInit {
  //static faz o componente ser criado antes do ngOnInit rodar
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member  = {} as Member;
  images: GalleryItem[] = [];
  activeTabs?: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute, private messageService: MessageService) {

  }

  ngOnInit(): void {
    //Vewm do member-detailed.resolver, ver app-routing
  //isso é feito para que ao abrir com a querystring já abra na aba certa, e para  isso o componente tabs tem q ter sido criado antes do ngOnInit
    this.route.data.subscribe({
      next: p => this.member = p['member']
    });


    this.route.queryParams.subscribe({
      next: p => {
        p['tab'] && this.selectTab(p['tab'])
      }
    });

    this.getImages();
  }

  onTabActivated(data: TabDirective){
    this.activeTabs = data;
    if(this.activeTabs.heading === "Messages")
    {
      this.loadMessages();
    }
  }


  loadMessages(){

    if(this.member)
    {
      this.messageService.getMessagesThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }

  }




  getImages(){
    if(!this.member)return;

    for(const photo  of this.member?.photos){
      this.images.push(new ImageItem({
        src:photo.url,
        thumb: photo.url
      }))
    }
  }

  selectTab(heading: string){
    if(this.memberTabs){
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

}
