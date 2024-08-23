import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_modules/user';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent  implements OnInit {
  username = '';
  avaliableRoles: any[] = [];
  selectedRoles: any[] = [];
  user: User = {} as User;



  constructor(public bsModalRef: BsModalRef, private adminService: AdminService) {


  }
  ngOnInit(): void {

  }

  updateChecked(checkValue: string)
  {
    const index = this.selectedRoles.indexOf(checkValue);
    index != -1 ? this.selectedRoles.splice(index, 1) : this.selectedRoles.push(checkValue);


  }

  save(){
    const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if(!this.arrayEquals(selectedRoles!, this.user.roles))
        {
          this.adminService.updateUserRoles(this.user.username, selectedRoles!.join(',')).subscribe({
            next: roles => {
              this.user.roles = roles;
              this.bsModalRef.hide();
            }
          });

        }

  }

  private arrayEquals(arr1: any[], arr2: any[]){
    return JSON.stringify(arr1.sort()) == JSON.stringify(arr2.sort());
  }

}
