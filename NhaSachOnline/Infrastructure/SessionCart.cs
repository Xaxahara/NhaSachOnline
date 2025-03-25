using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NhaSachOnline.Models;

namespace NhaSachOnline.Infrastructure
{
    public class SessionCart : Cart
    {
        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        [JsonIgnore]
        public ISession? Session { get; set; }

        public override void AddItem(Product product, int quantity)
        {
            base.AddItem(product, quantity);
            Session?.SetJson("Cart", this);
        }

        public override void RemoveItem(int productId)
        {
            base.RemoveItem(productId);
            Session?.SetJson("Cart", this);
        }

        public override void UpdateQuantity(int productId, int quantity)
        {
            base.UpdateQuantity(productId, quantity);
            Session?.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session?.Remove("Cart");
        }
    }

    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}