namespace NutriBest.Server.Features.Images.Models
{
    public interface IFileData
    {
        string ImageData { get; set; }

        string ContentType { get; set; }
    }
}
