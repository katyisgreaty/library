using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Xunit;
using System.Data;

namespace Library
{
    public class CheckoutTest : IDisposable
    {
        public CheckoutTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void Checkout_EmptyOnLoad_0()
        {
            Assert.Equal(0, Checkout.GetAll().Count);
        }
        [Fact]
        public void Checkout_EqualityTest_1()
        {
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            Checkout secondCheckout = new Checkout("2017-05-1", 1, 1);

            Assert.Equal(newCheckout, secondCheckout);
        }
        [Fact]
        public void Save_SavesCheckoutToDatabase_2()
        {
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            newCheckout.Save();

            List<Checkout> expectedList = new List<Checkout> {newCheckout};
            List<Checkout> actualList = Checkout.GetAll();

            Assert.Equal(expectedList, actualList);
        }

        [Fact]
        public void Checkout_Find_3()
        {
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            newCheckout.Save();

            Checkout foundCheckout = Checkout.Find(newCheckout.GetId());

            Assert.Equal(newCheckout, foundCheckout);
        }

        [Fact]
        public void Checkout_Delete_RemoveFromDatabase_4()
        {
            Checkout newCheckout = new Checkout("2017-05-1", 1, 1);
            newCheckout.Save();

            Checkout.Delete(newCheckout.GetId());

            Assert.Equal(0, Checkout.GetAll().Count);
        }

        public void Dispose()
        {
            Checkout.DeleteAll();

        }
    }
}
