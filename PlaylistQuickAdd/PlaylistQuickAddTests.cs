using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlaylistQuickAdd
{
    [TestClass]
    public class PlaylistQuickAddTests
    {
        [TestMethod]
        public async Task GetSpotifyAccessToken_ReturnsAccessToken()
        {
            var authorization = new Authorization();
            var token = await authorization.GetSpotifyAccessToken();
            Assert.IsNotNull(token.AccessToken);
        }
    }
}
