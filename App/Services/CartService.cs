using System;
using System.Collections.ObjectModel;
using System.Linq;
using PCFirmApp.Models;

namespace PCFirmApp.Services;

public class CartItem
{
    public required Product Product { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice => Product.Price;
    public decimal TotalPrice => UnitPrice * Quantity;

    public bool HasPromotion { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountValue => HasPromotion ? TotalPrice * DiscountPercent / 100 : 0;
    public decimal FinalPrice => TotalPrice - DiscountValue;
}

public class CartService
{
    public ObservableCollection<CartItem> Items { get; } = new();

    public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    public decimal TotalDiscount => Items.Sum(i => i.DiscountValue);
    public decimal Total => Items.Sum(i => i.FinalPrice);

    public void AddProduct(Product product, bool hasPromotion = false, decimal discountPercent = 0)
    {
        var existingItem = Items.FirstOrDefault(i => i.Product.Id == product.Id);

        if (existingItem != null)
        {
            existingItem.Quantity++;

            if (hasPromotion)
            {
                existingItem.HasPromotion = true;
                existingItem.DiscountPercent = discountPercent;
            }
        }
        else
        {
            Items.Add(new CartItem
            {
                Product = product,
                Quantity = 1,
                HasPromotion = hasPromotion,
                DiscountPercent = discountPercent
            });
        }
    }

    public void RemoveProduct(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.Product.Id == productId);

        if (item != null)
            Items.Remove(item);
    }

    public void Clear()
    {
        Items.Clear();
    }
}