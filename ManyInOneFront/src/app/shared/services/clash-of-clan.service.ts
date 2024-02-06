import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClashOfClanService {

  baseURL: string = environment.apiBaseUrl;


  constructor(private http: HttpClient, private cookie: CookieService) { }

  //#region  Clans Specific
  //   

  //     GET
  //     / clans / { clanTag } / currentwar
  // Retrieve information about clan's current clan war

  // GET
  //   / clans / { clanTag }
  // Get clan information

  // GET
  //   / clans / { clanTag } / members
  // List clan members


  // // For war log need access token [priority : after all those implememtnted]
  // GET
  //   / clans / { clanTag } / warlog
  // Retrieve clan's clan war log

  //#endregion

  //#region Clans Specific Players specific

  //   GET
  //   / players / { playerTag }
  // Get player information

  //#endregion

  //#region  Locations and Rankings Specific

  //   GET
  //   / locations / { locationId } / rankings / clans
  // Get clan rankings for a specific location

  // GET
  //     / locations / { locationId } / rankings / players
  // Get player rankings for a specific location

  // GET
  //     / locations / { locationId } / rankings / players - builder - base
  // Get player Builder Base rankings for a specific location

  // GET
  //     / locations / { locationId } / rankings / clans - builder - base
  // Get clan Builder Base rankings for a specific location

  // GET
  //     / locations
  // List locations
  // will store in local storage when logged in and will stay for some time
  public getAllLocations(): Observable<any> {
    return this.http.get(`${this.baseURL}Clash/GetAllLocations`, {withCredentials : true});
  }

  // GET
  //   / locations / { locationId } / rankings / capitals
  // Get capital rankings for a specific location

  // GET
  //     / locations / { locationId }
  // Get location information

  //#endregion
}
