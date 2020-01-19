using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rougelike.Core;

namespace Rougelike.Core.UnitTests
{
    [TestClass]
    public class WorldTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            World world = new World();

            Assert.AreEqual(10, world.GetSum());
        }
    }
}
