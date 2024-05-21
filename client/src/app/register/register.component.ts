import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  //@Input() userFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  model: any ={};
  constructor(private accountService: AccountService, private toastr: ToastrService){

  }

  ngOnInit():void{}

  register(){
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: retorno => this.toastr.error(retorno.error)
    })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
