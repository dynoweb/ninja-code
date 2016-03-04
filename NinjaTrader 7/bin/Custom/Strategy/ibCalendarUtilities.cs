#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	partial class Strategy
    {
		public enum MarketName { US, UK, EU };
        public enum AssetClass { BOND, EQUITY, FX, FUTURES };
		
		#region calendarUtilities
		
		public char futureTickerMonthCode(int monthNumber)
        {
            switch (monthNumber)
            {
                case 1:
                    return ('F');
                case 2:
                    return ('G');
                case 3:
                    return ('H');
                case 4:
                    return ('J');
                case 5:
                    return ('K');
                case 6:
                    return ('M');
                case 7:
                    return ('N');
                case 8:
                    return ('Q');
                case 9:
                    return ('U');
                case 10:
                    return ('V');
                case 11:
                    return ('X');
                case 12:
                    return ('Z');
                default:
                    throw new ArgumentOutOfRangeException("Invalid month number");
            }
        }
		
       public int futuresCodeToMonthNumber(char ticker)
        {
            switch (ticker)
            {
                case 'F':
                    return (1);
                case 'G':
                    return (2);
                case 'H':
                    return (3);
                case 'J':
                    return (4);
                case 'K':
                    return (5);
                case 'M':
                    return (6);
                case 'N':
                    return (7);
                case 'Q':
                    return (8);
                case 'U':
                    return (9);
                case 'V':
                    return (10);
                case 'X':
                    return (11);
                case 'Z':
                    return (12);
                default:
                    return (1);
            }
        }
		
		public DateTime nthDayOfWeekInMonth(int year, int month, int nthDay, DayOfWeek dayOfWeek)
        {
            if ((month < 1) || (month > 12)) throw new ArgumentOutOfRangeException("Invalid month value.");

            if ((nthDay < 1) || (nthDay > 5)) throw new ArgumentOutOfRangeException("Invalid nthDay value.");

            // start from the first day of the month
            DateTime date = new DateTime(year, month, 1);

            // loop until we find our first matching day of the week
            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1);
            }

            if (date.Month != month)
                throw new ArgumentOutOfRangeException(month + " has less than " + nthDay.ToString() + " occurrences of " + dayOfWeek.ToString());

            // add days to returns the nth day of week
            date = date.AddDays((nthDay - 1) * 7);

            return (date.Date);
        }

        public List<DateTime> marketHolidaysInYear(MarketName marketName, AssetClass assetClass, int year)
        {
            switch (marketName)
            {
                case MarketName.US:
                    switch (assetClass)
                    {
                        case AssetClass.EQUITY:
                            return (yearlyEquityMarketHolidays_US(year));
                        case AssetClass.BOND:
                            return (yearlyBondMarketHolidays_US(year));
                        default:
                            return (yearlyEquityMarketHolidays_US(year));
                    }
                default:
                    return (yearlyEquityMarketHolidays_US(year));
            }
        }

        private DateTime dateOfEasterSunday(int year)
        {
            int day = 0;
            int month = 0;
            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day).Date;
        }

        public List<DateTime> yearlyEquityMarketHolidays_US(int year)
        {
            List<DateTime> holidayList = new List<DateTime>();

            #region fixedCalendarHolidays

            //New Years
            DateTime newYearsDay = new DateTime(year, 1, 1);

            if (newYearsDay.DayOfWeek == DayOfWeek.Sunday)
                holidayList.Add(new DateTime(year, 1, 2).Date);
            else
                holidayList.Add(newYearsDay.Date);

            //xmas
            DateTime xmas = new DateTime(year, 12, 25);

            if (xmas.DayOfWeek == DayOfWeek.Saturday)
                holidayList.Add(new DateTime(year, 12, 24).Date);
            if (xmas.DayOfWeek == DayOfWeek.Sunday)
                holidayList.Add(new DateTime(year, 12, 26).Date);
            else
                holidayList.Add(xmas.Date);

            //July 4th
            DateTime july4 = new DateTime(year, 7, 4);

            if (july4.DayOfWeek == DayOfWeek.Saturday)
                holidayList.Add(new DateTime(year, 7, 3).Date);
            if (july4.DayOfWeek == DayOfWeek.Sunday)
                holidayList.Add(new DateTime(year, 7, 5).Date);
            else
                holidayList.Add(july4.Date);

            #endregion

            #region variableCalendarHolidays
            //deals with holidays occuring on variable days (MLK, presidents day, good friday, memorial day, labor day, thanksgiving)

            DateTime mlk = nthDayOfWeekInMonth(year, 1, 3, DayOfWeek.Monday);
            holidayList.Add(mlk.Date);

            DateTime presDay = nthDayOfWeekInMonth(year, 2, 3, DayOfWeek.Monday);
            holidayList.Add(presDay.Date);

            //good friday
            DateTime easter = dateOfEasterSunday(year);
            DateTime goodFriday = easter.AddDays(-2);
            holidayList.Add(goodFriday.Date);

            //memorial day:  last monday in may
            DateTime firstMondayInJune = nthDayOfWeekInMonth(year, 6, 1, DayOfWeek.Monday);
            DateTime memDay = firstMondayInJune.AddDays(-7).Date;
            holidayList.Add(memDay.Date);

            //labor day: 1st monday is september
            DateTime laborDay = nthDayOfWeekInMonth(year, 9, 1, DayOfWeek.Monday);
            holidayList.Add(laborDay.Date);

            //thanksgiving: 4th thursday in novmeber
            DateTime thanksgiving = nthDayOfWeekInMonth(year, 11, 4, DayOfWeek.Thursday);
            holidayList.Add(thanksgiving.Date);

            #endregion

            holidayList.Sort();
            return (holidayList);
        }

        public List<DateTime> yearlyBondMarketHolidays_US(int year)
        {
            List<DateTime> holidayList = new List<DateTime>();

            //begin with US equity market holidays, then add columbus day and veterans day
            holidayList = yearlyEquityMarketHolidays_US(year);

            //2nd Monday of October
            DateTime columbusDay = nthDayOfWeekInMonth(year, 10, 2, DayOfWeek.Monday);
            holidayList.Add(columbusDay.Date);

            // november 11th 
            DateTime veteransDay = new DateTime(year, 11, 11);

            if (veteransDay.DayOfWeek == DayOfWeek.Saturday)
                holidayList.Add(new DateTime(year, 11, 10).Date);
            if (veteransDay.DayOfWeek == DayOfWeek.Sunday)
                holidayList.Add(new DateTime(year, 11, 12).Date);
            else
                holidayList.Add(veteransDay.Date);

            holidayList.Sort();
            return (holidayList);
        }

        public bool isBondMarketEarlyCloseDay(DateTime date)
        {
            //Thanksgiving
            DateTime thanksgiving     = nthDayOfWeekInMonth(date.Year, 11, 4, DayOfWeek.Thursday).Date;
            DateTime dayBeforeThxGvng = previousBusinessDate(thanksgiving, MarketName.US, AssetClass.BOND).Date;

            //xmas
            DateTime xmas = new DateTime(date.Year, 12, 25).Date;

            if (xmas.DayOfWeek == DayOfWeek.Saturday)  xmas = new DateTime(date.Year, 12, 24).Date;
            if (xmas.DayOfWeek == DayOfWeek.Sunday)    xmas = new DateTime(date.Year, 12, 26).Date;

            DateTime dayBeforeXmas = previousBusinessDate(xmas, MarketName.US, AssetClass.BOND).Date;

            if ((date.Date == dayBeforeThxGvng) || (date.Date == dayBeforeXmas)) return (true); 

            return (false); 
        }
		
		public bool isWeekend(DateTime date)
        {
            bool isWknd = false;

            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday)) isWknd = true;

            return (isWknd);
        }
		
        public bool isHoliday(DateTime date, MarketName marketName, AssetClass assetClass)
        {
            bool isHoliday = false;

            List<DateTime> holidayList = marketHolidaysInYear(marketName, assetClass, date.Year);

            isHoliday = holidayList.Contains(date.Date);

            return (isHoliday);
        }

        public bool isValidBusinessDay(DateTime date, MarketName marketName, AssetClass assetClass)
        {
            bool isValidDay = true;
            bool isHol = isHoliday(date, marketName, assetClass);

            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday) || (isHol == true)) isValidDay = false;

            return (isValidDay);
        }
		
		public DateTime addBusinessDaysToDate(DateTime date, int daysToAdd, MarketName marketName, AssetClass assetClass)
        {
            date = date.AddDays(1 * Math.Sign(daysToAdd)); //so as to not count the current day
            int bdCount = 0;

            DateTime priorDate = DateTime.MinValue;

            List<DateTime> holidays = marketHolidaysInYear(marketName, assetClass, date.Year);

            while (bdCount < Math.Abs(daysToAdd))
            {
                if ((holidays.Contains(date) == false) && (isWeekend(date) == false))
                {
                    bdCount++;
                }

                priorDate = date;
                date = date.AddDays(1 * Math.Sign(daysToAdd));
                if (date.Year != priorDate.Year) holidays = marketHolidaysInYear(marketName, assetClass, date.Year);
            }

            return (priorDate);
        }

		public DateTime previousBusinessDate(DateTime date, MarketName marketName, AssetClass assetClass)
        {
            return addBusinessDaysToDate(date, -1, marketName, assetClass);
        }
		
       	public int nextQuarterlyImmMonth(DateTime date, int numOccurrence, DayOfWeek dayOfWeek)
        {
            bool notPastImmDate = (date.Day < nthDayOfWeekInMonth(date.Year, date.Month, numOccurrence, dayOfWeek).Day);

            if (notPastImmDate == true)
            {
                if (date.Month % 3 == 0) return (date.Month);
                else
                {
                    int i = date.Month;
                    while (i % 3 != 0)
                    {
                        i++;
                    }
                    return (i);
                }
            }
            else
            {
                if (date.Month == 12) return (3);
                else
                {
                    int i = date.Month + 1;
                    while (i % 3 != 0)
                    {
                        i++;
                    }
                    return (i);
                }
            }
        }
	
        public DateTime nextContractExpirationDate(DateTime date, bool quarterly, int numOccurrence, DayOfWeek dayOfWeek)
        {
            if ((numOccurrence < 1) || (numOccurrence > 5)) throw new ArgumentOutOfRangeException("numOccurrences must be between 1 and 5.");

            DateTime nextExpiry = DateTime.MinValue;
            DateTime nthOccurDate = nthDayOfWeekInMonth(date.Year, date.Month, numOccurrence, dayOfWeek);

            if ((date.Day < nthOccurDate.Day) && ((quarterly == false) || (date.Month % 3 == 0)))
                nextExpiry = nthOccurDate;

            if ((date.Day < nthOccurDate.Day) && (quarterly == true))
            {
                int immMonth = nextQuarterlyImmMonth(date, numOccurrence, dayOfWeek);
                int immYear = date.Year;
                nextExpiry = new DateTime(immYear, immMonth, nthDayOfWeekInMonth(immYear, immMonth, numOccurrence, dayOfWeek).Day);
            }

            if ((date.Day >= nthOccurDate.Day) && (quarterly == false))
                if (date.Month < 12)
                {
                    nextExpiry = new DateTime(date.Year, date.Month + 1, nthDayOfWeekInMonth(date.Year, date.Month + 1, numOccurrence, dayOfWeek).Day);
                }
                else
                {
                    nextExpiry = new DateTime(date.Year + 1, 1, nthDayOfWeekInMonth(date.Year + 1, 1, numOccurrence, dayOfWeek).Day);
                }

            if ((date.Day >= nthOccurDate.Day) && (quarterly == true))
                if (date.Month < 12)
                {
                    int immMonth = nextQuarterlyImmMonth(date, numOccurrence, dayOfWeek);
                    nextExpiry = new DateTime(date.Year, immMonth, nthDayOfWeekInMonth(date.Year, immMonth, numOccurrence, dayOfWeek).Day);
                }
                else
                {
                    nextExpiry = new DateTime(date.Year + 1, 3, nthDayOfWeekInMonth(date.Year + 1, 3, numOccurrence, dayOfWeek).Day);
                }

            return (nextExpiry);
        }
		
		public AssetClass getInstrumentAssetClass(string cntPrefix)
		{
			switch (cntPrefix)
			{
				case "YM":
				case "NQ":
				case "ES":
				case "TF":
					return(AssetClass.EQUITY); 
					
				case "CL":
					return(AssetClass.BOND); 
					
				default:
					return(AssetClass.EQUITY); 
			}
		}
		
		public MarketName getInstrumentMarketName(string cntPrefix)
		{
			switch (cntPrefix)
			{
				case "YM":
				case "NQ":
				case "ES":
				case "TF":
				case "CL":
					return(MarketName.US); 
				default:
					return(MarketName.US);
			}
		}
		
		public DateTime futuresLastTradingDate(DateTime anchorDate, string cntPrefix, MarketName mktNm, AssetClass asstCls) 
		{			
			switch (cntPrefix)
			{
				case "YM":
				case "NQ":
				case "ES":
				case "TF":	
					return(this.nextContractExpirationDate(anchorDate, true, 3, DayOfWeek.Wednesday)); 
				
				case "CL":
					DateTime settleDate;
					
					if (anchorDate.Month != 1) 
					{
						settleDate = new DateTime(anchorDate.Year, anchorDate.Month - 1, 25);  //subtract 1 from month because CL trades 1 month ahead (i.e. Aug contract expires Jul 25)
					}
					else
					{
						settleDate = new DateTime(anchorDate.Year - 1, 12, 25);
					}
					
					bool isValid = isValidBusinessDay(settleDate, mktNm, asstCls);

					if (!isValid)
					{
						DateTime prevBD = this.previousBusinessDate(settleDate, mktNm, asstCls);
						settleDate = prevBD; 
					}

					return(this.addBusinessDaysToDate(settleDate, -3, mktNm, asstCls)); 
					
				default:
					string dataMsg = String.Format("{1} is not a supported futures contract. Cannot determine contract expiration. ", cntPrefix); 
					Print(dataMsg); 
					DrawTextFixed("dataMsg", dataMsg, TextPosition.Center, Color.Navy, new Font("Arial", 12), Color.LightGray, Color.LightGreen, 6);
					return(DateTime.Today); 
			}
		}
		
		#endregion //closes calendarUtilities	
    }
}
