﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private IAddressRepository _addressRepo;
        public AddressController(IAddressRepository addressRepo)
        {
            _addressRepo = addressRepo;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressRepo.GetAddressAsync(id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Address address)
        {
            var result = await _addressRepo.InsertAddressAsync(address);
            return CreatedAtAction(actionName: nameof(GetById), routeValues: new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Address address)
        {
            //address.Line2 = string.Empty; // this is a bug
            var result = await _addressRepo.UpdateAddressAsync(id, address);
            return Ok(result);
        }
    }
}
