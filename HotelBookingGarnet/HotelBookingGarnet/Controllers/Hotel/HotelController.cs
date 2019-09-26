
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingGarnet.Services;
using HotelBookingGarnet.Models;
using HotelBookingGarnet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingGarnet.Controllers.Hotel
{
    public class HotelController : Controller
    {
        private readonly IHotelService hotelService;
        private readonly UserManager<User> userManager;
        private readonly IPropertyTypeService propertyTypeService;

        public HotelController(IHotelService hotelService, UserManager<User> userManager, IPropertyTypeService propertyTypeService)
        {
            this.hotelService = hotelService;
            this.userManager = userManager;
            this.propertyTypeService = propertyTypeService;
        }

        [HttpPost("/info/{HotelId}")]
        public async Task<IActionResult> HotelInfo(long HotelId)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var hotel = await hotelService.FindHotelByIdAsync(HotelId);
            //var property = await propertyTypeService.FindByIdAsync(hotel.HotelPropertyTypes);
            return View(new IndexViewModel { User = currentUser, hotel = hotel });
        }

        
        [Authorize(Roles = "Hotel Manager")]
        [HttpGet("/edit/{HotelId}")]
        public async Task<IActionResult> EditHotel(long HotelId, HotelViewModel hotelViewModel)
        {
            var hotel = await hotelService.FindHotelByIdAsync(HotelId);
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            hotelViewModel.User = currentUser;
            ViewData["HotelId"] = hotel.HotelId;
            hotelViewModel.Address = hotel.Address;
            hotelViewModel.City = hotel.City;
            hotelViewModel.Country = hotel.Country;
            hotelViewModel.Description = hotel.Description;
            hotelViewModel.Price = hotel.Price;
            hotelViewModel.Region = hotel.Region;
            hotelViewModel.HotelName = hotel.HotelName;
            hotelViewModel.PropertyType = hotel.PropertyType.Type;
            hotelViewModel.StarRating = hotel.StarRating;
            return View(hotelViewModel);
        }

        [HttpPost("/edit/{HotelId}")]
        public async Task<IActionResult> EditHotel(HotelViewModel editHotel, long HotelId)
        {
            if (ModelState.IsValid)
            {
                await hotelService.EditHotelAsync(HotelId, editHotel);
                return RedirectToAction("Home", "Home");
            }

            return View(editHotel);
        }

        [Authorize(Roles = "Hotel Manager, Admin")]
        [HttpGet("/addhotel")]
        public IActionResult AddHotel()
        {
            return View(new HotelViewModel());
        }

        [Authorize(Roles = "Hotel Manager, Admin")]
        [HttpPost("/addhotel")]
        public async Task<IActionResult> AddHotel(HotelViewModel newHotel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.GetUserAsync(HttpContext.User);
                await hotelService.AddHotelAsync(newHotel, currentUser.Id);
                RedirectToAction("Home", "Home");
            }
            return View(newHotel);
        }
    }
}