using ManyInOneAPI.Configurations;
using ManyInOneAPI.Models.Clasher;
using Microsoft.Extensions.Options;
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

                    return new ClashResponse<Player>() { Result = playerInfo, Succeed = true };
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
                    return new ClashResponse<ClanInfo> { Result = clanInfo, Succeed = true };
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

        public async Task<ClashResponse<List<Info>>> SearchClans(string name, string warFrequency, int locationId, int minMembers, int maxMembers, int minClanPoints, int minClanLevel, int limit = 10)
        {
            try
            {
                // Set request headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_clasherConfig.API_TOKEN}");
                // OR
                //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var address = $"clans?name={name}&warFrequency={warFrequency}&locationId={locationId}&minMembers={minMembers}&maxMembers={maxMembers}&minClanPoints={minClanPoints}&minClanLevel={minClanLevel}&limit={limit}"; // by default taking limit as 100

                var response = await _httpClient.GetAsync(address);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var clansRes = JsonSerializer.Deserialize<SearchClans>(responseString)!;
                    return new ClashResponse<List<Info>> { Result = clansRes.items, Succeed = true };
                }
                else
                {
                    return new ClashResponse<List<Info>> { Result = null, Succeed = false };
                }
            }
            catch (Exception)
            {
                return new ClashResponse<List<Info>> { Succeed = false };
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
    }
}
