import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ClashOfClanService } from '../../shared/services/clash-of-clan.service';
import { LocationItem } from '../../shared/models/Clasher/LocationItem';
import { ILabel } from '../../shared/interfaces/clan-info';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-clashing',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './clashing.component.html',
  styles: ``
})
export class ClashingComponent {

  locations : LocationItem[] = [];

  allPlayerLabels: [{}] = [{}];
  allClanLabels: [{}] = [{}];


  constructor(private router: Router, private clashingService: ClashOfClanService, @Inject(PLATFORM_ID) private platformId: Object) {
    if(isPlatformBrowser(platformId))
    {
      this.fetchAllLocations();
      this.fetchAllClanLabels();
      this.fetchAllPlayerLabels();
    }
  }

  fetchAllLocations()
  {
    // first if not in localstorage then will call API and get and set in localstorage
    const localLoc = localStorage.getItem("loc");
    // const newwww = JSON.parse(localLoc);
    if (localLoc !== null && localLoc !== undefined && localLoc !== "") {
      const newww = JSON.parse(localLoc!);
      this.locations = newww as LocationItem[];
    }
    else {
      this.clashingService.getAllLocations().subscribe({
        next: res => {
          this.locations = res.data.result as LocationItem[];

          localStorage.setItem("loc", JSON.stringify(this.locations));
        },
        error: err => {
        }
      });
    }
  }

  fetchAllClanLabels() {
    // first if not in localstorage then will call API and get and set in localstorage
    const clnlbl = localStorage.getItem("clanlabels");
    // const newwww = JSON.parse(clnlbl);
    if (clnlbl !== null && clnlbl !== undefined && clnlbl !== "") {
      const newww = JSON.parse(clnlbl!);
      this.allClanLabels.push(newww as ILabel[]);
    }
    else {
      this.clashingService.getClanLabels().subscribe({
        next: res => {
          this.allClanLabels.push(res.data.result as ILabel[]);

          localStorage.setItem("clanlabels", JSON.stringify(res.data.result));
        },
        error: err => {
        }
      });
    }
  }

  fetchAllPlayerLabels() {
    // first if not in localstorage then will call API and get and set in localstorage
    const localLoc = localStorage.getItem("playerlabels");
    // const newwww = JSON.parse(localLoc);
    if (localLoc !== null && localLoc !== undefined && localLoc !== "") {
      const newww = JSON.parse(localLoc!);
      this.allPlayerLabels.push(newww as ILabel[]);
    }
    else {
      this.clashingService.getPlayerLabels().subscribe({
        next: res => {
          this.allPlayerLabels.push(res.data.result as ILabel[]); 

          localStorage.setItem("playerlabels", JSON.stringify(res.data.result));
        },
        error: err => {
        }
      });
    }
  }
}
