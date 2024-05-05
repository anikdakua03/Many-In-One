import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { Images } from '../../../../shared/constants/StaticImage';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { IPlayer } from '../../../../shared/interfaces/player';
import { IClanInfo } from '../../../../shared/interfaces/clan-info';
import { PaginationComponent } from "../../../pagination/pagination.component";
import { isPlatformBrowser } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { ClashOfClanService } from '../../../../shared/services/clash-of-clan.service';

@Component({
  selector: 'app-clan-details',
  standalone: true,
  templateUrl: './clan-details.component.html',
  styles: ``,
  imports: [RouterLink, PaginationComponent]
})
export class ClanDetailsComponent implements OnInit{

  clanData?: IClanInfo;
  staticImgs: any = Images;
  memberData?: IPlayer;
  isLoading : boolean = false;
  itemsPerPage: number = 10;
  currentPage: number = 1;

  isMembersListOpen: boolean = true;
  isWarStatsOpen: boolean = false;

  constructor(private clashingService : ClashOfClanService, private router: Router, private route: ActivatedRoute, private toaster : ToastrService, @Inject(PLATFORM_ID) private platformId: Object) {
    // already set from when called this so can get that from local storage
    if (isPlatformBrowser(this.platformId)) {
    // const data = localStorage.getItem("clan");
    // this.clanData = JSON.parse(data!);
    }
  }

  ngOnInit(): void {
    // will use query param
    const qp = this.route.snapshot.queryParamMap;
    // 3 cases , normal search , from clan details player search , user intentional wrong search
    if (qp.keys.length === 0) // normal search
    {
      // route to search clan
      this.router.navigateByUrl('/clashOfClans/search-clan');
    }
    else if (qp.get('clanTag') !== null && qp.get('clanTag') !== '' && qp.keys.includes('clanTag')) {
      // correct query param
      this.isLoading = true;
      // check local storage player tag if there
      const data = localStorage.getItem("clan");
      const cTag = qp.get('clanTag');
      if (data === null || data === undefined || JSON.parse(data!).tag !== cTag) {
        this.clashingService.getClanByTag(cTag!).subscribe(
        {
          next: res => {
            if (res.isSuccess) {
              this.isLoading = false;
              this.clanData = res.data.result;
              // storing in local storage for dev purpose to restrict api call
              localStorage.setItem("clan", JSON.stringify(res.data.result));
              this.toaster.success("Clan found", "Clan  found!!");
            }
            else {
              this.isLoading = false;
            }
          },
          error: err => {
            this.isLoading = false;
            this.toaster.error("No clan found with this tag", "No clan  found!!");
          }
        });
      }
      else {
        // get from localstorage
        this.isLoading = false;
        this.clanData = JSON.parse(data!);
        this.toaster.success("Clan found", "Clan  found!!");
      }
    }
    else {
      // intentional wrong url
      this.router.navigateByUrl('/clashOfClans/search-clan');
    }
  }

  get paginatedData() {
    const start = (this.currentPage - 1) * (this.itemsPerPage);
    const end = start + this.itemsPerPage;

    return this.clanData?.memberList.slice(start, end);
  }

  changePage(page: number) {
    this.currentPage = page;
  }

  getPlayerDetails(playerTag: string) {
    this.router.navigate(['/clashOfClans/search-player'], { queryParams: {playerTag : playerTag}});
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
