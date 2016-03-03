using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Models;
using VendingMachine.Validators;

namespace VendingMachine.Test
{
    [TestFixture]
    public class MachineShould
    {
        private IProduct _product;
        private ICoin _coin;
        private ICoinValidator _coinValidator;
        private IMachine _machine;

        [SetUp]
        public void Setup()
        {
            _product = MockRepository.GenerateStub<IProduct>();
            _coin = MockRepository.GenerateStub<ICoin>();
            _coinValidator = MockRepository.GenerateStub<ICoinValidator>();

            _machine = new Machine(_coinValidator);
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

            _coinValidator.AssertWasCalled(c => c.CoinValid(_coin));
        }

        [Test]
        public void GetCoinValue()
        {
            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            _machine.InsertCoin(_coin);

            _coinValidator.AssertWasCalled(c => c.CoinValue(_coin));
        }

        [Test]
        public void DisplayTotalOfEnteredCoins()
        {
            var expected = "$0.90";

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.10M).Repeat.Once();
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.05M).Repeat.Once();

            for (var i = 0; i < 5; i++)
                _machine.InsertCoin(_coin);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void NotAddInvalidCoins()
        {
            var expected = "$0.75";

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true).Repeat.Times(3);
            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(false).Repeat.Once();

            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.01M).Repeat.Once();

            for (var i = 0; i < 4; i++)
                _machine.InsertCoin(_coin);

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

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Matches(g => g == goodCoin))).Return(true);
            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Matches(b => b == badCoin))).Return(false);

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
            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.DispensedProduct.Should().Be(_product);
        }

        [Test]
        public void DisplayThankYouMessageWhenProductDispensed()
        {
            var expected = "THANK YOU";

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.25M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DisplayInsertCoinMessageAfterCheckingThankYouMessageWhenProductDispensed()
        {
            var expected = "INSERT COIN";

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M);

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

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M);

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

            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.50M;

            _machine.InsertCoin(_coin);
            _machine.SelectProduct(_product);

            _machine.Display.Should().NotBe(expected);
            _machine.Display.Should().Be(expected);
        }

        [Test]
        public void DispenseSelectedProductAfterAppropreateCoinsAdded()
        {
            _coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);
            _coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M);

            _product.Price = 0.25M;

            _machine.SelectProduct(_product);
            _machine.InsertCoin(_coin);

            _machine.DispensedProduct.Should().Be(_product);
        }
    }
}