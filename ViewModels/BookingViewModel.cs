using DierenManagement.Models;
using Domain;

namespace DierenManagement.ViewModels
{
    public class BookingViewModel
    {
        public DateOnly Date { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        //public int AnimalId { get; set; }
        //public Animal Animal { get; set; }

        public List<Animal> Animals { get; set; } = new List<Animal>();
    }
}
