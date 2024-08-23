import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';
import { User } from 'src/app/_modules/user';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  avaliableRoles = [
    'Admin',
    'Moderator',
    'Member'
  ]

  constructor(private adminService: AdminService, private modalService: BsModalService) {

  }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.getUsersWithRoles().subscribe({
      next: u => this.users = u
    });
  }

  openRolesModal(user: User){
    const config = {
      class: 'modal-dialog-centered',
      initialState:{
        username: user.username,
        avaliableRoles: this.avaliableRoles,
        selectedRoles: [...user.roles],
        user: user
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);

  }






}
