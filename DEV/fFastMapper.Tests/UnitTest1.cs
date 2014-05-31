using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Grax.fFastImplementer;
using System.Diagnostics;

namespace fFastMapper.Tests
{
    [TestClass]
    public class fFastImplementerTests
    {
        [TestMethod]
        public void fFastImplementer_Test1()
        {
            var impl1 = fFastImplementInterface.CreateInterfaceImplementer(typeof(ITestfFastImplementerPropertyOnly));

            var newObject1 = fFastImplementInterface.CreateNew<ITestfFastImplementerPropertyOnly>();

            newObject1.Thing1 = "George";
            newObject1.Thing2 = 783;

            Assert.AreEqual("George", newObject1.Thing1);
            Assert.AreEqual(783, newObject1.Thing2);

            var newObject2 = fFastImplementInterface.CreateNewWithBase<ITestfFastImplementerPropertyOnly, ImplementerBaseSetSetting>();
            var newObject2Base = (ImplementerBaseSetSetting)newObject2;

            newObject2.Thing1 = "simonize";

            Assert.AreEqual("Thing1", newObject2Base.LastOnValueSettingCalledForProperty);
            Assert.AreEqual("Thing1", newObject2Base.LastOnValueSetCalledForProperty);

            var newObject3 = fFastImplementInterface.CreateNewWithBase<ITestfFastImplementerPropertyOnly, ImplementerBaseSetSettingPlusAbort>();
            var newObject3Base = (ImplementerBaseSetSettingPlusAbort)newObject3;

            newObject3Base.AbortReturnValue = true;
            newObject3.Thing1 = "simonize";

            Assert.AreEqual("Thing1", newObject3Base.LastOnValueSettingCalledForProperty);
            Assert.AreEqual(null, newObject3Base.LastOnValueSetCalledForProperty);
            Assert.AreEqual(null, newObject3.Thing1);

            newObject3Base.AbortReturnValue = false;
            newObject3.Thing1 = "simonize";

            Assert.AreEqual("Thing1", newObject3Base.LastOnValueSettingCalledForProperty);
            Assert.AreEqual("Thing1", newObject3Base.LastOnValueSetCalledForProperty);

            var baseEmpty = fFastImplementInterface.CreateNewWithBase<ITestfFastImplementerPropertyOnly, ImplementerBaseEmpty>();

            baseEmpty.Thing1 = "asdf";
            baseEmpty.Thing2 = 12345;

            Assert.AreEqual("asdf", baseEmpty.Thing1);
            Assert.AreEqual(12345, baseEmpty.Thing2);

            var myBase = fFastImplementInterface.CreateNewWithBase<ITestfFastImplementerPropertyOnly, MyImplementerBase>();

            myBase.Thing1 = "goo";
            myBase.Thing1 = "abort";
            Assert.AreEqual("goo", myBase.Thing1);

            myBase.Thing2 = 11234;
            Assert.AreEqual(11234, myBase.Thing2);
        }

        public class ImplementerTestActual : ITestfFastImplementerPropertyOnly
        {
            public string Thing1 { get; set; }

            public int Thing2 { get; set; }
        }

        public class MyImplementerBase : fFastImplementInterface.fFastSelfNotifyingBase
        {
            public override bool OnValueSetting<T>(string propertyName, T newValue)
            {
                if (propertyName == "Thing1")
                {
                    var str = newValue as string;
                    if (str == "abort")
                    {
                        return fFastImplementInterfaceConstants.AbortPropertySave;
                    }
                }
                return base.OnValueSetting<T>(propertyName, newValue);
            }
        }

        public class ImplementerBaseEmpty { }
        public class ImplementerBaseSetSetting
        {
            public string LastOnValueSettingCalledForProperty { get; set; }
            public string LastOnValueSetCalledForProperty { get; set; }

            public void OnValueSetting(string propertyName)
            {
                LastOnValueSettingCalledForProperty = propertyName;
            }

            public void OnValueSet(string propertyName)
            {
                LastOnValueSetCalledForProperty = propertyName;
            }
        }

        public class ImplementerBaseSetSettingPlusAbort
        {
            public string LastOnValueSettingCalledForProperty { get; private set; }
            public string LastOnValueSetCalledForProperty { get; private set; }

            public bool AbortReturnValue { private get; set; }

            public bool OnValueSetting(string propertyName)
            {
                LastOnValueSettingCalledForProperty = propertyName;
                return AbortReturnValue;
            }

            public void OnValueSet(string propertyName)
            {
                LastOnValueSetCalledForProperty = propertyName;
            }
        }

        public interface ITestfFastImplementerComplex
        {
            string this[int index]
            {
                get;
                set;
            }

            event EventHandler SoThisHappened;

            string MyProperty { get; set; }

            DateTime MyMethod(string thing1, int thing2);
        }

        public interface ITestfFastImplementerPropertyOnly
        {
            string Thing1 { get; set; }
            int Thing2 { get; set; }
        }

        [TestMethod]
        public void fFastImplementer_TestFull()
        {
            Exception exception = null;
            try
            {
                var x = fFastImplementInterface.CreateNew<ITestfFastImplementerComplex>();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            //x.MyProperty = "some prop";
            //x[12344] = "ikdid";
            //var z = x[12344];
            //x.MyMethod("first", 2);
            //x.SoThisHappened += x_SoThisHappened;
        }

        void x_SoThisHappened(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
