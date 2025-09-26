using System.Collections.Generic;
using webBanSach.Models;

namespace webBanSach.ViewModels
{
    public class HomeViewModel
    {
        public List<SachViewModel> SachMoi { get; set; }
        public List<SachViewModel> SachBanChay { get; set; }
        public List<SachViewModel> SachNoiBat { get; set; }
        public List<TheLoai> TheLoais { get; set; }
    }
}