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
        [Test]
        public void DefaultDisplayToInsertCoin()
        {
            var expected = "INSERT COIN";

            IMachine machine = new Machine(null);

            machine.Display.Should().Be(expected);
        }

        [Test]
        public void ValidateCoin()
        {
            var coinValidator = MockRepository.GenerateStub<ICoinValidator>();
            var coin = MockRepository.GenerateStub<ICoin>();

            IMachine machine = new Machine(coinValidator);

            machine.InsertCoin(coin);

            coinValidator.AssertWasCalled(c => c.CoinValid(coin));
        }

        [Test]
        public void GetCoinValue()
        {
            var coinValidator = MockRepository.GenerateStub<ICoinValidator>();
            var coin = MockRepository.GenerateStub<ICoin>();

            coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            IMachine machine = new Machine(coinValidator);

            machine.InsertCoin(coin);

            coinValidator.AssertWasCalled(c => c.CoinValue(coin));
        }

        [Test]
        public void DisplayTotalOfEnteredCoins()
        {
            var expected = "$0.90";

            var coin = MockRepository.GenerateStub<ICoin>();
            var coinValidator = MockRepository.GenerateStub<ICoinValidator>();

            coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true);

            coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.10M).Repeat.Once();
            coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.05M).Repeat.Once();

            IMachine machine = new Machine(coinValidator);

            for (var i = 0; i < 5; i++)
                machine.InsertCoin(coin);

            machine.Display.Should().Be(expected);
        }

        [Test]
        public void NotAddInvalidCoins()
        {
            var expected = "$0.75";

            var coin = MockRepository.GenerateStub<ICoin>();
            var coinValidator = MockRepository.GenerateStub<ICoinValidator>();

            coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(true).Repeat.Times(3);
            coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Is.Anything)).Return(false).Repeat.Once();

            coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.25M).Repeat.Times(3);
            coinValidator.Stub(c => c.CoinValue(Arg<ICoin>.Is.Anything)).Return(0.01M).Repeat.Once();

            IMachine machine = new Machine(coinValidator);

            for (var i = 0; i < 4; i++)
                machine.InsertCoin(coin);

            machine.Display.Should().Be(expected);
        }

        [Test]
        public void DefaultToEmptyCoinReturn()
        {
            var expected = new List<ICoin>();

            IMachine machine = new Machine(null);

            machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }

        [Test]
        public void AddInvalidCoinsToCoinReturn()
        {
            var goodCoin = MockRepository.GenerateStub<ICoin>();
            var badCoin = MockRepository.GenerateStub<ICoin>();

            var expected = new List<ICoin> { badCoin };

            var coinValidator = MockRepository.GenerateStub<ICoinValidator>();

            coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Matches(g => g == goodCoin))).Return(true);
            coinValidator.Stub(c => c.CoinValid(Arg<ICoin>.Matches(b => b == badCoin))).Return(false);

            IMachine machine = new Machine(coinValidator);

            machine.InsertCoin(goodCoin);
            machine.InsertCoin(badCoin);

            machine.CoinReturn.ShouldBeEquivalentTo(expected);
        }
    }
}