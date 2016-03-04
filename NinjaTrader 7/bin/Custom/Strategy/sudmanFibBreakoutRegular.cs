#region Using declarations
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
using pasb = PriceActionSwing.Base;
#endregion

namespace NinjaTrader.Strategy
{
    public class sudmanFibBreakoutRegular : Strategy
    {
		#region enums
		public enum ExitReason {EndOfFirstSession, EndOfSession, StopLoss, TakeProfit, TrailExit, None}; 
		public enum OrderPurpose {Flatten, HandleRejection, NewEntry, StopExit, None}; 
		public enum StrategyTimeZone {Pacific, Mountain, Central, Eastern, UTC};
		public enum MarketName { US, UK, EU };
        public enum AssetClass { BOND, EQUITY, FX, FUTURES };
		public enum BarData {Open, High, Low, Close, Volume}; 
		public enum ExtremaType {Max, Min}; 
		public enum HistoricalData {YdyOpen, YdyHigh, YdyLow, YdyClose, ONHigh, ONLow, IBHigh, IBLow}; 
		public enum ValueAreaData {Y_POC, Y_VAH, Y_VAL, Y_VWAP}; 
		public enum SwingClass {DoubleBottom, DoubleTop, HigherHigh, HigherLow, LowerHigh, LowerLow, None}; 
		public enum PendingEntryType {Long, Short, None }; 
		public enum FibLevels {_0, _236, _270, _382, _400, _500, _618, _700, _764, _880, _1000, _1270, _1380, _1500, _1618}; 
		
		private static readonly Dictionary<string, OrderPurpose> dictionary = new Dictionary<string, OrderPurpose>
		{
			{"Flatten", OrderPurpose.Flatten},
			{"HandleRejection", OrderPurpose.HandleRejection},
			{"NewEntry", OrderPurpose.NewEntry},
			{"None", OrderPurpose.None}, 
			{"StopExit", OrderPurpose.StopExit},
		};
		#endregion //closes enums
		
		#region parameters
		
		private TimeSpan pitSessionOpenTime		 	= new TimeSpan (09,00,00);
		private TimeSpan pitSessionCloseTime		= new TimeSpan (14,30,00);
		private TimeSpan pitSessionEarlyCloseTime	= new TimeSpan (13,30,00);
		private TimeSpan initialBalanceStartTime	= new TimeSpan (09,00,00);
		private TimeSpan initialBalanceEndTime	    = new TimeSpan (10,00,00);
		
		private TimeSpan firstSessionStartTime 		= new TimeSpan (09,01,00); 
		private TimeSpan firstSessionEndTime 		= new TimeSpan (12,00,00);
		private TimeSpan secondSessionStartTime 	= new TimeSpan (12,00,00); 
		private TimeSpan secondSessionEndTime 		= new TimeSpan (14,30,00);
		private TimeSpan endOfNewEntriesTime		= new TimeSpan (14,15,00);
		private TimeSpan maxTimeToEnterTrade		= new TimeSpan (00,04,00); 
		private TimeSpan lineDuration				= new TimeSpan (00,04,00);	
		
		private StrategyTimeZone stratTimeZone 		= StrategyTimeZone.Eastern;
		private bool exitAtMarketClose 				= true;									//Force exit of position at end of session
		
		private string stratNamePrefix 				= "(FibR)-";
		private uint numContracts 					= 2;
		private uint numBizDaysToRollBeforeExpiry   = 1; 
		private uint consecutiveWLForTradingHalt 	= 3;
		private uint numTrianglePoints				= 2; 
		private double valueAreaPercent 			= 0.70d; 
		
		private uint swingStrength					= 5; 
		private uint dtbStrength 					= 10;
		private pasb.SwingStyle swingStyle 			= pasb.SwingStyle.Standard;
		private bool useCloseValues					= false; 
		
		private uint ticksFromPullbackToEnter		= 4; 
		
		private FibLevels fibLevels_Entry       	= FibLevels._500;
		private FibLevels fibLevels_TakeProfit		= FibLevels._382;
		private FibLevels fibLevels_StopLoss		= FibLevels._618;
		
		private double percentTakeProfit_Trigger1	= 0.40d; 
		private double percentTakeProfit_Trigger2	= 0.50d;
		private double percentTakeProfit_Trigger3	= 0.90d;
		
		private double percentTakeProfit_Exit1		= 0.00d;
		private double percentTakeProfit_Exit2		= 0.25d;
		private double percentTakeProfit_Exit3		= 0.80d;

		private double stepInterval					= 0.25d; 
		private uint minStopLossTicks				= 10; 
		
		private bool useTrailingStops 				= true; 
		
		#endregion //closes parameters
			
		#region globalVariables
		
		private Indicator.PriceActionSwingPro pas;  

		private IOrder entryOrder = null, basicExitOrder = null, rejectionExitOrder = null; 
		private OrderState updatedOrderState = OrderState.Unknown; 
							
		private string stratName, futuresCode;
		private double instrumentPointValue = 1.0d, positionPnl, dollarPnl, sessionPnl, maxSessionPnl;
		private double tickSize, currentPrice, positionOpenPrice, entryPrice, entryAdjust, fibLevelEntry, fibLevelTarget, fibLevelStop, distanceBtwSwings;
		private double takeProfit, stopLoss, trailTrigger1, trailTrigger2, trailTrigger3, trailExit1, trailExit2, trailExit3;
		
		private int numDecimals, openContracts, prevOpenCont, entryContracts, tradeCount = 1, subTradeCount, fillCount, numTrail, exitLevel, trianglePoints;
		private int barCount, swingCount, triangleCnt, tCntAtEntry, consecutiveWinners, consecutiveLosers, pasTextOffset = 15, ptLabelOffset, lineWidth = 2, textBars = 10, pixelOffset = 5; 
			
		private DateTime session1StartTime, session1EndTime, session2StartTime, session2EndTime, lastNewEntryTime, eventTime, orderSubmissionTime;
		private DateTime o_nOpen, o_nClose, ibStart, ibEnd; 
		private TimeSpan timeZoneOffset;
		
		private bool isFirstTick = true, canSendNewEntry = true, autoScale = false, exitOrderSent, hasFirstTriangleFormed;
		private bool isTradingHalted, hasTerminated, hasSession1Ended, hasSession2Ended, hitHighestTrail, firstPlot = true;
		private Color triColorLong = Color.Green, triColorShort = Color.Red, triColorNone = Color.Blue, histLineColor = Color.Cyan, segmentColor, textColorPas1 = Color.Black;
		private DashStyle dStyle = DashStyle.Solid;
		private ILine entryPointLine, takeProfitLine, stopExitLine; 
			
		private readonly object _object 		= new object(); 
		private List<double> sessionPnlList 	= new List<double>(); 
		private List<double> dollarPnlList 		= new List<double>(); 
		private List<double> positionPnlList 	= new List<double>(); 
		private List<SwingClass> swingsList     = new List<SwingClass>(); 
		private List<double> swingPrices		= new List<double>(); 
		private List<int> barCountAtSwing		= new List<int>(); 
		private List<double> trailTriggerList   = new List<double>();
		private List<double> trailExitList      = new List<double>(); 
	
		private ExitReason exitRsn 				= ExitReason.None; 
		private OrderPurpose oPurpose 			= OrderPurpose.None;  
		private OrderAction orderAction			= OrderAction.Buy;  
		MarketName mktName 						= MarketName.US;
		AssetClass assetCls 					= AssetClass.BOND; 
		PendingEntryType pendingEntry 			= PendingEntryType.None;
		
		private Dictionary<HistoricalData, double> histData = new Dictionary<HistoricalData, double>(); 
		private Dictionary<ValueAreaData, double> valAreaData = new Dictionary<ValueAreaData, double>(); 

		#endregion //globalVariables

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
					DateTime settleDate = new DateTime(anchorDate.Year, anchorDate.Month - 1, 25);  //subtract 1 from month because CL trades 1 month ahead (i.e. Aug contract expires Jul 25)
		
					bool isValid = this.isValidBusinessDay(settleDate, mktName, asstCls);

					if (!isValid)
					{
						DateTime prevBD = this.previousBusinessDate(settleDate, mktNm, asstCls);
						settleDate = prevBD; 
					}

					return(this.addBusinessDaysToDate(settleDate, -3, mktNm, asstCls)); 
					
