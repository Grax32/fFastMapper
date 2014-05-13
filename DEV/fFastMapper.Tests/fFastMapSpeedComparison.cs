using Grax.fFastMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace fFastMapper.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class fFastMapSpeedComparison
    {
        [TestMethod]
        public void TestUsage()
        {
            n1 source = new n1();
            n2 dest = new n2();

            fFastMap.Map(source, dest);
            fFastMap.MapperFor<n1, n2>().Map(source, dest);

            fFastMap.AutoInitialize = false;
            fFastMap.DefaultMappingDirection = fFastMap.MappingDirection.Bidirectional;
            fFastMap
                .GlobalSettings()
                .SetAutoInitialize(false)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.LeftToRight);

            fFastMap
                .MapperFor<n1, n2>()
                .SetMappingDirection(fFastMap.MappingDirection.LeftToRight);


            fFastMap
                .MapperFor<n1, n2>()
                .ClearMappers()
                .AddDefaultPropertyMappers()
                .DeletePropertyMapper(left => left.Id)
                .AddPropertyMapper(left => left.Id, right => right.Id);

            var z = fFastMap.MapperFor<n1, n2>().Map(new n1 { Id = 45 });

            fFastMap.MapperFor<n1, n2>().MappingView();

        }

        [TestMethod]
        public void TestNested()
        {
            var mapper = fFastMap
                .MapperFor<n1, n2>()
                .AddPropertyMapper(v => v.MultiLevelLeft.L2.Id, v => v.MultiLevelRight.L2.Id)
                .DeletePropertyMapper(v => v.MultiLevelLeft)
                .DeletePropertyMapper(v => v.MultiLevelLeft.L2.Id);

            var source = new n1
            {
                Id = 4,
                Description = "DK",
                Url = "kid",
                RowVersion = new byte[] { },
                UpdatedBy = 34,
                UpdatedOn = DateTime.Now,
                CreatedBy = 43,
                CreatedOn = DateTime.Now,
                MultiLevelLeft = new nMultiLevel { L2 = new nLevelTwo { Id = 47 } }
            };

            var dest = new n2 { MultiLevelRight = new nMultiLevelZ { L2 = new nLevelTwoZ() } };

            var result = mapper.Map(source, dest);

            Assert.IsNull(result.MultiLevelLeft);
            Assert.AreEqual(default(int), result.MultiLevelRight.L2.Id);
        }

//        [TestMethod]
        public void CompareSpeed()
        {

            var mapper = fFastMap.MapperFor<c1, c2>()
                .AddPropertyMapper(v => v.CreatedBy, v => v.CreationInfo.CreatedBy)
                .AddPropertyMapper(v => v.CreatedOn, v => v.CreationInfo.CreatedOn);

            var source = new c1
            {
                Id = 4,
                Description = "DK",
                Url = "kid",
                RowVersion = new byte[] { },
                UpdatedBy = 34,
                UpdatedOn = DateTime.Now,
                CreatedBy = 43,
                CreatedOn = DateTime.Now
            };


            var z = mapper.Map(source);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var x = mapper.Map(source);
            }

            Debug.Print("Took {0} ticks (new() in extension method)", stopwatch.ElapsedTicks);

            var convertFunction = mapper.GetMapFunction();
            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                var x = convertFunction(source, new c2());
            }
            Debug.Print("Took {0} ticks (new() passed in, function before loop)", stopwatch.ElapsedTicks);

            var dest = new c2();

            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                var x = mapper.Map(source, dest);
            }

            Debug.Print("Took {0} ticks (new() before loop)", stopwatch.ElapsedTicks);

            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                var x = convertFunction(source, dest);
            }
            Debug.Print("Took {0} ticks (new() before loop, function before loop)", stopwatch.ElapsedTicks);

            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                var x = Convert(source, new c2());
            }

            Debug.Print("Took {0} ticks (static function)", stopwatch.ElapsedTicks);
        }

        public static c2 Convert(c1 left, c2 right)
        {
            right.Id = left.Id;
            right.Description = left.Description;
            right.Url = left.Url;
            right.RowVersion = left.RowVersion;
            right.UpdatedBy = left.UpdatedBy;
            right.UpdatedOn = left.UpdatedOn;
            right.CreationInfo.CreatedBy = left.CreatedBy;
            right.CreationInfo.CreatedOn = left.CreatedOn;
            return right;
        }

        public class c1
        {
            public int Id { get; set; }

            public string Description { get; set; }

            public string Url { get; set; }

            public byte[] RowVersion { get; set; }

            public int UpdatedBy { get; set; }
            public DateTime UpdatedOn { get; set; }

            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class c2
        {
            public c2()
            {
                CreationInfo = new cx();
            }

            public int Id { get; set; }

            public string Description { get; set; }

            public string Url { get; set; }

            public byte[] RowVersion { get; set; }

            public int UpdatedBy { get; set; }
            public DateTime UpdatedOn { get; set; }

            public cx CreationInfo { get; set; }
        }

        public class cx
        {
            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class nCreationInfo
        {
            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class nMultiLevel
        {
            public string XX { get; set; }
            public nLevelTwo L2 { get; set; }

        }

        public class nLevelTwo
        {
            public int Id { get; set; }
            public string YY { get; set; }
        }

        public class nMultiLevelZ
        {
            public string XX { get; set; }
            public nLevelTwoZ L2 { get; set; }

        }

        public class nLevelTwoZ
        {
            public int Id { get; set; }
            public string YY { get; set; }
        }

        public class n1
        {
            public int Id { get; set; }

            public string Description { get; set; }

            public string Url { get; set; }

            public byte[] RowVersion { get; set; }

            public int UpdatedBy { get; set; }
            public DateTime UpdatedOn { get; set; }

            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }

            public nCreationInfo CreationInfo { get; set; }

            public nMultiLevel MultiLevelLeft { get; set; }
        }

        public class n2
        {
            public int Id { get; set; }

            public string Description { get; set; }

            public string Url { get; set; }

            public byte[] RowVersion { get; set; }

            public int UpdatedBy { get; set; }
            public DateTime UpdatedOn { get; set; }

            public int CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }

            public nCreationInfo CreationInfo { get; set; }

            public nMultiLevel MultiLevelLeft { get; set; }
            public nMultiLevelZ MultiLevelRight { get; set; }
        }
    }
}
