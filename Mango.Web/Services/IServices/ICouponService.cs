namespace Mango.Web.Services.IServices
{
    public interface ICouponService : IBaseService
    {
        Task<T> GetCouponByCode<T>(string code, string token);
    }
}
