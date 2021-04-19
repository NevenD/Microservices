using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : DapperBaseRepository, IDiscountRepository
    {
        public DiscountRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE LOWER(ProductName) = LOWER(@ProductName)",  new { ProductName = productName });
            if (coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            }
            return coupon;
        }


        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var isSaved = await InsertData("INSERT INTO Coupon(ProductName, Description, Amount) VALUES(@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return isSaved;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var isUpdated = await UpdateData("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            return isUpdated;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var isDeleted = await UpdateData("DELETE FROM Coupon WHERE ProductName = @ProductName",
                             new { ProductName = productName });

            return isDeleted;
        }



       
    }
}
