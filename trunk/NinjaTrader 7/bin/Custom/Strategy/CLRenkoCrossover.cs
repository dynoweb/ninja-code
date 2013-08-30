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
    public class CLRenkoCrossover : Strategy
    {
        #region Variables
        // Property variables
			int adxValue = 20;
			int adxPeriod = 17;
			int hmaFastPeriod = 10;
			int hmaMedPeriod = 25;
			int hmaSlowPeriod = 55;
			HMA hmaFast;
			HMA hmaMed;
			HMA hmaSlow;
			ADX adx;
        // Local variables
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(HMA(hmaFastPeriod));
			HMA(hmaFastPeriod).Plots[0].Pen.Color = Color.Yellow;
			Add(HMA(hmaMedPeriod));
			HMA(hmaMedPeriod).Plots[0].Pen.Color = Color.Blue;
			Add(HMA(hmaSlowPeriod));
			HMA(hmaSlowPeriod).Plots[0].Pen.Color = Color.Lime;
			Add(ADX(adxPeriod));
			ADX(adxPeriod).Plots[0].Pen.Color = Color.DarkOrange;
			
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			hmaFast = HMA(HmaFastPeriod);
			hmaMed = HMA(HmaMedPeriod);
			hmaSlow = HMA(HmaSlowPeriod);
			adx = ADX(adxPeriod);

			if (Position.MarketPosition != MarketPosition.Flat) {
				ManageTrade();	
				return;
			}
			
			if (
				adx.Value[0] > AdxValue
				&& Rising(hmaFast)
				//&& Rising(hmaMed)
				//&& hmaFast[0] > hmaMed[0]
				//&& hmaFast[0] > hmaSlow[0]
				&& Close[0] > hmaFast[0]
				&& Close[0] > hmaMed[0]
				&& Close[0] > hmaSlow[0]
				&& Open[0] > Open[1]
				//&& Open[1] > Open[2]
				&& Close[0] > Close[1]
				//&& Close[1] > Close[2]
				&& (hmaSlow[0] - hmaFast[0])/TickSize < 5
				)
			{
				GoLong();
			}
				
			if (
				adx.Value[0] > AdxValue
				&& Falling(hmaFast)
				//&& Falling(hmaSlow)
				//&& hmaFast[0] < hmaMed[0]
				//&& hmaFast[0] < hmaSlow[0]
				&& Close[0] < hmaFast[0]
				&& Close[0] < hmaMed[0]
				&& Close[0] < hmaSlow[0]
				&& Open[0] < Open[1]
				//&& Open[1] < Open[2]
				&& Close[0] < Close[1]
				//&& Close[1] < Close[2]
				&& (hmaFast[0] - hmaSlow[0])/TickSize < 5
				//&& (BarsSinceExit("Short") > 2 || BarsSinceExit("Short") == -1)
				)
				GoShort();		
			
			// This call to go short doesn't depend on ADX
			if (
				   Falling(hmaFast)
				&& hmaFast[0] < hmaMed[0]
				&& hmaMed[0] < hmaSlow[0]
				&& Open[0] < Open[1]
				&& Open[1] < Open[2]
				&& Close[0] < Close[1]
				&& Close[1] < Close[2]
				&& (BarsSinceExit("Short") > 2 || BarsSinceExit("Short") == -1)
				&& 2 == 3
				)
				GoShort();
		}
		
		private void GoLong() 
		{
			//SetTrailStop("Long", CalculationMode.Ticks, TrailStop, false);
			//SetProfitTarget("Long", CalculationMode.Price, Close[0] + (ProfitTarget*TickSize));
			
			string signalName = "Long";
			double limitPrice = High[0];
			double stopPrice = High[0];
			// has been ignored since the stop price is less than or equal to the close price of the current bar
			if (stopPrice <= Close[0]) {
				stopPrice = Close[0];
			}
			EnterLongStopLimit(DefaultQuantity, limitPrice, stopPrice, signalName); 
			EnterLongStop(limitPrice - 10 * TickSize, signalName);
		}

		private void GoShort() 
		{
			//SetTrailStop("Short", CalculationMode.Ticks, TrailStop, false);
			//SetProfitTarget("Short", CalculationMode.Price, Close[0] - (ProfitTarget*TickSize));
			
			string signalName = "Short";
			double limitPrice = Low[0];
			double stopPrice = Low[0];
			// has been ignored since the stop price is greater than or equal to the close price of the current bar
			// Limit price can't be greater than stop price
			if (stopPrice >= Close[0]) {
				stopPrice = Close[0];
			}
			EnterShortStopLimit(DefaultQuantity, limitPrice, stopPrice, signalName); 
		}
		
		private void ManageTrade() 
		{
			if (Position.MarketPosition == MarketPosition.Long) 
			{
				double stop = Math.Min(Low[0], Low[1]);
				// has been ignored since the stop price is greater than or equal to close price of the current bar
				if (stop >= Close[0]) {
					stop = Close[0] - 1 * TickSize;
				}
				this.ExitLongStop(stop, "Long");
				return;
			}

			if (Position.MarketPosition == MarketPosition.Short) 
			{
				double stop = Math.Max(High[0], High[1]);
				// has been ignored since the stop price is less than or equal to close price of the current bar
				if (stop <= Close[0]) {
					stop = Close[0] + 1 * TickSize;
				}
				this.ExitShortStop(stop, "Short");
				return;
			}
		}

        #region Properties
		
        [Description("")]
        [GridCategory("Parameters")]
		public int AdxValue
        {
            get { return adxValue; }
            set { adxValue = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
		public int HmaFastPeriod
        {
            get { return hmaFastPeriod; }
            set { hmaFastPeriod = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
		public int HmaMedPeriod
        {
            get { return hmaMedPeriod; }
            set { hmaMedPeriod = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
		public int HmaSlowPeriod
        {
            get { return hmaSlowPeriod; }
            set { hmaSlowPeriod = Math.Max(1, value); }
        }
		

		#endregion
    }
}
