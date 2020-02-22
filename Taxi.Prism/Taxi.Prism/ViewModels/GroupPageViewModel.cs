using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class GroupPageViewModel : ViewModelBase
    {
        public GroupPageViewModel(INavigationService navigationService): base(navigationService)
        {
            Title = Languages.AdminUserGroup;
        }
    }
}
