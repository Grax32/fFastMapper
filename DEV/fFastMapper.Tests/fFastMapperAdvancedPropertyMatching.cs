using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Grax.fFastMapper;
using System.Diagnostics;

namespace fFastMapper.Tests
{
    [TestClass]
    public class fFastMapperAdvancedPropertyMatching
    {
        //[TestMethod]
        //public void FindMatchesTest()
        //{
        //    //PropertyList<Left>.InitializeProperties();
        //    //PropertyList<Right>.InitializeProperties();

        //    var leftList = PropertyList<Left>.Properties;
        //    var rightList = PropertyList<Right>.Properties;

        //    var keyList = leftList.Keys.Intersect(rightList.Keys);

        //    foreach (var key in keyList)
        //    {
        //        Debug.Print("{0} == {1}", leftList[key].ToString(), rightList[key].ToString());
        //    }

        //    var start = new Left
        //    {
        //        SubParentSubSubSubMyString = "12345",
        //        SubParent = new SubType()
        //    };

        //    var result = new Right();
        //    fFastMap.Map<Left, Right>(new Left(), result);
        //}

        class Left
        {
            public string SubParentSubSubSubMyString { get; set; }
            public SubType SubParent { get; set; }
        }

        class Right
        {
            public SubType SubParent { get; set; }
        }

        class SubType
        {
            public SubType Sub { get; set; }

            public string MyString { get; set; }
        }


        [TestMethod]
        public void fFastMapper_AdvancedPropertyMatching()
        {
            fFastMap
                .GlobalSettings()
                .SetAutoInitialize(true)
                .SetDefaultMappingDirection(fFastMap.MappingDirection.Bidirectional);

            var left = new Invoice
            {
                Person = new Person
                {
                    Address = new Address { Street1 = "Person.Address.Street1" },
                    AddressStreet1 = "Person.AddressStreet1"
                },
                PersonAddress = new Address { Street1 = "PersonAddress.Street1" }
            };

            var right = fFastMap.MapperFor<Invoice, ModelView>().Map(left);

            Assert.IsTrue(left.Person.AddressStreet1 == right.PersonAddressStreet1);
            Assert.IsFalse(left.Person.Address.Street1 == right.PersonAddressStreet1);
            Assert.IsFalse(left.PersonAddress.Street1 == right.PersonAddressStreet1);


        }

        class Invoice
        {
            public Person Person { get; set; }

            public Address PersonAddress { get; set; }
        }

        class Person
        {
            public Address Address { get; set; }

            public string AddressStreet1 { get; set; }
        }

        class Address
        {
            public string Street1 { get; set; }
        }

        class ModelView
        {
            public string PersonAddressStreet1 { get; set; }
        }
    }
}
