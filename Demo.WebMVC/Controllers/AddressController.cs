using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebMVC.Controllers
{
    [Route("[controller]")]
    public class AddressController : Controller
    {
        private IAddressRepository _addressRepo;
        public AddressController(IAddressRepository addressRepo)
        {
            _addressRepo = addressRepo;
        }

        [HttpGet("")]
        public IActionResult Get()
        {            
            return View("Index", new Address());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var address = await _addressRepo.GetAddressAsync(id);
            return View("Index",address);
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(Address address)
        {
            Address result;
            if (address.Id == 0)
            {
                result = await _addressRepo.InsertAddressAsync(address);                
            }
            else
            {
                //address.Line3 = string.Empty; // this is a bug
                result = await _addressRepo.UpdateAddressAsync(address.Id, address);                
            }
            return Redirect($"/address/{result.Id}");
        }        
    }
}
