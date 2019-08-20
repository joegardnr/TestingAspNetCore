using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecureAddressController : ControllerBase
    {
        private IAddressRepository _addressRepo;
        public SecureAddressController(IAddressRepository addressRepo)
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
            //address.Line3 = string.Empty; // This is a bug
            var result = await _addressRepo.UpdateAddressAsync(id, address);
            return Ok(result);
        }
    }
}
