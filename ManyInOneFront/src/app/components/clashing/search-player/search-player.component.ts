import { NgClass } from '@angular/common';
import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ToastrService } from 'ngx-toastr';
import { Images } from '../../../shared/constants/StaticImage';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { IPlayer } from '../../../shared/interfaces/player';
import { ClashOfClanService } from '../../../shared/services/clash-of-clan.service';
import { ClashData } from '../../../shared/constants/static.clashing-data';

@Component({
  selector: 'app-search-player',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgClass, FontAwesomeModule],
  templateUrl: './search-player.component.html',
  styles: ``
})
export class SearchPlayerComponent implements OnInit {
  playerForm: FormGroup = new FormGroup({
    playerTag: new FormControl("", [Validators.required, Validators.pattern(/^#[A-Za-z0-9]{1,16}/)]),
  });

  playerData?: IPlayer = ClashData.PLAYER_DATA;

  isTroopsOpen: boolean = false;
  isAchievementOpen: boolean = false;
  isBuilderBaseOpen: boolean = false;
  isLoading: boolean = false;

  staticImg = Images;
  dots = FAIcons.ELLIPSES;

  constructor(private clashingService: ClashOfClanService, private router: Router, private toaster: ToastrService, @Inject(PLATFORM_ID) private platformId: Object, private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    // will use query param
    const qp = this.route.snapshot.queryParamMap;
    // 3 cases , normal search , from clan details player search , user intentional wrong search
    if (qp.keys.length === 0) // normal search
    {
      // nothing to do
    }
    else if (qp.get('playerTag') !== null && qp.get('playerTag') !== '' && qp.keys.includes('playerTag')) {
      // correct query param
      this.isLoading = true;
      // check local storage player tag if there
      const data = localStorage.getItem("player");
      const pTag = qp.get('playerTag');
      if (data === null || data === undefined || JSON.parse(data!).tag !== pTag) {
        this.clashingService.getPlayer(pTag!).subscribe({
          next: res => {
            if (res.data.result !== null) {
              this.isLoading = false;
              this.playerData = res.data.result;
              // storing in local storage for dev purpose to restrict api call
              localStorage.setItem("player", JSON.stringify(res.data.result));
              this.toaster.success("Player found", "Player  found!!");
            }
            else {
              this.isLoading = false;
              this.toaster.error(res.data?.message, "No player  found!!");
            }
          },
          error: err => {
            this.isLoading = false;
          }
        });
      }
      else {
        // get from localstorage
        this.isLoading = false;
        this.playerData = JSON.parse(data!);
        this.toaster.success("Player found", "Player  found!!");
      }
    }
    else {
      // intentional wrong url
      this.router.navigateByUrl('/clashOfClans/search-player');
    }
  }


  onSearchPlayer() {
    if (this.playerForm.valid) {
      this.isLoading = true;
      // check local storage player tag if there
      const data = localStorage.getItem("player");

      if (data === null || data === undefined || JSON.parse(data!).tag !== this.playerForm.value.playerTag) {

        this.clashingService.getPlayer(this.playerForm.value.playerTag).subscribe({
          next: res => {
            if (res.data.result !== null) {
              this.isLoading = false;
              this.playerData = res.data.result;
              // storing in local storage for dev purpose to restrict api call
              localStorage.setItem("player", JSON.stringify(res.data.result));
              this.toaster.success("Player found", "Player  found!!");
            }
            else {
              this.isLoading = false;
              this.toaster.error(res.data?.message, "No player  found!!");
            }
          },
          error: err => {
            this.isLoading = false;
          }
        });
      }
      else {
        // get from localstorage
        this.isLoading = false;
        this.playerData = JSON.parse(data!);
        this.toaster.success("Player found", "Player  found!!");
      }
    }
    else {
      this.toaster.error("Put player tag ...", "Search Player");
    }
  }

  onClear() {
    this.playerForm.reset();
  }

  onClickTroops() {
    this.isTroopsOpen = true;
    this.isAchievementOpen = false; // make other false
    this.isBuilderBaseOpen = false;
  }
  onClickBuilderBase() {
    this.isTroopsOpen = false;
    this.isBuilderBaseOpen = true;
    this.isAchievementOpen = false;
  }
  onClickAchievement() {
    this.isTroopsOpen = false;
    this.isAchievementOpen = true;
    this.isBuilderBaseOpen = false;
  }
}
