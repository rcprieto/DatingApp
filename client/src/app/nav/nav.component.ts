import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_modules/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})


export class NavComponent implements OnInit {
  model: any = {}
  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) {}

  ngOnInit(): void {


  }

  //Ao clicar em login faz o login pelo accountService, seta o user e mudar a variável para loggedIn
  login(){
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl(`/members`),
      error: retorno => this.toastr.error(retorno.error),
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}