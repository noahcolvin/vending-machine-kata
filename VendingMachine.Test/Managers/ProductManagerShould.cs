using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;
using VendingMachine.Managers;
using VendingMachine.Models;

namespace VendingMachine.Test.Managers
{
    [TestFixture]
    public class ProductManagerShould
    {
        private IProduct _genericProduct;
        private IProductManager _productManager;

        [SetUp]
        public void Setup()
        {
            _genericProduct = MockRepository.GenerateStub<IProduct>();
            _productManager = new ProductManager();
        }

        [Test]
        public void NotFindUnaddedProduct()
        {
            _productManager.ProductAvailable(_genericProduct).Should().BeFalse();
        }

        [Test]
        public void AddProductToInventory()
        {
            _productManager.AddStock(_genericProduct, 1);

            _productManager.ProductAvailable(_genericProduct).Should().BeTrue();
        }

        [Test]
        public void RemoveProductFromInventory()
        {
            _productManager.AddStock(_genericProduct, 1);
            _productManager.RemoveProduct(_genericProduct);

            _productManager.ProductAvailable(_genericProduct).Should().BeFalse();
        }

        [Test]
        public void AddMultipleOfSameProduct()
        {
            _productManager.AddStock(_genericProduct, 1);
            _productManager.AddStock(_genericProduct, 1);
            _productManager.RemoveProduct(_genericProduct);

            _productManager.ProductAvailable(_genericProduct).Should().BeTrue();
        }

        [Test]
        public void AddMultipleProducts()
        {
            var otherProduct = MockRepository.GenerateStub<IProduct>();

            _productManager.AddStock(_genericProduct, 1);
            _productManager.AddStock(otherProduct, 1);
            
            _productManager.ProductAvailable(_genericProduct).Should().BeTrue();
            _productManager.ProductAvailable(otherProduct).Should().BeTrue();
        }
    }
}