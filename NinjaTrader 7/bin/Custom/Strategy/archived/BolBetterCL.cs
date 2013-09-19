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
    /// BetterRenko 4 CL Bollinger Bands
    /// </summary>
    [Description("BetterRenko 4 CL Bollinger Bands")]
    public class BolBetterCL : Strategy
    {
        #region Variables
        // Properties
        	int period = 70; // Default setting for Period
        // Local variables 
			Bollinger bolCyan;
			Bollinger bolGreen;
			Bollinger bolBlue;
		
			int lastTouchedBlue = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(Bollinger(0.5, Period));
			Add(Bollinger(1.0, Period));
			Add(Bollinger(2.0, Period));
			
			Bollinger(0.5, Period).Plots[0].Pen.Color = Color.Cyan;
			Bollinger(0.5, Period).Plots[1].Pen.Color = Color.Transparent;
			Bollinger(0.5, Period).Plots[2].Pen.Color = Color.Cyan;
			
			Bollinger(1.0, Period).Plots[0].Pen.Color = Color.Lime;
			Bollinger(1.0, Period).Plots[1].Pen.Color = Color.Transparent;
			Bollinger(1.0, Period).Plots[2].Pen.Color = Color.Lime;
			
			Bollinger(2.0, Period).Plots[0].Pen.Color = Color.Blue;
			Bollinger(2.0, Period).Plots[1].Pen.Color = Color.White;
			Bollinger(2.0, Period).Plots[2].Pen.Color = Color.Blue;
		
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			bolCyan = Bollinger(0.5, Period);
			bolGreen = Bollinger(1.0, Period);
			bolBlue = Bollinger(2.0, Period);
			
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (Low[0] <= bolBlue.Lower[0]) {
					lastTouchedBlue = CurrentBar;
				}

				//  && rewardRiskRatio > 1.0
				double rewardRiskRatio = (bolBlue.Middle[0] - bolGreen.Lower[0])
					/ (bolGreen.Lower[0] - bolBlue.Lower[0]);
					
				double barHeight = Math.Max(High[0] - Low[0], High[1] - Low[1]);
				barHeight = Math.Max(barHeight, High[2] - Low[2]);
				
				if (CurrentBar - lastTouchedBlue < 20
					//&& (bolGreen.Lower[0] - bolBlue.Lower[0]) > barHeight
					) {
					EnterLongStopLimit(bolGreen.Lower[0], bolGreen.Lower[0]);
					ExitLongStop(bolBlue.Lower[0]);
					ExitLongLimit(bolBlue.Middle[0]);
				}
			} 
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				lastTouchedBlue = 0;
				double stop = Math.Max(bolBlue.Lower[0], bolBlue.Lower[BarsSinceEntry()]); 	
				double limit = bolBlue.Middle[0];
				ExitLongStop(stop);
				ExitLongLimit(limit);
			}
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
        #endregion
    }
}
