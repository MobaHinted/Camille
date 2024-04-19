using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Camille.Enums;

namespace Camille.RiotGames.Test
{
    [TestClass]
    public class ApiCancellationTest : ApiTest
    {
        [TestMethod]
        [Ignore("Eats rate limit.")]
        public async Task SummonerCancellationTest()
        {
            var tokenSource = new CancellationTokenSource();
            var tasks = Enumerable.Range(0, 1000)
                .Select(n => Api.AccountV1().GetByRiotIdAsync(PlatformRoute.NA1.ToRegional(), n.ToString(), "NA1", tokenSource.Token))
                .ToList();
            tokenSource.CancelAfter(1000);
            for (var n = 0; n < tasks.Count; n++)
            {
                var task = tasks[n];
                try
                {
                    var account = await task;
                    if (account == null)
                        Console.WriteLine($"Summoner {n} is null.");
                    else
                        Assert.AreEqual(n.ToString(), Regex.Replace(account.GameName, @"\D", ""));
                }
                catch (OperationCanceledException e) // And TaskCanceledException.
                {
                    Console.WriteLine($"Summoner {n} cancelled: {e.GetType().Name}.");
                }
            }
        }
    }
}
