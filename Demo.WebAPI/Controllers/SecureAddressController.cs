using Demo.WebAPI.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SecureAddressController : ControllerBase
{
    private readonly IValidator _validator;
    private readonly IMapper _mapper;
    private IAddressRepository _addressRepo;
    public SecureAddressController(
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
        return CreatedAtAction(actionName: nameof(GetById), routeValues: new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] AddressRequest addressRequest)
    {
        var errors = _validator.Validate(addressRequest);
        if (errors.Count > 0) { return BadRequest(errors); }
        var address = _mapper.MapFromRequest(addressRequest);
        var result = await _addressRepo.UpdateAddressAsync(id, address);
        return Ok(result);
    }
}
