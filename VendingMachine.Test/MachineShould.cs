using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Helpers;
using VendingMachine.Models;

namespace VendingMachine.Test
{
    [TestFixture]
    public class MachineShould
    {
        private IProduct _product;
        private ICoin _coin;
        private ICoinHelper _coinHelper;
        private IMachine _machine;

        [SetUp]
        public void Setup()
        {
            _product = MockRepository.GenerateStub<IProduct>();
            _coin = MockRepository.GenerateStub<ICoin>();
            _coinHelper = MockRepository.GenerateStub<ICoinHelper>();

            _machine = new Machine(_coinHelper);
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

            _coinHelper.AssertWasCalled(c => c.CoinValid(_coin));
        }

        [Test]
        public void GetCoinValue()
        {
            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            _machine.InsertCoin(_coin);

            _coinHelper.AssertWasCalled(c => c.CoinValueByWeight(_coin));
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

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.10M).Repeat.Once();
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.05M).Repeat.Once();

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

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true).Repeat.Times(3);
            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(false).Repeat.Once();

            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.01M).Repeat.Once();

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

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Matches(g => g == goodCoin))).Return(true);
            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Matches(b => b == badCoin))).Return(false);

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
            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.DispensedProduct.Should().Be(_product);
        }

        [Test]
        public void DisplayThankYouMessageWhenProductDispensed()
        {
            var expected = "THANK YOU";

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayInsertCoinMessageAfterCheckingThankYouMessageWhenProductDispensed()
        {
            var expected = "INSERT COIN";

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

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

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

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

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.50M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().NotBe(expected);
            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DispenseSelectedProductAfterAppropreateCoinsAdded()
        {
            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.25M;

            _machine.SelectProduct(_product);
            _machine.InsertCoin(_coin);

            _machine.DispensedProduct.Should().Be(_product);
        }

        [Test]
        public void GetChange()
        {
            var expectedChange = 0.15M;

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.40M);

            _coinHelper.Stub(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange))).Return(new List<ICoin>());

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _coinHelper.AssertWasCalled(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange)));
        }

        [Test]
        public void NotGetChangeOnFullAmount()
        {
            var expectedChange = 0.00M;

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _coinHelper.Stub(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange))).Return(new List<ICoin>());

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _coinHelper.AssertWasNotCalled(c => c.DistributeChange(Arg<decimal>.Matches(d => d == expectedChange)));
        }

        [Test]
        public void DispenseChange()
        {
            var expected = new List<ICoin> { new Dime(), new Nickel() };

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.40M);

            _coinHelper.Stub(c => c.DistributeChange(Arg<decimal>.Is.Anything)).Return(expected);

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

            _coinHelper.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinHelper.Stub(c => c.CoinValueByWeight(Arg<ICoin>.Is.Anything)).Return(0.05M);

            var expected = new List<ICoin> { coin1, coin2, coin3 };

            _machine.InsertCoin(coin1);
            _machine.InsertCoin(coin2);
            _machine.InsertCoin(coin3);

            _machine.ReturnCoins();

            _machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }
    }
}