import { Component } from '@angular/core';
import { Member } from '../_modules/member';
import { MembersService } from '../services/members.service';
import { Pagination } from '../_modules/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent {
  members: Member[] | undefined;
  predicate = "liked";
  pageNumber = 1;
  pageSize = 3;
  pagination: Pagination | undefined;

  /**
   *
   */
  constructor(private memberService: MembersService) {

  }
  ngOnInit():void{
    this.loadLikes();

  }

  loadLikes(){
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response =>{
        this.members = response.result;
        this.pagination = response.pagination;
        console.log(response.pagination);
      }
    })
  }


  pageChanged(event: any){
    if(this.pageNumber != event.page)
    {
      this.pageNumber = event.page;
      this.loadLikes();
    }

  }




}
