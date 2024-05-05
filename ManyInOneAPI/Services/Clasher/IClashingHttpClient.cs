using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Clasher;

namespace ManyInOneAPI.Services.Clasher
{
    public interface IClashingHttpClient
    {
        #region Players related
        public Task<Result<ClashResponse<Player>>> GetPlayerById(string playerTag, CancellationToken cancellationToken = default);

        #endregion

        #region Locations and rankings related
        public Task<Result<ClashResponse<List<Item>>>> GetAllLocations(CancellationToken cancellationToken = default);
        public Task<Result<ClashResponse<List<ClanRanking>>>> GetClanRankingsByLocation(string locationId, int limit = 10);
        public Task<Result<ClashResponse<List<PlayerRanking>>>> GetPlayerRankingsByLocation(string locationId, int limit = 10);
        public Task<Result<ClashResponse<List<BuilderPlayerRanking>>>> GetPlayerBuilderBaseRankingsByLoaction(string locationId, int limit = 10);
        public Task<Result<ClashResponse<List<BuilderClanRanking>>>> GetClanBuilderBaseRankingsByLoaction(string locationId, int limit = 10);
        public Task<Result<ClashResponse<List<ClanCapitalRanking>>>> GetCapitalRankingsByLoaction(string locationId, int limit = 10);

        #endregion

        #region Clans related
        public Task<Result<ClashResponse<ClanInfo>>> GetClanById(string clanTag, CancellationToken cancellationToken = default);
        public Task<Result<ClashResponse<CurrentClanWarLeagueGroup>>> GetClansCurrentClanWarLeagueGroup(string clanTag);
        public Task<Result<ClashResponse<WarLog>>> GetClansWarLog(string clanTag);
        public Task<Result<ClashResponse<List<SearchClanInfo>>>> SearchClans(SearchClansRequest searchClansRequest, CancellationToken cancellationToken = default);
        public Task<Result<ClashResponse<CapitalRaidSeason>>> GetClansCapitalRaidSeasons(string clanTag, int limit = 100);
        public Task<Result<ClashResponse<CurrentWar>>> GetClansCurrentWar(string clanTag);

        #endregion

        #region Leagues Related
        public Task<Result<ClashResponse<List<Normal>>>> GetAllLeagues();
        public Task<Result<ClashResponse<List<Other>>>> GetAllWarLeagues();
        public Task<Result<ClashResponse<List<Other>>>> GetAllBuilderBaseLeagues();
        public Task<Result<ClashResponse<List<Other>>>> GetAllCapitaLeagues();

        #endregion

        #region Labels Related
        public Task<Result<ClashResponse<List<Label>>>> GetAllClanLabels(CancellationToken cancellationToken = default);
        public Task<Result<ClashResponse<List<Label>>>> GetAllPlayerLabels(CancellationToken cancellationToken = default);
        #endregion
    }
}
