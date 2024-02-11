using ManyInOneAPI.Configurations;
using ManyInOneAPI.Models.Clasher;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace ManyInOneAPI.Services.Clasher
{
    public class ClashingHttpClient : IClashingHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClasherConfig _clasherConfig;

        public ClashingHttpClient(HttpClient httpClient, IOptionsMonitor<ClasherConfig> optionsMonitor)
        {
            _httpClient = httpClient;
            _clasherConfig = optionsMonitor.CurrentValue;
        }

        #region Player related
        public async Task<ClashResponse<Player>> GetPlayerById(string playerTag)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Need to encode the input string to url
                var address = $"players/{HttpUtility.UrlEncode(playerTag)}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var playerInfo = JsonSerializer.Deserialize<Player>(responseString)!;

                    return new ClashResponse<Player>() { Result = playerInfo, Succeed = true, Message = playerInfo is not null ? "Player found" : "No player found !!" };
                }
                else
                {
                    return new ClashResponse<Player>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<Player>() { Succeed = false };
            }
        }

        #endregion

        #region Location and ranking related
        public async Task<ClashResponse<List<Item>>> GetAllLocations()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = "locations";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<Location>(responseString)!;

                    return new ClashResponse<List<Item>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Item>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Item>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<ClanRanking>>> GetClanRankingsByLocation(string locationId, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"locations/{locationId}/rankings/clans?limit={limit}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<RankedClan>(responseString)!;

                    return new ClashResponse<List<ClanRanking>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<ClanRanking>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<ClanRanking>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<PlayerRanking>>> GetPlayerRankingsByLocation(string locationId, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"locations/{locationId}/rankings/players?limit={limit}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<RankedPlayer>(responseString)!;

                    return new ClashResponse<List<PlayerRanking>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<PlayerRanking>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<PlayerRanking>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<BuilderPlayerRanking>>> GetPlayerBuilderBaseRankingsByLoaction(string locationId, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"locations/{locationId}/rankings/players-builder-base?limit={limit}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<RankedBuilderBasePlayer>(responseString)!;

                    return new ClashResponse<List<BuilderPlayerRanking>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<BuilderPlayerRanking>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<BuilderPlayerRanking>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<BuilderClanRanking>>> GetClanBuilderBaseRankingsByLoaction(string locationId, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"locations/{locationId}/rankings/clans-builder-base?limit={limit}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<RankedBuilderBaseClan>(responseString)!;

                    return new ClashResponse<List<BuilderClanRanking>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<BuilderClanRanking>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<BuilderClanRanking>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<ClanCapitalRanking>>> GetCapitalRankingsByLoaction(string locationId, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"locations/{locationId}/rankings/capitals?limit={limit}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<RankedClanCapital>(responseString)!;

                    return new ClashResponse<List<ClanCapitalRanking>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<ClanCapitalRanking>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<ClanCapitalRanking>>() { Succeed = false };
            }
        }

        #endregion

        #region Clans related

        public async Task<ClashResponse<ClanInfo>> GetClanById(string clanTag)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Need to encode the input string to url
                var address = $"clans/{HttpUtility.UrlEncode(clanTag)}";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var clanInfo = JsonSerializer.Deserialize<ClanInfo>(responseString)!;
                    return new ClashResponse<ClanInfo> { Result = clanInfo, Succeed = true, Message = clanInfo is not null ? "Clan found with given tag found" : "No clan found !!" };
                }
                else
                {
                    return new ClashResponse<ClanInfo> { Result = null, Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<ClanInfo> { Result = null, Succeed = false };
            }
        }

        public async Task<ClashResponse<CurrentClanWarLeagueGroup>> GetClansCurrentClanWarLeagueGroup(string clanTag)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Need to encode the input string to url
                var address = $"clans/{HttpUtility.UrlEncode(clanTag)}/currentwar/leaguegroup";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var clanWarRes = JsonSerializer.Deserialize<CurrentClanWarLeagueGroup>(responseString)!;
                    return new ClashResponse<CurrentClanWarLeagueGroup> { Result = clanWarRes, Succeed = true };
                }
                else
                {
                    return new ClashResponse<CurrentClanWarLeagueGroup> { Result = null, Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<CurrentClanWarLeagueGroup> { Result = null, Succeed = false };
            }
        }

        public async Task<ClashResponse<WarLog>> GetClansWarLog(string clanTag)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Need to encode the input string to url
                var address = $"clans/{HttpUtility.UrlEncode(clanTag)}/warlog?limit=100"; // by default taking limit as 100

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var clanWarRes = JsonSerializer.Deserialize<WarLog>(responseString)!;
                    return new ClashResponse<WarLog> { Result = clanWarRes, Succeed = true };
                }
                else
                {
                    return new ClashResponse<WarLog> { Result = null, Succeed = false };
                }
            }
            catch (Exception )
            {
                return new ClashResponse<WarLog> { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<SearchClanInfo>>> SearchClans(SearchClansRequest searchClansRequest)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = SearchClansAddressMaker(searchClansRequest);

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var clansRes = JsonSerializer.Deserialize<SearchClans>(responseString)!;
                    return new ClashResponse<List<SearchClanInfo>> { Result = clansRes.items, Succeed = true, Message = clansRes.items!.Count() > 0 ? "Clans found with given criteria" : "No clan found with given filters !!" };
                }
                else
                {
                    return new ClashResponse<List<SearchClanInfo>> { Result = null, Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<SearchClanInfo>> { Succeed = false };
            }
        }

        public async Task<ClashResponse<CapitalRaidSeason>> GetClansCapitalRaidSeasons(string clanTag, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Need to encode the input string to url
                var address = $"clans/{clanTag}?limit=10"; // by default taking limit as 100

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var clanWarRes = JsonSerializer.Deserialize<CapitalRaidSeason>(responseString)!;
                    return new ClashResponse<CapitalRaidSeason> { Result = clanWarRes, Succeed = true };
                }
                else
                {
                    return new ClashResponse<CapitalRaidSeason> { Result = null, Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<CapitalRaidSeason> { Succeed = false };
            }
        }

        // pending 
        // v1/clans/{tag}/currentwar
        public Task<ClashResponse<CurrentWar>> GetClansCurrentWar(string clanTag)
        {
            // TODO : To be implemented after getting the proper response model
            return null!;
        }

        private string SearchClansAddressMaker(SearchClansRequest searchClansRequest)
        {
            string allLabels = "";
            for (int i = 0; i < searchClansRequest.Labels!.Count(); i++)
            {
                // if we are getting int from client , can convert it here also
                allLabels +=  searchClansRequest.Labels![i].ToString() + ",";
            }

            // Uri address making according to params , only name is required , others are optional
            var address = "clans?name=" + searchClansRequest.Name;
            if (!searchClansRequest.WarFrequency.IsNullOrEmpty())
            {
                address += "&warFrequency=" + searchClansRequest.WarFrequency;
            }
            if (searchClansRequest.LocationId > 0)
            {
                address += "&locationId=" + searchClansRequest.LocationId;
            }
            if (searchClansRequest.MinMembers > 0)
            {
                address += "&minMembers=" + searchClansRequest.MinMembers;
            }

            if (searchClansRequest.MaxMembers > 0)
            {
                address += "&maxMembers=" + searchClansRequest.MaxMembers;
            }

            if (searchClansRequest.MinClanPoints > 0)
            {
                address += "&minClanPoints=" + searchClansRequest.MinClanPoints;
            }

            if (searchClansRequest.MinClanLevel > 0)
            {
                address += "&minClanLevel=" + searchClansRequest.MinClanLevel;
            }

            address += "&limit=" + (searchClansRequest.Limit > 100 ? 50 : searchClansRequest.Limit); // manual limitting 

            if (searchClansRequest.Labels!.Count() > 0 && !searchClansRequest.Labels.IsNullOrEmpty())
            {
                // for removing extra ,
                string labelIds = allLabels.Remove(allLabels.Length - 1, 1);
                address += "&labelIds=" + HttpUtility.UrlEncode(labelIds);
            }

            return address;
        }
        #endregion

        #region Leagues Related

        public async Task<ClashResponse<List<Normal>>> GetAllLeagues()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"{_clasherConfig.ClasherAPIURL}leagues";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<NormalLeague>(responseString)!;
                    return new ClashResponse<List<Normal>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Normal>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Normal>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<Other>>> GetAllWarLeagues()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"{_clasherConfig.ClasherAPIURL}warleagues";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<OtherLeague>(responseString)!;
                    return new ClashResponse<List<Other>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Other>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Other>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<Other>>> GetAllBuilderBaseLeagues()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"{_clasherConfig.ClasherAPIURL}builderbaseleagues";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<OtherLeague>(responseString)!;
                    return new ClashResponse<List<Other>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Other>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Other>>() { Succeed = false };
            }
        }

        public async Task<ClashResponse<List<Other>>> GetAllCapitaLeagues()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"{_clasherConfig.ClasherAPIURL}capitalleagues";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<OtherLeague>(responseString)!;
                    return new ClashResponse<List<Other>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Other>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Other>>() { Succeed = false };
            }
        }

        #endregion

        #region Labels Related
        public async Task<ClashResponse<List<Label>>> GetAllClanLabels()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = "labels/clans";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<ClanLabel>(responseString)!;

                    return new ClashResponse<List<Label>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Label>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Label>>() { Succeed = false };
            }
        }
        public async Task<ClashResponse<List<Label>>> GetAllPlayerLabels()
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = "labels/players";

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonData = JsonSerializer.Deserialize<PlayerLabel>(responseString)!;

                    return new ClashResponse<List<Label>>() { Result = jsonData.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Label>>() { Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Label>>() { Succeed = false };
            }
        }
        #endregion
    }
}
