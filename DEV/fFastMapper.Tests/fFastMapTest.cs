using Grax.fFastMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace fFastMapper.Tests
{
    
    
    /// <summary>
    ///This is a test class for fFastMapTest and is intended
    ///to contain all fFastMapTest Unit Tests
    ///</summary>
    [TestClass()]
    public class fFastMapTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Map
        ///</summary>
        public void MapTestHelper<TFrom, TTo>()
        {
            TFrom from = default(TFrom); // TODO: Initialize to an appropriate value
            TTo to = default(TTo); // TODO: Initialize to an appropriate value
            TTo expected = default(TTo); // TODO: Initialize to an appropriate value
            TTo actual;
            actual = fFastMap.Map<TFrom, TTo>(from, to);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [Ignore]
        [TestMethod()]
        public void MapTest()
        {
            MapTestHelper<GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for MapperFor
        ///</summary>
        public void MapperForTestHelper<TLeft, TRight>()
        {
            fFastMapperFluent<TLeft, TRight> expected = null; // TODO: Initialize to an appropriate value
            fFastMapperFluent<TLeft, TRight> actual;
            actual = fFastMap.MapperFor<TLeft, TRight>();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        [Ignore]
        public void MapperForTest()
        {
            MapperForTestHelper<GenericParameterHelper, GenericParameterHelper>();
        }
    }
}
