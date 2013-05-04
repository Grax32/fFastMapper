using Grax.fFastMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.Serialization;
using fFastMapper.Tests.Library;

namespace fFastMapper.Tests
{


    /// <summary>
    ///This is a test class for fFastMapExceptionTest and is intended
    ///to contain all fFastMapExceptionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class fFastMapExceptionTest
    {
        const string GenericErrorMessage = "Generic Error Message";

        /// <summary>
        ///A test for fFastMapException Constructor
        ///</summary>
        [TestMethod()]
        public void fFastMapException_ConstructorTest()
        {
            // arrange
            var innerErrorMessage = "Inner Exception";

            // act
            var target = new fFastMap.fFastMapException(GenericErrorMessage, new Exception(innerErrorMessage));

            // assert
            Assert.IsTrue(target is Exception);
            Assert.IsTrue(target.Message == GenericErrorMessage);
            Assert.IsTrue(target.InnerException.Message == innerErrorMessage);
        }

        /// <summary>
        ///A test for fFastMapException Constructor
        ///</summary>
        [TestMethod()]
        public void fFastMapException_ConstructorTest1()
        {
            // arrange
            var info = Functions.GetExceptionSerializationInfo(typeof(fFastMap.fFastMapException), GenericErrorMessage);
            var context = new StreamingContext();

            // act
            var target = new fFastMap.fFastMapException(info, context);

            // assert
            Assert.IsTrue(target is Exception);
            Assert.IsTrue(target.Message == GenericErrorMessage);
        }

        /// <summary>
        ///A test for fFastMapException Constructor
        ///</summary>
        [TestMethod()]
        public void fFastMapException_ConstructorTest2()
        {
            var target = new fFastMap.fFastMapException();

            Assert.IsTrue(target is Exception);
        }

        /// <summary>
        ///A test for fFastMapException Constructor
        ///</summary>
        [TestMethod()]
        public void fFastMapException_ConstructorTest3()
        {
            // act
            var target = new fFastMap.fFastMapException(GenericErrorMessage);

            // assert
            Assert.IsTrue(target is Exception);
            Assert.IsTrue(target.Message == GenericErrorMessage);
        }
    }
}
