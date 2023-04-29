using YankiApi.DTOs.BasketDTOs;
using YankiApi.Entities;

namespace YankiApi.DTOs.OrderDTOs
{
    public class OrderPostDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Postalcode { get; set; }

    }
}
