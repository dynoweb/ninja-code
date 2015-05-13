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
	/// Strategy Details
	/// On the price action swing indicator. 
	///   When we get a: "HL" we want to buy 1 tick above the candle. 
	///   When we get a: "LH" we want to sell tick below the candle. 
	///  If the candle that follows a LH or HL is an inside candle compared to the prior candle. Of course the 
	///  trade would not have been picked up and filled yet - and we will want to move the order above/below that 
	///  inside candle depending on trade direction.
	/// 
	/// The PriceActionSwing seems to be quite straight forward to obtain the swings. Its simply this:
	/// 
	/// UP Swing:
	/// High[0] > High[1] && Low[0] >= Low[1] // this is just saying a candle has a higher high and not a lower low. Thus up.
	/// 
	/// DOWN Swing:
	/// Low[0] < Low[1] && High[0] <= High[1] // this is just saying a candle has a lower low and not a higher high. Thus down.
	/// We ignore all inside and outside candles to determine swing direction.
	/// 
	/// INSIDE Candle:
	/// High[0] <= High[1] && Low[0] >=Low[1]
	/// 
	/// OUTSIDE Candle:
	/// High[0] > High[1] && Low[0] <Low[1]
	/// 
	/// So if we can use the above information to determine if a Swing is up or Down. Then the next step is working out if we have a:
	/// Swing Higher Low (HL - for a Long): Compared to PRIOR Swing down /or/
	/// Swing Lower High (LH - for a Short): Compared to PRIOR Swing up
	/// 
	/// Stop loss goes under Lh for shorts and Hl for longs,
	/// If a setup occurs the opposite direction when in a trade, close out and enter the other direction 
	/// when entry point is touched(reverse).
	/// 
	/// And for the target I see you've done a 3 bar exit, that's interesting as I've never looked at that concept, 
	/// only thing is it has nothing to do with market psychology. I know static targets don't help either. I can 
	/// give you a peice of code I wrote for predictive volatility which you can set targets at, just between us 
	/// though. If we did that option for targets, then do you think it's a good idea to come to break even plus 
	/// commissions after trade goes 50% towards target?
	/// 
	/// These ideas could be merged, have an option to choose 3 bar exit on or off plus my predictive volatility target.
	/// 
	/// Up to you what you think about reversing a trade if an opposing one sets up and technically triggers? 
	/// With stop loss risk placed where mentioned above.
	/// 
	/// Primary thing I believe to be true is being professional risk controllers vs profit Takers. Profit comes naturally
	/// 
	/// 
	/// 
    /// Used to test new key trading ideas - this should be used only as a template.
	/// 
	/// Step 1: KeyIdea or trade idea 
	/// 		pseudo code entry
	/// 		example entry: if close > highest(close, x) then buy next bar at market
	/// 		exit: reverse position when entry reverses
	/// 		   or technical-based - support/resistance, moving average 
	///                 needs to work with entry to not give conflicting signals
	///            breakeven stops, stop-loss, profit targets, trailing stops 
	/// 		market selection
	/// 			oil, index etc
	/// 			over night multi-day 
	/// 			day trading
	/// 		time frame/bar size
	/// 			range
	/// 			tick
	/// 			time
	/// 		data considerations 
	/// 			not much can be done with ninja - continuous adjusted historical
	/// 	limited texting - check if there's any merit to the entry idea
	/// 		test against only 10-20% of data
	/// 			Entry Testing
	/// 				usefullness test - without commission/slippage goal > 52% profitable
	/// 					Fixed-Stop and Target Exit
	/// 						SetProfitTarget
	/// 						SetStopLoss
	/// 					Fixed-Bar Exit
	/// 					Random Exit
	/// 	
    /// </summary>
    [Description("ES 1 min - Candlestick scalper")]
    public class KeyIdea14 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private double dparm1 = 0.009; // Default setting for Dparm1
        private double dparm2 = 0.009; // Default setting for Dparm2
        private int iparm1 = 6; // Default setting for Iparm1
        private int iparm2 = 10; // Default setting for Iparm2
        private int period1 = 20; // Default setting for Period1
        private int period2 = 20; // Default setting for Period2
        // User defined variables (add any user defined variables below)
		
		double prevHigh = 0;
		double prevLow = 0;
		bool lowerHigh = false;
		bool higherLow = false;
		
		T3 t3 = null;
		HMARick hma = null;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			t3 = this.T3(14, 3, 0.7);
			Add(t3);
			
			hma = this.HMARick(55,50);
			Add(hma);
			
			Add(VHF(14,0.35));
			
			Add(PitColor(Color.Black, 80000, 25, 161500));
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (ToTime(Time[0]) < 80000 || ToTime(Time[0]) > 150000)
			{
				return;
			}
			
			if (isLong() && BarsSinceEntry() > Iparm1) 
			{
				ExitLong(1);
			}
			
			if (isShort() && BarsSinceEntry() > Iparm1) 
			{
				ExitShort(1);
			}
			
			if (isBull())
			{
				EnterLong(1);
				//SetProfitTarget(CalculationMode.Ticks, Iparm1);
				SetStopLoss(CalculationMode.Ticks, Iparm2);
			}
			
        }

		private bool isBull() 
		{
			bool bull = false;
			
			// Morning star or Abandoned baby bottom
			if (Close[2] < Close[3] && Close[3] < Close[4] 			// 2 descending bars
				&& Close[2] < ((High[2] - Close[2]) * 0.25) + Close[2]  // closed in bottom 25% of bar
			 	&& High[1] < ((High[2] - Close[2]) * 0.25) + Close[2]	// doji high, not too high
				&& Math.Abs(Open[1] - Close[1]) < (Open[2] - Close[2])/2	// doji body is smalled than down candle
				&& Close[0] > (Open[2] - Close[2]) * 0.5 + Close[2]	// bull bar higher than mid body
				)
			{
				//Print(Time + " (Open[2] "+Open[2]+" - Close[2] "+Close[2]+") * 0.5 + Close[2] = " + ((Open[2] - Close[2]) * 0.5 + Close[2]));
				//Print(Time + " " + (((High[2] - Close[2]) * 0.25) + Close[2]));
				//SetStopLoss(CalculationMode.Price, Low[0] - 2 * TickSize);
				//DrawDiamond(CurrentBar + "bull", false, 0, Low[0] - 2 * TickSize, Color.Green);
				//bull = true;
			}
			
			// T3/HMA cross
			if (CrossAbove(hma, t3, 2))
			{
				DrawDiamond(CurrentBar + "bull", false, 0, Low[0] - 2 * TickSize, Color.Green);
				bull = true;
			}
			
			return bull;
		}		

		private bool isBear() 
		{
			bool bear = false;
//			if (pas.GannSwing[0] == High[0] && lowerHigh)
//			{
//				SetStopLoss(CalculationMode.Price, High[0] + 2 * TickSize);
//				DrawDiamond(CurrentBar + "bearStop", false, 0, High[0] + 2 * TickSize, Color.Red);
//				bear = true;
//			}
			return bear;
		}

		private bool isFlat()
		{
			if (Position.MarketPosition == MarketPosition.Flat)
				return true;
			else
				return false;
		}
		
		private bool isLong()
		{
			if (Position.MarketPosition == MarketPosition.Long)
				return true;
			else
				return false;
		}
		
		private bool isShort()
		{
			if (Position.MarketPosition == MarketPosition.Short)
				return true;
			else
				return false;
		}
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm1
        {
            get { return iparm1; }
            set { iparm1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Iparm2
        {
            get { return iparm2; }
            set { iparm2 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(0, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm1
        {
            get { return dparm1; }
            set { dparm1 = Math.Max(0.000, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Dparm2
        {
            get { return dparm2; }
            set { dparm2 = Math.Max(0.000, value); }
        }
        #endregion
    }
}
