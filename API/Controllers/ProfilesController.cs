using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Profiles;

namespace API.Controllers
{
    public class ProfilesController : BaseController
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> Get(string username)
        {
            return await Mediator.Send(new Application.Profiles.Details.Query{Username = username});
        }
    }
}