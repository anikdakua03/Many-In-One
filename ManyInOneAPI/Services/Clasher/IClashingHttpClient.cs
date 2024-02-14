using ManyInOneAPI.Models.Clasher;

namespace ManyInOneAPI.Services.Clasher
{
    public interface IClashingHttpClient
    {
        #region Players related
        public Task<ClashResponse<Player>> GetPlayerById(string playerTag);

        #endregion

        #region Locations and rankings related
        public Task<ClashResponse<List<Item>>> GetAllLocations();
        public Task<ClashResponse<List<ClanRanking>>> GetClanRankingsByLocation(string locationId, int limit = 10);
        public Task<ClashResponse<List<PlayerRanking>>> GetPlayerRankingsByLocation(string locationId, int limit = 10);
        public Task<ClashResponse<List<BuilderPlayerRanking>>> GetPlayerBuilderBaseRankingsByLoaction(string locationId, int limit = 10);
        public Task<ClashResponse<List<BuilderClanRanking>>> GetClanBuilderBaseRankingsByLoaction(string locationId, int limit = 10);
        public Task<ClashResponse<List<ClanCapitalRanking>>> GetCapitalRankingsByLoaction(string locationId, int limit = 10);

        #endregion

        #region Clans related
        public Task<ClashResponse<ClanInfo>> GetClanById(string clanTag);
        public Task<ClashResponse<CurrentClanWarLeagueGroup>> GetClansCurrentClanWarLeagueGroup(string clanTag);
        public Task<ClashResponse<WarLog>> GetClansWarLog(string clanTag);
        public Task<ClashResponse<List<SearchClanInfo>>> SearchClans(SearchClansRequest searchClansRequest);
        public Task<ClashResponse<CapitalRaidSeason>> GetClansCapitalRaidSeasons(string clanTag, int limit = 100);
        public Task<ClashResponse<CurrentWar>> GetClansCurrentWar(string clanTag);

        #endregion

        #region Leagues Related
        public Task<ClashResponse<List<Normal>>> GetAllLeagues();
        public Task<ClashResponse<List<Other>>> GetAllWarLeagues();
        public Task<ClashResponse<List<Other>>> GetAllBuilderBaseLeagues();
        public Task<ClashResponse<List<Other>>> GetAllCapitaLeagues();

        #endregion

        #region Labels Related
        public Task<ClashResponse<List<Label>>> GetAllClanLabels();
        public Task<ClashResponse<List<Label>>> GetAllPlayerLabels();
        #endregion
    }
}
