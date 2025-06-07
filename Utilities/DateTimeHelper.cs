namespace NutriBest.Server.Utilities
{
    using System.Web;

    public static class DateTimeHelper
    {
        public static (DateTime? startDate, DateTime? endDate) ParseDates(string? startDate, string? endDate)
        {
            string decodedStartDate = HttpUtility.UrlDecode(startDate ?? "");
            string decodedEndDate = HttpUtility.UrlDecode(endDate ?? "");

            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            if (!string.IsNullOrEmpty(decodedStartDate))
            {
                if (DateTime.TryParse(decodedStartDate, out DateTime tempStartDate))
                {
                    parsedStartDate = tempStartDate;
                }
            }

            if (!string.IsNullOrEmpty(decodedEndDate))
            {
                if (DateTime.TryParse(decodedEndDate, out DateTime tempEndDate))
                {
                    parsedEndDate = tempEndDate;
                }
            }

            return (parsedStartDate, parsedEndDate);
        }
    }
}
