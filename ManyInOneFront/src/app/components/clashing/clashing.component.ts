import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ClashOfClanService } from '../../shared/services/clash-of-clan.service';
import { LocationItem } from '../../shared/models/Clasher/LocationItem';
import { ILabel } from '../../shared/interfaces/clan-info';

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

  constructor(private router: Router, private clashingService: ClashOfClanService) {
  }

  fetchAllLocations()
  {
    // first if not in localstorage then willcall API and get and set inlocalstorage
    const localLoc = localStorage.getItem("loc");
    // const coo = this.cookie.get("loc") || '{}';
    // console.log("From cookie", JSON.parse(coo));
    // const newwww = JSON.parse(coo);
    if (localLoc !== null && localLoc !== undefined) {
      // console.log("From local", JSON.parse(localLoc!));
      const newww = JSON.parse(localLoc!);
      this.locations = newww as LocationItem[];
    }
    else {
      this.clashingService.getAllLocations().subscribe({
        next: res => {
          this.locations = res.result as LocationItem[];

          localStorage.setItem("loc", JSON.stringify(res.result));
          window.location.reload();
        },
        error: err => {
          console.log("Errorr", err);
        }
      });
    }
  }

  fetchAllClanLabels() {
    // first if not in localstorage then willcall API and get and set inlocalstorage
    const clnlbl = localStorage.getItem("clanlabels");
    // const coo = this.cookie.get("loc") || '{}';
    // console.log("From cookie", JSON.parse(coo));
    // const newwww = JSON.parse(coo);
    if (clnlbl !== null && clnlbl !== undefined) {
      console.log("From local", JSON.parse(clnlbl!));
      const newww = JSON.parse(clnlbl!);
      this.allClanLabels.push(newww as ILabel[]);
    }
    else {
      this.clashingService.getClanLabels().subscribe({
        next: res => {
          this.allClanLabels.push(res.result as ILabel[]);

          localStorage.setItem("clanlabels", JSON.stringify(res.result));
        },
        error: err => {
          console.log("Errorr", err);
        }
      });
    }
  }

  fetchAllPlayerLabels() {
    // first if not in localstorage then willcall API and get and set inlocalstorage
    const localLoc = localStorage.getItem("playerlabels");
    // const coo = this.cookie.get("loc") || '{}';
    // console.log("From cookie", JSON.parse(coo));
    // const newwww = JSON.parse(coo);
    if (localLoc !== null && localLoc !== undefined) {
      console.log("From local", JSON.parse(localLoc!));
      const newww = JSON.parse(localLoc!);

      this.allPlayerLabels.push(newww as ILabel[]);
    }
    else {
      this.clashingService.getPlayerLabels().subscribe({
        next: res => {
          this.allPlayerLabels.push(res.result as ILabel[]); 

          localStorage.setItem("playerlabels", JSON.stringify(res.result));
        },
        error: err => {
          console.log("Errorr", err);
        }
      });
    }
  }
}
