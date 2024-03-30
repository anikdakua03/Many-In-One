using ManyInOneAPI.Configurations;
using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Clasher;
using ManyInOneAPI.Models.GenAI;
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
        private readonly string ClasherAPIURL = "https://api.clashofclans.com/v1/";
        public ClashingHttpClient(HttpClient httpClient, IOptionsMonitor<ClasherConfig> optionsMonitor)
        {
            _httpClient = httpClient;
            _clasherConfig = optionsMonitor.CurrentValue;
        }

        #region Player related
        public async Task<Result<ClashResponse<Player>>> GetPlayerById(string playerTag, CancellationToken cancellationToken)
        {
            if (playerTag.IsNullOrEmpty())
            {
                return Result<ClashResponse<Player>>.Failure(Error.Validation("Player tag empty or invalid. ", $"Failed with status code :--> {ErrorType.Validation}"));
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Need to encode the input string to url
            var address = $"{ClasherAPIURL}players/{HttpUtility.UrlEncode(playerTag)}";

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var playerInfo = JsonSerializer.Deserialize<Player>(responseString)!;

                return Result<ClashResponse<Player>>.Success(new ClashResponse<Player>() { Result = playerInfo, Succeed = true, Message = playerInfo is not null ? "Player found" : "No player found !!" });
            }
            else
            {
                return Result<ClashResponse<Player>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        #endregion

        #region Location and ranking related
        public async Task<Result<ClashResponse<List<Item>>>> GetAllLocations(CancellationToken cancellationToken)
        {

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}locations";

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<Location>(responseString)!;

                return Result<ClashResponse<List<Item>>>.Success(new ClashResponse<List<Item>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Item>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<List<ClanRanking>>>> GetClanRankingsByLocation(string locationId, int limit = 10)
        {

            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}locations/{locationId}/rankings/clans?limit={limit}";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<RankedClan>(responseString)!;

                return Result<ClashResponse<List<ClanRanking>>>.Success(new ClashResponse<List<ClanRanking>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<ClanRanking>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }

        }

        public async Task<Result<ClashResponse<List<PlayerRanking>>>> GetPlayerRankingsByLocation(string locationId, int limit = 10)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}locations/{locationId}/rankings/players?limit={limit}";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<RankedPlayer>(responseString)!;

                return Result<ClashResponse<List<PlayerRanking>>>.Success(new ClashResponse<List<PlayerRanking>>() { Result = jsonData.items, Succeed = true });

            }
            else
            {
                return Result<ClashResponse<List<PlayerRanking>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }

        }

        public async Task<Result<ClashResponse<List<BuilderPlayerRanking>>>> GetPlayerBuilderBaseRankingsByLoaction(string locationId, int limit = 10)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL} locations/{locationId}/rankings/players-builder-base?limit={limit}";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<RankedBuilderBasePlayer>(responseString)!;

                return Result<ClashResponse<List<BuilderPlayerRanking>>>.Success(new ClashResponse<List<BuilderPlayerRanking>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<BuilderPlayerRanking>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<List<BuilderClanRanking>>>> GetClanBuilderBaseRankingsByLoaction(string locationId, int limit = 10)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL} locations/{locationId}/rankings/clans-builder-base?limit={limit}";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<RankedBuilderBaseClan>(responseString)!;

                return Result<ClashResponse<List<BuilderClanRanking>>>.Success(new ClashResponse<List<BuilderClanRanking>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<BuilderClanRanking>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }

        }

        public async Task<Result<ClashResponse<List<ClanCapitalRanking>>>> GetCapitalRankingsByLoaction(string locationId, int limit = 10)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}locations/{locationId}/rankings/capitals?limit={limit}";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<RankedClanCapital>(responseString)!;
                return Result<ClashResponse<List<ClanCapitalRanking>>>.Success(new ClashResponse<List<ClanCapitalRanking>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<ClanCapitalRanking>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }

        }

        #endregion

        #region Clans related

        public async Task<Result<ClashResponse<ClanInfo>>> GetClanById(string clanTag, CancellationToken cancellationToken)
        {
            if (clanTag.IsNullOrEmpty())
            {
                return Result<ClashResponse<ClanInfo>>.Failure(Error.Validation("Clan tag empty or invalid. ", $"Failed with status code :--> {ErrorType.Validation}"));
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Need to encode the input string to url
            var address = $"{ClasherAPIURL}clans/{HttpUtility.UrlEncode(clanTag)}";

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var clanInfo = JsonSerializer.Deserialize<ClanInfo>(responseString)!;

                return Result<ClashResponse<ClanInfo>>.Success(new ClashResponse<ClanInfo> { Result = clanInfo, Succeed = true, Message = clanInfo is not null ? "Clan found with given tag found" : "No clan found !!" });
            }
            else
            {
                return Result<ClashResponse<ClanInfo>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<CurrentClanWarLeagueGroup>>> GetClansCurrentClanWarLeagueGroup(string clanTag)
        {
            if (clanTag.IsNullOrEmpty())
            {
                return Result<ClashResponse<CurrentClanWarLeagueGroup>>.Failure(Error.Validation("Clan tag empty or invalid. ", $"Failed with status code :--> {ErrorType.Validation}"));
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Need to encode the input string to url
            var address = $"{ClasherAPIURL}clans/{HttpUtility.UrlEncode(clanTag)}/currentwar/leaguegroup";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var clanWarRes = JsonSerializer.Deserialize<CurrentClanWarLeagueGroup>(responseString)!;


                return Result<ClashResponse<CurrentClanWarLeagueGroup>>.Success(new ClashResponse<CurrentClanWarLeagueGroup> { Result = clanWarRes, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<CurrentClanWarLeagueGroup>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }

        }

        public async Task<Result<ClashResponse<WarLog>>> GetClansWarLog(string clanTag)
        {
            if (clanTag.IsNullOrEmpty())
            {
                return Result<ClashResponse<WarLog>>.Failure(Error.Validation("Clan tag empty or invalid. ", $"Failed with status code :--> {ErrorType.Validation}"));
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Need to encode the input string to url
            var address = $"{ClasherAPIURL}clans/{HttpUtility.UrlEncode(clanTag)}/warlog?limit=100"; // by default taking limit as 100

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var clanWarRes = JsonSerializer.Deserialize<WarLog>(responseString)!;

                return Result<ClashResponse<WarLog>>.Success(new ClashResponse<WarLog> { Result = clanWarRes, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<WarLog>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<List<SearchClanInfo>>>> SearchClans(SearchClansRequest searchClansRequest, CancellationToken cancellationToken)
        {
            if (searchClansRequest.Name.IsNullOrEmpty())
            {
                return Result<ClashResponse<List<SearchClanInfo>>>.Failure(Error.Validation("Search Clan name is empty or invalid. ", $"Failed with status code :--> {ErrorType.Validation}"));
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = SearchClansAddressMaker(searchClansRequest);

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var clansRes = JsonSerializer.Deserialize<SearchClans>(responseString)!;

                return Result<ClashResponse<List<SearchClanInfo>>>.Success(new ClashResponse<List<SearchClanInfo>> { Result = clansRes.items, Succeed = true, Message = clansRes.items!.Count() > 0 ? "Clans found with given criteria" : "No clan found with given filters !!" });
            }
            else
            {
                return Result<ClashResponse<List<SearchClanInfo>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<CapitalRaidSeason>>> GetClansCapitalRaidSeasons(string clanTag, int limit = 10)
        {
            if (clanTag.IsNullOrEmpty())
            {
                return Result<ClashResponse<CapitalRaidSeason>>.Failure(Error.Validation("Clan tag empty or invalid. ", $"Failed with status code :--> {ErrorType.Validation}"));
            }
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Need to encode the input string to url
            var address = $"{ClasherAPIURL}clans/{clanTag}?limit=10"; // by default taking limit as 100

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var clanWarRes = JsonSerializer.Deserialize<CapitalRaidSeason>(responseString)!;


                return Result<ClashResponse<CapitalRaidSeason>>.Success(new ClashResponse<CapitalRaidSeason> { Result = clanWarRes, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<CapitalRaidSeason>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        // pending 
        // v1/clans/{tag}/currentwar
        public Task<Result<ClashResponse<CurrentWar>>> GetClansCurrentWar(string clanTag)
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
                allLabels += searchClansRequest.Labels![i].ToString() + ",";
            }

            // Uri address making according to params , only name is required , others are optional
            var address = $"{ClasherAPIURL}clans?name=" + searchClansRequest.Name;
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

        public async Task<Result<ClashResponse<List<Normal>>>> GetAllLeagues()
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}leagues";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<NormalLeague>(responseString)!;
                return Result<ClashResponse<List<Normal>>>.Success(new ClashResponse<List<Normal>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Normal>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<List<Other>>>> GetAllWarLeagues()
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}warleagues";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<OtherLeague>(responseString)!;
                return Result<ClashResponse<List<Other>>>.Success(new ClashResponse<List<Other>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Other>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<List<Other>>>> GetAllBuilderBaseLeagues()
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}builderbaseleagues";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<OtherLeague>(responseString)!;
                return Result<ClashResponse<List<Other>>>.Success(new ClashResponse<List<Other>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Other>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        public async Task<Result<ClashResponse<List<Other>>>> GetAllCapitaLeagues()
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}capitalleagues";

            var response = await _httpClient.GetAsync(address);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<OtherLeague>(responseString)!;
                return Result<ClashResponse<List<Other>>>.Success(new ClashResponse<List<Other>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Other>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }

        #endregion

        #region Labels Related
        public async Task<Result<ClashResponse<List<Label>>>> GetAllClanLabels(CancellationToken cancellationToken)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}labels/clans";

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<ClanLabel>(responseString)!;

                return Result<ClashResponse<List<Label>>>.Success(new ClashResponse<List<Label>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Label>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }
        public async Task<Result<ClashResponse<List<Label>>>> GetAllPlayerLabels(CancellationToken cancellationToken)
        {
            // Set request headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
            // OR
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var address = $"{ClasherAPIURL}labels/players";

            var response = await _httpClient.GetAsync(address, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var jsonData = JsonSerializer.Deserialize<PlayerLabel>(responseString)!;

                return Result<ClashResponse<List<Label>>>.Success(new ClashResponse<List<Label>>() { Result = jsonData.items, Succeed = true });
            }
            else
            {
                return Result<ClashResponse<List<Label>>>.Failure(Error.Failure("Error", $"Failed with status code :--> {response.StatusCode}"));
            }
        }
        #endregion
    }
}
