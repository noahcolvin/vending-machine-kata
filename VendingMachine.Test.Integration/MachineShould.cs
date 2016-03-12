using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Managers;
using VendingMachine.Models;

namespace VendingMachine.Test.Integration
{
    [TestFixture]
    public class MachineShould
    {
        private IProduct _product;
        private ICoin _coin;
        private ICoinManager _coinManager;
        private IProductManager _productManager;
        private IBankManager _bankManager;
        private readonly decimal DefaultBank = 100M;

        [SetUp]
        public void Setup()
        {
            _product = MockRepository.GenerateStub<IProduct>();
            _coin = MockRepository.GenerateStub<ICoin>();
            _coinManager = MockRepository.GenerateStub<ICoinManager>();
            _productManager = MockRepository.GenerateStub<IProductManager>();
            _bankManager = MockRepository.GenerateStub<IBankManager>();
        }

        [Test]
        public void ReturnInvalidCoins()
        {
            var coinManager = new CoinManager();
            var machine = new Machine(coinManager, _productManager, _bankManager);

            _coin.Size = CoinSize.Medium;

            machine.InsertCoin(_coin);

            machine.CoinReturn.Count.Should().Be(1);
            machine.CoinReturn.First().Should().Be(_coin);
        }

        [Test]
        public void SetCorrectCoinValue()
        {
            var expected = 0.25M;

            var coinManager = new CoinManager();
            var machine = new Machine(coinManager, _productManager, _bankManager);

            _coin.Size = CoinSize.XLarge;

            machine.InsertCoin(_coin);

            _coin.Value.Should().Be(expected);
        }

        [Test]
        public void CorrectlyDistributeChangeWhenVendingProduct()
        {
            var expected = new List<ICoin> { new Quarter(), new Dime(), new Nickel() };

            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);
            _product.Price = 0.05M;

            var coinManager = new CoinManager();
            var machine = new Machine(coinManager, _productManager, _bankManager);

            machine.InsertCoin(new Quarter());
            machine.InsertCoin(new Dime());
            machine.InsertCoin(new Nickel());
            machine.InsertCoin(new Nickel());

            machine.SelectProduct(_product);

            machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void ShowWhenProductNotInStock()
        {
            var expected = "SOLD OUT";

            var productManager = new ProductManager();
            var machine = new Machine(_coinManager, productManager, _bankManager);

            machine.SelectProduct(_product);

            machine.Display.Should().Be(expected);
        }

        [Test]
        public void RemoveProductFromInventoryWhenVending()
        {
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.10M);

            var productManager = new ProductManager();
            productManager.AddStock(_product, 1);

            _product.Price = 0.10M;

            var machine = new Machine(_coinManager, productManager, _bankManager);
            machine.InsertCoin(_coin);
            machine.SelectProduct(_product);

            productManager.ProductAvailable(_product).Should().BeFalse();
        }

        [Test]
        public void RequestExactChangeWhenAnyAvailableProductPriceGreaterThanBank()
        {
            var expected = "EXACT CHANGE ONLY";
            
            _bankManager.Amount = 0.10M;

            var otherProduct = MockRepository.GenerateStub<IProduct>();
            otherProduct.Price = 0.25M;

            _product.Price = 0.10M;

            var productManager = new ProductManager();
            productManager.AddStock(_product, 1);
            productManager.AddStock(otherProduct, 1);
            
            var machine = new Machine(_coinManager, productManager, _bankManager);

            machine.Display.Should().Be(expected);
        }

        [Test]
        public void AddMoneyToBankWhenProductVended()
        {
            var expected = 0.25M;

            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.50M);
            _coinManager.Stub(c => c.DistributeChange(Arg<decimal>.Is.Anything)).Return(new List<ICoin>());

            _product.Price = 0.25M;

            var bankManager = new BankManager();
            var machine = new Machine(_coinManager, _productManager, bankManager);
            
            machine.InsertCoin(_coin);
            machine.SelectProduct(_product);

            bankManager.Amount.Should().Be(expected);
        }
    }
}
