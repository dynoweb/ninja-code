#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Users several hull MAs to determine entry point. Based on CL with 10 bar Better Renko Bars
    /// </summary>
    [Description("Uses several hull MAs to determine entry point. Based on CL with 10 bar BetterRenko Bars")]
    public class CLStats : Strategy
    {
        #region Variables
		
			// channel
			private double channelHigh = 0;
			private double channelLow = 0;
			private int adjustedChannelSize = 0;		// channel size in ticks
		
			private int hour = 7;
			private int minute = 0;
			private int period = 2;
		
			StatStrut stat;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Bars.BarsSinceSession == 1)
			{
				stat = new StatStrut();
			}
			
			if (ToTime(Time[0]) == ToTime(hour, minute, 0))
			{
				if (null == stat)
					stat = new StatStrut();
				
				stat.high1 =  High[HighestBar(High, period * 1)];
				stat.low1 = Low[LowestBar(Low, period * 1)];
				stat.high2 =  High[HighestBar(High, period * 2)];
				stat.low2 = Low[LowestBar(Low, period * 2)];
				stat.high3 =  High[HighestBar(High, period * 4)];
				stat.low3 = Low[LowestBar(Low, period * 4)];
				stat.sessionHigh =  High[HighestBar(High, Bars.BarsSinceSession)];
				stat.sessionLow=  Low[LowestBar(Low, Bars.BarsSinceSession)];
				stat.sessionPeriod = Bars.BarsSinceSession;
				
				adjustedChannelSize = calcAdjustedChannelSize();
				
				Print(Time + ", " + stat.stringOf() + ", " + adjustedChannelSize);		
			
				DrawDot(CurrentBar + "channelHigh", false, 0, channelHigh, Color.Blue);
				DrawDot(CurrentBar + "channelLow", false, 0, channelLow, Color.Cyan);
				DrawHorizontalLine("channelHigh", channelHigh, Color.Blue);
				DrawHorizontalLine("channelLow", channelLow, Color.Cyan);
			}
		}
		
		private int calcAdjustedChannelSize()
		{
			double channelSize = 0;
			int adjustedChannelSize = 0;
			
			int checkPeriod = Math.Min(period, Bars.BarsSinceSession);  // Bars.PercentComplete
			
			channelHigh = High[HighestBar(High, checkPeriod)];
			channelLow = Low[LowestBar(Low, checkPeriod)];
			BackColor = Color.Coral;

			channelSize = (Instrument.MasterInstrument.Round2TickSize(channelHigh - channelLow))/TickSize;
			adjustedChannelSize = (int) channelSize;
			int endRectangle = 0;
			if (BarsPeriod.Id == PeriodType.Minute) {
				endRectangle = 240/BarsPeriod.Value;
			}			
			DrawRectangle(CurrentBar + "rectangle", checkPeriod - 1, channelLow, -endRectangle, channelHigh, Color.DarkBlue);
			
			return adjustedChannelSize;
		}
			
		private void GoLong() 
		{
		}

		private void GoShort() 
		{
		}
		
		private void ManageTrade() 
		{
		}

    }
	
	class StatStrut
	{
	
		public double high1;
		public double low1;
		public double high2;
		public double low2;
		public double high3;
		public double low3;
		public double sessionHigh;
		public double sessionLow;
		public int sessionPeriod;
		
		public string stringOf()
		{
			return sessionHigh + ", " + sessionLow + ", " + sessionPeriod
				+ ", " + high1 + ", " + low1
				+ ", " + high2 + ", " + low2
				+ ", " + high3 + ", " + low3;			
		}
	}
}
