//
// BetterRenkoBarsType
//
// written by aslan
//
// 20100807 - created BetterRenko to address issues with other types of Renko bars
// 20101118 - changed initial brick alignment to brick size
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace NinjaTrader.Data
{
	public class BetterRenkoBarsType : BarsType
	{
		private static bool	registered	= Data.BarsType.Register(new BetterRenkoBarsType());

		// Note: change Custom4 if conflicting with another custom bar type
		public BetterRenkoBarsType() : base(PeriodType.Custom4) { }

		public override object Clone() { return new BetterRenkoBarsType(); }

		public override string DisplayName { get { return "BetterRenko"; } }

		public override string ToString(Period period) { return String.Format("{0} BetterRenko", period.Value);	}


		private enum State { BarComplete, BarAccumulating };
		private State barState = State.BarComplete;

		private double renkoHigh;
		private double renkoLow;

		private void MoveLimits(Data.Bars bars, double price, double brickSize)
		{
			if (bars.Instrument.MasterInstrument.Compare(price, renkoHigh) >= 0)
			{
				do
				{
					renkoHigh += brickSize;
					renkoLow  = renkoHigh - 3.0 * brickSize;
				} while (bars.Instrument.MasterInstrument.Compare(price, renkoHigh) > 0);  // stops if price in range, including edge
			}
			else
			{
				do
				{
					renkoLow -= brickSize;
					renkoHigh = renkoLow + 3.0 * brickSize;
				} while (bars.Instrument.MasterInstrument.Compare(price, renkoLow) < 0);
			}
		}

		private void CheckBarComplete(Data.Bars bars, double price, double brickSize)
		{
			int cmpHi = bars.Instrument.MasterInstrument.Compare(price, renkoHigh);
			int cmpLo = bars.Instrument.MasterInstrument.Compare(price, renkoLow);

			if (cmpHi == 0 || cmpLo == 0)
			{
				barState = State.BarComplete;
				MoveLimits(bars, price, brickSize);  // will move limits once since equal
			}
			else barState = State.BarAccumulating;
		}

		private bool RangeExceeded(Data.Bars bars, double price)
		{
			int cmpHi = bars.Instrument.MasterInstrument.Compare(price, renkoHigh);
			int cmpLo = bars.Instrument.MasterInstrument.Compare(price, renkoLow);

			return (cmpHi > 0 || cmpLo < 0);
		}

		#if NT7
		public override void Add(Data.Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isRealtime)
		#else
		public override void Add(Data.Bars bars, double open, double high, double low, double close, DateTime time, int volume, bool isRealtime)
		#endif
		{
			double brickSize = bars.Instrument.MasterInstrument.Round2TickSize(bars.Period.Value * bars.Instrument.MasterInstrument.TickSize);  // #ticks per brick * tickSize

			// starting new bar at session boundary makes sure you have the same bars
			// reguadless of the first date that is loaded in the chart, feel free to remove
			#if NT7
			if ((bars.Count == 0) || bars.IsNewSession(time))
			#else
			if (bars.Count == 0)
			#endif
			{
				#if NT7
				AddBar(bars, close, close, close, close, time, volume, isRealtime);
				#else
				AddBar(bars, close, close, close, close, time, volume);
				#endif
				barState = State.BarAccumulating;
				double mod = bars.Instrument.MasterInstrument.Round2TickSize(close % brickSize); 
				double mid = bars.Instrument.MasterInstrument.Compare(mod, brickSize) == 0 ? close : close - mod;
				renkoHigh = mid + brickSize;
				renkoLow  = mid - brickSize;
			}
			else 
			{
				if (barState == State.BarComplete)
				{
					// this tick creates a new bar
					#if NT7
					AddBar(bars, close, close, close, close, time, volume, isRealtime);
					#else
					AddBar(bars, close, close, close, close, time, volume);
					#endif
					if (RangeExceeded(bars, close))
						MoveLimits(bars, close, brickSize);
				}
				else
				{
					if (RangeExceeded(bars, close))
					{
						#if NT7
						AddBar(bars, close, close, close, close, time, volume, isRealtime);
						#else
						AddBar(bars, close, close, close, close, time, volume);
						#endif
						MoveLimits(bars, close, brickSize);
					}
					else
					{
						Data.Bar bar = (Bar)bars.Get(bars.Count - 1);
						#if NT7
						UpdateBar(bars, bar.Open, (close > bar.High ? close : bar.High), (close < bar.Low ? close : bar.Low), close, time, volume, isRealtime);
						#else
						UpdateBar(bars, bar.Open, (close > bar.High ? close : bar.High), (close < bar.Low ? close : bar.Low), close, time, volume);
						#endif
					}
				}
				CheckBarComplete(bars, close, brickSize);
			}
			#if NT7
			bars.LastPrice = close;
			#endif
		}

#if NT7
		// this restricts which ChartStyles can be used with this BarType
		//		public override Gui.Chart.ChartStyleType[] ChartStyleTypesSupported
		//		{
		//			get { return new Gui.Chart.ChartStyleType[] { Gui.Chart.ChartStyleType.OpenClose, Gui.Chart.ChartStyleType.CandleStick  }; }
		//		}

		public override PropertyDescriptorCollection GetProperties(PropertyDescriptor propertyDescriptor, Period period, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(propertyDescriptor, period, attributes);

			// here is how you remove properties not needed for that particular bars type
			properties.Remove(properties.Find("BasePeriodType", true));
			properties.Remove(properties.Find("BasePeriodValue", true));
			properties.Remove(properties.Find("PointAndFigurePriceType", true));
			properties.Remove(properties.Find("ReversalType", true));
			properties.Remove(properties.Find("Value2", true));

			// here is how you change the display name of the property on the properties grid
			Gui.Design.DisplayNameAttribute.SetDisplayName(properties, "Value",  "\rBrick size");

			return properties;
		}

		public override void ApplyDefaults(Gui.Chart.BarsData barsData)
		{
			barsData.DaysBack		= 3;
			barsData.Period.Value	= 4;
		}

		public override int GetInitialLookBackDays(Period period, int barsBack)
		{
			return 1;  // TBD a better value???
		}
#else
		// Pre-NT7 Specific
		public override int DaysBack
		{
			get { return Gui.Chart.ChartData.DaysBackTick; }
		}
		
		public override bool IsTimeBased
		{
			get { return false; }
		}

		public override int MaxLookBackDays
		{ 
			get { return 10;} 
		}

		public override int MaxValue
		{
			get { return -1; }
		}

		public override int SortOrder
		{
			get { return 14004; }
		}
#endif

		public override int DefaultValue
		{
			get { return 5; }
		}

		public override bool IsIntraday
		{
			get { return true; }
		}

		public override PeriodType BuiltFrom
		{
			get { return PeriodType.Tick; }
		}

		public override string ChartDataBoxDate(DateTime time)
		{
			return time.ToString(Cbi.Globals.CurrentCulture.DateTimeFormat.ShortDatePattern);
		}

		public override string ChartLabel(NinjaTrader.Gui.Chart.ChartControl chartControl, DateTime time)
		{
			return time.ToString(chartControl.LabelFormatTick, Cbi.Globals.CurrentCulture);
		}

		public override double GetPercentComplete(Data.Bars bars, DateTime now)
		{
			//throw new ApplicationException("GetPercentComplete not supported in " + DisplayName);
			return barState == State.BarComplete ? 100 : 0;
		}
	}
}
