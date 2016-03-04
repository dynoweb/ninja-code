// #############################################################
// #														   #
// #                      CJBooth6e                            #
// #														   #
// #   09/07/2013 by Rick Cromer, rick.cromer@hotmail.com      #
// #														   #
// #############################################################
//
#region Updates
// Version 1 - code started on 09/07/2013 (Sept)

#endregion

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

//using PriceActionSwingOscillator.Utility;
//using PriceActionSwingPro.Utility;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Written by Rick Cromer Based on trading 6E using the 1507 & 377 tick charts. This strategy is based on the trade rules which Charles (CJBOOTH) posted on this bigMikes trading forum. https://www.bigmiketrading.com/forex-currency-trading/10920-my-6e-euro-eur-usd-futures-contract-trading-strategy.html  
    /// </summary>
    [Description("Written by Rick Cromer Based on trading 6E using the 1507 & 377 tick charts. This strategy is based on the trade rules which Charles posted on this bigMikes trading forum. https://www.bigmiketrading.com/forex-currency-trading/10920-my-6e-euro-eur-usd-futures-contract-trading-strategy.html  ")]
    public class CJBooth6E : Strategy
    {
        #region Variables
        // Wizard generated variables
        private bool tradeShort = true; // Default setting for TradeShort
        private bool tradeLong = true; // Default setting for TradeLong
        private bool showDebugMarkings = true; // Default setting for ShowDebugMarkings
        // User defined variables (add any user defined variables below)
		
		private int swingSize = 1;
		///
		/// I commented out the following line to get it to compile after getting a different version 
		/// of PriceActionSwingPro
		/// private SwingTypes swingType = SwingTypes.Standard;
		///
		private int dtbStrength = 15;

		//NinjaTrader.Indicator.PriceActionSwing pas;
		//NinjaTrader.Indicator.PriceActionSwingOscillator paso;
		NinjaTrader.Indicator.PriceActionSwingPro pasp;
		ZigZagUTC zz;
		IchiCloud ichi;
		EMA ema;

		// ZigZag parameters
		static int ZZ_SHOW_LINES = 1;
		static int ZZ_SHOW_SWING_DOTS = 2;
		static int ZZ_SHOW_BOTH = 3;		
		private int zzSpan = 1;
		private bool zzUseHighLow = true;
		
		// IchiCloud parameters
        private int periodFast = 9;			// Default setting for PeriodFast
        private int periodMedium = 26;		// Default setting for PeriodMedium
        private int periodSlow = 52;		// Default setting for PeriodSlow
		
		// EMA parameters
		private int emaPeriod = 13;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			ClearOutputWindow();
			
            // Add a 1508 tick Bars object to the strategy
			// the chart bar will be BarsArray[0] and this added timeframe will be BarsArray[1]
			Add(PeriodType.Tick, 377);		// normally this will be 1508
			//Add(PeriodType.Tick, 1508);		

			//pas = PriceActionSwing(dtbStrength, swingSize, SwingTypes.Standard);
			//Add(pas);
			
			//paso = PriceActionSwingOscillator(dtbStrength, swingSize, SwingTypes.Standard);
			//Add(paso);
			
///			pasp = PriceActionSwingPro(dtbStrength, swingSize, SwingTypes.Standard);
///			Add(pasp);
			
			//zz = ZigZagUTC(ZZ_SHOW_LINES, zzSpan, zzUseHighLow, Color.Black);
			//Add(zz);
			
			ichi = IchiCloud(periodFast, periodMedium, periodSlow);
			Add(ichi);
			
			ema = EMA(emaPeriod);
			Add(ema);

			SetProfitTarget(CalculationMode.Ticks, 22);		// 22 * 12.5 = 275
			SetStopLoss(CalculationMode.Ticks, 12);			// 12 * 12.5 = 150

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 100)
				return;
			
			//zz = ZigZagUTC(BarsArray[0], ZZ_SHOW_LINES, zzSpan, zzUseHighLow, Color.Black);
			ichi = IchiCloud(BarsArray[0], periodFast, periodMedium, periodSlow);
			ema = EMA(BarsArray[0], emaPeriod);
			//paso = PriceActionSwingOscillator(BarsArray[0], dtbStrength, swingSize, SwingTypes.Standard);
