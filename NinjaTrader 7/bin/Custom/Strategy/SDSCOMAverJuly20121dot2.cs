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
    /// Silver Dragons BOT from BigMikes Battle of the BOTS
	/// 
	/// 
    /// </summary>
    [Description("Trades 6E using 5 min bars - by Silver Dragon")]
    public class SDSCOMAverJuly20121dot2 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int mytargetlong		 = 22; // Default setting for Mytarget
		private int mystoplong			 = 26; // Default setting for Mystop
		private int mytargetshort 		 = 36;
		private int mystopshort 		 = 90;
		private int malong1 			 = 30;
		private int malong2 			 = 52;
		private int mashort1			 = 96;
		private int mashort2			 = 33;
		private int tradeok 			 = 1;
		private double StandardDev		 = 0;
		private double stddev			 = 1;
		private double avgalltrades		 = 1;
		private double tstat			 = 0;
		private int alltradecount 		 = 0;
		private int tradeLong 			 = 1;
		private int tradeShort 			 = 1;
		private int useAsLongStop		 = 0;
		private int useAsShortStop 		 = 0;
		private int timeToTrade			 = 0;
		private int pointTarget 			    = 500;
		private int pointStop					=-500;
		private int		priorTradesCount		= 0;
		private double	priorTradesCumProfit	= 0;
		private MAType matype = MAType.EMA;
		bool maLongCrossOver = false;
		bool maShortCrossOver = false;
		

        #endregion

        /// <summary>
		/// 12/2/11 Silver Dragons Simple CrossOver Moving Average ver1 December 2011
		/// ---------------------------------------------------------------------------
        /// 12/6/11 Silver Dragons Simple CrossOver Moving Average ver2 December 2011
		/// 12/6/11 Broke out Long and Short moving averages into their own variables
		/// 12/6/11 Broke out Long and Short stops into their own variables
		/// 12/6/11 Broke out Long and Short targets into their own variables
		/// 12/6/11 Removed conditional exits - no longer needed because stops and targets are use
		/// 1/1/12  No code changes. Reoptimized settings
		/// 1/31/12 Added tradeok variable. This allows variables to be set to determine if the strategy will take the trade.
		/// 5/1/12  Added the ability to turn off long and short trades.
		/// 5/2/12	Added the ability to turn the long/short trades into stops. 
		/// 6/15/12 fixed issue with "using as stop" variable not working
		/// 6/20/12 added point targets for profit and loss
		/// 6/29/12 Added the ability to choose a moving average
		/// 6/30/12 cleaned up code

		/// ---------------------------------------------------------------------------
        /// </summary>
        protected override void Initialize()
        {
//           ***************************************************************
//           *************** Profit / Stop Targets && Misc *****************
//           ***************************************************************
			#region Profit / Stop Targets && Misc
				Add(EMA(malong2));
				Add(EMA(malong1));
				EMA(malong1).Plots[0].Pen.Color = Color.Blue;
				EMA(malong2).Plots[0].Pen.Color = Color.Magenta;
			
				SetProfitTarget("Long", CalculationMode.Ticks, Mytargetlong);
           		SetStopLoss("Long", CalculationMode.Ticks, Mystoplong, false);
			
				SetProfitTarget("Short" , CalculationMode.Ticks, Mytargetshort);
            	SetStopLoss("Short", CalculationMode.Ticks, Mystopshort, false);
			
            	CalculateOnBarClose = true;
				ExitOnClose = false;
			
			#endregion Profit / Stop Targets && Misc
				
		}	


        protected override void OnBarUpdate()
        {
				
//           ***************************************************************
//           *************** CrossOver based on MA Type ********************
//           ***************************************************************
//			 This Checks for CrossOver based on the MA Type
			#region CrossOver based on MA Type
			switch (matype)
			{
				case MAType.Moving_Median:
				{
					maLongCrossOver = CrossAbove(anaMovingMedian(malong2), anaMovingMedian(malong1), 1);
					maShortCrossOver = CrossBelow(anaMovingMedian(mashort2), anaMovingMedian(mashort1), 1);
					break;
				}
				
				case MAType.EMA:
				{
					maLongCrossOver = CrossAbove(EMA(malong2), EMA(malong1), 1);
					maShortCrossOver = CrossBelow(EMA(mashort2), EMA(mashort1), 1);
					
					break;
				}
				
				
				case MAType.Hull_MA:
				{
					maLongCrossOver = CrossAbove(HMA(malong2), HMA(malong1), 1);
					maShortCrossOver = CrossBelow(HMA(mashort2), HMA(mashort1), 1);
					break;
				}
				

				
				
				case MAType.Simple_MA:
				{
					maLongCrossOver = CrossAbove(SMA(malong2), SMA(malong1), 1);
					maShortCrossOver = CrossBelow(SMA(mashort2), SMA(mashort1), 1);
					break;
				}
				
				case MAType.Smoothed_MA:
				{
					maLongCrossOver = CrossAbove(SMMA(malong2), SMMA(malong1), 1);
					maShortCrossOver = CrossBelow(SMMA(mashort2), SMMA(mashort1), 1);
					break;
				}	
				
				case MAType.Triangular_MA:
				{
					maLongCrossOver = CrossAbove(TMA(malong2), TMA(malong1), 1);
					maShortCrossOver = CrossBelow(TMA(mashort2), TMA(mashort1), 1);
					break;
				}
				
				case MAType.Volume_MA:
				{
					maLongCrossOver = CrossAbove(VOLMA(malong2), VOLMA(malong1), 1);
					maShortCrossOver = CrossBelow(VOLMA(mashort2), VOLMA(mashort1), 1);
					break;
				}
				
				case MAType.Weighted_MA:
				{
					maLongCrossOver = CrossAbove(WMA(malong2), WMA(malong1), 1);
					maShortCrossOver = CrossBelow(WMA(mashort2), WMA(mashort1), 1);
					break;
				}
				
				case MAType.Zero_Lag_MA:
				{
					maLongCrossOver = CrossAbove(ZLEMA(malong2), ZLEMA(malong1), 1);
					maShortCrossOver = CrossBelow(ZLEMA(mashort2), ZLEMA(mashort1), 1);
					break;
					
				}
			
			}
			
			#endregion MA Type Logic
			
			
//           ***************************************************************
//           ******************* Point Target and losses *******************
//           ***************************************************************
//			 This will keep the strategy from trading if the targets or losses are hit
			#region Point Target and losses
//			if (Bars.FirstBarOfSession)
//			{
//				
//				priorTradesCumProfit = Performance.AllTrades.TradesPerformance.Points.CumProfit;	
//			}
//			
//			
//			if (Performance.AllTrades.TradesPerformance.Points.CumProfit - priorTradesCumProfit >= pointTarget
//				|| Performance.AllTrades.TradesPerformance.Currency.CumProfit - priorTradesCumProfit <= pointStop)
//			{
//				return;
//			}
			#endregion Point Target and losses

			
//           ***************************************************************
//           ******************* Begin Trade Logic *************************
//           ***************************************************************		
			#region Begin Trade Logic
			// Enter Long Position
           //if (CrossAbove(EMA(malong2), EMA(malong1), 1)
			if (maLongCrossOver ==true
			//if (maLong2 == maLong1
				//&& Position.MarketPosition == MarketPosition.Flat
                && ToTime(Time[0]) >= ToTime(1, 0, 0)
                && ToTime(Time[0]) <= ToTime(14, 30, 0)
				&& tradeLong == 1
				&& tradeok == 1)
					{
						EnterLong(DefaultQuantity, "Long" );
						BarColor = Color.Blue;
					}
			
			else
		
			//if (CrossAbove(EMA(malong2), EMA(malong1), 1)
			if (maLongCrossOver ==true
				//&& Position.MarketPosition == MarketPosition.Flat
                && ToTime(Time[0]) >= ToTime(1, 0, 0)
                && ToTime(Time[0]) <= ToTime(14, 30, 0)
				&& tradeLong == 1
				&& tradeok == 1
				&& useAsShortStop == 1)
			{
				ExitShort("Short" , "");
			}
			


            // Enter Short Position
           // if (CrossBelow(EMA(mashort2), EMA(mashort1), 1)
			if (maShortCrossOver ==true
                && ToTime(Time[0]) >= ToTime(1, 0, 0)
                && ToTime(Time[0]) <= ToTime(14, 30, 0)
				&& tradeShort == 1
				&& tradeok == 1
				&& useAsLongStop ==0)
            {
                BarColor = Color.Fuchsia;
                EnterShort(DefaultQuantity, "Short" );
            }
			
			else
			
			 //if (CrossBelow(EMA(mashort2), EMA(mashort1), 1)
			if (maShortCrossOver ==true
				//&& Position.MarketPosition == MarketPosition.Flat
				
                && ToTime(Time[0]) >= ToTime(1, 0, 0)
                && ToTime(Time[0]) <= ToTime(14, 30, 0)
				&& tradeShort == 1
				&& tradeok == 1
				&& useAsLongStop ==1)
            {
                ExitLong("Long" , "");
            }
			#endregion Begin Trade Logic
			
			
//           ***************************************************************
//           **************** Exit Trade During Off Hours ******************
//           ***************************************************************
			#region Exit Trade During Off Hours
			//if (CrossAbove(EMA(malong2), EMA(malong1), 1)
			if (maLongCrossOver ==true
				//&& Position.MarketPosition == MarketPosition.Flat
				&& timeToTrade == 0
				&& tradeok == 1)
			{
				//ExitShort("Short" + StratNumber, "");
				Position.Close();
			}
			
				//if (CrossBelow(EMA(mashort2), EMA(mashort1), 1)	
				if (maShortCrossOver ==true
				&& timeToTrade == 0
				&& tradeok == 1)
            {
				Position.Close();	
            }
			
			#endregion Exit Trade During Off Hours
			
			
//           ***************************************************************
//           **************** Dont Trade On Worse Day of The Week **********
//           ***************************************************************	
			#region Dont Trade On Worse Day of The Week
			if(Time[0].DayOfWeek == DayOfWeek.Friday)
			{
				
				tradeok = 0;
				
			}
			
			else
				//Worse performing hour was 8 to 9.
			if (ToTime(Time[0]) >= ToTime(7, 30, 0) && ToTime(Time[0]) <= ToTime(9, 30, 0))
			{
				
				tradeok = 0;
				
			}
			else
			{
			tradeok = 1;
			
			}
			#endregion Dont Trade On Worse Day of The Week

            
        }

        #region Properties
        
		
