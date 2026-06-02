using System;
using Xunit;
using PCFirmApp.Services;
using PCFirmApp.Models;

namespace PCFirmApp.Tests;

public class RaluCartServiceTests
{
    private Component CreateProduct()
    {
        return new Component
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Description = "Laptop de test",
            Price = 5000,

            Category = ProductCategory.Peripheral,

            StockCount = 10,

            ComponentType = ComponentType.Other
        };
    }

    [Fact]
    public void Cart_ShouldBeEmpty_OnInit()
    {
        var cart = new CartService();

        Assert.Empty(cart.Items);
    }

    [Fact]
    public void AddProduct_ShouldAddProductToCart()
    {
        var cart = new CartService();
        var product = CreateProduct();

        cart.AddProduct(product);

        Assert.Single(cart.Items);
    }

    [Fact]
    public void AddSameProductTwice_ShouldIncreaseQuantity()
    {
        var cart = new CartService();
        var product = CreateProduct();

        cart.AddProduct(product);
        cart.AddProduct(product);

        Assert.Equal(2, cart.Items[0].Quantity);
    }

    [Fact]
    public void RemoveProduct_ShouldRemoveItemFromCart()
    {
        var cart = new CartService();
        var product = CreateProduct();

        cart.AddProduct(product);
        cart.RemoveProduct(product.Id);

        Assert.Empty(cart.Items);
    }

    [Fact]
    public void Clear_ShouldEmptyCart()
    {
        var cart = new CartService();
        var product = CreateProduct();

        cart.AddProduct(product);

        cart.Clear();

        Assert.Empty(cart.Items);
    }

    [Fact]
    public void Total_ShouldCalculateCorrectly()
    {
        var cart = new CartService();
        var product = CreateProduct();

        cart.AddProduct(product);
        cart.AddProduct(product);

        Assert.Equal(10000, cart.Total);
    }
}