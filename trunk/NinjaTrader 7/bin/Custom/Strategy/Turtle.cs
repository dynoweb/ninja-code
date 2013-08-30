// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
using NinjaTrader.Strategy;
#endregion


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This is a sample multi-time frame strategy.
    /// </summary>
    [Description("This is a sample multi-time frame strategy.")]
    public class Turtle : Strategy
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
		double stop = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            // Add a 5 minute Bars object to the strategy
			//Add(PeriodType.Minute, 5);
			//Add(PeriodType.Minute, 55);
			
			// Add a 15 minute Bars object to the strategy
			//Add(PeriodType.Minute, 15);
			
			// Note: Bars are added to the BarsArray and can be accessed via an index value
			// E.G. BarsArray[1] ---> Accesses the 5 minute Bars object added above
			
			// Add simple moving averages to the chart for display
			// This only displays the SMA's for the primary Bars object on the chart
			
            //Add(EMA(15));
            //Add(SMARick(50));
			//Add(Stochastics(3,5,2));
			Add(PitColor(Color.Black, 70000, 25, 165000));
			
			Add(DonchianChannel(20));
			DonchianChannel(20).Plots[0].Pen.Color = Color.Red;	// mean
			DonchianChannel(20).Plots[1].Pen.Color = Color.Blue;		// upper
			DonchianChannel(20).Plots[2].Pen.Color = Color.Blue;		// lower
			
			Add(DonchianChannel(55));
			DonchianChannel(55).Plots[0].Pen.Color = Color.DarkGreen;	// mean
			DonchianChannel(55).Plots[1].Pen.Color = Color.Black;		// upper
			DonchianChannel(55).Plots[2].Pen.Color = Color.Black;		// lower

			Add(ATR(20));
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// OnBarUpdate() will be called on incoming tick events on all Bars objects added to the strategy
			// We only want to process events on our primary Bars object (index = 0) which is set when adding
			// the strategy to a chart
			if (BarsInProgress != 0)
				return;
			
			// Checks  if the 5 period SMA is above the 50 period SMA on both the 5 and 15 minute time frames
			//if (SMA(BarsArray[1], 5)[0] > SMA(BarsArray[1], 50)[0] && SMA(BarsArray[2], 5)[0] > SMA(BarsArray[2], 50)[0])
			
			if (High[0] > DonchianChannel(55).Upper[1])
			{
				// Checks for a breakout and enter long
				//if (CrossAbove(EMA(15), SMARick(50), 1))
				if (Position.MarketPosition == MarketPosition.Flat)
				{
					EnterLong(10, "Donch");
					stop = int.MinValue;
				}
			}
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				//int highBarSinceOpen = HighestBar(High, BarsSinceEntry());
				double highSinceOpen = Low[HighestBar(High, BarsSinceEntry())];
				stop = Math.Max(stop, highSinceOpen - (2 * ATR(20)[0]));
				Print(highSinceOpen+ " " +stop);
				DrawDot(CurrentBar + "stop", false, 0, stop, Color.Red);
			}
			
			// Checks for a cross below condition of the 5 and 15 period SMA on the 15 minute time frame and exits long
			if (Low[0] < stop)
				ExitLong(10);
        }

        #region Properties
        #endregion
    }
}
