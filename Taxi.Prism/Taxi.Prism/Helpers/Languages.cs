using Taxi.Prism.Interfaces;
using Taxi.Prism.Resources;
using Xamarin.Forms;

namespace Taxi.Prism.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string Accept => Resource.Accept;

        public static string ConnectionError => Resource.ConnectionError;

        public static string Error => Resource.Error;

        public static string PlaqueError1 => Resource.PlaqueError1;

        public static string PlaqueError2 => Resource.PlaqueError2;

        public static string TaxiHistory => Resource.TaxiHistory;
        public static string AdminUserGroup => Resource.AdminUserGroup;
        public static string Login => Resource.Login;
        public static string ModifyUser => Resource.ModifyUser;
        public static string NewTrip => Resource.NewTrip;
        public static string ReportIncident => Resource.ReportIncident;
        public static string SeeTaxiHistory => Resource.SeeTaxiHistory;
        public static string Menu => Resource.Menu;
        public static string TaxiQualified => Resource.TaxiQualified;
    }
}
