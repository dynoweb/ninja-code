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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class StatisticalTester : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
			int hour = 7;
			int minute = 0;
			double sessionHigh = 0;
			double sessionLow = 0;
			int tradingDaysBack = 1;			
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//Add(new NetVolDoublerAlert());
            CalculateOnBarClose = true;			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar == 1)
				Print("Date,open,high,low,close,sessionLow,sessionHigh");

			if (Bars.BarsSinceSession == 100000000)
			{
				double value = StdDev(High, 100)[0];
				Print(Time + " The current StdDev value is " + value.ToString());
				// Time Series Forecast function displays the statistical trend of a 
				// security's price over a specified time period based on linear regression analysis.
				value = TSF(6, 20)[0];
				Print(Time + " The current TSF value is " + value.ToString());
				double median =  (GetMedian(Open, 9));
				Print(Time + " The median of the last 10 open prices is: " + GetMedian(Open, 9).ToString());
				DrawVerticalLine(CurrentBar + "line", 0, Color.Blue);
			}
			
			
			if (Bars.FirstBarOfSession)
			{
				double open = Bars.GetDayBar(tradingDaysBack).Open;
				double high = Bars.GetDayBar(tradingDaysBack).High;
				double low = Bars.GetDayBar(tradingDaysBack).Low;
				double close = Bars.GetDayBar(tradingDaysBack).Close;
				Print(Bars.GetDayBar(tradingDaysBack).Time + "," + open + "," + high + "," + low + "," + close + "," + sessionLow + "," + sessionHigh);
			}

			if (ToTime(Time[0]) == ToTime(hour, minute, 0))
			{				
				sessionHigh = High[HighestBar(High, Bars.BarsSinceSession - 1)];
				sessionLow = Low[LowestBar(Low, Bars.BarsSinceSession - 1)];
			}
			
			//NetVolDoublerAlert nd = new NetVolDoublerAlert();
			
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
        #endregion
    }
}
