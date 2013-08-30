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
    public class MyCLChannelScalper : Strategy
    {
        #region Variables
        // Wizard generated variables
	        private int donchianPeriod = 91;
			private bool enableDebug = false;
			private bool enableLong = true;
			private bool enableShort = true;
	        private int hullPeriod = 20;
			private int profitTargetLong = 20; // <-- not used
			private int profitTargetShort = 20; // <-- not used
			private int stopLossLong = 7; 
			private int stopLossShort = 8;
			private int trailingStopLong = 10;
			private int trailingStopShort = 10;
        // User defined variables (add any user defined variables below)
			private int disp = 0;
			private bool inUpperChannel = false;	// these get updated after checking trade potential
			private bool inLowerChannel = false;

			private string signalNameLong = "BOC Long";
			private double stopPrice = 0;
			private double limitPrice = 0;
			private double donchianLow = 0;

        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DonchianChannel(DonchianPeriod));
			Add(HMA(hullPeriod));
			//Add(HMA(100));
            CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Open BOC Long
			if (isFlat()
				&& enableLong				
				&& inUpperChannel
			//	&& donchianLow == DonchianChannel(DonchianPeriod).Lower[disp]
				&& High[0] < DonchianChannel(DonchianPeriod).Mean[disp]
				)
			{
				//Print("Flat: " + isFlat() + " enableLong: " + enableLong + " isAlgoBarPos(): " + isAlgoBarPos() + " inUpperChannel: " + inUpperChannel);
				
				limitPrice = DonchianChannel(DonchianPeriod).Lower[disp];
				
				if (DonchianChannel(DonchianPeriod).Mean[disp] > limitPrice + 5 * TickSize) {
					//	EnterLongLimit(DefaultQuantity, limitPrice + TickSize * 5, signalNameLong);
				
					//DrawArrowUp(CurrentBar.ToString()+"boc", 0, HMA(hullPeriod)[0] + TickSize*2, Color.Blue);
					//DrawDiamond(CurrentBar.ToString()+"bocStop", true, 0, limitPrice + TickSize * 2, Color.Black);
				}
				
				double channelWidth = (DonchianChannel(DonchianPeriod).Upper[disp] - DonchianChannel(DonchianPeriod).Lower[disp]);
				double donchian15Percent =  channelWidth * 0.15 + DonchianChannel(DonchianPeriod).Lower[disp];
				double donchian20Percent =  channelWidth * 0.20 + DonchianChannel(DonchianPeriod).Lower[disp];
				
				if (Rising(HMA(hullPeriod)) && (HMA(hullPeriod)[1] < donchian15Percent)) {
					
					EnterLongLimit(DefaultQuantity, donchian20Percent, signalNameLong);
					DrawDiamond(CurrentBar.ToString()+"bocStop", true, 0, HMA(hullPeriod)[0] + TickSize*2, Color.Black);
					//string slope = Slope(HMA(hullPeriod),2,0).ToString("0.000");
					//this.DrawText(CurrentBar.ToString(), slope, 0, HMA(hullPeriod)[0] - TickSize * 5, Color.White);
				} else {
					if (HMA(hullPeriod)[0] > 96.8) {
						//string slope = Slope(HMA(hullPeriod),2,0).ToString("0.000");
						//this.DrawText(CurrentBar.ToString(), slope, 0, HMA(hullPeriod)[0] - TickSize * 5, Color.White);
					}
				}
			
			}
			
			// Open TOC Short
			if (isFlat()
				&& enableShort
				&&isAlgoBarNeg()
				)
			{
				//SetTrailStop(CalculationMode.Ticks, TrailingStopShort);
				//EnterShort(DefaultQuantity);
			}

//			if (!isFlat()) {
//				EnterLongStop(stopPrice);
//				DrawDiamond(CurrentBar.ToString()+"bocStop", true, 0, stopPrice, Color.Black);
//			}

			if (Low[0] > DonchianChannel(DonchianPeriod).Mean[disp]) {
				inUpperChannel = true;				
				donchianLow = DonchianChannel(DonchianPeriod).Lower[disp];
			}
			if (High[0] < DonchianChannel(DonchianPeriod).Mean[disp]) {
				inLowerChannel = true;				
			}
				
			closeLong();
			colorBars();			
        }
		
		
		protected void closeLong() {
			
			if (Position.MarketPosition != MarketPosition.Long) 
				return;
			
			double entryPrice = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			ExitLongStop(entryPrice - StopLossLong * TickSize, "BOC Long");
			ExitLongLimit(DonchianChannel(DonchianPeriod).Mean[disp]);			
			DrawDiamond(CurrentBar.ToString()+"bocStop", true, 0, entryPrice - 8 * TickSize, Color.Yellow);
		}

		protected override void OnOrderUpdate(IOrder order)
		{
			if (enableDebug) Print(order.ToString());
			if (order.OrderState == OrderState.Filled) {
				//entryOrder = null;
				inUpperChannel = false;			
				inLowerChannel = false;				
			}
		}

		
		#region utils
		protected Boolean isAlgoBarPos() {
			return Rising(HMA(HullPeriod));	
		}
		
		protected Boolean isAlgoBarNeg() {
			return Falling(HMA(HullPeriod));	
		}
		
		protected Boolean isFlat() {
			return Position.MarketPosition == MarketPosition.Flat;
		}
		
		protected void colorBars() 
		{
			// string slope = Slope(HMA(hullPeriod),2,0).ToString("0.000");
			//double slope = Slope(HMA(hullPeriod),1,0);
			double slope = Slope(HMA(100),4,0);
			if (Math.Abs(slope) < 0.005) {
				BarColor = Color.Black;
			} else if (slope > 0) {
						BarColor = Color.Blue;
					} else {
						BarColor = Color.Red;
					}
		}

		#endregion
		
		
        #region Properties
        [Description("Donchian Period")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
        }

		[Description("Enables or disables developer troubleshooting symbols")]
        [GridCategory("Parameters")]
        public bool EnableDebug
        {
            get { return enableDebug; }
            set { enableDebug = value; }
        }
		
		[Description("Enables or disables long trades")]
        [GridCategory("Parameters")]
        public bool EnableLong
        {
            get { return enableLong; }
            set { enableLong = value; }
        }
		
		[Description("Enables or disables short trades")]
        [GridCategory("Parameters")]
        public bool EnableShort
        {
            get { return enableShort; }
            set { enableShort = value; }
        }

        [Description("Hull Moving Average Period")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int ProfitTargetLong
        {
            get { return profitTargetLong; }
            set { profitTargetLong = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int ProfitTargetShort
        {
            get { return profitTargetShort; }
            set { profitTargetShort = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int StopLossLong
        {
            get { return stopLossLong; }
            set { stopLossLong = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int StopLossShort
        {
            get { return stopLossShort; }
            set { stopLossShort = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int TrailingStopLong
        {
            get { return trailingStopLong; }
            set { trailingStopLong = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int TrailingStopShort
        {
            get { return trailingStopShort; }
            set { trailingStopShort = Math.Max(1, value); }
        }

        #endregion
    }
}
