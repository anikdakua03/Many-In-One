import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClashOfClanService } from '../../../shared/services/clash-of-clan.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { IClanInfo, ILabel, ILocation } from '../../../shared/interfaces/clan-info';
import { NgSelectModule } from '@ng-select/ng-select';
import { SearchClansRequest } from '../../../shared/models/Clasher/search-clans-request.model';
import { IResultClan } from '../../../shared/interfaces/search-clan-response';
import { NgTemplateOutlet, isPlatformBrowser } from '@angular/common';
import { PaginationComponent } from "../../pagination/pagination.component";
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FAIcons } from '../../../shared/constants/font-awesome-icons';
import { ClashData } from '../../../shared/constants/static.clashing-data';

@Component({
    selector: 'app-search-clan',
    standalone: true,
    templateUrl: './search-clan.component.html',
    styles: ``,
    imports: [ReactiveFormsModule, NgSelectModule, NgTemplateOutlet, PaginationComponent, FontAwesomeModule]
})
export class SearchClanComponent implements OnInit {

  clanByTagForm: FormGroup = new FormGroup({
    clanTag: new FormControl("", [Validators.required]),
  });

  clanSearchForm: FormGroup = new FormGroup({
    clanNamePhrase: new FormControl("", [Validators.required, Validators.minLength(3)]),
    clanLocation: new FormControl(""),
    warFrequency: new FormControl("",),
    minMembers: new FormControl(""),
    maxMembers: new FormControl("", [Validators.max(50)]),
    minClanPoints: new FormControl(""),
    minClanLevel: new FormControl(""),
    clanLabels: new FormControl([]),
    searchLimit: new FormControl("", [Validators.required, Validators.max(75)]),
  });


  clanData?: IClanInfo = ClashData.CLAN_INFO;
  searchedClans: IResultClan[] = ClashData.SEARCHED_CLAN_RESULT;

  isTroopsOpen: boolean = false;
  isAchievementOpen: boolean = false;
  isBuilderBaseOpen: boolean = true;
  isLoading : boolean = false;
  itemsPerPage : number = 5;
  currentPage : number = 1;

  allPlayerLabels: ILabel[] = ClashData.PLAYER_LABELS;

  allClanLabels: ILabel[] = ClashData.CLAN_LABELS;

  allLocs: ILocation[]  = ClashData.ALL_LOCATIONS;

  preparedSearchReq: SearchClansRequest = {}
  dots = FAIcons.ELLIPSES;

  constructor(private clashingService: ClashOfClanService, private toaster: ToastrService, private router: Router, @Inject(PLATFORM_ID) private platformId: Object) {
  }

  ngOnInit(): void {
    if(isPlatformBrowser(this.platformId))
    {
      // const clans = localStorage.getItem("clans");
      // const clan = JSON.parse(clans!);
      // this.searchedClans = (clan!);
      // const locsItems = localStorage.getItem("loc");
      // const locs = JSON.parse(locsItems!) as ILocation[];
      // const processedLocs = locs.filter(a => a.name !== "");
      // this.allLocs = (processedLocs!);
      // const lableItems = localStorage.getItem("clanlabels");
      // const labels = JSON.parse(lableItems!) as ILabel[];
      // this.allClanLabels = (labels!);
    }
  }

  get paginatedData()
  {
    const start = (this.currentPage - 1) * (this.itemsPerPage);
    const end = start + this.itemsPerPage;

    return this.searchedClans.slice(start, end);
  }

  changePage(page : number)
  {
    this.currentPage = page;
  }

  onSearchClanByTag() {
    if (this.clanByTagForm.valid) {
      this.router.navigate(['/clashOfClans/search-clan/clan-details'], { queryParams: { clanTag: this.clanByTagForm.value.clanTag } });
    }
    else {
      this.toaster.error("Put clan tag ...", "Search Clan");
    }
  }

  onSearchByFilter() {

    if (this.clanSearchForm.valid) {
      this.isLoading = true;
      this.preparedSearchReq.name = this.clanSearchForm.value.clanNamePhrase;
      this.preparedSearchReq.warFrequency = this.clanSearchForm.value.warFrequency;
      this.preparedSearchReq.locationId = this.clanSearchForm.value.clanLocation === "" ? 0 : this.clanSearchForm.value.clanLocation;
      this.preparedSearchReq.minMembers = this.clanSearchForm.value.minMembers === "" ? 0 : this.clanSearchForm.value.minMembers;
      this.preparedSearchReq.maxMembers = this.clanSearchForm.value.maxMembers === "" ? 0 : this.clanSearchForm.value.maxMembers;
      this.preparedSearchReq.minClanLevel = this.clanSearchForm.value.minClanLevel === "" ? 0 : this.clanSearchForm.value.minClanLevel;
      this.preparedSearchReq.minClanPoints = this.clanSearchForm.value.minClanPoints === "" ? 0 : this.clanSearchForm.value.minClanPoints;
      this.preparedSearchReq.labels = this.clanSearchForm.value.clanLabels;
      this.preparedSearchReq.limit = this.clanSearchForm.value.searchLimit;
      
      // send to get results
      this.clashingService.searchClan(this.preparedSearchReq).subscribe({
        next: res => {
          if (res.isSuccess) {
            this.isLoading = false;
            this.searchedClans = res.data.result;
            localStorage.setItem("clans", JSON.stringify(res.data.result));
            this.toaster.success(res.data.message, "Clans  found!!");
          }
          else {
            this.isLoading = false;
            this.toaster.error("No clan found with given filters !!", "No Clans found!!");
          }
        },
        error: err => {
          this.isLoading = false;
        }
      });
    }
    else {
      this.toaster.error("Enter clan name and Search limit !!", "Clan search error !!");
    }
  }

  getClanDetails(clanTag: string) {
    // route to clan details with this tag
    this.router.navigate(['/clashOfClans/search-clan/clan-details'], { queryParams: { clanTag: clanTag } });
  }

  onClear() {
    this.clanByTagForm.reset();
    this.clanSearchForm.reset();
  }
}
