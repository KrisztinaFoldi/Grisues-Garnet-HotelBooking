﻿using System.Threading.Tasks;
using HotelBookingGarnet.Models;
using HotelBookingGarnet.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelBookingGarnet.Controllers.Home
{
    public class HomeController : Controller
    {
        private readonly IHotelService hotelService;

        public HomeController(IHotelService hotelService)
        {
            this.hotelService = hotelService;
        }

        [HttpGet("/")]
        public  IActionResult Home(User user ,int page = 1)
        {
            ViewData["User"] = user;

            var hotels = hotelService.GetHotels();
            var model =  PagingList.Create(hotels, 3, page);
            return View(model);
        }
    }
}
