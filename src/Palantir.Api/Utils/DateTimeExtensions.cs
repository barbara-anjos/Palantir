namespace Palantir.Api.Utils
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// List of national holidays with fixed day (can be expanded as needed)
		/// </summary>
		private static readonly List<DateTime> Holidays = new List<DateTime>
		{
			new DateTime(DateTime.Now.Year, 1, 1),  // New Year
			new DateTime(DateTime.Now.Year, 4, 21), // Tiradentes
			new DateTime(DateTime.Now.Year, 5, 1),  // Labor Day
			new DateTime(DateTime.Now.Year, 9, 7),  // Independence Day
			new DateTime(DateTime.Now.Year, 10, 12),// Nossa Senhora Aparecida
			new DateTime(DateTime.Now.Year, 11, 2), // All Souls' Day
			new DateTime(DateTime.Now.Year, 11, 15),// Republic Proclamation Day
			new DateTime(DateTime.Now.Year, 12, 25) // Christmas
		};

		/// <summary>
		/// Returns the due date of a task based on the number of working hours and SLA.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="slaHours"></param>
		/// <returns></returns>
		public static DateTime WorkingHours(this DateTime dateTime, int slaHours)
		{
			const int workingHours = 8;

			//Check the SLA hours amount
			while (slaHours > 0)
			{
				if (dateTime.IsBusinessDay())
				{
					if (slaHours <= workingHours)
					{
						dateTime = dateTime.AddHours(slaHours);
						slaHours = 0;
					}
					else
					{
						dateTime = dateTime.AddHours(workingHours);
						slaHours -= workingHours;
					}
				}

				// If the due date is on a weekend or holiday, add a day
				dateTime = dateTime.AddDays(1).Date;
			}

			return dateTime;
		}

		/// <summary>
		/// Returns whether a given date is a business day.
		/// </summary>
		public static bool IsBusinessDay(this DateTime data)
		{
			return data.DayOfWeek != DayOfWeek.Saturday
				   && data.DayOfWeek != DayOfWeek.Sunday
				   && !Holidays.Contains(data.Date);
		}
	}
}
