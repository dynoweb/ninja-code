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
    /// Written by Rick Cromer
	/// Uses the @Stochastics indicator to find Highs/Lows 
	/// 
	/// Strategy not specific to a particular instrument or period
    /// This straegy is based on the Top Dog Trading rules.
	/// I'm currently working with ES 610 Tick
    /// </summary>
    [Description("This strategy is based on the Top Dog Trading rules.")]
    public class Dog : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int smaPeriod = 50; // Default setting for SmaPeriod
        private int emaPeriod = 15; // Default setting for EmaPeriod
        // User defined variables (add any user defined variables below)
		int trend = 0;  // +1 = rising, -1 = falling
		int trendStartBar = 0;
		int cycleHigh = 0;  // bar away from current cycle high
		int cycleLow = 99999;
		
		Stochastics stoc;
		int stocD = 3;
		int stocK = 5;
		int stocS = 2;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(EMA(EmaPeriod));
            Add(SMA(SmaPeriod));
			SMA(SmaPeriod).Plots[0].Pen.Color = Color.Red;	
            Add(Stochastics(stocD, stocK, stocS));
			Stochastics(stocD, stocK, stocS).Plots[0].Pen.Color = Color.Blue; // D color
			Stochastics(stocD, stocK, stocS).Plots[1].Pen.Color = Color.DarkGray; // K color
			Stochastics(stocD, stocK, stocS).Lines[0].Pen.Color = Color.Black; // Lower
			Stochastics(stocD, stocK, stocS).Lines[1].Pen.Color = Color.Black; // Upper
			ConstantLines(45,55,0,0).Panel = 1;	// specifying to use the first indicator's panel
			Add(ConstantLines(45,55,0,0)); 
			ConstantLines(45,55,0,0).Plots[0].Pen.Color = Color.Red;
			ConstantLines(45,55,0,0).Plots[1].Pen.Color = Color.Red;			
			
			SetProfitTarget("", CalculationMode.Ticks, 14);
            //SetTrailStop("", CalculationMode.Ticks, 5, false);
			SetStopLoss("", CalculationMode.Ticks, 10, false);

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			stoc = Stochastics(stocD, stocK, stocS);
			
			int currentTrend = Rising(SMA(SmaPeriod)) ? 1: -1;
			
			MarkCycleHigh();			
			MarkCycleLow();			
			
			if ((trend <= 0 && currentTrend == 1) 
				|| (trend >= 0 && currentTrend == -1))
			{
				// reset trend
				trend = currentTrend;
				trendStartBar = CurrentBar;
			}
			
			/*
			uptrend conditions
			1 - new
			2 - cross above 45, look for cycle high
			3 - cross above 55
			4 - cross below 55, stop cycle high search
			*/
			if (currentTrend == 1) 
			{
				switch (trend) {
					case 1:
						if (stoc.D[0] > 45)
							trend = 2;
						if (stoc.D[0] > 55)
							trend = 3;						
						break;
					case 2:
						if (stoc.D[0] > 55)
							trend = 3;						
						if (CrossBelow(stoc.D, 45, 1))
							trend = 1;
						break;
					case 3:
						if (CrossBelow(stoc.D, 55, 1))
							trend = 4;
						break;
					default:
						
						break;
				}
			}
			if (currentTrend == -1) 
			{
				switch (trend) {
					case -1:
						if (stoc.D[0] < 55)
							trend = -2;
						if (stoc.D[0] < 45)
							trend = -3;						
						break;
					case -2:
						if (stoc.D[0] < 45)
							trend = -3;						
						if (CrossAbove(stoc.D, 55, 1))
							trend = -1;
						break;
					case -3:
						if (CrossAbove(stoc.D, 55, 1))
							trend = -4;
						break;
					default:
						
						break;
				}
			}
			
			if (trend >= 1) 
			{
				cycleHigh = HighestBar(High, CurrentBar - trendStartBar);
			}
			if (trend <= -1)
				cycleLow = LowestBar(Low, CurrentBar - trendStartBar);	
			
            if (trend < 0)
			{
				//DrawDot("Low" + CurrentBar, false, 0, Low[cycleLow], Color.Black);
			}
			
            if (trend > 0)
			{
				//DrawDot("High" + CurrentBar, false, 0, High[cycleHigh], Color.Black);
				Print(Time + " cycleHigh: " + cycleHigh);
				
				if (stoc.D[2] > stoc.D[1]
	                && stoc.D[1] < stoc.D[0]
					)
				{
					DrawArrowUp("My up arrow" + CurrentBar, false, 0, Low[0], Color.Black);
					
					if (Position.MarketPosition == MarketPosition.Flat)
					{
						EnterLongStop(High[0] + 1 * TickSize);
					}
				}            
            }
        }

		void MarkCycleHigh()
		{
			if (CrossBelow(stoc.D, 55.0, 1)) 
			{
				int barsAgo = 1;
				DrawDot(CurrentBar + "CrossedBelow", true, 0, High[0] + 1 * TickSize, Color.White);
				while (stoc.D[barsAgo] > 45)
				{
					barsAgo++;
				}
				int highBar = HighestBar(Close, barsAgo);
				DrawDot(CurrentBar + "BarsAgo", true, barsAgo, High[0] + 1 * TickSize, Color.Yellow);
				DrawDot(CurrentBar + "High", true, highBar, 2 * High[highBar] - Low[highBar], Color.Blue);
			}
		}
		
		void MarkCycleLow()
		{
			if (CrossAbove(stoc.D, 45.0, 1)) 
			{
				int barsAgo = 1;
				//DrawDot(CurrentBar + "CrossedBelow", true, 0, High[0] + 1 * TickSize, Color.White);
				while (stoc.D[barsAgo] < 55)
				{
					barsAgo++;
				}
				int lowBar = LowestBar(Close, barsAgo);
				//DrawDot(CurrentBar + "BarsAgo", true, barsAgo, High[0] + 1 * TickSize, Color.Yellow);
				DrawDot(CurrentBar + "Low", true, lowBar, 2 * Low[lowBar] - High[lowBar], Color.Red);
			}
		}
		
        #region Properties
        [Description("SMA Period")]
        [GridCategory("Parameters")]
        public int SmaPeriod
        {
            get { return smaPeriod; }
            set { smaPeriod = Math.Max(1, value); }
        }

        [Description("EMA Period")]
        [GridCategory("Parameters")]
        public int EmaPeriod
        {
            get { return emaPeriod; }
            set { emaPeriod = Math.Max(1, value); }
        }
        #endregion
    }
}
