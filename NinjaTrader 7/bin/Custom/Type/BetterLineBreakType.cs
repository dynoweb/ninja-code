// Created 1/24/2011.  Gordo.

#region Using declarations
	using System;
	using System.ComponentModel;
#endregion

namespace NinjaTrader.Data
{
	public class BetterLineBreak : BarsType
	{
		#region Variables
			private	int				newSessionIdx		= 0;
			private int				tmpCount			= 0;
			private int				tmpDayCount			= 0;
			private	int				tmpTickCount		= 0;
			
			private long			tmpVolume			= 0;
			
			private double			anchorPrice			= double.MinValue;
			private double			switchPrice			= double.MinValue;
			private double			tmpHigh				= 0;
			private double			tmpLow				= 0;
			
			private bool			upTrend				= true;
			private bool			firstBarOfSession	= true;
			private	bool			newSession			= false;
			
			private static bool		registered			= Register(new BetterLineBreak());
			
			private	DateTime		tmpTime				= Cbi.Globals.MinDate;
		#endregion
		
		#region Add bars
			public override void Add(Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isRealtime)
			{
			#region Reset cache
				if (bars.Count == 0 && tmpTime != Cbi.Globals.MinDate) // reset caching when live request trimmed existing bars
					tmpTime = Cbi.Globals.MinDate;
				
				bool endOfBar = true;
				if (tmpTime == Cbi.Globals.MinDate)
				{
					tmpTime			= time;
					tmpDayCount		= 1;
					tmpTickCount	= 1;
				}
				else if (bars.Count < tmpCount && bars.Count == 0) // reset cache when bars are trimmed
				{
					tmpTime			= Cbi.Globals.MinDate;
					tmpVolume		= 0;
					tmpDayCount		= 0;
					tmpTickCount	= 0;
				}
				else if (bars.Count < tmpCount && bars.Count > 0) // reset cache when bars are trimmed
				{
					tmpTime			= bars.GetTime(bars.Count - 1); 
					tmpVolume		= bars.GetVolume(bars.Count - 1);
					tmpTickCount	= bars.TickCount;
					tmpDayCount		= bars.DayCount;
				}
			#endregion
			
			#region Switch Periods
			switch (bars.Period.BasePeriodType)
			{
				#region tick
				case PeriodType.Tick:
				{
					if (bars.IsNewSession(time, isRealtime))
					{
						newSession = true;
					    tmpTime = time;
						tmpTickCount = 1;
						
						if (bars.Count == 0)
							break;
						
						endOfBar = false;
					}
					else
					{
						if(bars.Period.BasePeriodValue > 1 && tmpTickCount < bars.Period.BasePeriodValue)
						{
						    tmpTime = time;
							tmpVolume += volume;
							tmpTickCount++;
							bars.LastPrice = close;
							endOfBar = false;
						}
						else 
							tmpTime = time; // there can't be a situation when new ticks go into old bar, this would mean peeking into future. Fixed in NT7B14 20100416 CH
					}
					break;
				}
				#endregion
				#region Volume
				case PeriodType.Volume:
	            {
					if (bars.IsNewSession(time, isRealtime))
					{
						newSession = true;
					}
					else if (bars.Count == 0 && volume > 0)
						break;
					else
					{
					    tmpVolume += volume;
					    if (tmpVolume < bars.Period.BasePeriodValue)
					    {
					        bars.LastPrice = close;
					        endOfBar = false;
					    }
					    else if (tmpVolume == 0)
					        endOfBar = false;
					}
					
					tmpTime = time; // there can't be a situation when new ticks go into old bar, this would mean peeking into future. Fixed in NT7B14 20100416 CH
					
	                break;
	            }
				#endregion
				#region Second
				case PeriodType.Second:
				{
					if (bars.IsNewSession(time, isRealtime))
					{
					    tmpTime = TimeToBarTimeSecond(bars, time, new DateTime(bars.Session.NextBeginTime.Year, bars.Session.NextBeginTime.Month, bars.Session.NextBeginTime.Day, bars.Session.NextBeginTime.Hour, bars.Session.NextBeginTime.Minute, 0), bars.Period.BasePeriodValue);
						
						if (bars.Count == 0)
							break;
						
						endOfBar = false;
						newSession = true;
					}
					else
					{
						if (time <= tmpTime)
						{
							tmpVolume += volume;
							bars.LastPrice = close;
							endOfBar = false;
						}
						else
							tmpTime = TimeToBarTimeSecond(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue);
					}
					break;
				}
				#endregion
				#region Day
				case PeriodType.Day:
				{
					if (bars.Count == 0 || (bars.Count > 0 && (bars.TimeLastBar.Month < time.Month || bars.TimeLastBar.Year < time.Year)))
					{
					    tmpTime	= time.Date;
						bars.LastPrice	= close;
						newSession = true;
					}
					else
					{
					    tmpTime	= time.Date;
						tmpVolume += volume;
						bars.LastPrice = close;
						tmpDayCount++;
						
						if (tmpDayCount < bars.Period.BasePeriodValue || (bars.Count > 0 && bars.TimeLastBar.Date == time.Date))
							endOfBar = false;
					}
					break;
				}
				#endregion
				#region Minute
				case PeriodType.Minute:
				{
					if (bars.Count == 0 || bars.IsNewSession(time, isRealtime))
					{
						tmpTime 	= TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);
						newSession	= true;
						tmpVolume 	= 0;
					}
					else
					{
						if (isRealtime && time < bars.TimeLastBar || !isRealtime && time <= bars.TimeLastBar)
						{
							tmpTime		= bars.TimeLastBar;
							endOfBar	= false;
						}
						else
							tmpTime = TimeToBarTimeMinute(bars, time, bars.Session.NextBeginTime, bars.Period.BasePeriodValue, isRealtime);
						
						tmpVolume += volume;
					}
					break;
				}
				#endregion
				#region Week
				case PeriodType.Week:
				{
					if (tmpTime == Cbi.Globals.MinDate)
					{
					    tmpTime = TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.Period.BasePeriodValue);
						
						if (bars.Count == 0)
							break;
						
						endOfBar = false;
					}
					else
					{
						if (time.Date <= tmpTime.Date)
						{
						    tmpVolume += volume;
							bars.LastPrice	= close;
							endOfBar = false;
						}		
					}
					break;
				}
				#endregion
				#region Month
				case PeriodType.Month:
				{
					if (tmpTime == Cbi.Globals.MinDate)
					{
					    tmpTime	= TimeToBarTimeMonth(time, bars.Period.BasePeriodValue);
						
						if (bars.Count == 0)
							break;
						
						endOfBar = false;
					}
					else
					{
						if ((time.Month <= tmpTime.Month && time.Year == tmpTime.Year) || time.Year < tmpTime.Year)
						{
						    tmpVolume += volume;
							bars.LastPrice = close;
							endOfBar = false;
						}		
					}
					break;
				}
				#endregion
				#region Year
				case PeriodType.Year:
				{
					if (tmpTime == Cbi.Globals.MinDate)
					{
					    tmpTime = TimeToBarTimeYear(time, bars.Period.Value);
						
						if (bars.Count == 0)
							break;
						
						endOfBar = false;
					}
					else
					{
						if (time.Year <= tmpTime.Year)
						{
						    tmpVolume += volume;
							bars.LastPrice = close;
							endOfBar = false;
						}		
					}
					break;
				}
				#endregion
				#region Default
				default:
					break;
				#endregion
			}
			#endregion
			