				default:
					string dataMsg = String.Format("{0} {1} is not a supported futures contract. Cannot determine contract expiration. ", stratName, cntPrefix); 
					Print(dataMsg); 
					DrawTextFixed("dataMsg", dataMsg, TextPosition.Center, Color.Navy, new Font("Arial", 12), Color.LightGray, Color.LightGreen, 6);
					return(DateTime.Today); 
			}
		}
		
		#endregion //closes calendarUtilities
		
		#region historicalData
		
		//GetBar returns the number of bars ago the DateTime occurred.  
		//Bars.GetBar returns the number of the bar that occurred at the DateTime.
		//Use of Bars.GetBar necessitates subtracting from CurrentBar to get the number of bars ago. 
		//CaptureTime should be adjusted for the local time zone before being passed to method. 
		public double getBarDataAtTime(int barInProgress, DateTime captureTime, BarData bd)
		{
			int barNumber  = Bars.GetBar(captureTime);
			int barsAgo    = CurrentBars[barInProgress] - barNumber; 
			
			switch (bd)
			{
				case BarData.Open:
					return(Opens[barInProgress][barsAgo]); 
				case BarData.High:
					return(Highs[barInProgress][barsAgo]); 
				case BarData.Low:
					return(Lows[barInProgress][barsAgo]); 
				case BarData.Close:
					return(Closes[barInProgress][barsAgo]); 
				case BarData.Volume:
					return(Volumes[barInProgress][barsAgo]); 
				default:
					return(0.0); 
			}
		}
		
		//startTime and endTime should be adjusted for the local time zone before being passed to method. 
		public double getMaxOrMinBetweenTimes(int barInProgress, DateTime startTime, DateTime endTime, ExtremaType et)
		{
			int barNumber    = Bars.GetBar(startTime);
			int barsAgoStart = CurrentBars[barInProgress] - barNumber; 
			
			barNumber        = Bars.GetBar(endTime);
			int barsAgoEnd   = CurrentBars[barInProgress] - barNumber; 
			
			double extremeValue = 0.0d; 
			if (et == ExtremaType.Max)
			{
				extremeValue = Double.MinValue;
				for(int j = barsAgoStart; j >= barsAgoEnd; j--)
				{
					if(extremeValue < Highs[barInProgress][j]) extremeValue = Highs[barInProgress][j];
				}
				
			}
			if (et == ExtremaType.Min)
			{
				extremeValue = Double.MaxValue;
				for(int j = barsAgoStart; j >= barsAgoEnd; j--)
				{
					if(extremeValue > Lows[barInProgress][j])  extremeValue = Lows[barInProgress][j];
				}
			}
	
			return(extremeValue); 
		}
		
		//startTime and endTime should be adjusted for the local time zone before being passed to method. 
		public Dictionary<double, double> getPriceAndVolumeBetweenTimes(int barInProgress, DateTime startTime, DateTime endTime)
		{
			int barNumber    = Bars.GetBar(startTime);
			int barsAgoStart = CurrentBars[barInProgress] - barNumber; 
			
			barNumber        = Bars.GetBar(endTime);
			int barsAgoEnd   = CurrentBars[barInProgress] - barNumber; 
		
			Dictionary<double, double> d = new Dictionary<double, double>(); 
			double volume; 
			for(int j = barsAgoStart; j >= barsAgoEnd; j--)
			{
				if(d.TryGetValue(Closes[1][j], out volume))
				{
					d[Closes[1][j]] = volume + Volumes[1][j]; 
				}
				else
				{
					d.Add(Closes[1][j], Volumes[1][j]); 
				}
			}
			
			return(d); 
		}
		
		public double getPointOfControl(Dictionary<double, double> d)
		{
			if (d.Count() == 0) return(0.0);
			
			double dPoc = 0; 
			double maxVolume = Double.MinValue; 
			foreach (KeyValuePair<double, double> kvp in d)
			{
				if (kvp.Value > maxVolume)
				{
					maxVolume = kvp.Value;
					dPoc = kvp.Key; 
				}
			}	
			return(dPoc); 
		}
		
		public double getVWAP(Dictionary<double, double> d, double sumVolume)
		{		
			if (d.Count() == 0) return(0.0); 
			
			double sumPrd = 0;
			foreach (KeyValuePair<double, double> kvp in d)
			{
				sumPrd = sumPrd + (kvp.Key * kvp.Value); 
			}	
			return(sumPrd/sumVolume); 
		}
		
		public KeyValuePair<double, double> getValueAreaHighAndLow(double poc, double totalVolume, double valueAreaPercent, List<double> prices, List<double> volumes)
		{
			if ((prices.Count == 0) || (volumes.Count == 0) || (prices.Count != volumes.Count) || (poc == 0.0) ) return(new KeyValuePair<double, double>(0.0, 0.0)); 
			
			int numBrackets  = 2; 
			int pocIdx 		 = prices.IndexOf(poc); 
			int upIdx  		 = pocIdx;
			int dnIdx  		 = pocIdx; 
			
			double vaVolume  = volumes[pocIdx]; //start with the volume at POC
			double targetVol = totalVolume * valueAreaPercent; 
			double upVol     = 0.0d;
			double dnVol     = 0.0d; 
			
			bool isTargetHit = false; 
			
			while (vaVolume < targetVol)
			{
				if ((!isTargetHit) && (upIdx < (volumes.Count - numBrackets)) && (dnIdx >= numBrackets))
				{
					upVol = volumes[upIdx + 1] + volumes[upIdx + 2]; 
					dnVol = volumes[dnIdx + 1] + volumes[dnIdx + 2]; 
					
					if ( ((vaVolume + upVol) >= targetVol) || ((vaVolume + dnVol) >= targetVol)) 
					{
						isTargetHit = true; 
					}
					else
					{
						if (upVol > dnVol)
						{
							upIdx+=numBrackets; 
							vaVolume+=upVol; 
						}
						else if (upVol < dnVol)
						{
							dnIdx-=numBrackets; 
							vaVolume+=dnVol; 
						}
						else if ((vaVolume + upVol + dnVol) >= targetVol)
						{
							isTargetHit = true; 
						}
						else
						{
							upIdx+=numBrackets; 
							dnIdx-=numBrackets;
							vaVolume+=(upVol + dnVol); 
						}	
					}
				}
				else
				{
					if (upIdx == (prices.Count - (numBrackets - 1)))
					{
						vaVolume+=volumes[dnIdx - (numBrackets - 1)]; 	
						dnIdx-=(numBrackets - 1); 
					}
					else if (dnIdx == 0)
					{
						vaVolume+=volumes[upIdx - (numBrackets - 1)]; 
						upIdx+=(numBrackets - (numBrackets - 1)); 
					}
					else
					{
						upVol = volumes[upIdx + (numBrackets - 1)];
						dnVol = volumes[dnIdx - (numBrackets - 1)];
						
						if (upVol > dnVol)
						{
							upIdx+=(numBrackets - 1); 
							vaVolume+=upVol;
						}
						else if (upVol < dnVol)
						{
							dnIdx-=numBrackets; 
							vaVolume+=dnVol; 
						}
						else
						{
							upIdx+=(numBrackets - 1); 
							dnIdx-=(numBrackets - 1); 
							vaVolume+=(upVol + dnVol);
						}
					}	
				}
			}

			double vaHigh = prices[upIdx];  
			double vaLow  = prices[dnIdx]; 
			
			return(new KeyValuePair<double, double>(vaHigh, vaLow)); 
		}
		
		public void retrieveHistoricalData(DateTime currentTime, TimeSpan tzOffset, int barInProgress)
		{
			TimeSpan sundayOpen   = new TimeSpan(18,00,00); 
			
			o_nClose    = currentTime.Date.Add(this.pitSessionOpenTime).AddSeconds(-1).Subtract(tzOffset); //one minute before today's pit session opens
			ibStart     = currentTime.Date.Add(this.initialBalanceStartTime).Subtract(tzOffset); 
			ibEnd 		= currentTime.Date.Add(this.initialBalanceEndTime).Subtract(tzOffset);
			
			histData.Add(HistoricalData.IBHigh, Double.MinValue);
			histData.Add(HistoricalData.IBLow,  Double.MaxValue);
			
			DateTime priorDay;
			
			if (currentTime.DayOfWeek == DayOfWeek.Monday)
			{
				priorDay = currentTime.AddDays(-3).Date; //if prior day is Sunday, use pit session from Friday 
				o_nOpen  = priorDay.Add(sundayOpen).Subtract(tzOffset); 
			} 
			else
			{
				priorDay = currentTime.AddDays(-1).Date; 
				o_nOpen  = priorDay.Date.Add(this.pitSessionCloseTime).AddSeconds(1).Subtract(tzOffset);   //one minute after yesterday's pit close
			}
			
			DateTime priorPitOpen = priorDay.Date.Add(this.pitSessionOpenTime).AddMinutes(1).Subtract(tzOffset); //add one minute since the timestamp refers to the time a bar closed
			DateTime priorPitCls  = priorDay.Date.Add(this.pitSessionCloseTime).Subtract(tzOffset); 
			if (this.isBondMarketEarlyCloseDay(priorDay.Date)) priorPitCls  = priorDay.Date.Add(this.pitSessionEarlyCloseTime).Subtract(tzOffset); 
			
			double keyValue       = getBarDataAtTime(barInProgress, priorPitOpen, BarData.Open); 
			histData.Add(HistoricalData.YdyOpen, keyValue); 
			
			keyValue			  = getBarDataAtTime(barInProgress, priorPitCls, BarData.Close);
			histData.Add(HistoricalData.YdyClose, keyValue);
			
			keyValue			  = this.getMaxOrMinBetweenTimes(barInProgress, priorPitOpen, priorPitCls, ExtremaType.Max);
			histData.Add(HistoricalData.YdyHigh, keyValue);
			
			keyValue			  = this.getMaxOrMinBetweenTimes(barInProgress, priorPitOpen, priorPitCls, ExtremaType.Min);
			histData.Add(HistoricalData.YdyLow, keyValue);
			
			keyValue			  = this.getMaxOrMinBetweenTimes(barInProgress, o_nOpen, o_nClose, ExtremaType.Max);
			histData.Add(HistoricalData.ONHigh, keyValue);
			
			keyValue			  = this.getMaxOrMinBetweenTimes(barInProgress, o_nOpen, o_nClose, ExtremaType.Min);
			histData.Add(HistoricalData.ONLow, keyValue);
			
			if (currentTime >= this.ibStart)
			{
				keyValue = this.getMaxOrMinBetweenTimes(BarsInProgress, ibStart, ibEnd, ExtremaType.Max);
				histData[HistoricalData.IBHigh] = keyValue;
			
				keyValue = this.getMaxOrMinBetweenTimes(BarsInProgress, ibStart, ibEnd, ExtremaType.Min);
				histData[HistoricalData.IBLow]  = keyValue;
			}
			
			this.plotHistoricalLines(currentTime, histData); 
			
			//populate tick and volume data list for ydy pit session.  Get POC, VWAP, VAH, VAL
			Dictionary<double, double> ydyPitData = new Dictionary<double, double>(); 
			ydyPitData = this.getPriceAndVolumeBetweenTimes(barInProgress, priorPitOpen, priorPitCls); 
			
			double totalVolume = ydyPitData.Sum(x => x.Value); 
			
			valAreaData[ValueAreaData.Y_POC]  = this.getPointOfControl(ydyPitData); 
			valAreaData[ValueAreaData.Y_VWAP] = this.getVWAP(ydyPitData, totalVolume); 
			
			ydyPitData = ydyPitData.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);   //sort by price
			List<double> pxLevels = ydyPitData.Keys.ToList(); 
			List<double> volumes  = ydyPitData.Values.ToList(); 
			
			KeyValuePair<double, double> vaHighAndLow = this.getValueAreaHighAndLow(valAreaData[ValueAreaData.Y_POC], totalVolume, this.valueAreaPercent, pxLevels, volumes); 
			valAreaData[ValueAreaData.Y_VAH] = vaHighAndLow.Key;
			valAreaData[ValueAreaData.Y_VAL] = vaHighAndLow.Value;
			 
			this.updateValueArea(); 
		}
				
		public void plotHistoricalLines(DateTime timeNow, Dictionary<HistoricalData, double> hd)
		{		
			this.updateHistoricalPlots(HistoricalData.YdyOpen, "Y-Open",  histData[HistoricalData.YdyOpen]); 
			this.updateHistoricalPlots(HistoricalData.YdyHigh, "Y-High",  histData[HistoricalData.YdyHigh]); 
			this.updateHistoricalPlots(HistoricalData.YdyLow,  "Y-Low",   histData[HistoricalData.YdyLow]); 
			this.updateHistoricalPlots(HistoricalData.YdyClose,"Y-Cls",   histData[HistoricalData.YdyClose]); 
			this.updateHistoricalPlots(HistoricalData.ONHigh,  "O/N-High",histData[HistoricalData.ONHigh]); 
			this.updateHistoricalPlots(HistoricalData.ONLow,   "O/N-Low", histData[HistoricalData.ONLow]); 

			if (timeNow >= this.ibStart)
			{
				this.updateHistoricalPlots(HistoricalData.IBHigh, "IB-High", histData[HistoricalData.IBHigh]); 
				this.updateHistoricalPlots(HistoricalData.IBLow,  "IB-Low",  histData[HistoricalData.IBLow]); 
			}
		}
		
		public void updateValueArea()
		{
			this.drawValueArea("Y-POC",  valAreaData[ValueAreaData.Y_POC]);
			this.drawValueArea("Y-VAH",  valAreaData[ValueAreaData.Y_VAH]); 
			this.drawValueArea("Y-VAL",  valAreaData[ValueAreaData.Y_VAL]);  
			this.drawValueArea("Y-VWAP", valAreaData[ValueAreaData.Y_VWAP]);
		}
		
		public void drawValueArea(string drawTag, double pValue)
		{
			DrawLine(drawTag + "_line", autoScale, textBars * 2, pValue, -1, pValue, Color.DarkSlateBlue, dStyle, lineWidth); 
			DrawText(drawTag, autoScale, drawTag, textBars * 2, pValue, pixelOffset, Color.Black, new Font("Arial", 6), StringAlignment.Near, Color.Aqua, Color.Lime, 5);	
		}
		
		public void updateHistoricalPlots(HistoricalData hd, string drawTag, double pValue)
		{
			DrawLine(hd.ToString(), autoScale, textBars, pValue, -1, pValue, histLineColor, dStyle, lineWidth); 
			DrawText(drawTag, autoScale, drawTag, textBars, pValue, pixelOffset, Color.Black, new Font("Arial", 6), StringAlignment.Near, Color.Aqua, Color.Lime, 5);	
		}
		
		#endregion //closes historicalData
		
		#region userMethods_TradingTimes
		
		private TimeSpan getOffsetBetweenTimeZones(DateTime timeToConvert, string fromZone, string toZone )
		{
			DateTime specTime = DateTime.SpecifyKind(timeToConvert.Date, DateTimeKind.Unspecified);
			DateTime destTime = this.convertSourceTimeToDestinationTime(specTime, fromZone, toZone);
			return (destTime - specTime);
		}

		private DateTime convertSourceTimeToDestinationTime(DateTime timeToConvert, 
			string sourceTimeZoneID, string destinationTimeZoneID)
		{
			try
			{
					DateTime destinationTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(timeToConvert, 
						sourceTimeZoneID, destinationTimeZoneID);
					return (destinationTime);
			}
			catch
			{
					return (timeToConvert);
			}
		}
		
		public string mapStrategyTimeZoneToWindowsID(StrategyTimeZone stz)
        {
            switch (stz)
            {
                case StrategyTimeZone.Pacific: 		return ("Pacific Standard Time");
          		case StrategyTimeZone.Mountain: 	return ("Mountain Standard Time");
          		case StrategyTimeZone.Central: 		return ("Central Standard Time");
          		case StrategyTimeZone.Eastern: 		return ("Eastern Standard Time");
          		case StrategyTimeZone.UTC: 			return ("UTC");
          		default: 							return ("UTC");
            }
        }
		
		private void adjustForTimeZones(DateTime currentTime)
		{
			StrategyTimeZone myTZ 	= stratTimeZone;
			string stratTzString 	= this.mapStrategyTimeZoneToWindowsID(myTZ);
			timeZoneOffset 			= getOffsetBetweenTimeZones(DateTime.Now, TimeZoneInfo.Local.Id.ToString(), stratTzString);
			
			Print(String.Format("{0} Local time zone: {1}", stratName, TimeZoneInfo.Local.Id ));
			Print(String.Format("{0} Strategy time zone: {1}", stratName, stratTzString));

			session1StartTime 	= currentTime.Date.Add(this.firstSessionStartTime); 
			session1EndTime     = currentTime.Date.Add(this.firstSessionEndTime); 
			session2StartTime 	= currentTime.Date.Add(this.secondSessionStartTime); 
			session2EndTime     = currentTime.Date.Add(this.secondSessionEndTime); 
			lastNewEntryTime    = currentTime.Date.Add(this.endOfNewEntriesTime); 
			
			Print(String.Format("{0} Session 1 start time: {1:MMM d yyyy HH:mm}. Session 1 end time: {2:T}", stratName, session1StartTime, session1EndTime ));
			Print(String.Format("{0} Session 2 start time: {1:T}. Last entry allowed at: {2:T}. Session 2 end time: {3:T}", stratName, session2StartTime, lastNewEntryTime, session2EndTime));
		}
		
		private bool isBlockedTime(DateTime dt)
		{
			return(false); 
		}
		
		#endregion //closes userMethods_TradingTimes
		
		#region positionExitTests
		
		public ExitReason onTickExitTests(IPosition position)
		{	
			#region calculateCurrentPnl
			positionPnl = Math.Round(Instrument.MasterInstrument.Round2TickSize((currentPrice - positionOpenPrice)), numDecimals) * Math.Sign(openContracts);
			
			double inTradeDollarPnl = (Math.Abs(openContracts) * positionPnl * instrumentPointValue); //use ABS because positionPnl already reflects market direction 
			double inTradeSessionPnl = sessionPnl + inTradeDollarPnl;
			
 			positionPnlList.Add(positionPnl);  
			double maxPositionPnl    = positionPnlList.Max();
			
			#endregion //closes calculateCurrentPnl
			
			if (positionPnl >= takeProfit)	 return(ExitReason.TakeProfit);
			if (positionPnl <= -stopLoss) 	 return(ExitReason.StopLoss);
			
			#region trailingStops
			if (useTrailingStops)
			{
				if ((!hitHighestTrail) && (numTrail == (trailTriggerList.Count - 1)))
				{
					if (positionPnl >= trailTriggerList[numTrail])
					{
						hitHighestTrail = true; 
						adjustStopForTrail(position, numTrail); 
						exitLevel = numTrail; 
					}
				}
				else
				{
					for (int i= numTrail; i < trailTriggerList.Count - 1; i++)
					{
						if ( (positionPnl >= trailTriggerList[i]) && (positionPnl < trailTriggerList[i+1]))
						{
							adjustStopForTrail(position, i); 
							exitLevel = i; 
							numTrail  = i+1; 
							break; 
						}
					}
				}
				
				if ((numTrail != 0) && (positionPnl <= trailExitList[exitLevel])) return(ExitReason.TrailExit);
			}			
			#endregion //closes trailingStops
			
			if ((eventTime > this.session1EndTime) && (eventTime < this.session2StartTime)) return(ExitReason.EndOfFirstSession); 
			
			if ((exitAtMarketClose == true) && (eventTime >= this.session2EndTime)) return(ExitReason.EndOfSession); 
		
			return(ExitReason.None); 
		}

		public void adjustStopForTrail(IPosition position, int trailLevel)
		{
			int tickPnl 		= (int)(trailTriggerList[trailLevel]/tickSize);
			this.RemoveDrawObject(stopExitLine);
			double stopLevel 	= positionOpenPrice + (Math.Sign(openContracts) * trailExitList[trailLevel]);
			
			Print(String.Format("{0} {1} position in {2} has hit trigger level {3} of {4} ticks. Stop moved to {5:N2} at {6:T}.", 
				stratName, position.MarketPosition, Instrument.FullName, (trailLevel + 1), tickPnl, stopLevel, eventTime )); 
			
			stopExitLine		= DrawLine("stopExitLine", autoScale, eventTime.Subtract(timeZoneOffset), stopLevel, 
									eventTime.Subtract(timeZoneOffset).Add(this.lineDuration), stopLevel, Color.Red, DashStyle.Solid, 2); 	
		}
		#endregion //positionExitTests
	
		#region parsingStatements
		
		public string returnStringTitleCase(string str)
		{
			string sOut = string.Empty; 	
			sOut = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
			return(sOut); 
		}
			
		public string returnSignalName(OrderPurpose op, OrderAction oa, int trdNum, int subTrdNum)
		{
			string sOut = string.Empty;
			
			string actionToPosition = string.Empty;	
			actionToPosition = MarketPosition.Long.ToString();
			if (oa == OrderAction.Sell) actionToPosition = MarketPosition.Short.ToString();
			
			sOut = stratName + op.ToString() + "-" + actionToPosition + "-Trd:" + trdNum.ToString("N0") ; 
			return(sOut); 
		}
		
		public OrderPurpose extractOrderPurposeFromSignalName(string signalName, char delimiter, int itemNumber)
		{
			try
			{
				string[] sArray = signalName.ToString().Split(delimiter);
				string x 		= sArray[itemNumber];	

				OrderPurpose op;
				OrderPurpose op1 = OrderPurpose.None;
    			if (dictionary.TryGetValue(x.Trim(), out op)) op1 = op; 
				return(op1); 
			}
			catch
			{
				return(OrderPurpose.None); 	
			}	
		}
		
		public string extractTrdAndSubTrdNumFromSignalName(string signalName, int itemNumber)
		{
			try
			{
				string[] digits = System.Text.RegularExpressions.Regex.Split(signalName, @"\D+");
				return(digits[itemNumber]);	
			}
			catch
			{
				return(string.Empty); 	
			}
		}
		
		public void printOrderStateString(IExecution ex, int trdNum, int subTrdNum, OrderPurpose op)
		{
			string sPx 			= ex.Order.AvgFillPrice.ToString("N2");
			
			OrderState os 		= updatedOrderState; //ex.Order.OrderState; 
			string oState 		= os.ToString().ToUpper();
			string oStateTitle 	= returnStringTitleCase(oState);
			MarketPosition ps 	= MarketPosition.Long;
			if (openContracts < 0) ps = MarketPosition.Short; 
			string psString 	= ps.ToString();
			if (openContracts == 0) psString = MarketPosition.Flat.ToString(); 
			
			Print(String.Format("{0} {1} {2} order [TrdNum: {3} SubTrd: {4}] to {5} {6} at {7:T}. ", 
				stratName, oState, ex.Order.OrderType, trdNum, subTrdNum, ex.Order.OrderAction, Instrument.FullName, eventTime));
			Print(String.Format("{0} Order purpose: {1}. Order price: {2:N2}. {3} qty: {4}. Current market position: {5} {6}.", 
				stratName, op, ex.Order.AvgFillPrice, os, ex.Quantity, ps, Math.Abs(openContracts))); 
			Print(String.Empty);
			
			if (ex.Order.OrderState == OrderState.Cancelled)
			{
				if (ex.Order == entryOrder)  entryOrder = null;
			}
		}
		
		public void printPositionClosingMsg()
		{
			sessionPnl += dollarPnl;
			sessionPnlList.Add(sessionPnl);
			maxSessionPnl = sessionPnlList.DefaultIfEmpty(0.0).Max(); 
			
			double maxTradePnl = positionPnlList.DefaultIfEmpty(0.0).Max() * instrumentPointValue * this.numContracts;
			double minTradePnl = positionPnlList.DefaultIfEmpty(0.0).Min() * instrumentPointValue * this.numContracts;

			Print(String.Empty);
			Print(String.Format("{0} Closed position in {1} [Trade number: {2}] at {3:T}. Exit reason: {4}.",
				stratName, Instrument.FullName, tradeCount, eventTime,exitRsn));
			Print(String.Format("{0} Dollar pnl: {1:N2}. Max dollar pnl: {2:N2}. Min dollar pnl: {3:N2}. Session pnl: {4:N2}.", 
				stratName, dollarPnl, maxTradePnl, minTradePnl, sessionPnl)); 
			Print(String.Empty);
			
			if ((consecutiveWinners >= this.consecutiveWLForTradingHalt) || (consecutiveLosers >= this.consecutiveWLForTradingHalt)) isTradingHalted = true; 
			if (isTradingHalted) 
			{
				if (entryOrder != null)  processCanceledOrder(entryOrder, "TrdHalt");
				
				Print(String.Format("{0} {1} trading is halted at {2:T}. Consecutive winning trades: {3}. Consecutive losing trades: {4}.", 
					stratName, Instrument.FullName, eventTime, consecutiveWinners, consecutiveLosers));
				Print(String.Empty);
			}
		}	
		#endregion //closes parsingStatements
		
		#region orderProcessing
		
		public int adjustOpenContracts(int alreadyOpenContracts, int qtyFilled, OrderAction oa)
		{
			int factor = 1;
			if (oa == OrderAction.Sell) factor = -1;
			int adjustment = factor * qtyFilled; 
			
			return(alreadyOpenContracts + adjustment);
		}
		
		public void submitNewEntryOrder(OrderAction orderAction, double orderPrice)
		{
			canSendNewEntry = false;
			string msg 		= returnSignalName(OrderPurpose.NewEntry, orderAction, tradeCount, subTradeCount); 
			entryOrder 		= SubmitOrder(0, orderAction, OrderType.Limit, Convert.ToInt32(this.numContracts), orderPrice, 0, String.Empty, msg); 
		}
		
		public void processCanceledOrder(IOrder cxlOrder, string cxlReason)
		{
			double orderPx = cxlOrder.LimitPrice;
			if ((cxlOrder.OrderType == OrderType.Stop) || (cxlOrder.OrderType == OrderType.StopLimit)) orderPx = cxlOrder.StopPrice; 
			
			Print(String.Format("{0} Order to {1} at {2} (submitted at: {3:T}) canceled at: {4:T}. Reason: {5}", 
				stratName, cxlOrder.OrderAction, orderPx, orderSubmissionTime, eventTime, cxlReason));
			canSendNewEntry = true; 
			CancelOrder(cxlOrder);
			
			if (cxlOrder == entryOrder) entryOrder  = null;
		}
		
		public void flattenPositions(string reason)
		{
			if (Position.MarketPosition != MarketPosition.Flat)
			{
				OrderAction oa = OrderAction.Buy; 
				if (Position.MarketPosition == MarketPosition.Long) oa = OrderAction.Sell; 
				SubmitOrder(0, oa, OrderType.Market, Position.Quantity, 0, 0, String.Empty, reason);	
			}	
		}
		
		public void processMarketOrders(IExecution ex, int trdNum, int subTrdNum, OrderPurpose op)
		{		
			if (ex.Order == entryOrder)
			{
				//capture number of entry contracts filled.  Return signed value adjusted for market direction.
				entryContracts = ex.Order.Filled; 
				if (ex.Order.OrderAction == OrderAction.Sell) entryContracts *=-1; 
				openContracts = entryContracts;
				positionOpenPrice 	   = ex.Order.AvgFillPrice;
				
				if (fillCount == 0) 
				{
					tradeCount++;
					fillCount++;
					exitOrderSent 	   = false;	
				}
				
				this.RemoveDrawObject(entryPointLine);
				this.RemoveDrawObject(takeProfitLine); 
				this.RemoveDrawObject(stopExitLine); 
				
				#region setExitLevels
				takeProfit             = distanceBtwSwings * (this.fibLevelEntry - this.fibLevelTarget); 
				takeProfit 			   = Instrument.MasterInstrument.Round2TickSize(Math.Max((this.minStopLossTicks * tickSize), takeProfit)); 
				stopLoss 	    	   = distanceBtwSwings * (this.fibLevelStop - this.fibLevelEntry);
				stopLoss               = Instrument.MasterInstrument.Round2TickSize(Math.Max((this.minStopLossTicks * tickSize), stopLoss)); 
				
				trailTrigger1          = Instrument.MasterInstrument.Round2TickSize(this.percentTakeProfit_Trigger1 * takeProfit);
				trailTrigger2          = Instrument.MasterInstrument.Round2TickSize(this.percentTakeProfit_Trigger2 * takeProfit);
				trailTrigger3          = Instrument.MasterInstrument.Round2TickSize(this.percentTakeProfit_Trigger3 * takeProfit);
				
				trailExit1             = Instrument.MasterInstrument.Round2TickSize(this.percentTakeProfit_Exit1 * takeProfit);
				trailExit2             = Instrument.MasterInstrument.Round2TickSize(this.percentTakeProfit_Exit2 * takeProfit);
				trailExit3             = Instrument.MasterInstrument.Round2TickSize(this.percentTakeProfit_Exit3 * takeProfit);
				
				double distToTrigger3  = this.percentTakeProfit_Trigger3 - this.percentTakeProfit_Trigger2; 
				double distToExit3     = this.percentTakeProfit_Exit3 - this.percentTakeProfit_Exit2; 
				
				double pnlToTrigger3   = Instrument.MasterInstrument.Round2TickSize(takeProfit * distToTrigger3); 
				double pnlToExit3      = Instrument.MasterInstrument.Round2TickSize(takeProfit * distToExit3); 
				
				double stepInPnl	   = Instrument.MasterInstrument.Round2TickSize(Math.Max(tickSize, (this.stepInterval * pnlToTrigger3))); 
				
				int numIntervals       = Math.Max(1, (int)(pnlToTrigger3/stepInPnl) - 1);
				
				trailTriggerList.Add(trailTrigger1);
				trailTriggerList.Add(trailTrigger2);
				
				trailExitList.Add(trailExit1);
				trailExitList.Add(trailExit2);
				
				double trigg, exit; 
				for (int i = 0; i < numIntervals; i++)
				{
					trigg = trailTrigger2 + (stepInPnl * (i + 1));
					exit  = trailExit2 + (stepInPnl * (i + 1));
					trailTriggerList.Add(Instrument.MasterInstrument.Round2TickSize(trigg)); 
					trailExitList.Add(Instrument.MasterInstrument.Round2TickSize(exit)); 
				}
				
				trailTriggerList.Add(trailTrigger3);
				trailExitList.Add(trailExit3);		
				#endregion //closes setExitLevels
				
				double tpLevel 			= positionOpenPrice + (Math.Sign(openContracts) * takeProfit);
				double stopLevel		= positionOpenPrice - (Math.Sign(openContracts) * stopLoss);
				double trExit1 			= positionOpenPrice + (Math.Sign(openContracts) * trailExit1);
				double trExit2 			= positionOpenPrice + (Math.Sign(openContracts) * trailExit2);
				double trExit3 			= positionOpenPrice + (Math.Sign(openContracts) * trailExit3);
				
				takeProfitLine  		= DrawLine("takeProfitLine", autoScale, eventTime.Subtract(timeZoneOffset), tpLevel, 
											eventTime.Subtract(timeZoneOffset).Add(this.lineDuration), tpLevel, Color.Green, DashStyle.Solid, 2); 
				stopExitLine			= DrawLine("stopExitLine", autoScale, eventTime.Subtract(timeZoneOffset), stopLevel, 
											eventTime.Subtract(timeZoneOffset).Add(this.lineDuration), stopLevel, Color.Red, DashStyle.Solid, 2);  
				
				int tCnt = tradeCount - 1; 
				if (this.useTrailingStops)
				{
					Print(String.Format("{0} TrdNum: {1}. Direction: {2}. Take profit at {3:N2}. Stop loss at {4:N2}. Trail Exit1 at {5:N2}. Trail Exit2 at {6:N2}. Trail Exit3 at {7:N2}", 
						stratName, tCnt, ex.MarketPosition, tpLevel, stopLevel, trExit1, trExit2, trExit3)); 
				}
				else
				{
					Print(String.Format("{0} Trade number: {1}. Direction: {2}. Take profit at {3:N2}. Stop loss at {4:N2}. Trailing stops not used.", 
						stratName, tCnt, ex.MarketPosition, tpLevel, stopLevel)); 	
				} 
			}
			else
			{	//return the number of open contracts (returns signed value adjusted for market direction)
				prevOpenCont = openContracts; 
				openContracts = this.adjustOpenContracts(openContracts, ex.Quantity, ex.Order.OrderAction);
			}

			this.printOrderStateString(ex, trdNum, subTrdNum, op); 
			
			#region handleExits
			//if executed order is an exit order
			if ((ex.Order == basicExitOrder) || (ex.Order == rejectionExitOrder)) 
			{	
				dollarPnl += ( -1 * (openContracts - prevOpenCont) * (ex.Price - positionOpenPrice) * instrumentPointValue); 
				dollarPnlList.Add(dollarPnl); 
				
				#region exitMgmt
				//done if the executed order is not a new entry
				if (op != OrderPurpose.NewEntry) 
				{	
					if ((openContracts == 0) || (Position.MarketPosition == MarketPosition.Flat) )
					{	
						exitOrderSent = false; 
						this.checkForConsecutiveWinsOrLosses(dollarPnl);
						this.printPositionClosingMsg(); 
						this.resetFlags(); 
					}
				}
				#endregion //closes exitMgmt
			}
			#endregion //closes handleExits
			
			#region processFullyFilledOrders
			//if the order is completely filled, set orders to null to allow future orders to be assigned
			if ((entryOrder != null) && (ex.Order == entryOrder)  && (Math.Abs(openContracts) == this.numContracts)) entryOrder = null;
			if ((rejectionExitOrder != null) && (ex.Order == rejectionExitOrder) && (Position.MarketPosition == MarketPosition.Flat)) rejectionExitOrder = null;
			if ((basicExitOrder != null) && (ex.Order == basicExitOrder) && (Position.MarketPosition == MarketPosition.Flat)) basicExitOrder = null;
			#endregion //closes processFullyFilledOrders
		}
		
		public OrderAction reversePosition(Position p)
        {
            OrderAction oa = OrderAction.Buy; 
            if (p.MarketPosition == MarketPosition.Long) oa = OrderAction.Sell;

            return (oa);
		}
		
		public OrderAction reversePosition(IOrder order)
        {
            OrderAction oa = OrderAction.Buy; 
            if (order.OrderAction == OrderAction.Buy) oa = OrderAction.Sell;

            return (oa);
		}
		
		public void resetFlags()
		{	
			exitOrderSent 			 = false; 			// resets the flag to allow future exits 
			canSendNewEntry 		 = true;
			
			dollarPnlList.Clear();
			positionPnlList.Clear(); 		// clears the position pnl list for use in the next entry
			trailTriggerList.Clear();
			trailExitList.Clear(); 
			
			openContracts 			 = 0; 
			prevOpenCont 			 = 0; 
			entryContracts 			 = 0;
			subTradeCount   		 = 0;
			dollarPnl 				 = 0; 
			positionPnl 			 = 0;				// resets the position PNL for the next trade
			fillCount 				 = 0;
			numTrail 				 = 0; 
			exitLevel 				 = 0; 
			updatedOrderState 		 = OrderState.Unknown;
			exitRsn					 = ExitReason.None;
			hitHighestTrail          = false; 
			pendingEntry  			 = PendingEntryType.None;
			
			this.RemoveDrawObject(takeProfitLine); 
			this.RemoveDrawObject(stopExitLine); 
			this.RemoveDrawObject(entryPointLine); 
		}
		
		public void checkForConsecutiveWinsOrLosses(double pnl)
		{
			if (pnl > 0) 
			{
				consecutiveWinners++;	
				consecutiveLosers = 0;
			}
			else if (pnl < 0) 
			{
				consecutiveWinners = 0;	
				consecutiveLosers++;
			}
			else
			{
				consecutiveWinners = 0;	
				consecutiveLosers  = 0;
			}
			Print(String.Format("{0} Consecutive winners: {1}. Consecutive losers: {2}.", stratName, consecutiveWinners, consecutiveLosers));
		}
		
		#endregion //closes orderProcessing
						
		#region dataIntegrity
		
		private void validateParameters()
		{
			if (Instrument.MasterInstrument.InstrumentType == InstrumentType.Future)
			{
				instrumentPointValue = Instrument.MasterInstrument.PointValue; 
				
				string codePrefix = futuresCode.Substring(0,2); //get first two letters of code (i.e. CLM5 returns CL)
				
				assetCls = this.getInstrumentAssetClass(codePrefix);
				mktName  = this.getInstrumentMarketName(codePrefix); 
				
				int mnth = this.futuresCodeToMonthNumber(Convert.ToChar(futuresCode.Substring(2,1)));
				int yr   = Instrument.Expiry.Year; 
				
				DateTime anchorDate  = new DateTime(yr, mnth, 1); 
				DateTime lastTrdDate = this.futuresLastTradingDate(anchorDate, codePrefix, mktName, assetCls); 
				DateTime dateToRoll  = this.addBusinessDaysToDate(lastTrdDate, -1 * Convert.ToInt32(this.numBizDaysToRollBeforeExpiry), mktName, assetCls);  
				
				Print(String.Format("{0} Last trading date for {1} futures contract is {2:MMMM d yyyy}.", stratName, codePrefix, lastTrdDate)); 
				Print(String.Format("{0} Contract should be rolled forward on or before {1:MMMM d yyyy}.", stratName, dateToRoll)); 
				Print(String.Empty);
	
				if (DateTime.Now.Add(this.timeZoneOffset).Date >= dateToRoll)
				{
					string dataMsg = String.Format("{0} Contract is either unsupported or within {1} days of expiration.  Roll to next contract.", 
						stratName, this.numBizDaysToRollBeforeExpiry); 
					Print(dataMsg); 	
					DrawTextFixed("dataMsg", dataMsg, TextPosition.BottomRight, Color.Navy, new Font("Arial", 12), Color.LightGray, Color.LightGreen, 6);
				}
			}
	
			if (this.session1StartTime.Second != 0)
				Print(String.Format("{0} Session1 start time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.firstSessionStartTime.Seconds != 0)
				Print(String.Format("{0} Session1 end time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.secondSessionStartTime.Seconds != 0)
				Print(String.Format("{0} Session2 start time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.secondSessionEndTime.Seconds != 0)
				Print(String.Format("{0} Session2 end time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.endOfNewEntriesTime.Seconds != 0)
				Print(String.Format("{0} Last new entry time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.pitSessionOpenTime.Seconds != 0)
				Print(String.Format("{0} Pit session open time must be a round number of minutes (seconds must equal zero).",stratName));	
			
			if (this.pitSessionCloseTime.Seconds != 0)
				Print(String.Format("{0} Pit session close time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.pitSessionEarlyCloseTime.Seconds != 0)
				Print(String.Format("{0} Pit session early close time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.initialBalanceStartTime.Seconds != 0)
				Print(String.Format("{0} Initial balance start time must be a round number of minutes (seconds must equal zero).",stratName));
			
			if (this.initialBalanceEndTime.Seconds != 0)
				Print(String.Format("{0} Initial balance end time must be a round number of minutes (seconds must equal zero).",stratName));
	
			if (this.pitSessionCloseTime.CompareTo(this.pitSessionOpenTime) != 1)
				Print(String.Format("{0} Pit session close time must be strictly greater than pit session open time.",stratName));
			
			if (this.pitSessionCloseTime.CompareTo(this.pitSessionEarlyCloseTime) != 1)
				Print(String.Format("{0} Pit session close time must be strictly greater than pit session early close time.",stratName));
			
			if (this.initialBalanceEndTime.CompareTo(this.initialBalanceStartTime) != 1)
				Print(String.Format("{0} Initial balance start time must be strictly greater than initial balance end time.",stratName));
			
			if (this.firstSessionEndTime.CompareTo(this.firstSessionStartTime) != 1)
				Print(String.Format("{0} Session1 start time must be strictly less than Session1 end time.",stratName));
			
			if (this.secondSessionEndTime.CompareTo(this.secondSessionStartTime) != 1)
				Print(String.Format("{0} Session2 start time must be strictly less than Session2 end time.",stratName));
			
			if (this.secondSessionStartTime.CompareTo(this.firstSessionEndTime) == -1)
				Print(String.Format("{0} Session2 start time must be greater than or equal to Session1 end time.",stratName));
			
			if (this.endOfNewEntriesTime.CompareTo(this.secondSessionEndTime) == 1)
				Print(String.Format("{0} Last new entry time must be less than or equal to Session2 end time.",stratName));
	
			if ((this.valueAreaPercent <= 0) || (this.valueAreaPercent > 1.0d) )
				Print(String.Format("{0} valueAreaPercent must be greater than zero and less than or equal to 1.0.", stratName));
			
			if (this.fibLevelEntry <= this.fibLevelTarget)
				Print(String.Format("{0} fibLevelEntry must be greater than or equal to fibLevelTarget.", stratName));
			
			if (this.fibLevelEntry >= this.fibLevelStop)
				Print(String.Format("{0} fibLevelEntry must be greater than or equal to fibLevelStop.", stratName));
			
			if ((this.percentTakeProfit_Trigger1 <= 0) || (this.percentTakeProfit_Trigger1 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Trigger1 must be greater than zero and less than or equal to 1.0.", stratName));

			if ((this.percentTakeProfit_Trigger2 <= 0) || (this.percentTakeProfit_Trigger2 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Trigger2 must be greater than zero and less than or equal to 1.0.", stratName));
			
			if ((this.percentTakeProfit_Trigger3 <= 0) || (this.percentTakeProfit_Trigger3 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Trigger3 must be greater than zero and less than or equal to 1.0.", stratName));
			
			if ((this.percentTakeProfit_Exit1 < 0) || (this.percentTakeProfit_Exit1 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Exit1 must be greater than or equal to zero and less than or equal to 1.0.", stratName));

			if ((this.percentTakeProfit_Exit2 <= 0) || (this.percentTakeProfit_Exit2 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Exit2 must be greater than zero and less than or equal to 1.0.", stratName));
			
			if ((this.percentTakeProfit_Exit3 <= 0) || (this.percentTakeProfit_Exit2 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Exit3 must be greater than zero and less than or equal to 1.0.", stratName));
			
			if ((this.stepInterval <= 0) || (this.stepInterval > 1.0d) )
				Print(String.Format("{0} stepInterval must be greater than zero and less than or equal to 1.0.", stratName));
			
			if (this.percentTakeProfit_Trigger2 <= this.percentTakeProfit_Trigger1)
				Print(String.Format("{0} percentTakeProfit_Trigger2 ({1:N2}) must be strictly greater than percentTakeProfit_Trigger1 ({2:N2}).",
					stratName, this.percentTakeProfit_Trigger2, this.percentTakeProfit_Trigger1));
			
			if (this.percentTakeProfit_Trigger3 <= this.percentTakeProfit_Trigger2)
				Print(String.Format("{0} percentTakeProfit_Trigger3 ({1:N2}) must be strictly greater than percentTakeProfit_Trigger2 ({2:N2}).",
					stratName, this.percentTakeProfit_Trigger3, this.percentTakeProfit_Trigger2));
			
			if (this.percentTakeProfit_Exit2 <= this.percentTakeProfit_Exit1)
				Print(String.Format("{0} percentTakeProfit_Exit2 ({1:N2}) must be strictly greater than percentTakeProfit_Exit1 ({2:N2}).",
					stratName, this.percentTakeProfit_Exit2, this.percentTakeProfit_Exit1));
			
			if (this.percentTakeProfit_Exit3 <= this.percentTakeProfit_Exit2)
				Print(String.Format("{0} percentTakeProfit_Exit3 ({1:N2}) must be strictly greater than percentTakeProfit_Exit2 ({2:N2}).",
					stratName, this.percentTakeProfit_Exit3, this.percentTakeProfit_Exit2));
			
			if (this.percentTakeProfit_Trigger1 <= this.percentTakeProfit_Exit1)
				Print(String.Format("{0} percentTakeProfit_Trigger1 ({1:N2}) must be strictly greater than percentTakeProfit_Exit1 ({2:N2}).",
					stratName, this.percentTakeProfit_Trigger1, this.percentTakeProfit_Exit1));
			
			if (this.percentTakeProfit_Trigger2 <= this.percentTakeProfit_Exit2)
				Print(String.Format("{0} percentTakeProfit_Trigger2 ({1:N2}) must be strictly greater than percentTakeProfit_Exit2 ({2:N2}).",
					stratName, this.percentTakeProfit_Trigger2, this.percentTakeProfit_Exit2));
			
			if (this.percentTakeProfit_Trigger3 <= this.percentTakeProfit_Exit3)
				Print(String.Format("{0} percentTakeProfit_Trigger3 ({1:N2}) must be strictly greater than percentTakeProfit_Exit3 ({2:N2}).",
					stratName, this.percentTakeProfit_Trigger3, this.percentTakeProfit_Exit3));			
		}
		
		#endregion //closes dataIntegrity
		
		#region triangleRules
		
		public void addPointLabel(int pointNumber, int barNumber, int barsAgo, SwingClass sc, double swingPx)
		{
			Color ptColor    = this.triColorLong;
			int offsetFactor = -1; 
			
			if (   (sc == SwingClass.DoubleTop) 
				|| (sc == SwingClass.HigherHigh) 
				|| (sc == SwingClass.LowerHigh) )
			{
				ptColor      = this.triColorShort;
				offsetFactor = 1; 
			}
			
			DrawText("Pt_" + barNumber, autoScale, "P" + pointNumber, barsAgo, swingPx, ptLabelOffset * offsetFactor, 
				ptColor, new Font("Arial", 10),StringAlignment.Center, Color.LightGray, Color.LightGray, 6); 
		}
		
		public void updateLastTrianglePoint(int pointNumber, int barNumber, int barsAgo, SwingClass sc, double swingPx)
		{	
			this.RemoveDrawObject("Pt_" + barCountAtSwing[pointNumber - 1]); 
			swingsList[pointNumber - 1]      = sc;  
			swingPrices[pointNumber - 1]     = swingPx;
			barCountAtSwing[pointNumber - 1] = barNumber;
			addPointLabel(pointNumber, barNumber, barsAgo, sc, swingPx);		
		}

		#endregion //closes triangleRules
		
		#region priceActionSwings
		
		public SwingClass getSwingClass(Indicator.PriceActionSwingPro p, int barsBack)
		{
			// if Swing High....if CurRelation = 1, HH. If 0, DT. If - 1, LH
			// if Swing Low ....if CurRelation = 1, HL. If 0, DB. If - 1, LL
			// Swing High/Low CurPrice gives price of current swing
			if (p.HasNewSwingLow[barsBack] || p.HasUpdatedSwingLow[barsBack])
			{
				if (p.SwingLowRelation[barsBack] == 1)   return(SwingClass.HigherLow); 
				if (p.SwingLowRelation[barsBack] == 0)   return(SwingClass.DoubleBottom); 
				if (p.SwingLowRelation[barsBack] == -1)  return(SwingClass.LowerLow); 
			}
			
			if (p.HasNewSwingHigh[barsBack] || p.HasUpdatedSwingHigh[barsBack])
			{
				if (p.SwingHighRelation[barsBack] == 1)  return(SwingClass.HigherHigh); 
				if (p.SwingHighRelation[barsBack] == 0)  return(SwingClass.DoubleTop); 
				if (p.SwingHighRelation[barsBack] == -1) return(SwingClass.LowerHigh); 
			}
			return(SwingClass.None); 
		}
		
		public double getSwingPrice(Indicator.PriceActionSwingPro p, SwingClass sc, int barsBack)
		{
			switch (sc)
			{
				case SwingClass.DoubleBottom:
					return(p.DoubleBottom[barsBack]);
				case SwingClass.DoubleTop:
					return(p.DoubleTop[barsBack]); 
				case SwingClass.HigherHigh:
					return(p.HigherHigh[barsBack]); 
				case SwingClass.HigherLow:
					return(p.HigherLow[barsBack]); 
				case SwingClass.LowerHigh:
				 	return(p.LowerHigh[barsBack]);
				case SwingClass.LowerLow:
					return(p.LowerLow[barsBack]); 
				case SwingClass.None:
					return(0.0d); 
				default:
					return(0.0d); 
			}	
		}
		
		#endregion //closes priceActionSwings
		
		#region entryRules
		
		public double getFibLevelFromEnum(FibLevels fLevel)
		{
			switch (fLevel)
			{
				case FibLevels._0:	
					return(0.0d); 
				case FibLevels._236:	
					return(0.236d); 
				case FibLevels._270:	
					return(0.27d); 
				case FibLevels._382:	
					return(0.382d); 		
				case FibLevels._400:	
					return(0.40d); 
				case FibLevels._500:	
					return(0.50d); 
				case FibLevels._618:	
					return(0.618d); 
				case FibLevels._700:	
					return(0.70d); 
				case FibLevels._764:	
					return(0.764d); 
				case FibLevels._880:	
					return(0.88d);
				case FibLevels._1000:	
					return(1.0d); 
				case FibLevels._1270:	
					return(1.27d); 
				case FibLevels._1380:	
					return(1.38d); 
				case FibLevels._1500:	
					return(1.5d); 
				case FibLevels._1618:	
					return(1.618d);
				default:
					return(0.50d); 
			}
		}
		
		public PendingEntryType getEntryType(List<SwingClass> scList)
		{
			if (   (scList[0] == SwingClass.DoubleBottom) 
				|| (scList[0] == SwingClass.HigherLow) 
				|| (scList[0] == SwingClass.LowerLow) )  return(PendingEntryType.Long); 
			
			if (   (scList[0] == SwingClass.DoubleTop) 
				|| (scList[0] == SwingClass.HigherHigh) 
				|| (scList[0] == SwingClass.LowerHigh) ) return(PendingEntryType.Short); 
			
			return(PendingEntryType.None);
		}
		
		public double getEntryOrderPrice(OrderAction oa, List<double> swingPxs) 
		{
			double pullBackLevel = 0.0;
			
			if (oa == OrderAction.Buy)
			{
				pullBackLevel = swingPxs[1] - (this.fibLevelEntry * distanceBtwSwings); 
				return(pullBackLevel + entryAdjust); 
			}
			if (oa == OrderAction.Sell)
			{
				pullBackLevel = swingPxs[1] + (this.fibLevelEntry * distanceBtwSwings); 
				return(pullBackLevel - entryAdjust); 
			}
			
			return(pullBackLevel); 
		}
		
		public void sendNewOrder(DateTime barStartTime)
		{
			if (entryOrder == null)
			{
				if ((!isBlockedTime(eventTime))
					&& (((barStartTime >= this.session1StartTime) && (barStartTime < this.session1EndTime))
						|| ((barStartTime >= this.session2StartTime) && (barStartTime <= this.lastNewEntryTime))) )	
				{
					if ((!isTradingHalted) && (hasFirstTriangleFormed))
					{
						if ((canSendNewEntry) && (Position.MarketPosition == MarketPosition.Flat) )
						{
							if ( pendingEntry == PendingEntryType.Long) 
							{
								orderAction          = OrderAction.Buy;
								entryPrice           = getEntryOrderPrice(orderAction, swingPrices);  
								segmentColor         = this.triColorLong;
							}
							
							if ( pendingEntry == PendingEntryType.Short) 
							{
								orderAction          = OrderAction.Sell;
								entryPrice           = getEntryOrderPrice(orderAction, swingPrices);
								segmentColor         = this.triColorShort;
							}
							
							if ( pendingEntry != PendingEntryType.None) 
							{
								triangleCnt++;
								tCntAtEntry = triangleCnt; 
								
								this.submitNewEntryOrder(orderAction, entryPrice); 
								orderSubmissionTime  = eventTime; 
								
								Print(String.Format("{0} {1} order to {2} will be placed at {3:N2}. Time: {4:T}.", 
									stratName, OrderType.Limit, orderAction, entryPrice, eventTime));
								
								entryPointLine		 = DrawLine("entryPointLine", autoScale, eventTime.Subtract(timeZoneOffset), entryPrice, 
									eventTime.Subtract(timeZoneOffset).Add(this.lineDuration), entryPrice, Color.Blue, DashStyle.Solid, 2);
								
								DrawLine("L1_" + triangleCnt, autoScale, barCount - barCountAtSwing[0], swingPrices[0], 
									barCount - barCountAtSwing[1], swingPrices[1], segmentColor, DashStyle.Solid, 2);   			//connects P1 to P2
							}
						}
					}
				}
			}	
		}
		
		#endregion //closes entryRules
				
        protected override void Initialize()
        {
			EntriesPerDirection 	= 1;
    		EntryHandling 			= EntryHandling.AllEntries;
			Unmanaged 				= true; 
			CalculateOnBarClose 	= true; 
			ExitOnClose 			= exitAtMarketClose;
			
			DisconnectDelaySeconds 	= 20;
			ConnectionLossHandling 	= ConnectionLossHandling.KeepRunning;
			MaxRestartMinutes 		= 2;
			MaxRestartAttempts 		= 500; 
			
			Add(PeriodType.Tick, 1);
			
			#region priceActionSwingSettings
			pas = PriceActionSwingPro(Convert.ToInt32(this.dtbStrength), Convert.ToInt32(this.swingStrength), this.swingStyle, this.useCloseValues);
			pas.CalculateOnBarClose    = true; 
			pas.PaintPriceMarkers 	   = false;
			pas.ShowSwingLabel 		   = true; 
			pas.ShowSwingPercent 	   = false;
			pas.ShowSwingPrice  	   = false; 
			pas.ShowEntryArrows        = false;
			pas.ShowSwingSwitch		   = false;
			pas.ShowDivergenceHidden   = false;
			pas.ShowDivergenceRegular  = false;
			pas.ShowNakedSwings        = false; 
			pas.SwingDurationType 	   = pasb.SwingDurationStyle.False;
			pas.SwingLengthType 	   = pasb.SwingLengthStyle.False; 
			pas.SwingTimeType 		   = pasb.SwingTimeStyle.False; 
			pas.SwingVolumeType 	   = pasb.SwingVolumeStyle.False;
			pas.VisualizationType 	   = pasb.VisualizationStyle.Dots; 
			pas.RiskManagementPosition = pasb.RiskManagementStyle.False;
			pas.AbcPattern			   = pasb.AbcPatternMode.False; 
			pas.TextColorHigherHigh    = this.textColorPas1; 
			pas.TextColorHigherLow     = this.textColorPas1; 
			pas.TextColorDoubleTop     = this.textColorPas1;  
			pas.TextColorLowerHigh 	   = this.textColorPas1; 
			pas.TextColorLowerLow 	   = this.textColorPas1; 
			pas.TextColorDoubleBottom  = this.textColorPas1; 
			pas.TextOffsetLabel 	   = this.pasTextOffset;
			pas.DisplayInDataBox 	   = true; 
			#endregion //closes priceActionSwingSettings
			
			Add(pas); 
		}
		
		protected override void OnBarUpdate()
        {	
			eventTime = DateTime.Now.Add(timeZoneOffset);
			if (Historical) eventTime = Time[0].Add(timeZoneOffset);
			DateTime barStartTime = new DateTime(eventTime.Year, eventTime.Month, eventTime.Day, eventTime.Hour, eventTime.Minute, 0);
			
			try
			{					
				if (Time[0].Date == DateTime.Today.Date)
				{
					#region firstTickCode
					if (isFirstTick)
					{
						isFirstTick = false; 
						
						string instCode = Instrument.MasterInstrument.Name;
						char monthCode  = this.futureTickerMonthCode(Instrument.Expiry.Month); 
						string yrString = Instrument.Expiry.Year.ToString().Substring(3,1); 
						string code     = instCode + monthCode + yrString; 
						futuresCode     = instCode + monthCode + yrString; 
						
						stratName       = stratNamePrefix; 
						stratName 	    = String.Format("{0}[{1}]-({2}{3})_{4}_{5}-", stratNamePrefix, code, 
							BarsPeriods[0].Value, BarsPeriods[0].Id.ToString().Substring(0,1).ToUpper(), "A", useTrailingStops.ToString()[0]);
						
						tickSize       = Instrument.MasterInstrument.TickSize; 
						numDecimals    = tickSize.ToString().Substring(tickSize.ToString().IndexOf(".")+1).Length; 
					
						trianglePoints = Convert.ToInt32(this.numTrianglePoints); 
						
						fibLevelEntry  = getFibLevelFromEnum(this.fibLevels_Entry);
						fibLevelTarget = getFibLevelFromEnum(this.fibLevels_TakeProfit);
						fibLevelStop   = getFibLevelFromEnum(this.fibLevels_StopLoss);
						
						int daysLoaded = Bars.BarsData.DaysBack;
						if (daysLoaded < 3) 
						{
							string dataMsg = String.Format("{0} Chart loads {1} days of data.  Needs {2} days of history to calculate ATR factors.",
								stratName, daysLoaded, 3);
							Print(dataMsg);
							DrawTextFixed("dataMsg", dataMsg, TextPosition.BottomRight, Color.Navy, new Font("Arial", 12), Color.LightGray, Color.LightGreen, 6); 
						}
						
						this.validateParameters(); 
						this.adjustForTimeZones(eventTime);
						
						ptLabelOffset  = this.pasTextOffset * 2; 
						entryAdjust    = this.ticksFromPullbackToEnter * tickSize;;
						
						double clsPx = Close[0]; 
						if (this.session1EndTime != this.session2StartTime)
							DrawRectangle("blockedTimes", autoScale, session1EndTime.Subtract(timeZoneOffset), clsPx - (clsPx/2) , 
								session2StartTime.Subtract(timeZoneOffset), clsPx + (clsPx/2), Color.Black, Color.LightGray, 6);
					}
					#endregion //closes firstTickCode	
		
					if (!isFirstTick)
					{
						#region tickCode
						if (BarsInProgress == 1)
						{	
							if (!Historical) 
							{
								if (firstPlot) 
								{
									firstPlot = false;	
									this.retrieveHistoricalData(eventTime, timeZoneOffset, BarsInProgress); 
								}
								
								#region updateHistoricalData
								if (eventTime <= this.o_nClose)
								{
									double keyValue = this.getMaxOrMinBetweenTimes(BarsInProgress, o_nOpen, o_nClose, ExtremaType.Max);
									histData[HistoricalData.ONHigh] = keyValue; 
									this.updateHistoricalPlots(HistoricalData.ONHigh, "O/N-High", keyValue); 
									
									keyValue	    = this.getMaxOrMinBetweenTimes(BarsInProgress, o_nOpen, o_nClose, ExtremaType.Min);
									histData[HistoricalData.ONLow] = keyValue; 
									this.updateHistoricalPlots(HistoricalData.ONLow, "O/N-Low", keyValue); 
								}	
								
								if ((eventTime >= this.ibStart) && (eventTime <= this.ibEnd))
								{
									double keyValue = this.getMaxOrMinBetweenTimes(BarsInProgress, ibStart, ibEnd, ExtremaType.Max);
									histData[HistoricalData.IBHigh] = keyValue; 
									this.updateHistoricalPlots(HistoricalData.IBHigh, "IB-High", keyValue); 
			
									keyValue	    = this.getMaxOrMinBetweenTimes(BarsInProgress, ibStart, ibEnd, ExtremaType.Min);
									histData[HistoricalData.IBLow] = keyValue; 
									this.updateHistoricalPlots(HistoricalData.IBLow, "IB-Low", keyValue);;
								}
								#endregion //closes updateHistoricalData
								
								if (barStartTime >= this.session1StartTime)
								{
									#region exitTests
									if (Position.MarketPosition != MarketPosition.Flat)
									{
										lock(_object)
										{
											currentPrice = Math.Round(Close[0], numDecimals); 
											exitRsn = this.onTickExitTests(Position);  
											if ((exitRsn != ExitReason.None) && (exitOrderSent == false)) 
											{	
												exitOrderSent = true; 
												
												OrderAction oa = OrderAction.Sell;
												if (Position.MarketPosition == MarketPosition.Short) oa = OrderAction.Buy; 
												
												oPurpose = OrderPurpose.Flatten;
												string msg = returnSignalName(oPurpose, oa, tradeCount, 0);
												basicExitOrder = SubmitOrder(0, oa, OrderType.Market, Position.Quantity, 0, 0, String.Empty, msg); 	
											}
										}	
									}
									#endregion //closes exitTests
									
									#region cancelOrders_ExceedsEntryTime							
									if ((entryOrder != null)  && (entryOrder.OrderState != OrderState.PartFilled) && (entryOrder.OrderState != OrderState.Filled))
									{
										if (eventTime >= orderSubmissionTime.Add(this.maxTimeToEnterTrade)) 
										{
											this.processCanceledOrder(entryOrder, "MaxTime"); 
											this.RemoveDrawObject(entryPointLine);
											this.RemoveDrawObject("L1_" + tCntAtEntry); 
											//this.RemoveDrawObject("L2_" + tCntAtEntry);
										}	
									}				
									#endregion //closes cancelOrdersIfPriceHitsP2_OrExceedsEntryTime
								}
							}
						}
						#endregion //closes tickCode
						
						#region primaryBarCode
						
						if (BarsInProgress == 0) //executed on the primary bar
						{
							if ((!firstPlot) && (!Historical)) 
							{
								this.plotHistoricalLines(eventTime, histData);
								this.updateValueArea();
							}
							
							SwingClass swingClass  = SwingClass.None;
							double swingPrice      = 0.0d; 
							
							#region duringTimeMarketIsOpen
							if ((barStartTime >= this.session1StartTime) && (barStartTime <= this.session2EndTime) ) //(!Historical) && 
							{
								barCount++; 
								
								#region createTriangles
								//get swing and price for current bar
								swingClass  = this.getSwingClass(pas, 0); 
								swingPrice  = this.getSwingPrice(pas, swingClass, 0);
								
								#region firstTriangle
								if (!hasFirstTriangleFormed)
								{
									if (   ((swingCount == 0)	&& (swingClass != SwingClass.None))			//First swing after initializing 
										|| ((pas.HasNewSwingLow[0])  && (!pas.HasUpdatedSwingLow[0]))		//New Swing Low.  No Updated Swing Low.
										|| ((pas.HasNewSwingHigh[0]) && (!pas.HasUpdatedSwingHigh[0])) )    //New Swing High. No Updated Swing High.
									{
										swingCount++; 
										swingsList.Add(swingClass); 
										swingPrices.Add(swingPrice); 
										barCountAtSwing.Add(barCount);
										this.addPointLabel(swingCount, barCount, 0, swingClass, swingPrice);	
									}
									if ( (swingCount > 0) && ((pas.HasUpdatedSwingLow[0]) || (pas.HasUpdatedSwingHigh[0])) )     ////Updated Swing High or Swing Low.
										this.updateLastTrianglePoint(swingCount, barCount, 0, swingClass, swingPrice); 
										
									if (swingsList.Count == numTrianglePoints)
									{
										hasFirstTriangleFormed = true; 	
										pendingEntry 		   = getEntryType(swingsList);
										distanceBtwSwings 	   = Math.Abs(swingPrices[1] - swingPrices[0]);
										if (!Historical) this.sendNewOrder(barStartTime);
									}
								}
								#endregion //closes firstTriangle
								
								#region otherTriangles
								else
								{
									//if first triangle is formed and a NEW swing occurs, shift points
									if ((pas.HasNewSwingLow[0]) && (!pas.HasUpdatedSwingLow[0])
									|| (pas.HasNewSwingHigh[0]) && (!pas.HasUpdatedSwingHigh[0]))
									{
										for (int i = 0; i < numTrianglePoints; i++) this.RemoveDrawObject("Pt_" + barCountAtSwing[i]); 	
										for (int i = 0; i < numTrianglePoints; i++) 
										{
											if (i < (trianglePoints - 1))
											{
												swingsList[i] 	   = swingsList[i+1];
												swingPrices[i]     = swingPrices[i+1];
												barCountAtSwing[i] = barCountAtSwing[i+1];
											}
											else
											{
												swingsList[i] 	   = swingClass;
												swingPrices[i]     = swingPrice;
												barCountAtSwing[i] = barCount;
											}
											
											this.addPointLabel((i + 1), barCountAtSwing[i], (barCount - barCountAtSwing[i]), swingsList[i], swingPrices[i]);
										}
										
											pendingEntry       = getEntryType(swingsList);
											distanceBtwSwings  = Math.Abs(swingPrices[1] - swingPrices[0]);
											
										if (!Historical)
										{		
											if ( (entryOrder != null)  && (entryOrder.OrderState != OrderState.PartFilled) && (entryOrder.OrderState != OrderState.Filled))
											{
												this.processCanceledOrder(entryOrder, "OppSwing"); 
												this.RemoveDrawObject(entryPointLine);
												this.RemoveDrawObject("L1_" + tCntAtEntry); 
											}
											
											if (entryOrder == null) this.sendNewOrder(barStartTime);
										}
									}
									
									//if first triangle is formed and a swing UPDATE occurs, update P4
									if (pas.HasUpdatedSwingLow[0] || pas.HasUpdatedSwingHigh[0])	
									{
										this.updateLastTrianglePoint(swingCount, barCount, 0, swingClass, swingPrice);
										distanceBtwSwings = Math.Abs(swingPrices[1] - swingPrices[0]);
										pendingEntry 	  = getEntryType(swingsList);
										
										if (!Historical)
										{
											if (entryOrder != null)
											{
												double newPx = getEntryOrderPrice(orderAction, swingPrices);
												Print(String.Format("{0} Changed {1} price of order to {2}. Old price {3:N2}. New price {4:N2}. Time: {5:T}.", 
													stratName, OrderType.Limit, orderAction, entryPrice, newPx, eventTime));
								
												this.RemoveDrawObject(entryPointLine);
												this.RemoveDrawObject("L1_" + tCntAtEntry); 
												ChangeOrder(entryOrder, entryOrder.Quantity, newPx, 0); 
												
												orderSubmissionTime   = eventTime; 
												entryPointLine		 = DrawLine("entryPointLine", autoScale, eventTime.Subtract(timeZoneOffset), newPx, 
													eventTime.Subtract(timeZoneOffset).Add(this.lineDuration), newPx, Color.Blue, DashStyle.Solid, 2);
								
												Color sColor = this.triColorLong;
												if (entryOrder.OrderAction == OrderAction.Sell) sColor = this.triColorShort;
												DrawLine("L1_" + triangleCnt, autoScale, barCount - barCountAtSwing[0], swingPrices[0], 
													barCount - barCountAtSwing[1], swingPrices[1], sColor, DashStyle.Solid, 2);   
											}
											else
											{
												this.sendNewOrder(barStartTime);
											}
										}
									}								
								}	
								#endregion //closes otherTriangles
		
								#endregion //closes createTriangles 
							}
							#endregion //closes duringTimeMarketIsOpen
						}
						#endregion //closes primaryBarCode
					}
				}
			}
			catch(Exception e)
			{
				Print(String.Format("{0} Exception occurred: {1}", stratName, e));
			}
		}

		protected override void OnOrderUpdate(IOrder order)
		{
			updatedOrderState = order.OrderState;
		}
		
		protected override void OnExecution(IExecution execution)
        {
			if (execution.Order != null) 
			{	 
				#region retrieveOrderProperties
				OrderPurpose oPurpose = extractOrderPurposeFromSignalName(execution.Name.ToString(), '-', 3);
				string sTrd = extractTrdAndSubTrdNumFromSignalName(execution.Name.ToString(), 4);
				string sSubTrd = "0"; //extractTrdAndSubTrdNumFromSignalName(execution.Name.ToString(), 2);
				
				int trdCnt, subTrdCnt;
				int.TryParse(sTrd, out trdCnt); 
				int.TryParse(sSubTrd, out subTrdCnt); 
				#endregion //closes retrieveOrderProperties
				
				#region unFilledOrders
				
				#region rejectedOrders
				if (execution.Order.OrderState == OrderState.Rejected)
				{
					canSendNewEntry = true;
					this.printOrderStateString(execution, trdCnt, subTrdCnt, oPurpose); //prints details of the rejected order
					
					OrderAction flattenAction = OrderAction.Buy;
					if ((openContracts != 0) || (Position.MarketPosition != MarketPosition.Flat))
					{
						if (openContracts < 0) flattenAction = OrderAction.Sell;
						oPurpose = OrderPurpose.HandleRejection;
						string msg = this.returnSignalName(oPurpose, flattenAction, trdCnt, subTrdCnt); 
						rejectionExitOrder = SubmitOrder(0, flattenAction, OrderType.Market, Math.Abs(openContracts), 0, 0, String.Empty, msg); 
					}
				}	
				#endregion //closes rejectedOrders
				
				if (execution.Order.OrderState == OrderState.Cancelled) this.printOrderStateString(execution, trdCnt, subTrdCnt, oPurpose); 
				#endregion //closes unFilledOrders	
				
				#region filledOrders
				if ((execution.Order.OrderState == OrderState.Filled) || (execution.Order.OrderState == OrderState.PartFilled))
				{	
					//if the filled order is an entry order
					if ((entryOrder != null) &&  (execution.Order == entryOrder))         this.processMarketOrders(execution, trdCnt, subTrdCnt, oPurpose); 
					//if the filled order is an exit order
					if ((basicExitOrder != null) && (execution.Order == basicExitOrder))  this.processMarketOrders(execution, trdCnt, subTrdCnt, oPurpose); 
				}
				#endregion //closes filledOrders	
			}
		}    
		
		protected override void OnTermination()
		{
			if (!hasTerminated)
			{
				hasTerminated = true; 
				
				Print(String.Empty); 
				Print(String.Format("{0} Strategy has been stopped at: {1:T}. Closing open positions and canceling unfilled orders.", stratName, DateTime.Now.Add(timeZoneOffset))); 
				if (entryOrder != null) CancelOrder(entryOrder);
				
				this.flattenPositions("StrategyStopped"); 
			}
		}

        #region Properties
		
		[XmlIgnore()]
		[Description("Pit Session Open")]
		[GridCategory("Session Parameters")]
		public TimeSpan PitSessionOpenTime
		{
			get { return pitSessionOpenTime; }
			set { pitSessionOpenTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Pit Session Close")]
		[GridCategory("Session Parameters")]
		public TimeSpan PitSessionCloseTime
		{
			get { return pitSessionCloseTime; }
			set { pitSessionCloseTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Pit Session Early Close")]
		[GridCategory("Session Parameters")]
		public TimeSpan PitSessionEarlyCloseTime
		{
			get { return pitSessionEarlyCloseTime; }
			set { pitSessionEarlyCloseTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Initial Balance Start Time")]
		[GridCategory("Session Parameters")]
		public TimeSpan InitialBalanceStartTime
		{
			get { return initialBalanceStartTime; }
			set { initialBalanceStartTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Initial Balance End Time")]
		[GridCategory("Session Parameters")]
		public TimeSpan InitialBalanceEndTime
		{
			get { return initialBalanceEndTime; }
			set { initialBalanceEndTime = value; }
		}

		[XmlIgnore()]
		[Description("Strategy Time Zone (associated with Trading Hours)")]
		[GridCategory("Time parameters")]
		public StrategyTimeZone StratTimeZone
		{
			get { return stratTimeZone; }
			set { stratTimeZone = value; }
		}
		
		[XmlIgnore()]
		[Description("First Session Start Time")]
		[GridCategory("Time parameters")]
		public TimeSpan FirstSessionStartTime
		{
			get { return firstSessionStartTime; }
			set { firstSessionStartTime = value; }
		}
		
		[XmlIgnore()]
		[Description("First Session End Time")]
		[GridCategory("Time parameters")]
		public TimeSpan FirstSessionEndTime
		{
			get { return firstSessionEndTime; }
			set { firstSessionEndTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Second Session Start Time")]
		[GridCategory("Time parameters")]
		public TimeSpan SecondSessionStartTime
		{
			get { return secondSessionStartTime; }
			set { secondSessionStartTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Second Session End Time")]
		[GridCategory("Time parameters")]
		public TimeSpan SecondSessionEndTime
		{
			get { return secondSessionEndTime; }
			set { secondSessionEndTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Last Time New Entry Can be Submitted")]
		[GridCategory("Time parameters")]
		public TimeSpan EndOfNewEntriesTime
		{
			get { return endOfNewEntriesTime; }
			set { endOfNewEntriesTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Max Time Allowed for Entry")]
		[GridCategory("Time parameters")]
		public TimeSpan MaxTimeToEnterTrade
		{
			get { return maxTimeToEnterTrade; }
			set { maxTimeToEnterTrade = value; }
		}

		[Description("Exit at End of Trading Hours")]
		[GridCategory("Time parameters")]
		public bool ExitAtMarketClose
		{
			get { return exitAtMarketClose; }
			set { exitAtMarketClose = value; }
		}
		
		[Description("Strategy Name Prefix")]
		[GridCategory("Strategy parameters")]
		public string StratNamePrefix
		{
			get { return stratNamePrefix; }
			set { stratNamePrefix = value; }
		}	
		
		[Description("Number of Contracts/Trade Size")]
		[GridCategory("Strategy parameters")]
		public uint NumberOfContracts
		{
			get { return numContracts; }
			set { numContracts = value; }
		}	
		
		[Description("Number of Days before Expiry to Roll Futures")]
		[GridCategory("Strategy parameters")]
		public uint NumBizDaysToRollBeforeExpiry
		{
			get { return numBizDaysToRollBeforeExpiry; }
			set { numBizDaysToRollBeforeExpiry = value; }
		}
		
		[Description("Consecutive Wins/Losses Before Trading Halt")]
		[GridCategory("Strategy Parameters")]
		public uint ConsecutiveWLForTradingHalt
		{
			get { return consecutiveWLForTradingHalt; }
			set { consecutiveWLForTradingHalt = value; }
		}
		
		[Description("Value Area Percent")]
		[GridCategory("Strategy Parameters")]
		public double ValueAreaPercent
		{
			get { return valueAreaPercent; }
			set { valueAreaPercent = value; }
		}
	
		[Description("Fib Levels - Entry")]
		[GridCategory("Entry Parameters")]
		public FibLevels FibLevels_Entry
		{
			get { return fibLevels_Entry; }
			set { fibLevels_Entry = value; }
		}
		
		[Description("Ticks from Pullback to Enter")]
		[GridCategory("Entry Parameters")]
		public uint TicksFromPullbackToEnter
		{
			get { return ticksFromPullbackToEnter; }
			set { ticksFromPullbackToEnter = value; }
		}

		[Description("Swing Strength1")]
		[GridCategory("Price Action Parameters")]
		public uint SwingStrength
		{
			get { return swingStrength; }
			set { swingStrength = value; }
		}
		
		[Description("PAS - Double Top/Bottom Strength1")]
		[GridCategory("Price Action Parameters")]
		public uint DtbStrength
		{
			get { return dtbStrength; }
			set { dtbStrength = value; }
		}
		
		[Description("PAS - Swing Style1")]
		[GridCategory("Price Action Parameters")]
		public pasb.SwingStyle SwingStyle
		{
			get { return swingStyle; }
			set { swingStyle = value; }
		}
		
		[Description("PAS - Use Close Value1")]
		[GridCategory("Price Action Parameters")]
		public bool UseCloseValues
		{
			get { return useCloseValues; }
			set { useCloseValues = value; }
		}
		
		[Description("Fib Levels - Take Profit")]
		[GridCategory("Exit Parameters")]
		public FibLevels FibLevels_TakeProfit
		{
			get { return fibLevels_TakeProfit; }
			set { fibLevels_TakeProfit = value; }
		}
		
		[Description("Fib Levels - Stop Loss")]
		[GridCategory("Exit Parameters")]
		public FibLevels FibLevels_StopLoss
		{
			get { return fibLevels_StopLoss; }
			set { fibLevels_StopLoss = value; }
		}
		
		[Description("Percent of TakeProfit - Trigger 1")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Trigger1
		{
			get { return percentTakeProfit_Trigger1; }
			set { percentTakeProfit_Trigger1 = value; }
		}
				
		[Description("Percent of TakeProfit - Trigger 2")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Trigger2
		{
			get { return percentTakeProfit_Trigger2; }
			set { percentTakeProfit_Trigger2 = value; }
		}
		
		[Description("Percent of TakeProfit - Trigger 3 ")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Trigger3
		{
			get { return percentTakeProfit_Trigger3; }
			set { percentTakeProfit_Trigger3 = value; }
		}
	
		[Description("Percent of TakeProfit - Exit 1")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Exit1
		{
			get { return percentTakeProfit_Exit1; }
			set { percentTakeProfit_Exit1 = value; }
		}
		
		[Description("Percent of TakeProfit - Exit 2")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Exit2
		{
			get { return percentTakeProfit_Exit2; }
			set { percentTakeProfit_Exit2 = value; }
		}
		
		[Description("Percent of TakeProfit - Exit 3")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Exit3
		{
			get { return percentTakeProfit_Exit3; }
			set { percentTakeProfit_Exit3 = value; }
		}
		
		[Description("Trail Stop - Step Interval")]
		[GridCategory("Exit Parameters")]
		public double StepInterval
		{
			get { return stepInterval; }
			set { stepInterval = value; }
		}
		
		[Description("Minimum Stop Loss (Ticks)")]
		[GridCategory("Exit Parameters")]
		public uint MinStopLossTicks
		{
			get { return minStopLossTicks; }
			set { minStopLossTicks = value; }
		}
				
		[Description("Use BE/BEPlus Stops")]
		[GridCategory("Exit Parameters")]
		public bool UseTrailingStops
		{
			get { return useTrailingStops; }
			set { useTrailingStops = value; }
		}
		
		#region timeSpanSerialization
	
		[Browsable(false)]
		public string srl_PitSessionOpenTime
		{
			get {return pitSessionOpenTime.ToString();}	
			set {pitSessionOpenTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_PitSessionCloseTime
		{
			get {return pitSessionCloseTime.ToString();}	
			set {pitSessionCloseTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_PitSessionEarlyCloseTime
		{
			get {return pitSessionEarlyCloseTime.ToString();}	
			set {pitSessionEarlyCloseTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_FirstSessionStartTime
		{
			get {return firstSessionStartTime.ToString();}	
			set {firstSessionStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_InitialBalanceStartTime
		{
			get {return initialBalanceStartTime.ToString();}	
			set {initialBalanceStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_InitialBalanceEndTime
		{
			get {return initialBalanceEndTime.ToString();}	
			set {initialBalanceEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}

		[Browsable(false)]
		public string srl_FirstSessionEndTime
		{
			get {return firstSessionEndTime.ToString();}	
			set {firstSessionEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_SecondSessionStartTime
		{
			get {return secondSessionStartTime.ToString();}	
			set {secondSessionStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_SecondSessionEndTime
		{
			get {return secondSessionEndTime.ToString();}	
			set {secondSessionEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}

		[Browsable(false)]
		public string srl_EndOfNewEntriesTime
		{
			get {return endOfNewEntriesTime.ToString();}	
			set {endOfNewEntriesTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_MaxTimeToEnterTrade
		{
			get {return maxTimeToEnterTrade.ToString();}	
			set {maxTimeToEnterTrade = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
	
		#endregion //closes timeSpanSerialization
		
		#endregion
    }
}
