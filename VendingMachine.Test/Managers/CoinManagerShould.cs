using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Managers;
using VendingMachine.Models;

namespace VendingMachine.Test.Managers
{
    [TestFixture]
    public class CoinManagerShould
    {
        private ICoin _coin;
        private ICoinManager _manager;

        [SetUp]
        public void Setup()
        {
            _coin = MockRepository.GenerateStub<ICoin>();
            _manager = new CoinManager();
        }

        [Test]
        public void ReturnCoinAsValid()
        {
            var valid = _manager.CoinValid(_coin);

            valid.Should().BeTrue();
        }

        [Test]
        public void ReturnMediumCoinsAsInvalid()
        {
            _coin.Size = CoinSize.Medium;
            
            var valid = _manager.CoinValid(_coin);

            valid.Should().BeFalse();
        }

        [Test]
        public void ReturnTenCentsForSmallCoin()
        {
            var expectedValue = 0.10M;
            
            _coin.Size = CoinSize.Small;
            
            var value = _manager.CoinValueByWeight(_coin);

            value.Should().Be(expectedValue);
        }

        [Test]
        public void ReturnFiveCentsForLargeCoin()
        {
            var expectedValue = 0.05M;
            
            _coin.Size = CoinSize.Large;
            
            var value = _manager.CoinValueByWeight(_coin);

            value.Should().Be(expectedValue);
        }

        [Test]
        public void ReturnTwentyFiveCentsForXlargeCoin()
        {
            var expectedValue = 0.25M;
            
            _coin.Size = CoinSize.XLarge;
            
            var value = _manager.CoinValueByWeight(_coin);

            value.Should().Be(expectedValue);
        }

        [Test]
        public void ReturnZeroCentsForMediumCoin()
        {
            var expectedValue = decimal.Zero;
            
            _coin.Size = CoinSize.Medium;
            
            var value = _manager.CoinValueByWeight(_coin);

            value.Should().Be(expectedValue);
        }

        [TestCase(typeof(Quarter), 0.25)]
        [TestCase(typeof(Dime), 0.10)]
        [TestCase(typeof(Nickel), 0.05)]
        public void ReturnCorrectSingleCoin(Type type, decimal amount)
        {
            var actual = _manager.DistributeChange(amount).ToList();

            actual.Count().Should().Be(1);
            actual.First().Should().BeOfType(type);
        }

        [Test]
        public void DistributeMultipleCoinsEfficently()
        {
            var expected = new List<ICoin>
            {
                new Quarter(),
                new Quarter(),
                new Quarter(),
                new Dime(),
                new Nickel()
            };

            var amount = 0.90M;

            var change = _manager.DistributeChange(amount).ToList();

            change.ShouldBeEquivalentTo(expected);
        }
    }
}