using Taxi.Common.Enums;

namespace Taxi.Common.Models
{
    public class UserResponse
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string RFC { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string PicturePath { get; set; }

        public UserType UserType { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string FullNameWithDocument => $"{FirstName} {LastName} - {RFC}";
        
        //public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
    //? "https://TaxiWeb0.azurewebsites.net//images/noimage.png"
    //: $"https://TaxiWeb0.azurewebsites.net{PicturePath.Substring(1)}";

        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
        ? "http://10.1.114.74:83//images/noimage.png"
        : $"http://10.1.114.74:83{PicturePath.Substring(1)}";

    }
}
