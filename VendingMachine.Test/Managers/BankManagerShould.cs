using System;
using FluentAssertions;
using NUnit.Framework;
using VendingMachine.Managers;

namespace VendingMachine.Test.Managers
{
    [TestFixture]
    public class BankManagerShould
    {
        [Test]
        public void AddMoneyToAmount()
        {
            var expected = 101M;

            IBankManager bankManager = new BankManager();
            bankManager.AddMoney(100M);
            bankManager.AddMoney(1M);

            bankManager.Amount.Should().Be(expected);
        }

        [Test]
        public void NotAddNegativeMoney()
        {
            IBankManager bankManager = new BankManager();
            bankManager.AddMoney(50M);

            Action addAction = () => bankManager.AddMoney(-1);

            addAction.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}