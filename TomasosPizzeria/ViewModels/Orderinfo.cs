using TomasosPizzeria.Models;

namespace TomasosPizzeria.ViewModels
{
    public class Orderinfo
    {
        public Cart Cart { get; set; }
        public Bestallning Bestallning { get; set; }
        public AppUser User { get; set; }
    }
}
