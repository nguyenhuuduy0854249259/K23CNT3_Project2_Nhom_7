using Microsoft.AspNetCore.Mvc;

namespace webBanSach.Models
{
    public static class OrderStatus
    {
        public const string ChoXuLy = "Chờ xử lý";
        public const string DangGiao = "Đang giao";
        public const string HoanTat = "Hoàn tất";
        public const string Huy = "Hủy";

        public static readonly string[] All = { ChoXuLy, DangGiao, HoanTat, Huy };
    }
}

