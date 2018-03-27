using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/Address")]
    [ApiVersion("1")]
    public class AddressController : Controller
    {
        private IUserAddressService _addressService;

        /// <inheritdoc />
        public AddressController(IUserAddressService userAddressService)
        {
            _addressService = userAddressService;
        }

        /// <summary>
        /// Update an address
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult CreateOrUpdate([FromRoute]int systemUserId, [FromBody]AddressViewModel model)
        {
            var address = new UserAddressModel()
            {
                GoogleLocation = model.GoogleLocation,
                Latitude = model.Latitude.ToString(),
                Longitude = model.Longitude.ToString(),
                PhysicalCity = model.PhysicalCity,
                PhysicalCountry = model.PhysicalCountry,
                PhysicalLine1 = model.PhysicalLine1,
                PhysicalLine2 = model.PhysicalLine2,
                PhysicalLine3 = model.PhysicalLine3,
                PhysicalPostCode = model.PhysicalPostCode,
                PhysicalState = model.PhysicalState,
                PhysicalSuburb = model.PhysicalSuburb,
                PostalCity = model.PostalCity,
                PostalCountry = model.PostalCountry,
                PostalLine1 = model.PostalLine1,
                PostalLine2 = model.PostalLine2,
                PostalLine3 = model.PostalLine3,
                PostalPostCode = model.PostalPostCode,
                PostalState = model.PostalState,
                PostalSuburb = model.PostalSuburb,
                SystemUserId = systemUserId
            };
            _addressService.Update(address);
            return Ok(MessageHelper.Success("The address has been updated."));
        }
    }
}