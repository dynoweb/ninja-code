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
    public class DogCycleCounter : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int profitTarget = 15; // Default setting for ProfitTarget
        private int stopLoss = 10; // Default setting for StopLoss
        private int trailingStop = 15; // Default setting for TrailingStop
		private double minMacdAvg = 0.0;
		private double maxRiskRewardRatio = 0.8;
		private double minSlope = 0.001;
        // User defined variables (add any user defined variables below)
		
		
		int EmaPeriod = 15;
		int SmaPeriod = 50;

		EMA ema;
		SMARick sma;
		ConstantLines constantLines;
		PitColor pitColor;
		
		Stochastics stoc;
		int stocD = 3;
		int stocK = 5; 
		int stocS = 2;
		
		// Cycle Counter Vars
		
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			pitColor = PitColor(Color.Black, 80000, 25, 161500);
			Add(pitColor);
			
			stoc = Stochastics(stocD, stocK, stocS);
            Add(stoc);
			stoc.Plots[0].Pen.Color = Color.Blue; // D color
			stoc.Plots[1].Pen.Color = Color.Black; // K color
			stoc.Plots[0].Pen.Width = 2; // D color
			stoc.Plots[1].Pen.Width = 1; // K color
			
			stoc.Lines[0].Pen.Color = Color.Black; // Lower
			stoc.Lines[1].Pen.Color = Color.Black; // Upper
			stoc.Lines[0].Pen.DashStyle = DashStyle.Dot; // Lower
			stoc.Lines[1].Pen.DashStyle = DashStyle.Dot; // Upper
			stoc.Lines[0].Pen.Width = 2; // Lower
			stoc.Lines[1].Pen.Width = 2; // Upper
			stoc.Lines[0].Value = 20; // Lower
			stoc.Lines[1].Value = 80; // Upper
			
			constantLines = ConstantLines(45,55,0,0);
			constantLines.Panel = 1;	// specifying to use the first indicator's panel
			Add(constantLines); 
			constantLines.Plots[0].Pen.Color = Color.Red;
			constantLines.Plots[1].Pen.Color = Color.Red;			
			
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			/*
			if (CrossAbove(stoc.D, 50, 1))
			{
				BackColor = Color.LimeGreen;
				DrawText(CurrentBar + "x", CurrentBar + "", 0, High[0] + 5 * TickSize, Color.Black);
			}
			if (CrossBelow(stoc.D, 50, 1))
			{
				BackColor = Color.Yellow;
				DrawText(CurrentBar + "x", CurrentBar + "", 0, Low[0] - 5 * TickSize, Color.Red);
			}
			*/
			
			/// Here’s how we define Cycle highs and lows:
			/// A Cycle high is the highest high in price after %D goes above 45 and before it gets back below 55.
			/// A Cycle low is the lowest low in price after %D goes below 55 and before it gets back above 45.
		
			// Mark Cycle High
			if (CrossBelow(stoc.D, 55, 1)) 
			{
				//BackColor = Color.LimeGreen;
				int i = 2;
				while (CurrentBar - i > 0 && !CrossAbove(stoc.D, 45, i)) 
				{
					i++;
				}
				int barsAgo = HighestBar(High, i);
				DrawDot(CurrentBar + "high", true, barsAgo, High[barsAgo] + 5 * TickSize, Color.Green);
			}
			
			// Mark Cycle Low
			if (CrossAbove(stoc.D, 45, 1)) 
			{
				//BackColor = Color.Yellow;
				int i = 2;
				while (CurrentBar - i > 0 && !CrossBelow(stoc.D, 55, i)) 
				{
					i++;
				}
				int barsAgo = LowestBar(Low, i);
				DrawDot(CurrentBar + "low", true, barsAgo, Low[barsAgo] - 5 * TickSize, Color.Red);
			}
			
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(1, value); }
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
        public Double MinMacdAvg
        {
            get { return minMacdAvg; }
            set { minMacdAvg = Math.Max(0.0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public Double MaxRiskRewardRatio
        {
            get { return maxRiskRewardRatio; }
            set { maxRiskRewardRatio = Math.Max(0.0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public Double MinSlope
        {
            get { return minSlope; }
            set { minSlope = Math.Max(0.0, value); }
        }
		#endregion
    }
}
