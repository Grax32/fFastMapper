using Grax.fFastMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace fFastMapper.Tests
{


    /// <summary>
    ///This is a test class for fFastMapExtensionsTest and is intended
    ///to contain all fFastMapExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class fFastMapExtensionsTest
    {
        [TestMethod()]
        public void fFastMapExtensions_Map()
        {
            // arrange
            fFastMap.AutoInitialize = true;
            fFastMap.DefaultMappingDirection = fFastMap.MappingDirection.Bidirectional;

            var fluent = fFastMapperInternal<LeftTestClass, RightTestClass>.fFastMapFluent;
            var expectedId = 57;
            var source = new LeftTestClass { Id = expectedId };

            // act
            RightTestClass actual = fFastMapExtensions.Map<LeftTestClass, RightTestClass>(fluent, source);

            // assert
            Assert.AreEqual(expectedId, actual.Id);
        }

        internal class LeftTestClass
        {
            public int Id { get; set; }
        }

        internal class RightTestClass
        {
            public int Id { get; set; }
        }
    }
}
