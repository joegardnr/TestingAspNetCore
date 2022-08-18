using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IValidator _validator;
    private readonly IMapper _mapper;
    private IAddressRepository _addressRepo;
    public AddressController(
        IValidator validator,
        IMapper mapper,
        IAddressRepository addressRepo
        )
    {
        _validator = validator;
        _mapper = mapper;
        _addressRepo = addressRepo;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var address = await _addressRepo.GetAddressAsync(id);
        var addressResponse = _mapper.MapToRequest(address);
        return Ok(addressResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddressRequest addressRequest)
    {
        var errors = _validator.Validate(addressRequest);
        if (errors.Count > 0) { return BadRequest(errors); }

        var address = _mapper.MapFromRequest(addressRequest);
        
        var result = await _addressRepo.InsertAddressAsync(address);
        
        var response = _mapper.MapToRequest(result);
        return CreatedAtAction(actionName: nameof(GetById), routeValues: new { id = result.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] AddressRequest addressRequest)
    {
        var errors = _validator.Validate(addressRequest);
        if (errors.Count > 0) { return BadRequest(errors); }
        
        var address = _mapper.MapFromRequest(addressRequest);
        
        var result = await _addressRepo.UpdateAddressAsync(id, address);
        
        var response = _mapper.MapToRequest(result);
        return Ok(response);
    }
}