			#region Add bars
			if (bars.Count == 0 || (newSession && IsIntraday))  // Very first bar setup.
			{
				#region First bar
				AddBar(bars, open, close, close, close, tmpTime, volume, isRealtime);
				upTrend				= (open < close);
				newSessionIdx		= bars.Count - 1;
				newSession			= false;  
				firstBarOfSession	= true;
				anchorPrice			= close;
				switchPrice			= open;
				tmpHigh				= close;
				tmpLow				= close;
				#endregion
			}
			else if (firstBarOfSession && endOfBar == false)
			{
				#region Start of subsiquent bars
				double prevOpen = bars.GetOpen(bars.Count - 1);
				bars.RemoveLastBar(isRealtime);
				AddBar(bars, prevOpen, close, close, close, tmpTime, tmpVolume, isRealtime);
				upTrend         = (prevOpen < close);
				anchorPrice     = close;
				#endregion
			}
			else
			{
				int breakCount  = bars.Period.Value;
				Bar bar         = (Bar)bars.Get(bars.Count - 1);
				double breakMax = double.MinValue;
				double breakMin = double.MaxValue;
				
				if (firstBarOfSession)
				{
					AddBar(bars, anchorPrice, close, close, close, tmpTime, volume, isRealtime);
					firstBarOfSession	= false;
					tmpVolume			= volume;
					tmpTime				= Cbi.Globals.MinDate;
					return;
				}
				
				if (bars.Count - newSessionIdx - 1 < breakCount)
					breakCount = bars.Count - (newSessionIdx + 1);
				
				for (int k = 1; k <= breakCount; k++)
				{
					Bar tmp	        = (Bar)bars.Get(bars.Count - k - 1);
					breakMax		= Math.Max(breakMax, tmp.Open);
					breakMax		= Math.Max(breakMax, tmp.Close);
					breakMin		= Math.Min(breakMin, tmp.Open);
					breakMin		= Math.Min(breakMin, tmp.Close);
				}
				
				bars.LastPrice = close;
				
				if (upTrend)
				{
					#region Up trend
					if (endOfBar)
					{
						bool adding = false;
						if (bars.Instrument.MasterInstrument.Compare(bar.Close, anchorPrice) > 0)
						{
							anchorPrice = bar.Close;
							switchPrice = bar.Open;
							tmpVolume   = volume;
							adding      = true;
							tmpHigh		= bar.Close;
							tmpLow		= bar.Close;
						}
						else if (bars.Instrument.MasterInstrument.Compare(breakMin, bar.Close) > 0)
						{
							anchorPrice = bar.Close;
							switchPrice = bar.Open;
							tmpVolume   = volume;
							upTrend     = false;
							adding      = true;
							tmpHigh		= bar.Close;
							tmpLow		= bar.Close;
						}
						
						if (adding)
						{
							double tmpOpen = upTrend ? Math.Min(Math.Max(switchPrice, close), anchorPrice) : Math.Max(Math.Min(switchPrice, close), anchorPrice);
							tmpHigh = Math.Max(tmpHigh, close);
							tmpLow = Math.Min(tmpLow, close);
							AddBar(bars, tmpOpen, tmpHigh, tmpLow, close, tmpTime, volume, isRealtime);
						}
						else
						{
							bars.RemoveLastBar(isRealtime);
							double tmpOpen = Math.Min(Math.Max(switchPrice, close), anchorPrice);
							tmpHigh = Math.Max(tmpHigh, close);
							tmpLow = Math.Min(tmpLow, close);
							AddBar(bars, tmpOpen, tmpHigh, tmpLow, close, tmpTime, tmpVolume, isRealtime);
						}
					}
					else
					{
						bars.RemoveLastBar(isRealtime);
						double tmpOpen = Math.Min(Math.Max(switchPrice, close), anchorPrice);
						tmpHigh = Math.Max(tmpHigh, close);
						tmpLow = Math.Min(tmpLow, close);
						AddBar(bars, tmpOpen, tmpHigh, tmpLow, close, tmpTime, tmpVolume, isRealtime);
					}
					#endregion
				}
				else
				{
					#region Down trend
					if (endOfBar)
					{
						bool adding = false;
						if (bars.Instrument.MasterInstrument.Compare(bar.Close, anchorPrice) < 0)
						{
							anchorPrice = bar.Close;
							switchPrice = bar.Open;
							tmpVolume   = volume;
							adding      = true;
							tmpHigh		= bar.Close;
							tmpLow		= bar.Close;
						}
						else if (bars.Instrument.MasterInstrument.Compare(breakMax, bar.Close) < 0)
						{
							anchorPrice = bar.Close;
							switchPrice = bar.Open;
							tmpVolume   = volume;
							upTrend     = true;
							adding      = true;
							tmpHigh		= bar.Close;
							tmpLow		= bar.Close;
						}
						
						if (adding)
						{
							double tmpOpen = upTrend ? Math.Min(Math.Max(switchPrice, close), anchorPrice) : Math.Max(Math.Min(switchPrice, close), anchorPrice);
							tmpHigh = Math.Max(tmpHigh, close);
							tmpLow = Math.Min(tmpLow, close);
							AddBar(bars, tmpOpen, tmpHigh, tmpLow, close, tmpTime, volume, isRealtime);
						}
						else
						{
							bars.RemoveLastBar(isRealtime);
							double tmpOpen = Math.Max(Math.Min(switchPrice, close), anchorPrice);
							tmpHigh = Math.Max(tmpHigh, close);
							tmpLow = Math.Min(tmpLow, close);
							AddBar(bars, tmpOpen, tmpHigh, tmpLow, close, tmpTime, tmpVolume, isRealtime);
						}
					}
					else
					{
						bars.RemoveLastBar(isRealtime);
						double tmpOpen = Math.Max(Math.Min(switchPrice, close), anchorPrice);
						tmpHigh = Math.Max(tmpHigh, close);
						tmpLow = Math.Min(tmpLow, close);
						AddBar(bars, tmpOpen, tmpHigh, tmpLow, close, tmpTime, tmpVolume, isRealtime);
					}
					#endregion
				}
			}
			#endregion
			
