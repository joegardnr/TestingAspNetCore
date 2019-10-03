using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Demo.WebAPI.Models;

namespace Demo.WebAPI
{
    public interface IAddressRepository
    {
        Task<Address> GetAddressAsync(int id);
        Task<Address> InsertAddressAsync(Address address);
        Task<Address> UpdateAddressAsync(int id, Address address);
    }

    // Pay no attention to main the behind the curtain.  (This is demo code.)
    public class AddressRepositoryInMemory : IAddressRepository
    {
        private readonly IList<Address> _addressDB;  // An in-memory list is totally a legit "Database"...
        public AddressRepositoryInMemory()
        {
            _addressDB = new List<Address>
            {
                new Address
                {
                    Id = 1,
                    Line1 = "Line 1",
                    Line2 = "Line 2",
                    City = "City",
                    State = "State",
                    Zip = "90210"
                }
            };            
        }

        public Task<Address> GetAddressAsync(int id)
        {
            var address = _addressDB.Single(a => a.Id == id);
            return Task.FromResult(address);
        }

        public Task<Address> InsertAddressAsync(Address address)
        {
            var maxId = _addressDB.Max(r => r.Id);  // This is not very thread safe. 
            address.Id = maxId + 1;
            _addressDB.Add(address);
            return Task.FromResult(address);
        }

        public Task<Address> UpdateAddressAsync(int id, Address address)
        {
            var existing = _addressDB.Single(a => a.Id == id);
            _addressDB.Remove(existing);
            _addressDB.Add(address);
            return Task.FromResult(address);
        }
    }
}