///			pasp = PriceActionSwingPro(BarsArray[0], dtbStrength, swingSize, SwingTypes.Standard);
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
			}

			// check if it's being called on the primary barsArray
			if (BarsInProgress == 0)
    		{
				//DrawText(CurrentBar.ToString(), pasp.SwingRelation[0].ToString(), 0, High[0]+1*TickSize, Color.Black);
				DrawText(CurrentBar + "HH", pasp.HigherHigh[0].ToString(), 0, High[0]+8*TickSize, Color.Black);
				DrawText(CurrentBar + "LH", pasp.LowerHigh[0].ToString(),  0, High[0]+6*TickSize, Color.DarkGreen);
				DrawText(CurrentBar + "HL", pasp.HigherLow[0].ToString(), 0, Low[0]-6*TickSize, Color.Red);
				DrawText(CurrentBar + "LL", pasp.LowerLow[0].ToString(),  0, Low[0]-8*TickSize, Color.Black);
			}
			
			/*
			// PriceActionSwingOscillator code
			switch ((int) pasp.SwingRelation[0]) {
				case 2:
					
					break;
				case 10:
					DrawText(CurrentBar.ToString(), "HH", 0, High[0]+1*TickSize, Color.DarkGreen);
					break;
				case -10:
					DrawText(CurrentBar.ToString(), "LL", 0, Low[0]-1*TickSize, Color.Red);
					break;
				case -2:
					
					break;
				default:
					
					break;
			}
			*/
			
			// ZigZagUTC Code
//			Print(Time + " zz.Hi " + zz.Hi[0] + " zz.Lo " + zz.Lo[0] + "  zz.ZigZagDot[0] " +  zz.ZigZagDot[0]);
//			DrawDot(CurrentBar + "hi", true, 0, zz.Hi[0], Color.Blue);
//			DrawDot(CurrentBar + "dot", true, 0, zz.ZigZagDot[0], Color.Cyan);
//			DrawDot(CurrentBar + "lo", true, 0, zz.Lo[0], Color.Yellow);
			
			// PriceActionSwing code
			//pas.HigherHigh
			//pas.HigherLow
			//pas.LowerLow
			//pas.LowerHigh
			//pas.DoubleBottom
			//pas.DoubleTop
			//Print(Time + " HigherHigh: " + pas.HigherHigh[0] + " HigherLow: " + pas.HigherLow[0]);
			//DrawDot(CurrentBar + "HH", true, 0, pas.HigherHigh[0], Color.Green);
			//DrawDot(CurrentBar + "HL", true, 0, pas.HigherLow[0], Color.LightGreen);
			//DrawDot(CurrentBar + "HHx", true, 1, pas.HigherHigh[1]+0 * TickSize, Color.Blue);
			//DrawDot(CurrentBar + "HLx", true, 1, pas.HigherLow[1]-0 * TickSize, Color.LightGreen);
//			if (CurrentBar % 10 == 0)
//				for (int i = 0; i < 5; i++) 
//				{
//					DrawDot(CurrentBar + "HH" + i, true, i, pas.HigherHigh[i], Color.Purple);	
//				}
			
			
        }

        #region Properties
        [Description("Enables Short Trades")]
        [GridCategory("Parameters")]
        public bool TradeShort
        {
            get { return tradeShort; }
            set { tradeShort = value; }
        }

        [Description("Enables Long Trades")]
        [GridCategory("Parameters")]
        public bool TradeLong
        {
            get { return tradeLong; }
            set { tradeLong = value; }
        }

        [Description("Shows chart markings to aid in debugging")]
        [GridCategory("Parameters")]
        public bool ShowDebugMarkings
        {
            get { return showDebugMarkings; }
            set { showDebugMarkings = value; }
        }
        #endregion
    }
}