			if (endOfBar)
				tmpTime = Cbi.Globals.MinDate;

			tmpCount = bars.Count;
		}
		#endregion

		#region Clone
		public override object Clone()
		{
			return new BetterLineBreak();
		}
		#endregion

		#region Display name
		public override string DisplayName
		{
			get { return "BetterLineBreak"; }
		}
		#endregion

		#region Default period value set in Data Series
		public BetterLineBreak() : base(PeriodType.Custom9)
		{
			Period.Value = 3;
		}
		#endregion

		#region Default string used in Image Save as... 
		public override string ToString(Period period)
		{
			switch (Period.BasePeriodType)
		        {
					case PeriodType.Day:		return string.Format("{0} {1} BetterLineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Daily" : "Day"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Minute:		return string.Format("{0} Min BetterLineBreak{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Month:		return string.Format("{0} {1} BetterLineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Monthly" : "Month"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Second:	    return string.Format("{0} {1} BetterLineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Second" : "Seconds"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Tick:		return string.Format("{0} Tick BetterLineBreak{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Volume:		return string.Format("{0} Volume BetterLineBreak{1}", period.BasePeriodValue, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Week:		return string.Format("{0} {1} BetterLineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Weekly" : "Weeks"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            case PeriodType.Year:		return string.Format("{0} {1} BetterLineBreak{2}", period.BasePeriodValue, (period.BasePeriodValue == 1 ? "Yearly" : "Years"), (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		            default:					return string.Format("{0} {1} BetterLineBreak{2}", period.BasePeriodValue, BuiltFrom, (period.MarketDataType != MarketDataType.Last ? " - " + period.MarketDataType : string.Empty));
		        }
		}
		#endregion

		#region Property Descriptor Collection
		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, Period period, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, period, attributes);

			// here is how you remove properties not needed for that particular bars type
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value", "\r\rBetter Line Breaks");

			return properties;
		}
		#endregion

		#region IsIntraday
		public override bool IsIntraday
		{
			get 
			{ 
				switch (Period.BasePeriodType)
		        {
		            case PeriodType.Day:		return false; 
		            case PeriodType.Minute:		return true; 
		            case PeriodType.Month:		return false; 
		            case PeriodType.Second:	    return true; 
		            case PeriodType.Tick:		return true; 
		            case PeriodType.Volume:		return true; 
		            case PeriodType.Week:		return false; 
		            case PeriodType.Year:		return false; 
		            default:					return false; 
		        }
			}
		}
		#endregion

		#region Select Period Type
		public override PeriodType BuiltFrom
		{
		    get 
		    {
		        switch (Period.BasePeriodType)
		        {
		            case PeriodType.Day   :		return PeriodType.Day; 
		            case PeriodType.Minute:		return PeriodType.Minute; 
		            case PeriodType.Month :		return PeriodType.Day; 
		            case PeriodType.Second:	    return PeriodType.Tick; 
		            case PeriodType.Tick  :		return PeriodType.Tick; 
		            case PeriodType.Volume:		return PeriodType.Tick; 
		            case PeriodType.Week  :		return PeriodType.Day; 
		            case PeriodType.Year  :		return PeriodType.Day;
		            default               :		return PeriodType.Minute;
		        }
		    }
		}
		#endregion

		#region Set Chart Label date format based on Selected Period Type
		public override string ChartLabel(Gui.Chart.ChartControl chartControl, DateTime time)
		{
			switch (Period.BasePeriodType)
		        {
		            case PeriodType.Day   :		return time.ToString(chartControl.LabelFormatDay, Cbi.Globals.CurrentCulture);
		            case PeriodType.Minute:		return time.ToString(chartControl.LabelFormatMinute, Cbi.Globals.CurrentCulture);
		            case PeriodType.Month :		return time.ToString(chartControl.LabelFormatMonth, Cbi.Globals.CurrentCulture);
		            case PeriodType.Second:	    return time.ToString(chartControl.LabelFormatSecond, Cbi.Globals.CurrentCulture);
		            case PeriodType.Tick  :		return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		            case PeriodType.Volume:		return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		            case PeriodType.Week  :		return time.ToString(chartControl.LabelFormatDay, Cbi.Globals.CurrentCulture);
		            case PeriodType.Year  :		return time.ToString(chartControl.LabelFormatYear, Cbi.Globals.CurrentCulture);
		            default               :		return time.ToString(chartControl.LabelFormatDay, Cbi.Globals.CurrentCulture);
		        }
		}
		#endregion

		#region GetPercentComplete
		public override double GetPercentComplete(Bars bars, DateTime now)
		{
			switch (Period.BasePeriodType)
			{
			    case PeriodType.Day   : return now.Date <= bars.TimeLastBar.Date ? 1.0 - (bars.TimeLastBar.AddDays(1).Subtract(now).TotalDays/bars.Period.Value) : 1;
			    case PeriodType.Minute: return now <= bars.TimeLastBar ? 1.0 - (bars.TimeLastBar.Subtract(now).TotalMinutes/bars.Period.Value) : 1;
			    case PeriodType.Month :
			        if (now.Date <= bars.TimeLastBar.Date)
			        {
			            int month = now.Month;
			            int daysInMonth = (month == 2) 
                            ? (DateTime.IsLeapYear(now.Year) ? 29 : 28) 
                            : (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12 ? 31 : 30);
			            return (daysInMonth - (bars.TimeLastBar.Date.AddDays(1).Subtract(now).TotalDays/bars.Period.Value))/
			                   daysInMonth; // not exact
			        }
			        return 1;
			    case PeriodType.Second: return now <= bars.TimeLastBar ? 1.0 - (bars.TimeLastBar.Subtract(now).TotalSeconds/bars.Period.Value) : 1;
			    case PeriodType.Tick  : return (double) bars.TickCount/bars.Period.Value;
			    case PeriodType.Volume: return (double) bars.Get(bars.CurrentBar).Volume/bars.Period.Value;
			    case PeriodType.Week  : return now.Date <= bars.TimeLastBar.Date ? (7 - (bars.TimeLastBar.AddDays(1).Subtract(now).TotalDays/bars.Period.Value))/7 : 1;
			    case PeriodType.Year  :
			        if (now.Date <= bars.TimeLastBar.Date)
			        {
			            double daysInYear = DateTime.IsLeapYear(now.Year) ? 366 : 365;
			            return (daysInYear - (bars.TimeLastBar.Date.AddDays(1).Subtract(now).TotalDays/bars.Period.Value))/
			                   daysInYear;
			        }
			        return 1;
			    default               : return now.Date <= bars.TimeLastBar.Date ? 1.0 - (bars.TimeLastBar.AddDays(1).Subtract(now).TotalDays/bars.Period.Value) : 1;
			}
		}
		#endregion

		#region Default values for all base periods set in Data Series
		public override int DefaultValue
		{ 
			get 
			{ 
				switch (Period.BasePeriodType)
				{
					case PeriodType.Day:		return 1; 
					case PeriodType.Minute:		return 1; 
					case PeriodType.Month:		return 1; 
					case PeriodType.Second:	    return 30; 
					case PeriodType.Tick:		return 150; 
					case PeriodType.Volume:		return 1000; 
					case PeriodType.Week:		return 1; 
					case PeriodType.Year:		return 1;
					default:					return 1;
				} 
			}
		}
		#endregion

		#region Set format for display in Data Box based on Selected Period Type
		public override string ChartDataBoxDate(DateTime time)
		{
			switch (Period.BasePeriodType)
			{
				case PeriodType.Day   :		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Minute:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Month :		return string.Format("{0}/{1}", time.Month, time.Year);
				case PeriodType.Second:	    return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Tick  :		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Volume:		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
				case PeriodType.Week  :		return string.Format("{0}/{1}", Gui.Globals.GetCalendarWeek(time), time.Year);
				case PeriodType.Year  :		return time.Year.ToString();
				default               :		return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);

			}
		}
		#endregion

		#region Apply Defaults to Selected Period Type
		// Apply default values to the Base Period Type
		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.Period.Value	= 3;
			
			switch (barsData.Period.BasePeriodType)
			{
				case PeriodType.Day   :		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 365;	break;
				case PeriodType.Minute:	    barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 5;		break;
				case PeriodType.Month :		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 5475;	break;
				case PeriodType.Second:	    barsData.Period.BasePeriodValue = 30;	barsData.DaysBack = 3;		break;
				case PeriodType.Tick  :		barsData.Period.BasePeriodValue = 150;	barsData.DaysBack = 3;		break;
				case PeriodType.Volume:	    barsData.Period.BasePeriodValue = 1000;	barsData.DaysBack = 3;		break;
				case PeriodType.Week  :		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 1825;	break;
				case PeriodType.Year  :		barsData.Period.BasePeriodValue = 1;	barsData.DaysBack = 15000;	break;
				default               :																			break;
			}
		}
		#endregion

		#region GetInitialLookBackDays
		public override int GetInitialLookBackDays(Period period, int barsBack)
		{ 
			switch (Period.BasePeriodType)
	        {
	            case PeriodType.Day   :		return new DayBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Minute:		return new MinuteBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Month :		return new MonthBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Second:	    return new SecondBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Tick  :		return new TickBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Volume:		return new VolumeBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Week  :		return new WeekBarsType().GetInitialLookBackDays(period, barsBack); 
	            case PeriodType.Year  :		return new YearBarsType().GetInitialLookBackDays(period, barsBack); 
	            default               :		return new MinuteBarsType().GetInitialLookBackDays(period, barsBack); 
	        }
		}
		#endregion

		#region Chart Style Type
		// This following code in the original code restricts which ChartStyles can be used with the Line Break BarType
		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported	
		{
			get 
			{ 
				return new Gui.Chart.ChartStyleType[] { Gui.Chart.ChartStyleType.CandleStick, Gui.Chart.ChartStyleType.OHLC }; 
			}	
		}
		#endregion

		#region barTimeStamp for Second Minute Month Week and Year
		#region Display date & time - Second
		private static DateTime TimeToBarTimeSecond(Bars bars, DateTime time, DateTime periodStart, int periodValue)
		{
			DateTime barTimeStamp = periodStart.AddSeconds(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(periodStart).TotalSeconds)) / periodValue) * periodValue);
			if (bars.Session.SessionsOfDay.Length > 0 && barTimeStamp > bars.Session.NextEndTime)
				barTimeStamp = (bars.Session.NextEndTime <= Cbi.Globals.MinDate ? barTimeStamp : bars.Session.NextEndTime);
			return barTimeStamp;
		}
		#endregion

		#region Display date & time - Minute
		private static DateTime TimeToBarTimeMinute(Bars bars, DateTime time, DateTime periodStart, int periodValue, bool isRealtime)
		{
		    DateTime barTimeStamp = isRealtime 
		                                ? periodStart.AddMinutes(periodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue) 
		                                : periodStart.AddMinutes(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(periodStart).TotalMinutes)) / periodValue) * periodValue);
			if (bars.Session.SessionsOfDay.Length > 0 && barTimeStamp > bars.Session.NextEndTime)
				barTimeStamp = (bars.Session.NextEndTime <= Cbi.Globals.MinDate ? barTimeStamp : bars.Session.NextEndTime);
			return barTimeStamp;
		}
		#endregion

		#region Display date & time - Month
		private static DateTime TimeToBarTimeMonth(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, time.Month, 1); 
			for (int i = 0; i < periodValue; i++)
				result = result.AddMonths(1);

			return result.AddDays(-1);
		}
		#endregion

		#region Display date & time - Week
		private static DateTime TimeToBarTimeWeek(DateTime time, DateTime periodStart, int periodValue)
		{
			return periodStart.Date.AddDays(Math.Ceiling(Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays) / (periodValue * 7)) * (periodValue * 7)).Date;
		}
		#endregion

		#region Display date & time - Year
		private static DateTime TimeToBarTimeYear(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, 1, 1); 
			for (int i = 0; i < periodValue; i++)
				result = result.AddYears(1);

			return result.AddDays(-1);
		}
		#endregion
		#endregion

	}
}
