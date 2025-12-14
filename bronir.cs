using System;
using System.Collections.Generic;
namespace CinemaBookingSystem
{
    public class Movie
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        public Movie(string title, int duration)
        {
            Title = title;
            Duration = duration;
        }
        public void DisplayInfo()
        {
            Console.WriteLine($"Название: {Title}, Длительность: {Duration} минут");
        }
    }
    public class Seat
    {
        public int SeatNumber { get; set; }
        public bool IsReserved { get; private set; }
        public Seat(int seatNumber)
        {
            SeatNumber = seatNumber;
            IsReserved = false;
        }
        public bool Reserve()
        {
            if (IsReserved)
            {
                Console.WriteLine($"Место {SeatNumber} уже занято.");
                return false;
            }
            IsReserved = true;
            Console.WriteLine($"Место {SeatNumber} забронировано.");
            return true;
        }
        public void DisplaySeatStatus()
        {
            string status = IsReserved ? "Занято" : "Свободно";
            Console.WriteLine($"Место {SeatNumber}: {status}");
        }
    }
    public class BookingService
    {
        public Movie SelectedMovie { get; private set; }
        public Seat SelectedSeat { get; private set; }
        public void SelectMovie(Movie movie)
        {
            SelectedMovie = movie;
            Console.WriteLine($"Вы выбрали фильм: {SelectedMovie.Title}");
        }
        public bool SelectSeat(CinemaHall cinemaHall, int seatNumber)
        {
            var seat = cinemaHall.GetSeat(seatNumber);
            if (seat != null && !seat.IsReserved)
            {
                SelectedSeat = seat;
                return true;
            }
            Console.WriteLine("Ошибка: выбранное место занято или не существует.");
            return false;
        }
        public void MakeReservation(Customer customer)
        {
            if (SelectedSeat != null && !SelectedSeat.IsReserved)
            {
                if (SelectedSeat.Reserve())
                {
                    customer.AddBooking(SelectedMovie, SelectedSeat);
                }
            }
        }
    }
    public class CinemaHall
    {
        private List<Seat> seats;
        public CinemaHall(int numberOfSeats)
        {
            seats = new List<Seat>();
            for (int i = 1; i <= numberOfSeats; i++)
            {
                seats.Add(new Seat(i));
            }
        }
        public Seat GetSeat(int seatNumber)
        {
            return seats.Find(seat => seat.SeatNumber == seatNumber);
        }
        public void DisplaySeats()
        {
            Console.WriteLine("Схема зала:");
            foreach (var seat in seats)
            {
                seat.DisplaySeatStatus();
            }
        }
    }
    public class Customer
    {
        public string Name { get; set; }
        private List<(Movie, Seat)> bookings;
        public Customer(string name)
        {
            Name = name;
            bookings = new List<(Movie, Seat)>();
        }
        public void AddBooking(Movie movie, Seat seat)
        {
            bookings.Add((movie, seat));
            Console.WriteLine($"Бронирование успешно для {Name}: Фильм '{movie.Title}' на место {seat.SeatNumber}");
        }
        public void CancelBooking(Movie movie)
        {
            var booking = bookings.Find(b => b.Item1 == movie);
            if (booking != default)
            {
                booking.Item2.DisplaySeatStatus();
                bookings.Remove(booking);
                Console.WriteLine($"Бронирование для фильма '{movie.Title}' отменено.");
            }
            else
            {
                Console.WriteLine($"Нет бронирования для фильма '{movie.Title}'.");
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Movie movie1 = new Movie("Бэтмен", 120);
            Movie movie2 = new Movie("Интерстеллар", 169);
            Movie movie3 = new Movie("Матрица", 136);
            CinemaHall cinemaHall = new CinemaHall(10);
            Console.Write("Введите имя клиента: ");
            string customerName = Console.ReadLine();
            Customer customer = new Customer(customerName);
            BookingService bookingService = new BookingService();
            Console.WriteLine("\nСписок фильмов:");
            movie1.DisplayInfo();
            movie2.DisplayInfo();
            movie3.DisplayInfo();
            Console.Write("\nВыберите фильм по номеру (1-3): ");
            int movieChoice = int.Parse(Console.ReadLine());
            switch (movieChoice)
            {
                case 1:
                    bookingService.SelectMovie(movie1);
                    break;
                case 2:
                    bookingService.SelectMovie(movie2);
                    break;
                case 3:
                    bookingService.SelectMovie(movie3);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    return;
            }
            cinemaHall.DisplaySeats();
            Console.Write("\nВыберите место (1-10): ");
            int seatChoice = int.Parse(Console.ReadLine());
            if (bookingService.SelectSeat(cinemaHall, seatChoice))
            {
                bookingService.MakeReservation(customer);
            }
            Console.WriteLine("\nСхема зала после бронирования:");
            cinemaHall.DisplaySeats();
            Console.Write("\nХотите отменить бронирование? (да/нет): ");
            string cancelChoice = Console.ReadLine();
            if (cancelChoice.ToLower() == "да")
            {
                customer.CancelBooking(bookingService.SelectedMovie);
            }
            Console.WriteLine("\nСхема зала после отмены:");
            cinemaHall.DisplaySeats();
        }
    }
}
