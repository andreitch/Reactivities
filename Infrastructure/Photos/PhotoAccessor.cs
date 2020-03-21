using Application.Interfaces;
using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Photos
{
  public class PhotoAccessor : IPhotoAccessor
  {
    public PhotoUploadResult AddPhoto(IFormFile file)
    {
      throw new System.NotImplementedException();
    }

    public string DeletePhoto(string publicId)
    {
      throw new System.NotImplementedException();
    }
  }
}