//		[Description("")]
//        [GridCategory("Strategy Parameters")]
//        public int PointTarget
//        {
//            get { return pointTarget; }
//            set { pointTarget = Math.Max(1, value); }
//        }
//		
//		[Description("")]
//        [GridCategory("Strategy Parameters")]
//        public int PointStop
//        {
//            get { return pointStop; }
//            set { pointStop = Math.Max(-1000, value); }
//        }
		
		
		[Description("")]
        [GridCategory("Long")]
        public int Mytargetlong
        {
            get { return mytargetlong; }
            set { mytargetlong = Math.Max(1, value); }
        }
		[Description("")]
        [GridCategory("Short")]
        public int Mytargetshort
        {
            get { return mytargetshort; }
            set { mytargetshort = Math.Max(1, value); }
        }
		

        [Description("")]
        [GridCategory("Long")]
        public int Mystoplong
        {
            get { return mystoplong; }
            set { mystoplong = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Short")]
        public int Mystopshort
        {
            get { return mystopshort; }
            set { mystopshort = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Short")]
        public int Mashort1
        {
            get { return mashort1; }
            set { mashort1 = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Short")]
        public int Mashort2
        {
            get { return mashort2; }
            set { mashort2 = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Short")]
        public int TradeShort
        {
            get { return tradeShort; }
            set { tradeShort = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Short")]
        public int UseAsStopForLong
        {
            get { return useAsLongStop; }
            set { useAsLongStop = Math.Max(0, value); }
        }
		
		
		[Description("")]
        [GridCategory("Long")]
        public int UseAsStopForShort
        {
            get { return useAsLongStop; }
            set { useAsLongStop = Math.Max(0, value); }
        }
		
		[Description("")]
        [GridCategory("Long")]
        public int Malong1
        {
            get { return malong1; }
            set { malong1 = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Long")]
        public int Malong2
        {
            get { return malong2; }
            set { malong2 = Math.Max(1, value); }
        }
		
		[Description("")]
        [GridCategory("Long")]
        public int TradeLong
        {
            get { return tradeLong; }
            set { tradeLong = Math.Max(0, value); }
        }
		
		[Description("Choose MA Type.")]
		[GridCategory("Moving Average Type")]
		public MAType MAtype
		{
			get { return matype; }
			set { matype = value; }
		}
		
		
	
        #endregion
		
		
		#region MA Types
				public enum MAType
				{
					
					EMA,
					Hull_MA,
					Moving_Median,
					Simple_MA,
					Smoothed_MA,
					Triangular_MA,
					Weighted_MA,
					Volume_MA,
					Zero_Lag_MA,
				}
		#endregion  MA Types
		

    }
}
