import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/services/members.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/services/message.service';
import { Message } from 'src/app/_modules/message';
import { PresenceService } from 'src/app/services/presence.service';
import { AccountService } from 'src/app/services/account.service';
import { User } from 'src/app/_modules/user';
import { take } from 'rxjs';

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
export class MemberDetailComponent implements OnInit, OnDestroy {
  //static faz o componente ser criado antes do ngOnInit rodar
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member: Member  = {} as Member;
  images: GalleryItem[] = [];
  activeTabs?: TabDirective;
  messages: Message[] = [];
  user?: User;

  constructor(private accountService: AccountService, private route: ActivatedRoute, private router: Router, private messageService: MessageService, public presenceService: PresenceService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: u => {
        if(u) this.user = u;
      }
    })

  }
  ngOnDestroy(): void {

    this.messageService.stopHubConnection();
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

  // onRoutParamsChange(){
  //   const user = this.accountService.currentUser$;
  // }

  onTabActivated(data: TabDirective){
    this.activeTabs = data;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {tab: this.activeTabs.heading},
      queryParamsHandling: 'merge'
    })
    if(this.activeTabs.heading === "Messages" && this.user)
    {
      this.messageService.createHubConnection(this.user, this.member.userName);
    }
    else{
      this.messageService.stopHubConnection();
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
