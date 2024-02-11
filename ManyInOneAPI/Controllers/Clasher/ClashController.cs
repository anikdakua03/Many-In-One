using ManyInOneAPI.Models.Clasher;
using ManyInOneAPI.Services.Clasher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ManyInOneAPI.Controllers.Clasher
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetPlayer([FromBody]string playerTag)
        {
            try
            {
                if (!playerTag.IsNullOrEmpty())
                {
                    var res = await _clashingHttpClient.GetPlayerById(playerTag);

                    //if (res.Succeed)
                    //{
                    //    return Ok(res);
                    //}

                    return Ok(res);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Locations and rankings related

        [HttpGet]
        [Route("GetAllLocations")]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllLocations();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ClanRankingsByLocation")]
        public async Task<IActionResult> ClanRankingsByLocation(string locationId, int limit)
        {
            try
            {
                var res = await _clashingHttpClient.GetClanRankingsByLocation(locationId, limit);

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPlayerRankingsByLocation")]
        public async Task<IActionResult> PlayerRankingsByLocation(string locationId, int limit)
        {
            try
            {
                var res = await _clashingHttpClient.GetPlayerRankingsByLocation(locationId, limit);

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPlayerBuilderBaseRankingsByLoaction")]
        public async Task<IActionResult> PlayerBuilderBaseRankingsByLoaction(string locationId, int limit)
        {
            try
            {
                var res = await _clashingHttpClient.GetPlayerBuilderBaseRankingsByLoaction(locationId, limit);

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetClanBuilderBaseRankingsByLoaction")]
        public async Task<IActionResult> ClanBuilderBaseRankingsByLoaction(string locationId, int limit)
        {
            try
            {
                var res = await _clashingHttpClient.GetClanBuilderBaseRankingsByLoaction(locationId, limit);

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCapitalRankingsByLoaction")]
        public async Task<IActionResult> CapitalRankingsByLoaction(string locationId, int limit)
        {
            try
            {
                var res = await _clashingHttpClient.GetCapitalRankingsByLoaction(locationId, limit);

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Clans related

        [HttpPost]
        [Route("GetClanInfoById")]
        public async Task<IActionResult> GetClan([FromBody]string clanTag)
        {
            try
            {
                if (!clanTag.IsNullOrEmpty())
                {
                    var res = await _clashingHttpClient.GetClanById(clanTag);

                    //if (res.Succeed)
                    //{
                    //    return Ok(res);
                    //}

                    return Ok(res);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCurrentClanWarLeagueById")]
        public async Task<IActionResult> GetCurrentClanWarLeague([FromQuery] string clanTag)
        {
            try
            {
                if (!clanTag.IsNullOrEmpty())
                {
                    var res = await _clashingHttpClient.GetClansCurrentClanWarLeagueGroup(clanTag);

                    if (res.Succeed)
                    {
                        return Ok(res);
                    }

                    return NotFound(res.Errors);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetWarLog")]
        public async Task<IActionResult> GetClanWarLogs([FromQuery] string clanTag)
        {
            try
            {
                if (!clanTag.IsNullOrEmpty())
                {
                    var res = await _clashingHttpClient.GetClansWarLog(clanTag);

                    if (res.Succeed)
                    {
                        return Ok(res);
                    }

                    return NotFound(res.Errors);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SearchClans")]
        public async Task<IActionResult> GetClans([FromBody]SearchClansRequest searchClansRequest)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                    var res = await _clashingHttpClient.SearchClans(searchClansRequest);

                    //if (res.Succeed)
                    //{
                    //    return Ok(res);
                    //}

                    return Ok(res);
                }
                return BadRequest($"Invalid parameter with --> {ModelState.ErrorCount} error count !!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetClanCapitalRaidSeason")]
        public async Task<IActionResult> ClansCapitalRaidSeason(string clanTag, int limit)
        {
            try
            {
                if (!clanTag.IsNullOrEmpty()) 
                {
                    var res = await _clashingHttpClient.GetClansCapitalRaidSeasons(clanTag, limit);

                    if (res.Succeed)
                    {
                        return Ok(res);
                    }

                    return NotFound(res.Errors);
                }
                return BadRequest("Invalid request !! ");
                }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetClansCurrentWar")]
        public async Task<IActionResult> ClansCurrentWar(string clanTag)
        {
            try
            {
                if (!clanTag.IsNullOrEmpty())
                {
                    var res = await _clashingHttpClient.GetClansCurrentWar(clanTag);

                    if (res.Succeed)
                    {
                        return Ok(res);
                    }

                    return NotFound(res.Errors);
                }
                return BadRequest("Invalid request !! ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Leagues Related

        [HttpGet]
        [Route("GetAllLeagues")]
        public async Task<IActionResult> GetLeagues()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllLeagues();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllWarLeagues")]
        public async Task<IActionResult> AllWarLeagues()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllWarLeagues();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllBuilderBaseLeagues")]
        public async Task<IActionResult> AllBuilderBaseLeagues()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllBuilderBaseLeagues();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllCapitaLeagues")]
        public async Task<IActionResult> AllCapitaLeagues()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllCapitaLeagues();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Labels Related

        [HttpGet]
        [Route("GetAllClanLabels")]
        public async Task<IActionResult> AllClanLabels()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllClanLabels();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllPlayerLabels")]
        public async Task<IActionResult> AllPlayerLabels()
        {
            try
            {
                var res = await _clashingHttpClient.GetAllPlayerLabels();

                if (res.Succeed)
                {
                    return Ok(res);
                }

                return NotFound(res.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
