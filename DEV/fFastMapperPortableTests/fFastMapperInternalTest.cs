using Grax.fFastMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace fFastMapperPortableTests
{
    /// <summary>
    ///This is a test class for fFastMapperInternalTest and is intended
    ///to contain all fFastMapperInternalTest Unit Tests
    ///</summary>
    [TestClass()]
    public class fFastMapperInternalTest
    {
        class fFastMapper_Left { public int Id { get; set; } }
        class fFastMapper_Right { public int Id { get; set; } }

        [TestMethod()]
        public void fFastMapperInternal_AddPropertyMapper_ByPropertyInfo()
        {
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.ClearMappers(true);
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.MappingDirection = fFastMap.MappingDirection.Bidirectional;

            PropertyInfo leftProperty = typeof(fFastMapper_Left).GetProperty("Id");
            PropertyInfo rightProperty = typeof(fFastMapper_Right).GetProperty("Id");
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.AddPropertyMapperByPropertyInfo(leftProperty, rightProperty);

            Assert.AreEqual(1, fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Count);
            Assert.IsTrue(fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Any(v => v.Item1 == "fFastMapper_Left.Id" && v.Item2 == "fFastMapper_Right.Id"));
        }


        [TestMethod()]
        public void fFastMapperInternal_AddPropertyMapper_ByExpression()
        {
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.ClearMappers(true);
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.MappingDirection = fFastMap.MappingDirection.Bidirectional;

            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.AddPropertyMapperByExpression(v => v.Id, v => v.Id);

            Assert.AreEqual(1, fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Count);
            Assert.IsTrue(fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Any(v => v.Item1 == "fFastMapper_Left.Id" && v.Item2 == "fFastMapper_Right.Id"));
        }

        [TestMethod]
        public void fFastMapperInternal_AddPropertyMapper_ByExpression_Inner()
        {
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.ClearMappers(true);
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.MappingDirection = fFastMap.MappingDirection.Bidirectional;

            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.AddPropertyMapperByExpression(v => v.Id, v => v.Id, fFastMap.CallReverseFalse);

            Assert.AreEqual(1, fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Count);
            Assert.IsTrue(fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Any(v => v.Item1 == "fFastMapper_Left.Id" && v.Item2 == "fFastMapper_Right.Id"));
        }

        [TestMethod()]
        public void fFastMapperInternal_AddPropertyMappingByMatchedNameAndTypeTest()
        {
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.ClearMappers(true);
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.MappingDirection = fFastMap.MappingDirection.Bidirectional;

            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.AddDefaultMappings(fFastMap.CallReverseFalse, false);

            Assert.AreEqual(1, fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Count);
            Assert.IsTrue(fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.Mappings().Any(v => v.Item1 == "fFastMapper_Left.Id" && v.Item2 == "fFastMapper_Right.Id"));
        }

        [TestMethod()]
        public void fFastMapperInternal_CompileMapper()
        {
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.ClearMappers(true);
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.MappingDirection = fFastMap.MappingDirection.Bidirectional;

            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.mapperFunc = null;
            fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.CompileMapper();

            Assert.IsNotNull(fFastMapperInternal<fFastMapper_Left, fFastMapper_Right>.mapperFunc);
        }

        [TestMethod()]
        public void fFastMapperInternal_Mappings()
        {
            fFastMap.MapperFor<Mappings_Left, Mappings_Right>()
                .AddPropertyMapper(v => v.Sub.Sub.Description, v => v.Sub.Sub.Sub.Description);
            var actual = fFastMapperInternal<Mappings_Left, Mappings_Right>.Mappings();

            Assert.IsTrue(actual.Any(v => v.Item1 == "Mappings_Left.Id" && v.Item2 == "Mappings_Right.Id"));
            Assert.IsTrue(actual.Any(v => v.Item1 == "Mappings_Left.Description" && v.Item2 == "Mappings_Right.Description"));
            Assert.IsTrue(actual.Any(v => v.Item1 == "Mappings_Left.Sub.Sub.Description" && v.Item2 == "Mappings_Right.Sub.Sub.Sub.Description"));
        }

        class MappingsSub
        {
            public MappingsSub Sub { get; set; }
            public string Description { get; set; }
        }

        class Mappings_Left
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public MappingsSub Sub { get; set; }
        }

        class Mappings_Right
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public MappingsSub Sub { get; set; }
        }

        [TestMethod()]
        public void fFastMapperInternal_MappingsView()
        {
            fFastMap.MapperFor<MappingsView_Left, MappingsView_Right>()
                .AddPropertyMapper(v => v.Sub.Sub.Description, v => v.Sub.Sub.Sub.Description);
            var actual = fFastMapperInternal<MappingsView_Left, MappingsView_Right>.MappingsView();

            Assert.IsTrue(actual.Contains("MappingsView_Left.Sub.Sub.Description"));
        }

        class MappingsViewSub
        {
            public MappingsViewSub Sub { get; set; }
            public string Description { get; set; }
        }

        class MappingsView_Left
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public MappingsViewSub Sub { get; set; }
        }

        class MappingsView_Right
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public MappingsViewSub Sub { get; set; }
        }

        [TestMethod()]
        public void fFastMapper_NullCheckExpressionTest()
        {
            var actual = fFastMapperInternal<fluentLeft, fluentRight>.NullCheckExpression(Expression.Parameter(typeof(fluentLeft),"value"));

            Assert.IsTrue(actual.ToString().Contains("value == null"));
        }


        [TestMethod()]
        public void fFastMapper_fFastMapFluentTest()
        {
            var actual = fFastMapperInternal<fluentLeft, fluentRight>.fFastMapFluent;

            Assert.IsInstanceOfType(actual, typeof(fFastMapperFluent<fluentLeft, fluentRight>));
        }

        class fluentLeft { }
        class fluentRight { }
    }
}
