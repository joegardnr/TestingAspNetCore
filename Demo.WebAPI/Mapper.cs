using Demo.WebAPI.Models.Business;
using Demo.WebAPI.Models.Request;

namespace Demo.WebAPI
{
    public interface IMapper
    {
        Address MapFromRequest(AddressRequest request);
        AddressRequest MapToRequest(Address address);
    }
    public class Mapper : IMapper
    {
        public Address MapFromRequest(AddressRequest request)
        {
            return new Address
            {
                Line1 = request.Line1,
                Line2 = request.Line2,
                City = request.City,
                State = request.State,
                Zip = request.Zip
            };
        }

        public AddressRequest MapToRequest(Address address)
        {
            return new AddressRequest
            {
                Id = address.Id,
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                Zip = address.Zip
            };
        }
    }
}
