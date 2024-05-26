using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.RiotGames.ChampionMasteryV4;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiChampionMasteryV4Test : ApiTest
    {

        [TestMethod]
        public void GetChampion()
        {
            var account = Api.AccountV1().GetByRiotId(RegionalRoute.AMERICAS, "LugnutsK", "000");
            CheckGetChampion(Api.ChampionMasteryV4().GetChampionMasteryByPUUID(PlatformRoute.NA1, encryptedPUUID: account.Puuid, championId: Champion.ZYRA));
        }

        [TestMethod]
        public async Task GetChampionAsync()
        {
            var account = await Api.AccountV1().GetByRiotIdAsync(RegionalRoute.AMERICAS, "LugnutsK", "000");
            CheckGetChampion(await Api.ChampionMasteryV4().GetChampionMasteryByPUUIDAsync(PlatformRoute.NA1, encryptedPUUID: account.Puuid, championId: Champion.ZYRA));
        }

        public static void CheckGetChampion(ChampionMastery result)
        {
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ChampionLevel >= 93, result.ChampionLevel.ToString());
            Assert.IsTrue(result.ChampionPoints > 1_000_000, result.ChampionPoints.ToString());
        }

        [TestMethod]
        public void GetChampions()
        {
            var account = Api.AccountV1().GetByRiotId(RegionalRoute.AMERICAS, "LugnutsK", "000");
            CheckGetChampions(Api.ChampionMasteryV4().GetAllChampionMasteriesByPUUID(PlatformRoute.NA1, account.Puuid));
        }

        [TestMethod]
        public async Task GetChampionsAsync()
        {
            var account = await Api.AccountV1().GetByRiotIdAsync(RegionalRoute.AMERICAS, "LugnutsK", "000");
            CheckGetChampions(await Api.ChampionMasteryV4().GetAllChampionMasteriesByPUUIDAsync(PlatformRoute.NA1, account.Puuid));
        }

        public static void CheckGetChampions(ChampionMastery[] champData)
        {
            var topChamps = new HashSet<Champion>
            {
                Champion.ZYRA, Champion.SORAKA, Champion.MORGANA, Champion.SONA, Champion.JANNA,
                Champion.EKKO, Champion.NAMI, Champion.TARIC, Champion.POPPY, Champion.BRAND
            };
            var topChampCount = topChamps.Count;
            for (var i = 0; i < topChampCount; i++)
                Assert.IsTrue(topChamps.Remove(champData[i].ChampionId), $"Unexpected top champ: {champData[i].ChampionId}.");
            Assert.AreEqual(0, topChamps.Count, $"Champions not found: {topChamps}.");
        }

        [TestMethod]
        public void GetScoreByPUUID()
        {
            var account = Api.AccountV1().GetByRiotId(RegionalRoute.AMERICAS, "Ma5tery", "EUW");
            CheckGetScore(Api.ChampionMasteryV4().GetChampionMasteryScoreByPUUID(PlatformRoute.EUW1, account.Puuid));
        }

        [TestMethod]
        public async Task GetScoreByPUUIDAsync()
        {
            var account = await Api.AccountV1().GetByRiotIdAsync(RegionalRoute.AMERICAS, "Ma5tery", "EUW");
            CheckGetScore(await Api.ChampionMasteryV4().GetChampionMasteryScoreByPUUIDAsync(PlatformRoute.EUW1, account.Puuid));
        }

        public static void CheckGetScore(int score)
        {
            Assert.IsTrue(1000 <= score && score < 1100, score.ToString());
        }
    }
}
