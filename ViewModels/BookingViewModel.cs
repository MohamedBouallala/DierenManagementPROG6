using DierenManagement.Models;
using Domain;
using Interface;
using Interface.DTOs;

namespace DierenManagement.ViewModels
{
    public class BookingViewModel
    {
        public DateOnly Date { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public List<Animal> Animals { get; set; } = new List<Animal>();

        // dit heb ik toegevoegd zou dat het handig is met alle dieren + geselecteerde dieren te werken (controller)
        public List<Animal> listWithSelectedAnimals { get; set; } = new List<Animal>();

        public BookingDto ConvertToBookingDto()
        {
            BookingDto dto = new BookingDto();
           
            dto.Date = Date;
            dto.UserId = UserId;

            dto.User = new UserDto();

            if (User != null)
            {
                dto.User.LoyaltyCard = LoyaltyCardMapper.MapToDto(User.LoyaltyCard);
            }
            
            dto.listWithSelectedAnimals = listWithSelectedAnimals
                .Select(a => new AnimalDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    AnimalType = AnimalTypeMapper.MapToDto(a.AnimalType),
                    Price = a.Price,

                }).ToList();
            return dto;
        }
    }
}
