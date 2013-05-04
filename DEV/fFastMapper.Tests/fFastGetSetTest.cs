using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Grax.fFastMapper;
using fFastMapper.Tests.Library;

namespace fFastMapper.Tests
{
    [TestClass]
    public class fFastGetSetTest
    {
        [TestMethod]
        public void fFastMapper_MapAnonymousType()
        {
            Functions.ResetGlobalSettings();
            var anonLeft = new { Id = 47, DryerSheets = true };
            var dest = new AnonDest();
            var mapper = fFastMap.MapperFor(anonLeft, dest);
            mapper.AddPropertyMapper(v => v.Id, v => v.Id, true);

            mapper.Map(anonLeft, dest);

            Assert.AreEqual(47, dest.Id);
            Assert.AreEqual(true, dest.DryerSheets);
        }

        class AnonDest
        {
            public int Id { get; set; }
            public bool DryerSheets { get; set; }
        }

        [TestMethod]
        public void fFastMapper_GetOnlyTest()
        {
            Functions.ResetGlobalSettings();
            var mapper = fFastMap.MapperFor<GetOnly, GetSet>();

            var result = mapper.Map(new GetOnly(), new GetSet());
            var result2 = mapper.Reverse().Map(new GetSet(), new GetOnly());
        }

        [TestMethod]
        public void fFastMapper_SetOnlyTest()
        {
            Functions.ResetGlobalSettings();
            var mapper = fFastMap.MapperFor<SetOnly, GetSet>();

            var result = mapper.Map(new SetOnly(), new GetSet());
            var result2 = mapper.Reverse().Map(new GetSet(), new SetOnly());
        }

        class GetOnly
        {
            public int Id { get { return 48; } }
            public string Description { get { return "How-Dee"; } }
        }

        class SetOnly
        {
            int _id;
            string _description;
            public int Id { set { _id = value; } }
            public string Description { set { _description = value; } }
        }

        class GetSet
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
