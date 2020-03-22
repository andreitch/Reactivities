using System.Threading.Tasks;
using Application.Photos;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class PhotosController : BaseController
  {
    [HttpPost]
    public async Task<ActionResult<Photo>> Add([FromForm]Add.Command command)
    {
      return await Mediator.Send(command);
    }
  }
}