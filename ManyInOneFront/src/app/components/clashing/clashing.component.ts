import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ClashOfClanService } from '../../shared/services/clash-of-clan.service';
import { CookieService } from 'ngx-cookie-service';
import { LocationItem } from '../../shared/models/Clasher/LocationItem';

@Component({
  selector: 'app-clashing',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './clashing.component.html',
  styles: ``
})
export class ClashingComponent {

  locations : LocationItem[] = [];

  constructor(private cookie : CookieService, private clashingService : ClashOfClanService){}

  fetchAllLocations()
  {
    const localLoc = localStorage.getItem("loc") || '{}';
    console.log("From local", JSON.parse(localLoc));
    this.locations = JSON.parse(localLoc) as LocationItem[]; 

    // this.clashingService.getAllLocations().subscribe({
    //   next : res => {
    //     console.log(res);
    //     // this.locations = res as LocationItem[]; 
    //     const localLoc = localStorage.getItem("loc");
    //     console.log("From local",localLoc);

    //     // this.locations = localLoc as LocationItem[]; 
    //     localStorage.setItem("loc", JSON.stringify(res));
    //     this.cookie.set("loc", JSON.stringify(res));
    //   },
    //   error : err => {
    //     console.log("Errorr", err);
    //   }
    // });
  }
}
