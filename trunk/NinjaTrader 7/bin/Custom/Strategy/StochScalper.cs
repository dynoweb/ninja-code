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
    public class StochScalper : Strategy
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
		
		MACD macd;
		int macdFast = 5;
		int macdSlow = 20;
		int macdSmooth = 30;
		
		double cycleLow = Double.MaxValue;
		double cycleHigh = Double.MinValue;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			pitColor = PitColor(Color.Black, 80000, 25, 161500);
			Add(pitColor);
            
			ema = EMA(EmaPeriod);
			Add(ema);
			ema.Plots[0].Pen.Color = Color.Gray;
			
			sma = SMARick(SmaPeriod);
            Add(sma);
			//sma.Plots[0].Pen.Color = Color.Red;	

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
			
			macd = MACD(macdFast,macdSlow,macdSmooth);			
			Add(macd);			
			macd.CalculateOnBarClose = true;
			macd.Plots[0].Pen.Color = Color.Black;	// Macd
			macd.Plots[1].Pen.Color = Color.Blue;	// Avg
			macd.Plots[2].Pen.Color = Color.Transparent;		//downBar
			//macd.Plots[3].Pen.Color = Color.Transparent;	// Diff
			macd.Plots[0].Pen.Width = 2;
			macd.Plots[1].Pen.Width = 3;
			macd.Plots[0].PlotStyle = PlotStyle.Bar;			
			
            SetProfitTarget("", CalculationMode.Ticks, ProfitTarget);
            SetStopLoss("", CalculationMode.Ticks, StopLoss, false);
            SetTrailStop("", CalculationMode.Ticks, TrailingStop, false);

			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {			
			if (CurrentBar < SmaPeriod) 
				return;
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				if ((CurrentBar % 10)  == 0) {
					//DrawText(CurrentBar + "Txt", Slope(sma, 5, 0) + "", 0, High[0] + 10 * TickSize, Color.Black);	
				}
					
				if (isStocKLong()) 
				{					
					if (Rising(sma))
					{
						DrawArrowUp(CurrentBar + "Up", 0, cycleLow, Color.Yellow);
						
						if (Slope(sma, 3, 0) > MinSlope)
						{
							//DrawText(CurrentBar + "Txt", Slope(sma, 3, 0) + "", 0, High[0] + 20 * TickSize, Color.Black);
							//if (Math.Abs(macd.Avg[0]) > MinMacdAvg)
							//{
								DrawArrowUp(CurrentBar + "Up", 0, cycleLow, Color.LightGoldenrodYellow);
								double entryPrice = High[0] + 1 * TickSize;
								double stopPrice = cycleLow - 1 * TickSize;
								double risk = entryPrice - stopPrice;
								double myTarget = risk/MaxRiskRewardRatio + High[0] + 1 * TickSize;
							
								Print(Time + " risk/(profitTarget * TickSize) " + risk/(profitTarget * TickSize) + " < MaxRiskRewardRatio " + MaxRiskRewardRatio);
								//if (risk < profitTarget * TickSize)
							
								if (risk/(profitTarget * TickSize) < MaxRiskRewardRatio)
								{
									DrawArrowUp(CurrentBar + "Up", 0, cycleLow, Color.Green);
									EnterLongStop(High[0] + 1 * TickSize);
									//EnterLongLimit(Close[0]);
									SetStopLoss("", CalculationMode.Price, stopPrice, false);
									SetProfitTarget("", CalculationMode.Price, myTarget);
								}
								//else
								//{
									//DrawArrowUp(CurrentBar + "Up", 0, cycleLow, Color.Black);
								//}
							//}
						}
						//else
						//{
						//	DrawText(CurrentBar + "Txt", "0", 0, High[0] + 15 * TickSize, Color.Black);
						//}
					}
				}
				if (Math.Abs(macd.Avg[0]) > MinMacdAvg && Falling(sma) && isStocKShort())
				{
					DrawArrowDown(CurrentBar + "Down", 0, cycleHigh, Color.Red);
				}
			}			
        }
		
		bool isStocKLong() 
		{
			int i = 1;
			//Print(Time + " stoc.K[0]: " + stoc.K[0]);
			if (stoc.K[0] > stoc.K[1])
			{
				cycleLow = Low[0];
				while (stoc.K[i] < 50)
				{
					cycleLow = Math.Min(cycleLow, Low[i]);
					if (stoc.K[i] <= 20)
						return true;
					i++;
				}
			}
			return false;
		}
		
		bool isStocKShort() 
		{
			int i = 1;
			if (stoc.K[0] < stoc.K[1])
			{
				cycleHigh = High[0];
				while (stoc.K[i] > 50)
				{
					cycleHigh = Math.Max(cycleHigh, High[i]);
					if (stoc.K[i] >= 80)
						return true;
					i++;
				}
			}
			return false;
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
