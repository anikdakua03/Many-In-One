import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClashOfClanService } from '../../../shared/services/clash-of-clan.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { IClanInfo, ILabel, ILocation } from '../../../shared/interfaces/clan-info';
import { NgSelectModule } from '@ng-select/ng-select';
import { SearchClansRequest } from '../../../shared/models/Clasher/search-clans-request.model';
import { IResultClan } from '../../../shared/interfaces/search-clan-response';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { NgxLoadingModule } from 'ngx-loading';

@Component({
  selector: 'app-search-clan',
  standalone: true,
  imports: [ReactiveFormsModule, NgSelectModule, NgTemplateOutlet, NgxLoadingModule],
  templateUrl: './search-clan.component.html',
  styles: ``
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
    maxMembers: new FormControl("",),
    minClanPoints: new FormControl(""),
    minClanLevel: new FormControl(""),
    clanLabels: new FormControl([]),
    searchLimit: new FormControl("", [Validators.required, Validators.max(75)]),
  });


  clanData?: IClanInfo;
  searchedClans: IResultClan[] = [];

  isTroopsOpen: boolean = false;
  isAchievementOpen: boolean = false;
  isBuilderBaseOpen: boolean = true;
  isLoading : boolean = false;

  allPlayerLabels: ILabel[] = [];
  allClanLabels: ILabel[] = [];
  allLocs: ILocation[]  = [];

  preparedSearchReq: SearchClansRequest = {}

  constructor(private clashingService: ClashOfClanService, private toaster: ToastrService, private router: Router) {

    // const locsItems = localStorage.getItem("loc");
    // const locs = JSON.parse(locsItems!);
    // this.allLocs= (locs!);
    // const lableItems = localStorage.getItem("clanlabels");
    // const labels = JSON.parse(lableItems!) ;
    // this.allClanLabels= (labels);
    // console.log("loccs", this.allClanLabels);
  }

  ngOnInit(): void {
    const locsItems = localStorage.getItem("loc");
    const locs = JSON.parse(locsItems!) as ILocation[];
    this.allLocs = (locs!);
    const lableItems = localStorage.getItem("clanlabels");
    const labels = JSON.parse(lableItems!) as ILabel[];
    this.allClanLabels = (labels!);
    console.log("loccs", this.allClanLabels);
  }


  onSearchClanByTag() {
    debugger
    if (this.clanByTagForm.valid) {
      // check localstorage player tag if there
      this.isLoading = true;
      const data = localStorage.getItem("clan");
      console.log("clannnn", this.clanData);
      
      if (data === null || data === undefined || JSON.parse(data!).tag !== this.clanByTagForm.value.playerTag) {
        this.clashingService.getClanByTag(this.clanByTagForm.value.clanTag).subscribe({
          next: res => {
            if (res.result !== null) {
              this.isLoading = false;
              this.clanData = res.result;
              // storing in local storage for dev purpose to restrict api call
              localStorage.setItem("clan", JSON.stringify(res.result))
              console.log("clan data", this.clanData);
              this.toaster.success("Clan found", "Clan  found!!");
            }
            else {
              this.isLoading = false;
              this.toaster.error("No clan found with this tag", "No clan  found!!");
            }
          },
          error: err => {
            this.isLoading = false;
            console.log(err)
          }
        });
      }
      else {
        // get from localstorage
        this.isLoading = false;
        this.clanData = JSON.parse(data!);
      }
      // will re direct to clan details page
      this.isLoading = false;
      this.router.navigateByUrl(`clashOfClans/search-clan/clan-details`)
    }
    else {
      this.toaster.error("Put clan tag ...", "Search Clan");
    }
  }

  onSearchByFilter() {

    if (this.clanSearchForm.valid) {
      this.isLoading = true;
      const data = this.clanSearchForm.value.minMembers;
      console.log("Before preparation --> ", data);
      this.preparedSearchReq.name = this.clanSearchForm.value.clanNamePhrase;
      this.preparedSearchReq.warFrequency = this.clanSearchForm.value.warFrequency;
      this.preparedSearchReq.locationId = this.clanSearchForm.value.clanLocation === "" ? 0 : this.clanSearchForm.value.clanLocation;
      this.preparedSearchReq.minMembers = this.clanSearchForm.value.minMembers === "" ? 0 : this.clanSearchForm.value.minMembers;
      this.preparedSearchReq.maxMembers = this.clanSearchForm.value.maxMembers === "" ? 0 : this.clanSearchForm.value.maxMembers;
      this.preparedSearchReq.minClanLevel = this.clanSearchForm.value.minClanLevel === "" ? 0 : this.clanSearchForm.value.minClanLevel;
      this.preparedSearchReq.minClanPoints = this.clanSearchForm.value.minClanPoints === "" ? 0 : this.clanSearchForm.value.minClanPoints;
      this.preparedSearchReq.labels = this.clanSearchForm.value.clanLabels;
      this.preparedSearchReq.limit = this.clanSearchForm.value.searchLimit;
      console.log("After preparation --> ", this.preparedSearchReq);
      
      // send to get results
      this.clashingService.searchClan(this.preparedSearchReq).subscribe({
        next: res => {
          if (res.result !== null) {
            this.isLoading = false;
            this.searchedClans = res.result;
            localStorage.setItem("clans", JSON.stringify(res.result));
            this.toaster.success(res.message, "Clans  found!!");
          }
          else {
            this.isLoading = false;
            this.toaster.error("No clan found with given filters !!", "No Clans found!!");
          }
        },
        error: err => {
          this.isLoading = false;
          console.log("Any error ", err);
        }
      });
    }
    else {
      this.toaster.error("Enter clan name and Search limit !!", "Clan search error !!");
    }
  }

  getClanDetails(clanTag: string) {
        this.isLoading = true;
        this.clashingService.getClanByTag(clanTag).subscribe(
      {
        next: res => {
          if (res.result !== null) {
            this.isLoading = false;
            this.clanData = res.result;
            // storing in local storage for dev purpose to restrict api call
            localStorage.setItem("clan", JSON.stringify(res.result))
            console.log("clan data", this.clanData);
            this.router.navigateByUrl("clashOfClans/search-clan/clan-details");
            this.toaster.success("Clan found", "Clan  found!!");
          }
          else {
            this.isLoading = false;
            this.toaster.error("No clan found with this tag", "No clan  found!!");
          }
        },
        error: err => {
          this.isLoading = false;
          console.log(err)
        }
      });
  }

  onClear() {
    this.clanByTagForm.reset();
    this.clanSearchForm.reset();
  }
}
