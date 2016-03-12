using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Managers;
using VendingMachine.Models;

namespace VendingMachine.Test
{
    [TestFixture]
    public class MachineShould
    {
        private IProduct _product;
        private ICoin _coin;
        private ICoinManager _coinManager;
        private IMachine _machine;
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
            _productManager.Stub(p => p.Inventory).Return(new Dictionary<IProduct, int>());
            _productManager.Inventory.Add(_product, 1);

            _bankManager = MockRepository.GenerateStub<IBankManager>();
            _bankManager.Amount = DefaultBank;

            _machine = new Machine(_coinManager, _productManager, _bankManager);
        }

        [Test]
        public void DefaultDisplayToInsertCoin()
        {
            var expected = "INSERT COIN";

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void ValidateCoin()
        {
            _machine.InsertCoin(_coin);

            _coinManager.AssertWasCalled(c => c.CoinValid(_coin));
        }

        [Test]
        public void GetCoinValue()
        {
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            _machine.InsertCoin(_coin);

            _coinManager.AssertWasCalled(c => c.CoinValueByWeight(_coin));
        }

        [Test]
        public void DisplayTotalOfEnteredCoins()
        {
            var expected = "$0.90";

            var coin1 = MockRepository.GenerateStub<ICoin>();
            var coin2 = MockRepository.GenerateStub<ICoin>();
            var coin3 = MockRepository.GenerateStub<ICoin>();
            var coin4 = MockRepository.GenerateStub<ICoin>();
            var coin5 = MockRepository.GenerateStub<ICoin>();

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.10M).Repeat.Once();
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.05M).Repeat.Once();

            _machine.InsertCoin(coin1);
            _machine.InsertCoin(coin2);
            _machine.InsertCoin(coin3);
            _machine.InsertCoin(coin4);
            _machine.InsertCoin(coin5);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void NotAddInvalidCoins()
        {
            var expected = "$0.75";

            var coin1 = MockRepository.GenerateStub<ICoin>();
            var coin2 = MockRepository.GenerateStub<ICoin>();
            var coin3 = MockRepository.GenerateStub<ICoin>();
            var coin4 = MockRepository.GenerateStub<ICoin>();

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true).Repeat.Times(3);
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(false).Repeat.Once();

            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.01M).Repeat.Once();

            _machine.InsertCoin(coin1);
            _machine.InsertCoin(coin2);
            _machine.InsertCoin(coin3);
            _machine.InsertCoin(coin4);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DefaultToEmptyCoinReturn()
        {
            var expected = new List<ICoin>();

            _machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void AddInvalidCoinsToCoinReturn()
        {
            var goodCoin = MockRepository.GenerateStub<ICoin>();
            var badCoin = MockRepository.GenerateStub<ICoin>();

            var expected = new List<ICoin> { badCoin };

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Matches(g => g == goodCoin))).Return(true);
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Matches(b => b == badCoin))).Return(false);

            _machine.InsertCoin(goodCoin);
            _machine.InsertCoin(badCoin);

            _machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void DefaultToNoDispensedProduct()
        {
            _machine.DispensedProduct.Should().BeNull();
        }

        [Test]
        public void DispenseProductWhenSelectedAndEnoughMoneyInserted()
        {
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.DispensedProduct.Should().Be(_product);
        }

        [Test]
        public void DisplayThankYouMessageWhenProductDispensed()
        {
            var expected = "THANK YOU";

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayInsertCoinMessageAfterCheckingThankYouMessageWhenProductDispensed()
        {
            var expected = "INSERT COIN";

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().NotBe(expected);
            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayPriceMessageWhenProductSelectedWithNotEnoughMoneyInserted()
        {
            var expected = "PRICE $0.50";

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _product.Price = 0.50M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayInsertCoinMessageAfterPriceMessageWhenProductSelectedWithNoMoneyInserted()
        {
            var expected = "INSERT COIN";

            _product.Price = 0.50M;

            _machine.SelectProduct(_product);

            _machine.Display.Should().NotBe(expected);
            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayCurrentAmountMessageAfterPriceMessageWhenProductSelectedWithNoMoneyInserted()
        {
            var expected = "$0.25";

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.50M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().NotBe(expected);
            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DispenseSelectedProductAfterAppropreateCoinsAdded()
        {
            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _product.Price = 0.25M;

            _machine.SelectProduct(_product);
            _machine.InsertCoin(_coin);

            _machine.DispensedProduct.Should().Be(_product);
        }

        [Test]
        public void GetChange()
        {
            var expectedChange = 0.15M;

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.40M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _coinManager.Stub(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange))).Return(new List<ICoin>());

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _coinManager.AssertWasCalled(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange)));
        }

        [Test]
        public void NotGetChangeOnFullAmount()
        {
            var expectedChange = 0.00M;

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _coinManager.Stub(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange))).Return(new List<ICoin>());

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _coinManager.AssertWasNotCalled(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange)));
        }

        [Test]
        public void DispenseChange()
        {
            var expected = new List<ICoin> { new Dime(), new Nickel() };

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.40M);
            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(true);

            _coinManager.Stub(c => c.DistributeChange(Arg<decimal>.Is.Anything)).Return(expected);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void ReturnSameCoinsEntered()
        {
            var coin1 = MockRepository.GenerateStub<ICoin>();
            var coin2 = MockRepository.GenerateStub<ICoin>();
            var coin3 = MockRepository.GenerateStub<ICoin>();

            _coinManager.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinManager.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.05M);

            var expected = new List<ICoin> { coin1, coin2, coin3 };

            _machine.InsertCoin(coin1);
            _machine.InsertCoin(coin2);
            _machine.InsertCoin(coin3);

            _machine.ReturnCoins();

            _machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void DisplaySoldOutMessageForSoldOutProduct()
        {
            var expected = "SOLD OUT";

            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(false);

            _machine.SelectProduct(_product);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayInsertCoinMessageAfterSoldOutMessageForSoldOutProduct()
        {
            var expected = "INSERT COIN";

            _productManager.Stub(p => p.ProductAvailable(Arg<IProduct>.Is.Anything)).Return(false);

            _machine.SelectProduct(_product);

            _machine.Display.Should().NotBe(expected);
            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayExactChangeOnlyWhenNoChangeAvailable()
        {
            var expected = "EXACT CHANGE ONLY";

            _product.Price = 0.5M;

            _bankManager.Amount = 0M;

            _machine.Display.Should().Be(expected);
        }
    }
}