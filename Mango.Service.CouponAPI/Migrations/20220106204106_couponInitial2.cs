using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Service.CouponAPI.Migrations
{
    public partial class couponInitial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountAmin",
                table: "Coupons",
                newName: "DiscountAmount");

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount" },
                values: new object[] { 1, "10OFF", 10.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Coupons",
                newName: "DiscountAmin");
        }
    }
}
