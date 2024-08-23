import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../services/account.service';
import { take } from 'rxjs';
import { User } from '../_modules/user';

@Directive({
  selector: '[appHasRole]' //*appHasRoles='["Admin", "Thing"]'
})
export class HasRoleDirective implements OnInit {

  @Input() appHasRole: string[] =[];
  user: User = {} as User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) this.user = user;
      }
    });


  }

  ngOnInit(): void {
    //Se estiver na role cria a view senão limpa
    if(this.user.roles.some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }
    else{
      this.viewContainerRef.clear();
    }
  }

}
