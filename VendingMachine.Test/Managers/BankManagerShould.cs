using System;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using VendingMachine.Managers;

namespace VendingMachine.Test.Managers
{
    [TestFixture]
    public class BankManagerShould
    {
        [Test]
        public void SetBankAmount()
        {
            var expectedAmount = 100M;

            var bankManager = new BankManager { Amount = 100M };

            var actual = bankManager.Amount;

            actual.Should().Be(expectedAmount);
        }

        [Test]
        public void NotAllowNegativeSetBankAmount()
        {
            var bankManager = new BankManager();

            Action setAction = () => bankManager.Amount = -1;

            setAction.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void AddMoneyToAmount()
        {
            var expected = 101M;

            var bankManager = new BankManager();
            bankManager.Amount = 100M;
            bankManager.AddMoney(1M);

            bankManager.Amount.Should().Be(expected);
        }

        [Test]
        public void NotAddNegativeMoney()
        {
            var bankManager = new BankManager();
            bankManager.Amount = 50M;

            Action addAction = () => bankManager.AddMoney(-1);

            addAction.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}