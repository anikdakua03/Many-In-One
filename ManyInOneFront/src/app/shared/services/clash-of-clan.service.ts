import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { IClanInfoResponse } from '../interfaces/clan-info';
import { ISearchClanResponse } from '../interfaces/search-clan-response';
import { IPlayerResponse } from '../interfaces/player';
import { SearchClansRequest } from '../models/Clasher/search-clans-request.model';
import { SsrCookieService } from 'ngx-cookie-service-ssr';

@Injectable({
  providedIn: 'root'
})
export class ClashOfClanService {

  baseURL: string = environment.apiBaseUrl;


  constructor(private http: HttpClient, private cookie: SsrCookieService) { }

  //#region  Clans Specific
  //   

  //     GET
  //     / clans / { clanTag } / currentwar
  // Retrieve information about clan's current clan war

  // GET
  //   / clans / { clanTag }
  // Get clan information
  public getClanByTag(clanTag: string): Observable<IClanInfoResponse> {
    const header = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<IClanInfoResponse>(`${this.baseURL}Clash/GetClanInfoById`, JSON.stringify(clanTag), { headers: header, withCredentials: true });
  }
  
  // GET
  // Search clans by name and warfrequency or location, minimummembers , max members, min clan points, min clan level
  public searchClan(searchClansRq: SearchClansRequest): Observable<ISearchClanResponse> {
    const header = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<ISearchClanResponse>(`${this.baseURL}Clash/SearchClans`, searchClansRq, { headers: header, withCredentials: true });
  }


  // GET
  //   / clans / { clanTag } / members
  // List clan members


  // // For war log need access token [priority : after all those implememtnted]
  // GET
  //   / clans / { clanTag } / warlog
  // Retrieve clan's clan war log

  //#endregion

  //#region Specific Players specific

  //   GET
  //   / players / { playerTag }
  // Get player information
  public getPlayer(playerTag : string): Observable<IPlayerResponse> {
    const header = new HttpHeaders({ 'Content-Type': 'application/json' });
    console.log("Value ---> ", playerTag);
    return this.http.post<IPlayerResponse>(`${this.baseURL}Clash/GetPlayerInfo`, JSON.stringify(playerTag), { headers : header, withCredentials: true });
  }

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

  //#region Labels related
  // Get all player labels
  public getPlayerLabels(): Observable<any> {
    return this.http.get(`${this.baseURL}Clash/GetAllPlayerLabels`, { withCredentials: true });
  }
  // Get all clan labels
  public getClanLabels(): Observable<any> {
    return this.http.get(`${this.baseURL}Clash/GetAllClanLabels`, { withCredentials: true });
  }
  //#endregion
}
