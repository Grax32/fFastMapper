using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grax.fFastMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fFastMapper.Tests
{


    /// <summary>
    ///This is a test class for fFastMapperFluentTest and is intended
    ///to contain all fFastMapperFluentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class fFastMapperFluentTest
    {

        [TestMethod()]
        public void fFastMapperFluent_ConstructorTest()
        {
            // act
            fFastMapperFluent<GenericParameterHelper, GenericParameterHelper> target = new fFastMapperFluent<GenericParameterHelper, GenericParameterHelper>();

            // assert
            Assert.IsNotNull(target);
        }

        [TestMethod()]
        public void fFastMapperFluent_AddDefaultPropertyMappers()
        {
            // arrange
            fFastMap.GlobalSettings()
                .SetAutoInitialize(false)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);

            fFastMapperFluent<AddDefaultPropertyMappersLeft, AddDefaultPropertyMappersRight> target = fFastMap.MapperFor<AddDefaultPropertyMappersLeft, AddDefaultPropertyMappersRight>();

            // act
            var actual = target
                .ClearMappers()
                .AddDefaultPropertyMappers();

            // assert
            Assert.AreEqual(target, actual);

            // forward mappings
            //Assert.AreEqual(2, actual.Mappings().Count);
            //Assert.AreEqual("Id", actual.Mappings().First().Item1.Name);
            //Assert.AreEqual("Id", actual.Mappings().First().Item2.Name);

            //Assert.AreEqual("Description", actual.Mappings()[1].Item1.Name);
            //Assert.AreEqual("Description", actual.Mappings()[1].Item2.Name);

            //// reverse mappings
            //Assert.AreEqual(2, actual.Reverse().Mappings().Count);
            //Assert.AreEqual("Id", actual.Reverse().Mappings().First().Item1.Name);
            //Assert.AreEqual("Id", actual.Reverse().Mappings().First().Item2.Name);

            //Assert.AreEqual("Description", actual.Reverse().Mappings()[1].Item1.Name);
            //Assert.AreEqual("Description", actual.Reverse().Mappings()[1].Item2.Name);
        }

        class AddDefaultPropertyMappersLeft
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        class AddDefaultPropertyMappersRight
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        [TestMethod()]
        public void fFastMapperFluent_AddPropertyMapper_DefaultBidirectional()
        {
            // arrange
            fFastMap.GlobalSettings()
                .SetAutoInitialize(false)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);

            var target = fFastMap.MapperFor<AddPropertyMappersLeft, AddPropertyMappersRight>();

            // act
            var actual = target.AddPropertyMapper(v => v.Id, v => v.Id);

            // assert
            Assert.AreEqual(1, actual.Mappings().Count);
            Assert.AreEqual(1, actual.Reverse().Mappings().Count);
            //Assert.AreEqual("Id", actual.Mappings().First().Item1.Name);
            //Assert.AreEqual("Id", actual.Reverse().Mappings().First().Item1.Name);
        }

        class AddPropertyMappersLeft
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        class AddPropertyMappersRight
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }


        [TestMethod()]
        public void fFastMapperFluent_AddPropertyMapper_DefaultLeftToRight()
        {
            // arrange
            fFastMap.GlobalSettings()
                .SetAutoInitialize(false)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.LeftToRight);

            var target = fFastMap.MapperFor<AddPropertyMappersLeft2, AddPropertyMappersRight2>();

            // act
            var actual = target.AddPropertyMapper(v => v.Id, v => v.Id);

            // assert
            Assert.AreEqual(1, actual.Mappings().Count);
            Assert.AreEqual(0, actual.Reverse().Mappings().Count);
        }

        class AddPropertyMappersLeft2
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        class AddPropertyMappersRight2
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        [TestMethod()]
        public void fFastMapperFluent_AddPropertyMapper_LeftToRightWithDefaultBidirectional()
        {
            // arrange
            fFastMap.GlobalSettings()
                .SetAutoInitialize(false)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);

            var target = fFastMap
                .MapperFor<AddPropertyMappersLeft3, AddPropertyMappersRight3>()
                .SetMappingDirection(fFastMap.MappingDirection.LeftToRight);

            // act
            var actual = target.AddPropertyMapper(v => v.Id, v => v.Id);

            // assert
            Assert.AreEqual(1, actual.Mappings().Count);
            Assert.AreEqual(0, actual.Reverse().Mappings().Count);
        }

        class AddPropertyMappersLeft3
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        class AddPropertyMappersRight3
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        ///A test for ClearMappers
        ///</summary>
        public void fFastMapperFluent_ClearMappers()
        {
            fFastMapperFluent<ClearMappersLeft, ClearMappersRight> target = new fFastMapperFluent<ClearMappersLeft, ClearMappersRight>(); // TODO: Initialize to an appropriate value
            fFastMapperFluent<ClearMappersLeft, ClearMappersRight> expected = null; // TODO: Initialize to an appropriate value
            fFastMapperFluent<ClearMappersLeft, ClearMappersRight> actual;
            actual = target.ClearMappers();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        class ClearMappersLeft
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        class ClearMappersRight
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }


        /// <summary>
        ///A test for Map
        ///</summary>
        public void MapTest1Helper<TLeft, TRight>()
        {
            fFastMapperFluent<TLeft, TRight> target = new fFastMapperFluent<TLeft, TRight>(); // TODO: Initialize to an appropriate value
            TLeft source = default(TLeft); // TODO: Initialize to an appropriate value
            TRight destination = default(TRight); // TODO: Initialize to an appropriate value
            TRight expected = default(TRight); // TODO: Initialize to an appropriate value
            TRight actual;
            actual = target.Map(source, destination);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void MapTest1()
        {
            MapTest1Helper<GenericParameterHelper, GenericParameterHelper>();
        }

        /// <summary>
        ///A test for MappingView
        ///</summary>
        [TestMethod()]
        public void fFastMapper_MappingView()
        {
            fFastMap
                .GlobalSettings()
                .SetAutoInitialize(true)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);

            var target = new fFastMapperFluent<MappingViewLeft, MappingViewRight>();

            var actual = target.MappingView();

            Assert.IsTrue(actual.Contains("Property MappingViewLeft.Id maps to property MappingViewRight.Id"));
        }

        class MappingViewLeft { public int Id { get; set; } }
        class MappingViewRight { public int Id { get; set; } }

        [TestMethod()]
        public void fFastMapper_Mappings()
        {
            fFastMap
                .GlobalSettings()
                .SetAutoInitialize(true)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);

            var target = new fFastMapperFluent<MappingLeft, MappingRight>();

            var actual = target.Mappings();

            Assert.IsTrue(actual.Any(v => v.Item1 == "MappingLeft.Id" && v.Item2 == "MappingRight.Id"));
        }

        class MappingLeft { public int Id { get; set; } }
        class MappingRight { public int Id { get; set; } }
    }
}
