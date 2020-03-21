using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            var admin = await CheckUserAsync("CALD7808244AA", "David", "Chávez", "divadchl@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Admin);
            var driver = await CheckUserAsync("CALD7808244AA", "David", "Chávez", "divadchl@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Driver);
            var user1 = await CheckUserAsync("CALD7808244AA", "David", "Chávez", "dacalo.soporte@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            var user2 = await CheckUserAsync("CALD7808244AA", "David", "Chávez", "divadchl666@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            var user3 = await CheckUserAsync("CALD7808244AA", "David", "Chávez", "dacalo@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            var user4 = await CheckUserAsync("CALD7808244AA", "David", "Chávez", "divadchl@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            await CheckTaxisAsync(driver, user1, user2);
            await CheckUserGroups(user1, user2, user3, user4);
        }

        private async Task<UserEntity> CheckUserAsync(
            string rfc,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    RFC = rfc,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckTaxisAsync(
            UserEntity driver,
            UserEntity user1,
            UserEntity user2)
        {
            if (!_dataContext.Taxis.Any())
            {
                _dataContext.Taxis.Add(new TaxiEntity
                {
                    User = driver,
                    Plaque = "TPQ123",
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean vitae nisl vel neque euismod gravida. Morbi mattis, dui ut aliquet lacinia, eros massa laoreet risus, quis iaculis mi justo vel ex. Suspendisse in sagittis est. Nunc pretium elementum libero, hendrerit mollis ipsum faucibus non. Suspendisse pretium diam ut libero facilisis, non interdum diam sagittis. In id congue metus. Etiam commodo tellus quis elit tincidunt, ut posuere diam tempor. Etiam aliquet dolor quis velit dapibus finibus. Vivamus consectetur neque dolor, ut laoreet nibh condimentum sed. Pellentesque commodo, tellus at volutpat suscipit, magna dolor ullamcorper turpis, sit amet tincidunt purus erat nec lorem. Suspendisse accumsan eros vitae tellus pharetra, eu egestas augue egestas. Integer finibus tortor arcu, eu molestie est lobortis viverra. Duis pretium orci ante, nec porta urna lacinia sed. ",
                            User = user1
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "In hac habitasse platea dictumst. Pellentesque accumsan libero non vehicula convallis. Pellentesque sed laoreet risus. Sed ornare mauris in dictum rhoncus. Proin porta vitae ligula a tempor. Aenean pretium pulvinar lacinia. Nunc placerat feugiat felis, sodales fermentum turpis convallis eget. Sed quis nunc ut turpis faucibus maximus ornare et orci. Integer nec dignissim turpis. Nunc sed fringilla quam.",
                            User = user1
                        }
                    }
                });

                _dataContext.Taxis.Add(new TaxiEntity
                {
                    Plaque = "THW321",
                    User = driver,
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Muy buen servicio",
                            User = user2
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Conductor muy amable",
                            User = user2
                        }
                    }
                });

                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckUserGroups(UserEntity user1, UserEntity user2, UserEntity user3, UserEntity user4)
        {
            if (!_dataContext.UserGroups.Any())
            {
                _dataContext.UserGroups.Add(new UserGroupEntity
                {
                    User = user1,
                    Users = new List<UserGroupDetailEntity>
            {
                new UserGroupDetailEntity { User = user2 },
                new UserGroupDetailEntity { User = user3 },
                new UserGroupDetailEntity { User = user4 }
            }
                });

                _dataContext.UserGroups.Add(new UserGroupEntity
                {
                    User = user2,
                    Users = new List<UserGroupDetailEntity>
            {
                new UserGroupDetailEntity { User = user1 },
                new UserGroupDetailEntity { User = user3 },
                new UserGroupDetailEntity { User = user4 }
            }
                });

                await _dataContext.SaveChangesAsync();
            }
        }

    }
}
