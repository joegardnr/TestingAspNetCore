using Demo.WebAPI.Models.Request;

namespace Demo.WebAPI
{
    public interface IValidator
    {
        public IList<string> Validate(AddressRequest addressRequest);
    }
    public class Validator : IValidator
    {
        public IList<string> Validate(AddressRequest addressRequest)
        {
            var errors = new List<string>();
            if (addressRequest == null) { errors.Add("Request cannot be empty."); }
            if (string.IsNullOrWhiteSpace(addressRequest.Line1)) { errors.Add("Line 1 is required."); }
            if (string.IsNullOrWhiteSpace(addressRequest.City)) { errors.Add("City is required."); }
            if (string.IsNullOrWhiteSpace(addressRequest.State)) { errors.Add("State is required."); }
            if (string.IsNullOrWhiteSpace(addressRequest.Zip)) { errors.Add("Zip is required."); }
            return errors;
        }
    }
}
