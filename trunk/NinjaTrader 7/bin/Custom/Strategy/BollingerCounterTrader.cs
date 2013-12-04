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
    /// Counter Trades between Boolinger bands
    /// </summary>
    [Description("Counter Trades between Boolinger bands")]
    public class BollingerCounterTrader : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int bollingerPeriod = 30; // Default setting for bollingerPeriod
        private int donchianPeriod = 25; // Default setting for ChannelPeriod
        private double profitTarget = 2.000; // Default setting for ProfitTarget
        private double stdDevMax = 2.000; // Default setting for StdDevMax
        private double stdDevMin = 1.5; // Default setting for StdDevMin
        // User defined variables (add any user defined variables below)
		bool closedBelowDC = false;
		bool closedBelowBMax = false;
		bool closedBelowBMin = false;
		bool closedAboveDC = false;
		bool closedAboveBMax = false;
		bool closedAboveBMin = false;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DonchianChannelClose(DonchianPeriod));
			DonchianChannelClose(DonchianPeriod).Displacement = 2;
			DonchianChannelClose(DonchianPeriod).AutoScale = false;
			DonchianChannelClose(DonchianPeriod).Plots[0].Pen.Color = Color.Yellow;	// mean
			DonchianChannelClose(DonchianPeriod).Plots[1].Pen.Color = Color.Black;		// top plot
			DonchianChannelClose(DonchianPeriod).Plots[2].Pen.Color = Color.Black;		// bottom plot
			DonchianChannelClose(DonchianPeriod).PaintPriceMarkers = false;
			
            Add(Bollinger(StdDevMin, BollingerPeriod));
			Bollinger(StdDevMin, BollingerPeriod).Plots[0].Pen.Color = Color.Red;		// upper
			Bollinger(StdDevMin, BollingerPeriod).Plots[1].Pen.Color = Color.Transparent;		// mean
			Bollinger(StdDevMin, BollingerPeriod).Plots[2].Pen.Color = Color.Red; 	// lower
			Bollinger(StdDevMin, BollingerPeriod).PaintPriceMarkers = false;
			
			Add(Bollinger(StdDevMax, BollingerPeriod));
			Bollinger(StdDevMax, BollingerPeriod).Plots[0].Pen.Color = Color.Green;		// upper
			Bollinger(StdDevMax, BollingerPeriod).Plots[1].Pen.Color = Color.Transparent;		// mean
			Bollinger(StdDevMax, BollingerPeriod).Plots[2].Pen.Color = Color.Green; 	// lower
			Bollinger(StdDevMax, BollingerPeriod).PaintPriceMarkers = false;
			
            SetProfitTarget("", CalculationMode.Percent, ProfitTarget);

			Aggregated = true;
			CalculateOnBarClose = true;
			//ClearOutputWindow();
			EntriesPerDirection = 1; // Order Handling enforce only 1 position long or short at any time
			EntryHandling = EntryHandling.AllEntries;
			ExitOnClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Checks to make sure we have at least x or more bars
			if (CurrentBar < DonchianPeriod)
        		return;

			// Set States 
			// Above top max bollinger
			if (Close[0] > Bollinger(StdDevMax, BollingerPeriod).Upper[0])
			{
				closedBelowBMax = false;
				closedAboveBMax = true;
			}
			// Above top min bollinger
			if (Close[0] > Bollinger(StdDevMin, BollingerPeriod).Upper[0])
			{
				closedBelowBMax = false;
				closedAboveBMin = true;
			}
			// Below lower DC
			if (Close[0] < MIN(Close, DonchianPeriod)[1])
			{
				closedBelowDC = true;
				closedAboveDC = false;
				closedAboveBMin = false;
			}
			// Below lower max bollinger
			if (Close[0] < Bollinger(StdDevMax, BollingerPeriod).Lower[0])
			{
				closedBelowBMax = true;
				closedAboveBMax = false;
				closedAboveBMin = false;
			}
			
			if (Close[0] > MAX(Close, DonchianPeriod)[1])
			{
				closedBelowDC = false;
				closedAboveDC = true;
			}
			
			if (closedBelowDC && closedBelowBMax)
			{
				DrawDot(CurrentBar  + "Long Enabled", false, 0, Low[0] - 3 * TickSize, Color.Red);
			}
			if (closedAboveDC && closedAboveBMax)
			{
				DrawDot(CurrentBar  + "Short Enabled", false, 0, High[0] + 3 * TickSize, Color.Green);
			}
				
			// EXIT CODE			
			if (Position.MarketPosition == MarketPosition.Long)
			{
				// Exit after x bars
//				if (ExitAfterXBars != 0 
//					&& BarsSinceEntry() >= ExitAfterXBars
//					)
//				{
//					closeXBarsOrder = ExitLong("AfterXBars", "Reversal");
//					Print(Time + " Exit market on open due to time in trade " + Instrument.FullName);
//				}
				
				if (closedAboveBMin && Close[0] < Bollinger(StdDevMin, BollingerPeriod).Upper[1])
				{
					ExitLong("Long Exit", "");
					closedBelowDC = false;
					closedBelowBMax = false;
					closedAboveDC = false;
					closedAboveBMax = false;
				}
				
				// Exit if short reversal detected
				//if (Close[0] > Bollinger(StdDevMax, BollingerPeriod).Upper[1] && false)
				if (BarsSinceEntry() > 4 && Close[0] < MIN(Close, DonchianPeriod)[1])
				{
					ExitLong("Normal Exit", "");
					closedBelowDC = false;
					closedBelowBMax = false;
					//Print(Time + " closing since prior bar broke above DC");
				}
				if (false && Close[0] > Bollinger(StdDevMin, BollingerPeriod).Upper[0])
				{
					ExitLong("Early Exit", "");
					closedBelowDC = false;
					closedBelowBMax = false;
				}
			}			

			// ENTRY CODE
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				if (closedBelowDC && closedBelowBMax)
				{
					if (Close[0] > Bollinger(StdDevMin, BollingerPeriod).Lower[0])
					{
						EnterLong();
					}
				}
                //&& DonchianChannelClose(ChannelPeriod).Lower[1] == DonchianChannelClose(14).Lower[0])            
            }
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public double StdDevMax
        {
            get { return stdDevMax; }
            set { stdDevMax = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double StdDevMin
        {
            get { return stdDevMin; }
            set { stdDevMin = Math.Max(0.250, value); }
        }

        [Description("Used to reduce chop")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
        }

        [Description("Used to reduce chop")]
        [GridCategory("Parameters")]
        public int BollingerPeriod
        {
            get { return bollingerPeriod; }
            set { bollingerPeriod = Math.Max(1, value); }
        }

        [Description("Percent Profit")]
        [GridCategory("Parameters")]
        public double ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(0.500, value); }
        }
        #endregion
    }
}
