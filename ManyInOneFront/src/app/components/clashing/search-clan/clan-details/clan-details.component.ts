import { Component } from '@angular/core';
import { Images } from '../../../../shared/constants/StaticImage';
import { Router, RouterLink } from '@angular/router';
import { ClashOfClanService } from '../../../../shared/services/clash-of-clan.service';
import { IPlayer } from '../../../../shared/interfaces/player';
import { ToastrService } from 'ngx-toastr';
import { IClanInfo } from '../../../../shared/interfaces/clan-info';
import { NgxLoadingModule } from 'ngx-loading';

@Component({
  selector: 'app-clan-details',
  standalone: true,
  templateUrl: './clan-details.component.html',
  styles: ``,
  imports: [RouterLink, NgxLoadingModule]
})
export class ClanDetailsComponent {

  clanData?: IClanInfo;
  staticImgs: any = Images;
  memberData?: IPlayer;
  isLoading : boolean = false;

  isMembersListOpen: boolean = true;
  isWarStatsOpen: boolean = false;

  constructor(private router: Router, private clashingService: ClashOfClanService, private toaster :ToastrService) {
    // already set from when called this so can get that from local storage
    const data = localStorage.getItem("clan");
    this.clanData = JSON.parse(data!);
  }


  getPlayerDetails(playerTag: string) {
    // get player tag and get that player and show all details
    // console.log("After clicking", playerTag);
    // check localstorage player tag if there
    const data = localStorage.getItem("player");
    // // console.log("member from local", data);
    if (data === null || data === undefined || JSON.parse(data!).tag !== playerTag) {
      this.isLoading = true;
      this.clashingService.getPlayer(playerTag).subscribe({
        next: res => {
          this.isLoading = false;
          this.memberData = res.result as IPlayer;
          // storing in local storage for avoid same req calling server
          localStorage.setItem("player", JSON.stringify(res.result));
          this.router.navigateByUrl('/clashOfClans/search-player');
          this.toaster.success("Player details found !", "Player Details")
        },
        error: err => {
          this.isLoading = false;
          this.toaster.error("Player details not found !", "Player Details")
          // console.log(err);
        }
      });
    }
    else {
      // get from localstorage
      this.isLoading = false;
      this.memberData = JSON.parse(data!);
      this.router.navigateByUrl('/clashOfClans/search-player');
      this.toaster.success("Player details found !", "Player Details")
    }
  }

  onClickMemberList() {
    this.isMembersListOpen = true;
    this.isWarStatsOpen = false;
  }
  onClickWarStats() {
    this.isMembersListOpen = false;
    this.isWarStatsOpen = true;
  }
}
