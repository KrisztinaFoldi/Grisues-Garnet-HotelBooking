using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingGarnet.Models;
using HotelBookingGarnet.Utils;
using Microsoft.EntityFrameworkCore;
using HotelBookingGarnet.ViewModels;
using ReflectionIT.Mvc.Paging;

namespace HotelBookingGarnet.Services
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationContext applicationContext;
        private readonly IPropertyTypeService propertyTypeService;
        private readonly IRoomService roomService;

        public HotelService(ApplicationContext applicationContext, IPropertyTypeService propertyTypeService, IRoomService roomService)
        {
            this.applicationContext = applicationContext;
            this.propertyTypeService = propertyTypeService;
            this.roomService = roomService;
        }

        public async Task EditHotelAsync(long HotelId, HotelViewModel editHotel)
        {
            var hotelToEdit = await FindHotelByIdAsync(HotelId);
            var property = await propertyTypeService.AddPropertyTypeAsync(editHotel.PropertyType);
            if (hotelToEdit != null)
            {
                hotelToEdit.HotelName = editHotel.HotelName;
                hotelToEdit.Country = editHotel.Country;
                hotelToEdit.Region = editHotel.Region;
                hotelToEdit.City = editHotel.City;
                hotelToEdit.Address = editHotel.Address;
                hotelToEdit.Description = editHotel.Description;
                hotelToEdit.StarRating = editHotel.StarRating;
                hotelToEdit.Price = editHotel.Price;
            }

            applicationContext.Hotels.Update(hotelToEdit);
            await applicationContext.SaveChangesAsync();
        }

        public async Task<Hotel> FindHotelByIdAsync(long HotelId)
        {
            var foundHotel = await applicationContext.Hotels.Include(p => p.HotelPropertyTypes)
                .Include(h => h.Rooms).SingleOrDefaultAsync(x => x.HotelId == HotelId);

            return foundHotel;
        }

        public async Task AddHotelAsync(HotelViewModel newHotel, string userId)
        {
            var propertyType = await propertyTypeService.AddPropertyTypeAsync(newHotel.PropertyType);

            var hotel = new Hotel
            {
                HotelName = newHotel.HotelName,
                Country = newHotel.Country,
                Region = newHotel.Region,
                City = newHotel.City,
                Address = newHotel.Address,
                Description = newHotel.Description,
                StarRating = newHotel.StarRating,
                Price = newHotel.Price,
                UserId = userId
            };

            await applicationContext.Hotels.AddAsync(hotel);
            await applicationContext.SaveChangesAsync();

            propertyType.HotelPropertyTypes = new List<HotelPropertyType>();

            var connection = new HotelPropertyType();
            connection.Hotel = hotel;
            connection.HotelId = hotel.HotelId;
            connection.PropertyType = propertyType;
            connection.PropertyTypeId = propertyType.PropertyTypeId;

            propertyType.HotelPropertyTypes.Add(connection);

            await applicationContext.SaveChangesAsync();
        }
        
        public List<Hotel> GetHotels()
        {
            var qry = applicationContext.Hotels.Include(h => h.Rooms).AsQueryable().OrderBy(h => h.HotelName).ToList();
            return qry;
        }

        public async Task<PagingList<Hotel>> FilterHotelsAsync(QueryParam queryParam)
        {
            var allHotels = GetHotels();
            foreach (var hotel in allHotels)
            {
                if (hotel.Rooms != null)
                {
                    foreach (var room in hotel.Rooms)
                    {
                        if (room.NumberOfAvailablePlaces >= queryParam.Guest)
                        {
                            hotel.IsItAvailable = true;
                            applicationContext.SaveChanges(hotel.IsItAvailable);
                        }
                    }
                }
                
            }
            
            var hotels = await applicationContext.Hotels.Include(h => h.Rooms)
                .Where(h => h.City.Contains(queryParam.City) || String.IsNullOrEmpty(queryParam.City))
                .Where(h => h.IsItAvailable || queryParam.Guest == 0)
                .OrderBy(h => h.HotelName).ToListAsync();
            return PagingList.Create(hotels, 5, queryParam.Page);
        }
    }
}