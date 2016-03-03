using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Models;
using VendingMachine.Validators;

namespace VendingMachine.Test.Validators
{
    [TestFixture]
    public class CoinValidatorShould
    {
        [Test]
        public void ReturnCoinAsValid()
        {
            var coin = MockRepository.GenerateStub<ICoin>();

            ICoinValidator validator = new CoinValidator();
            var valid = validator.CoinValid(coin);

            valid.Should().BeTrue();
        }

        [Test]
        public void ReturnMediumCoinsAsInvalid()
        {
            var coin = MockRepository.GenerateStub<ICoin>();
            coin.Size = CoinSize.Medium;

            ICoinValidator validator = new CoinValidator();
            var valid = validator.CoinValid(coin);

            valid.Should().BeFalse();
        }

        [Test]
        public void ReturnTenCentsForSmallCoin()
        {
            var expectedValue = 0.10M;

            var coin = MockRepository.GenerateStub<ICoin>();
            coin.Size = CoinSize.Small;

            ICoinValidator validator = new CoinValidator();
            var value = validator.CoinValue(coin);

            value.Should().Be(expectedValue);
        }

        [Test]
        public void ReturnFiveCentsForLargeCoin()
        {
            var expectedValue = 0.05M;

            var coin = MockRepository.GenerateStub<ICoin>();
            coin.Size = CoinSize.Large;

            ICoinValidator validator = new CoinValidator();
            var value = validator.CoinValue(coin);

            value.Should().Be(expectedValue);
        }

        [Test]
        public void ReturnTwentyFiveCentsForXlargeCoin()
        {
            var expectedValue = 0.25M;

            var coin = MockRepository.GenerateStub<ICoin>();
            coin.Size = CoinSize.XLarge;

            ICoinValidator validator = new CoinValidator();
            var value = validator.CoinValue(coin);

            value.Should().Be(expectedValue);
        }

        [Test]
        public void ReturnZeroCentsForMediumCoin()
        {
            var expectedValue = decimal.Zero;

            var coin = MockRepository.GenerateStub<ICoin>();
            coin.Size = CoinSize.Medium;

            ICoinValidator validator = new CoinValidator();
            var value = validator.CoinValue(coin);

            value.Should().Be(expectedValue);
        }
    }
}