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
    /// Never did get this working, it was on a chart of TF with 3 Renko Bars
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class TFAnaSuper : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int atrPeriod = 3; // Default setting for AtrPeriod
        private double atrMultiplier = 1; // Default setting for AtrMultiplier
        private int medianPeriod = 3; // Default setting for MedianPeriod
        private int brickSize = 3; // Default setting for BrickSize
        private int profitTarget = 5; // Default setting for ProfitTarget
        private int trailingStop = 10; // Default setting for TrailingStop
        private int stopLoss = 10; // Default setting for StopLoss
        // User defined variables (add any user defined variables below)
		int currentHigh = 0;
		int previousHigh = 0;
		int currentLow = 0;
		int previousLow = 0;
		double channelHigh;
		double channelLow;
		
		bool enableShort = false;
		bool enableLong = false;
		
		// Indicator Variables
		anaSuperTrend ast;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(anaSuperTrend(AtrMultiplier, AtrPeriod, MedianPeriod));
            //SetProfitTarget("", CalculationMode.Ticks, ProfitTarget);
            //SetTrailStop("", CalculationMode.Ticks, trailingStop, false);
			EntryHandling = EntryHandling.UniqueEntries;

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			ast = anaSuperTrend(AtrMultiplier, AtrPeriod, MedianPeriod);

			//if (ast.StopLine[0] == High[0])
			
			// Switch to uptrend
			if (ast.UpTrend[0] && !ast.UpTrend[1])
            {
                //DrawArrowUp("My up arrow" + CurrentBar, false, 0, ast.StopLine[0] - 10 * TickSize, Color.Lime);
				if (previousLow > 0)
				{
					double bottom = Low[LowestBar(Low, CurrentBar - previousLow)];
                	DrawDot("My up dot" + CurrentBar, false, 0, bottom, Color.Black);
					channelLow = Math.Min(Low[LowestBar(Low, CurrentBar - previousLow)], Low[LowestBar(Low, CurrentBar - currentLow)]);
					DrawLine("Bottom line" + CurrentBar, 0, channelLow, -10, channelLow, Color.DarkRed);
					enableLong = true;
				}
				previousLow = currentLow;
				currentLow = CurrentBar;

				//double top = High[HighestBar(High, CurrentBar - previousHigh)];
                //DrawDot("My up dot" + CurrentBar, false, 0, top, Color.White);
				//channelHigh = Math.Max(High[HighestBar(High, CurrentBar - previousHigh)], High[HighestBar(High, CurrentBar - currentHigh)]);
				//DrawLine("Top line" + CurrentBar, 0, channelHigh, -10, channelHigh, Color.DarkBlue);
				//previousHigh = currentHigh;
				//currentHigh = CurrentBar;
			}
			
			// Switch to downtrend
			if (!ast.UpTrend[0] && ast.UpTrend[1])
            {				
                //DrawArrowLine("My down line" + CurrentBar, 5, ast.StopLine[0] + 10 * TickSize, 0, ast.StopLine[0] + 5 * TickSize, Color.Cyan);
                //DrawArrowDown("My down arrow" + CurrentBar, false, 0, ast.StopLine[0] + 10 * TickSize, Color.Lime);
				if (previousHigh > 0)
				{
					double top = High[HighestBar(High, CurrentBar - previousHigh)];
					DrawDot("My up dot" + CurrentBar, false, 0, top, Color.White);
					channelHigh = Math.Max(High[HighestBar(High, CurrentBar - previousHigh)], High[HighestBar(High, CurrentBar - currentHigh)]);
					DrawLine("Top line" + CurrentBar, 0, channelHigh, -10, channelHigh, Color.DarkBlue);
					enableShort = true;
				}
				previousHigh = currentHigh;
				currentHigh = CurrentBar;
				
				
				
		   	//	double bottom = Low[LowestBar(Low, CurrentBar - previousLow)];
           //     DrawDot("My down dot" + CurrentBar, false, 0, bottom, Color.Black);
				
			//	channelLow = Math.Min(Low[LowestBar(Low, CurrentBar - previousLow)], Low[LowestBar(Low, CurrentBar - currentLow)]);
			//	DrawLine("Bottom line" + CurrentBar, 0, channelLow, -10, channelLow, Color.DarkRed);

				//previousLow = currentLow;
				//currentLow = CurrentBar;
            }

			if (Position.MarketPosition == MarketPosition.Short 
				&& Close[0] > channelHigh) 
			{
				//ExitShort("");
			}

			if (Position.MarketPosition == MarketPosition.Long 
				&& Close[0] < channelLow) 
			{
				ExitLong("");
			}
			
			//SetTrailStop(CalculationMode.Ticks, 20);
			//SetTrailStop(CalculationMode.Price, (channelHigh - channelLow) * TickSize);

			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (enableLong && Close[0] > channelHigh					
					&& (channelHigh - channelLow) < 2)
				{
					EnterLong("");
					enableLong = false;
					//SetTrailStop("", CalculationMode.Ticks, (channelHigh - channelLow) / TickSize, false);
					SetProfitTarget("", CalculationMode.Ticks, (channelHigh - channelLow) / TickSize);
					SetStopLoss(CalculationMode.Ticks, (channelHigh - channelLow) / TickSize);
					//Print(Time + " channelHigh: " + channelHigh + "  Channel Width: " + (channelHigh - channelLow));
				}
				
				if (enableShort && Close[0] < channelLow
					&& (channelHigh - channelLow) < 2)
				{
					EnterShort("");
					enableShort = false;
					SetProfitTarget("", CalculationMode.Ticks, (channelHigh - channelLow) / TickSize);
					SetStopLoss(CalculationMode.Price, channelHigh);
					DrawDot("My stop" + CurrentBar, false, 0, channelHigh, Color.Red);
					//Print(Time + " channelLow: " + channelLow + "  Channel Width: " + (channelHigh - channelLow));
				}
			}	
        }

        #region Properties
        [Description("sample")]
        [GridCategory("Parameters")]
        public int AtrPeriod
        {
            get { return atrPeriod; }
            set { atrPeriod = Math.Max(1, value); }
        }

        [Description("sample")]
        [GridCategory("Parameters")]
        public double AtrMultiplier
        {
            get { return atrMultiplier; }
            set { atrMultiplier = Math.Max(0, value); }
        }

        [Description("sample")]
        [GridCategory("Parameters")]
        public int MedianPeriod
        {
            get { return medianPeriod; }
            set { medianPeriod = Math.Max(1, value); }
        }

        [Description("BetterRenko Brick Size")]
        [GridCategory("Parameters")]
        public int BrickSize
        {
            get { return brickSize; }
            set { brickSize = Math.Max(2, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailingStop
        {
            get { return trailingStop; }
            set { trailingStop = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
        }
        #endregion
    }
}
