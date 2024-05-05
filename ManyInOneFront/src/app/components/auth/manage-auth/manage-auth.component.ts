import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ToastrService } from 'ngx-toastr';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { AuthenticationService } from '../../../shared/services/authentication.service';
import { Disable2FAComponent } from "../2FA/disable2-fa/disable2-fa.component";
import { Enable2FAComponent } from "../2FA/enable2-fa/enable2-fa.component";
import { QuizzerManageComponent } from "../../quizzer/quizzer-manage/quizzer-manage.component";
import { NgClass } from '@angular/common';

@Component({
    selector: 'app-manage-auth',
    standalone: true,
    templateUrl: './manage-auth.component.html',
    styles: ``,
    imports: [RouterLink, Enable2FAComponent, Disable2FAComponent, FontAwesomeModule, QuizzerManageComponent, NgClass]
})
export class ManageAuthComponent {

  isLoading: boolean = false;
  dots = FAIcons.ELLIPSES;
  lock = FAIcons.LOCK;
  openedLock = FAIcons.LOCK_OPENED;
  delUser = FAIcons.USER_DELETE;
  qs = FAIcons.CIRCLE_QS;
  summary = FAIcons.LIST;

  isDefaultOpen : boolean = true;
  isEnable2FAOpen : boolean = false;
  isDisable2FAOpen : boolean = false;
  isQuizzerOpen : boolean = false;
  isDeleteOpen : boolean = false;

  constructor(private authService : AuthenticationService, private toaster : ToastrService, private route : Router)
  {}


  deleteAllUserData()
  {
    var res = confirm("Do you really want to delete all data ??");
    if(res)
    {
      this.isLoading = true;
      this.authService.deleteAllUserData().subscribe({
        next: re => {
          this.isLoading = false;
          this.authService.isAuthenticated$.next(false); // set false
          this.route.navigateByUrl('/');
          this.authService.removeToken();
          this.toaster.success("User's all data deleted successfully", "User data deletion");
        },
        error: err => {
          this.isLoading = false;
          this.toaster.error("Failed to delete , check after some time !!", "User data deletion");
        }
      });
    }
  }

  onClickOverview()
  {
    this.isDefaultOpen = true;
    this.isEnable2FAOpen = false;
    this.isDisable2FAOpen = false;
    this.isQuizzerOpen = false;
    this.isDeleteOpen = false;
  }

  onClickEnable()
  {
    this.isDefaultOpen = false;
    this.isEnable2FAOpen = true;
    this.isDisable2FAOpen = false;
    this.isQuizzerOpen = false;
    this.isDeleteOpen = false;
  }

  onClickDisable()
  {
    this.isDefaultOpen = false;
    this.isEnable2FAOpen = false;
    this.isDisable2FAOpen = true;
    this.isQuizzerOpen = false;
    this.isDeleteOpen = false;
  }

  onClickQuizzer()
  {
    this.isDefaultOpen = false;
    this.isEnable2FAOpen = false;
    this.isDisable2FAOpen = false;
    this.isQuizzerOpen = true;
    this.isDeleteOpen = false;
  }
  
  onClickDelete()
  {
    this.isDefaultOpen = false;
    this.isEnable2FAOpen = false;
    this.isDisable2FAOpen = false;
    this.isQuizzerOpen = false;
    this.isDeleteOpen = true;
  }
}
