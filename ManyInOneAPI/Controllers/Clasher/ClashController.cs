using ManyInOneAPI.Models.Clasher;
using ManyInOneAPI.Services.Clasher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ManyInOneAPI.Controllers.Clasher
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("bucket")]
    public class ClashController : ControllerBase
    {
        private readonly IClashingHttpClient _clashingHttpClient;

        public ClashController(IClashingHttpClient clashingHttpClient)
        {
            _clashingHttpClient = clashingHttpClient;
        }

        #region Players related

        [HttpPost]
        [Route("GetPlayerInfo")]
        public async Task<IActionResult> GetPlayer([FromBody] string playerTag)
        {
            var res = await _clashingHttpClient.GetPlayerById(playerTag, HttpContext.RequestAborted);

            return Ok(res);
        }

        #endregion

        #region Locations and rankings related

        [HttpGet]
        [Route("GetAllLocations")]
        public async Task<IActionResult> GetLocations()
        {
            var res = await _clashingHttpClient.GetAllLocations(HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpPost]
        [Route("ClanRankingsByLocation")]
        public async Task<IActionResult> ClanRankingsByLocation(string locationId, int limit)
        {
            var res = await _clashingHttpClient.GetClanRankingsByLocation(locationId, limit);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetPlayerRankingsByLocation")]
        public async Task<IActionResult> PlayerRankingsByLocation(string locationId, int limit)
        {
            var res = await _clashingHttpClient.GetPlayerRankingsByLocation(locationId, limit);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetPlayerBuilderBaseRankingsByLoaction")]
        public async Task<IActionResult> PlayerBuilderBaseRankingsByLoaction(string locationId, int limit)
        {
            var res = await _clashingHttpClient.GetPlayerBuilderBaseRankingsByLoaction(locationId, limit);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetClanBuilderBaseRankingsByLoaction")]
        public async Task<IActionResult> ClanBuilderBaseRankingsByLoaction(string locationId, int limit)
        {
            var res = await _clashingHttpClient.GetClanBuilderBaseRankingsByLoaction(locationId, limit);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetCapitalRankingsByLoaction")]
        public async Task<IActionResult> CapitalRankingsByLoaction(string locationId, int limit)
        {
            var res = await _clashingHttpClient.GetCapitalRankingsByLoaction(locationId, limit);

            return Ok(res);
        }

        #endregion

        #region Clans related

        [HttpPost]
        [Route("GetClanInfoById")]
        public async Task<IActionResult> GetClan([FromBody] string clanTag)
        {
            var res = await _clashingHttpClient.GetClanById(clanTag, HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetCurrentClanWarLeagueById")]
        public async Task<IActionResult> GetCurrentClanWarLeague([FromQuery] string clanTag)
        {
            var res = await _clashingHttpClient.GetClansCurrentClanWarLeagueGroup(clanTag);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetWarLog")]
        public async Task<IActionResult> GetClanWarLogs([FromQuery] string clanTag)
        {
            var res = await _clashingHttpClient.GetClansWarLog(clanTag);

            return Ok(res);
        }

        [HttpPost]
        [Route("SearchClans")]
        public async Task<IActionResult> GetClans([FromBody] SearchClansRequest searchClansRequest)
        {
            var res = await _clashingHttpClient.SearchClans(searchClansRequest, HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetClanCapitalRaidSeason")]
        public async Task<IActionResult> ClansCapitalRaidSeason(string clanTag, int limit)
        {
            var res = await _clashingHttpClient.GetClansCapitalRaidSeasons(clanTag, limit);

            return Ok(res);
        }

        [HttpPost]
        [Route("GetClansCurrentWar")]
        public async Task<IActionResult> ClansCurrentWar(string clanTag)
        {
            var res = await _clashingHttpClient.GetClansCurrentWar(clanTag);

            return Ok(res);
        }

        #endregion

        #region Leagues Related

        [HttpGet]
        [Route("GetAllLeagues")]
        public async Task<IActionResult> GetLeagues()
        {
            var res = await _clashingHttpClient.GetAllLeagues();

            return Ok(res);
        }

        [HttpGet]
        [Route("GetAllWarLeagues")]
        public async Task<IActionResult> AllWarLeagues()
        {
            var res = await _clashingHttpClient.GetAllWarLeagues();

            return Ok(res);
        }

        [HttpGet]
        [Route("GetAllBuilderBaseLeagues")]
        public async Task<IActionResult> AllBuilderBaseLeagues()
        {
            var res = await _clashingHttpClient.GetAllBuilderBaseLeagues();

            return Ok(res);
        }

        [HttpGet]
        [Route("GetAllCapitaLeagues")]
        public async Task<IActionResult> AllCapitaLeagues()
        {
            var res = await _clashingHttpClient.GetAllCapitaLeagues();

            return Ok(res);
        }
        #endregion

        #region Labels Related

        [HttpGet]
        [Route("GetAllClanLabels")]
        public async Task<IActionResult> AllClanLabels()
        {
            var res = await _clashingHttpClient.GetAllClanLabels(HttpContext.RequestAborted);

            return Ok(res);
        }

        [HttpGet]
        [Route("GetAllPlayerLabels")]
        public async Task<IActionResult> AllPlayerLabels()
        {
            var res = await _clashingHttpClient.GetAllPlayerLabels(HttpContext.RequestAborted);

            return Ok(res);
        }
        #endregion
    }
}
