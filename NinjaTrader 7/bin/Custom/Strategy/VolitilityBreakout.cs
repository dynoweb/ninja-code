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
    [Description("15 min CL - looks for moves following consolidation")]
    public class VolitilityBreakout : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double profitPercentage = 100;
		private double stopLossPercentage = 100;
		private int exitsAfterXBars = 2;
        // User defined variables (add any user defined variables below)
		rcVolatilityBreakout vbo;
		bool enableTrade = false;
		int squeezeSize = 0;
		int maxSinceSqueeze = 10;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			//ClearOutputWindow();

            //SetStopLoss("", CalculationMode.Percent, stopLossPercentage, false);
            //SetProfitTarget("", CalculationMode.Percent, profitPercentage);
			int length = 20;
			vbo = rcVolatilityBreakout(length);
			
			Add(vbo);
			vbo.AutoScale = false;	

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 20)
				return;
			
			int shares = 0;
			
			if (vbo.SqueezeOn[0] != 0)
				squeezeSize++;
			if (vbo.BarsSinceSqueeze[0] > this.maxSinceSqueeze)
				squeezeSize = 0;
			
			//Print(Time + " CurrentBar: " + CurrentBar + " BarsSinceSqueeze: " + vbo.BarsSinceSqueeze[0] + " SqueezeOn: " + vbo.SqueezeOn[0] + " SqueezeHigh: " + vbo.SqueezeHigh[0] + " SqueezeLow: " + vbo.SqueezeLow[0]);
			if (vbo.BarsSinceSqueeze[0] == 1) 
				enableTrade = true;
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
//				if (vbo.Squeeze[0] == 0.0)
//					DrawSquare(CurrentBar + "sq", false, 0, High[0], Color.DarkRed);
//				else
//					DrawSquare(CurrentBar + "sq", false, 0, High[0], Color.DarkGreen);
//				
//				if (vbo.SqueezeOn[0] == 0.0)
//					DrawSquare(CurrentBar + "sqo", false, 0, Low[0], Color.LightGray);
//				else
//					DrawSquare(CurrentBar + "sqo", false, 0, Low[0], Color.LightGreen);
				
				if (enableTrade && vbo.BarsSinceSqueeze[0] > 0 && squeezeSize > 1)
				{
					//DrawText(CurrentBar + "bss", vbo.BarsSinceSqueeze[0].ToString(), 0, ATR(5)[0] + High[0], Color.Black);
					if (vbo.BarsSinceSqueeze[0] == 1)
					{
						DrawText(CurrentBar + "sh", vbo.SqueezeHigh[0].ToString(), 4, vbo.SqueezeHigh[0] + 0.05, Color.Black);
						DrawText(CurrentBar + "sl", vbo.SqueezeLow[0].ToString(), 4, vbo.SqueezeLow[0] - 0.05, Color.Black);
					}
					
					if (vbo.SqueezeHigh[0] > 0 && Close[0] > vbo.SqueezeHigh[0])				
					{
						shares = (int) Math.Floor(5000/vbo.SqueezeHigh[0]);
						SetStopLoss(CalculationMode.Price, vbo.SqueezeHigh[0] + (vbo.SqueezeHigh[0] - vbo.SqueezeLow[0]));
						EnterShort(shares);
						//SetProfitTarget("", CalculationMode.Percent, 0.015);
						//SetStopLoss("", CalculationMode.Percent, 0.015, false);
						enableTrade = false;
					} 
					else if (vbo.SqueezeLow[0] > 0 && Close[0] < vbo.SqueezeLow[0])				
					{
						shares = (int) Math.Floor(5000/vbo.SqueezeLow[0]);
						SetStopLoss(CalculationMode.Price, vbo.SqueezeLow[0] - (vbo.SqueezeHigh[0] - vbo.SqueezeLow[0]));
						EnterLong(shares);
				//double ptarget = vbo.SqueezeLow[0] - (vbo.SqueezeHigh[0] - vbo.SqueezeLow[0]);
				//Print(Time + " ptarget: " + ptarget);
				//SetProfitTarget("", CalculationMode.Price, ptarget);
						enableTrade = false;
					}
				}
			}
//				Print(Time + " PMomentumUp: " + vbo.PMomentumUp + " NMomentumUp: " + vbo.NMomentumUp);
			if (Position.MarketPosition == MarketPosition.Long) 
			{
				if (BarsSinceEntry() == exitsAfterXBars)
					ExitLong();
				
				//SetProfitTarget("", CalculationMode.Price, vbo.SqueezeHigh[0] + (vbo.SqueezeHigh[0] - vbo.SqueezeLow[0]));
//				if (Low[0] < vbo.SqueezeLow[0] || vbo.SqueezeOn[0] != 0)	// new squeeze
//				{
//					this.ExitLong();
//				}
			}
			if (Position.MarketPosition == MarketPosition.Short)
			{
				if (BarsSinceEntry() == exitsAfterXBars)
					ExitShort();
				

//				double ptarget = vbo.SqueezeLow[0] - (vbo.SqueezeHigh[0] - vbo.SqueezeLow[0]);
//				Print(Time + " ptarget: " + ptarget);
//				SetProfitTarget("", CalculationMode.Price, ptarget);
//				
//				
//				if (High[0] > vbo.SqueezeHigh[0] || vbo.SqueezeOn[0] != 0)	// new squeeze
//				{
//					this.ExitShort();
//				}
			}
        }

        #region Properties
						
        [Description("")]
        [GridCategory("Parameters")]
        public int ExitsAfterXBars
        {
            get { return exitsAfterXBars; }
            set { exitsAfterXBars = Math.Max(0, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public double ProfitPercentage
        {
            get { return profitPercentage; }
            set { profitPercentage = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public double StopLossPercentage
        {
            get { return stopLossPercentage; }
            set { stopLossPercentage = Math.Max(1, value); }
        }
		
        #endregion
    }
}
