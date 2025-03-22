using NhaSachOnline.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace NhaSachOnline.Models
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items => items;

        public virtual void AddItem(Product product, int quantity)
        {
            if (product.Stock < quantity)
            {
                throw new InvalidOperationException($"Số lượng yêu cầu ({quantity}) vượt quá số lượng tồn kho ({product.Stock}) của sản phẩm {product.Name}.");
            }

            var item = items.FirstOrDefault(p => p.Product.Id == product.Id);
            if (item == null)
            {
                items.Add(new CartItem { Product = product, Quantity = quantity });
            }
            else
            {
                if (product.Stock < item.Quantity + quantity)
                {
                    throw new InvalidOperationException($"Số lượng yêu cầu ({item.Quantity + quantity}) vượt quá số lượng tồn kho ({product.Stock}) của sản phẩm {product.Name}.");
                }
                item.Quantity += quantity;
            }

            // Lưu vào session nếu là SessionCart
            if (this is SessionCart sessionCart && sessionCart.Session != null)
            {
                sessionCart.Session.SetJson("Cart", this);
            }
        }

        public virtual void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    items.Remove(item);
                }
                else
                {
                    if (item.Product.Stock < quantity)
                    {
                        throw new InvalidOperationException($"Số lượng yêu cầu ({quantity}) vượt quá số lượng tồn kho ({item.Product.Stock}) của sản phẩm {item.Product.Name}.");
                    }
                    item.Quantity = quantity;
                }
            }

            // Lưu vào session nếu là SessionCart
            if (this is SessionCart sessionCart && sessionCart.Session != null)
            {
                sessionCart.Session.SetJson("Cart", this);
            }
        }

        public virtual void RemoveItem(int productId)
        {
            items.RemoveAll(p => p.Product.Id == productId);

            // Lưu vào session nếu là SessionCart
            if (this is SessionCart sessionCart && sessionCart.Session != null)
            {
                sessionCart.Session.SetJson("Cart", this);
            }
        }

        public virtual void Clear()
        {
            items.Clear();

            // Lưu vào session nếu là SessionCart
            if (this is SessionCart sessionCart && sessionCart.Session != null)
            {
                sessionCart.Session.Remove("Cart");
            }
        }

        public decimal ComputeTotalValue()
        {
            return items.Sum(e => e.Product.Price * e.Quantity);
        }
    }

    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}