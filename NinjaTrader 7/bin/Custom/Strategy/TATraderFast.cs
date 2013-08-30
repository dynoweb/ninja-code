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
    /// Will Trade RTM, SLO-MOMO, MOMO Trades
	/// 
	/// Revision History
	///  v1 - 10/13/12 - Copied from TATrader strategy 
	/// 
	/// Ref
	/// http://www.solerinvestments.com/Online-Trading/Stock-Market-Order-Types.htm
	/// 
    /// </summary>
    [Description("CL, 89 Tick - Trades RTM, SLO-MOMO, MOMO Trades")]
    public class TATraderFast : Strategy
    {
        #region Variables
			internal struct Order
			{
				public bool isLong;
				public string type; // [m,l,s,sl] (Market, Limit Order, Stop Order, Stop Limit Order)
				public double limitPrice;
				public double stopPrice; 
				public string signalName;
			}
        private int aTRPeriod = 8; // 10 Default setting for ATRPeriod Optimized MOMO = 14		
        private double aTRRatched = 0.000; // 0.000 Default setting for ATRRatched
        private double aTRTimes = 10; // 4  Default setting for ATRTimes Optimized MOMO = 3
        private int donchianPeriod = 20; //20 Default setting for DonchianPeriod Optimized MOMO = 22
		private bool enableDebug = true;
		private bool enableMomo = true;
		private bool enableRtm = false;
		private bool enableSloMo = true;
		private bool enableLong = true;
		private bool enableShort = true;
        private int hullPeriod = 40;
        private int mACDFast = 35;  //18; // Default setting for MACDFast
        private int mACDSlow = 90;  //45; // Default setting for MACDSlow
		private int mACDSmooth = 12;
		//private int maxRiskRTM = 40;
		private int minFlatMeanBars = 0;
		private bool enableTraceOrders = false;
		private int ssAbePartialAdjustment1 = 7;  // Amount to move stopLoss below break even
		private int ssAbeProfitTrigger = 16; // Stop Strategy, Auto breakeven (ticks)
		private int ssAbeProfitTrigger1 = 12; // Stop Strategy, Partial move to breakeven (ticks)
		private int ssAtProfitTrigger = 25;	// Start auto adjusting our Stop Loss if this value is reached
		private int ssAtStopLoss = 19;		// Trailing stop

        // User defined variables (add any user defined variables below)
		private int disp = 2;
		private double momoEntry = 0.00;
		private double slmoEntry = 0.00;
		private double rtmEntry = 0.00;
		private int rtmFlag = 0;
		private int sloMoFlag = 0;
		private int momoFlag = 0;
		private double stopPrice = 0;
		private Order order = new Order();
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(DonchianRick(DonchianPeriod));
            //if (enableDebug) Add(MACDrick(mACDFast, mACDSlow, mACDSmooth));
            if (enableDebug) Add(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched));
			if (enableDebug) Add(HMARick(HullPeriod, 30));

			ClearOutputWindow();
    		TraceOrders = enableTraceOrders; 
			CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// After our strategy has a PnL greater than $1000 or less than -$400 we will stop our strategy
//			if (Performance.AllTrades.TradesPerformance.Currency.CumProfit > 1000 
//				|| Performance.AllTrades.TradesPerformance.Currency.CumProfit < -400)
			{
				/* A custom method designed to close all open positions and cancel all working orders will be called.
				This will ensure we do not have an unmanaged position left after we halt our strategy. */
//				StopStrategy();
				
				// Halt further processing of our strategy
//				return;
			}
			

			//colorBars();
			order.type = "";
			
        #region Momo
		if (EnableMomo) 
		{
			momoFlag = 0;
			// ===========================================================================
			// Condition set - Open MOMO Long
			// ===========================================================================
			if (isFlat()
				&& isAlgoBarPos()
				&& isAlgoStopPos()
				&& isMacdPos()
				&& Low[0] > DonchianRick(DonchianPeriod).Upper[disp]
				&& Close[0] > Open[0]
				&& enableLong
				)
			{
				DrawArrowUp(CurrentBar.ToString()+"momo", 0, High[0] + 10 * TickSize, Color.Aquamarine);

				//EnterLong(DefaultQuantity, "MOMO Long");
				order.isLong = true;
				order.type = "m";
				order.signalName = "MOMO Long";
			
				momoFlag = 1;
			}
			
			// ===========================================================================
			// Condition set - Open MOMO Short
			// ===========================================================================
			if (isFlat()
				&& isAlgoBarNeg()
				&& isAlgoStopNeg()
				&& isMacdNeg()
				&& High[0] < DonchianRick(DonchianPeriod).Lower[disp]
				&& enableShort
				)
			{
				DrawArrowDown(CurrentBar.ToString()+"momo", 0, Low[0] - 10 * TickSize, Color.Aquamarine);
				
				//EnterShort(DefaultQuantity, "MOMO Short");
				order.isLong = false;
				order.type = "m";
				order.signalName = "MOMO Short";

				momoFlag = -1;
			}
			
		}			
		#endregion

        #region SloMo
	// ===========================================================================
	// Condition set - Open Slo-MOMO Long
	// ===========================================================================
	if (EnableSloMo) 
	{
		if (isFlat())
		{
			switch (sloMoFlag) {
				// Check for channel extension
				case 0: {
					if (High[0] > DonchianRick(DonchianPeriod).Upper[disp]
						&& enableLong
						) 
					{
						if (enableDebug) BackColorAll = Color.Cyan;
						sloMoFlag = 1;
					} 
					if (Low[0] < DonchianRick(DonchianPeriod).Lower[disp]
						&& enableShort
						) 
					{
						if (enableDebug) BackColorAll = Color.Fuchsia;
						sloMoFlag = -1;
					}
					break;
				}
				//Look for pull back from new high
				case 1: {
					if (Low[0] < DonchianRick(DonchianPeriod).Mean[disp])
					{							
						// it went to far for a slo-mo, it could be a RTM reversal 
						if (enableDebug) 
							DrawText(CurrentBar.ToString()+"SMx", "SMx", 0, High[0] + 20 * TickSize, Color.Red);
						sloMoFlag = 0;
						break;
					}
					if (High[0] + 2*TickSize < DonchianRick(DonchianPeriod).Upper[disp]) {
						if (momoFlag == 0
							&& isAlgoBarPos()
							&& isAlgoStopPos()
							&& isMacdPos()
							&& enableLong
							) 
						{
							sloMoFlag = 2;
							//EnterLongStopLimit(DefaultQuantity, DonchianRick(DonchianPeriod).Upper[disp], DonchianRick(DonchianPeriod).Upper[disp], "Slo-Mo Long");

							//if (enableDebug) DrawArrowUp(CurrentBar.ToString()+"slomo", 0, DonchianRick(DonchianPeriod).Upper[disp] + 10 * TickSize, Color.LightBlue);
							//if (enableDebug) DrawText(CurrentBar.ToString()+"SML", "SML", 0, High[0] + 20 * TickSize, Color.White);
						}
					}
					break;
				}
				case -1: {
					if (High[0] > DonchianRick(DonchianPeriod).Mean[disp])
					{							
						// it went to far, it's not a RTM reversal 
						if (enableDebug) 
							DrawText(CurrentBar.ToString()+"SMy", "SMy", 0, Low[0] - 20 * TickSize, Color.Red);
						sloMoFlag = 0;
						break;
					}
					
					if (Low[0] - 2*TickSize > DonchianRick(DonchianPeriod).Lower[disp]) {
						if (momoFlag == 0
							&& isAlgoBarNeg()
							&& isAlgoStopNeg()
							&& isMacdNeg()
							&& enableShort
							)	
						{
							//TODO - NEED TO PLACE TRADE HERE TOO -- maybe
							sloMoFlag = -2;
						}
					}
					break;
				}
				
				// Looking for an uptick to enter position
				case 2: {
					// make sure it's still qualified
					if (Low[0] < DonchianRick(DonchianPeriod).Mean[disp]
						|| DonchianRick(DonchianPeriod).Upper[disp+15] >= DonchianRick(DonchianPeriod).Upper[disp] 
						)
					{							
						// it went too far, High below the mean or Low below BOC, it's not an Algo RTM reversal 
						sloMoFlag = 0;
						break;
					}
					
					// if it qualifies, place the order
					if (isAlgoBarPos()
						&& isAlgoStopPos()
						&& isMacdPos()
						&& momoFlag == 0
						)
					{
						slmoEntry = DonchianRick(DonchianPeriod).Upper[disp];			

						/*
						if (High[0] >= slmoEntry) {
							//EnterLong(DefaultQuantity, "Slo-Mo Mkt Long");							
							order.isLong = true;
							order.limitPrice = slmoEntry - 0 * TickSize;
							order.type = "l";
							order.signalName = "Slo-Mo L";
						} else {
						*/
							//EnterLongStopLimit(DefaultQuantity, slmoEntry, slmoEntry, "Slo-Mo Long");
							order.isLong = true;
							order.stopPrice = slmoEntry + 2 * TickSize;
							order.limitPrice = slmoEntry + 2 * TickSize;
							order.type = "sl";
							order.signalName = "Slo-Mo SL";
						//}
						
						Alert(CurrentBar.ToString(),  Priority.High, "Look for buyers and go long", "", 5, Color.Transparent, Color.Green);									
						//if (enableDebug) DrawArrowUp(CurrentBar.ToString()+"slomo", 0, DonchianRick(DonchianPeriod).Upper[disp] + 10 * TickSize, Color.Blue);
					}
					
					// Check for Risk/Reward Ratio before opening position
					//if (Low[0] < (ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] + MaxRiskRTM * TickSize)) 
					//{
					//	DrawDiamond(CurrentBar.ToString()+"ATR", true, 0, ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] + MaxRiskRTM * TickSize, Color.Black);
					//}
					
					break;
				}
				case -2: {
					if (High[0] > DonchianRick(DonchianPeriod).Mean[disp]
						|| DonchianRick(DonchianPeriod).Lower[disp+15] <= DonchianRick(DonchianPeriod).Lower[disp] 
						)
					{							
						// it went to far, Low above the mean or High above TOC, it's not an Algo RTM reversal 
						sloMoFlag = 0;
						break;
					}
					
					// Qualifies for a buy, should have 
					if (isAlgoBarNeg()
						&& isAlgoStopNeg()
						&& isMacdNeg()
						&& momoFlag == 0
						)
					{
						slmoEntry = DonchianRick(DonchianPeriod).Lower[disp];							
						/*if (Close[0] <= slmoEntry) {
							//EnterShort(DefaultQuantity, "Slo-Mo Mkt Short");
							order.isLong = false;
							order.type = "m";
							order.signalName = "Slo-Mo Short";
						} else {
						*/
							//EnterShortStopLimit(DefaultQuantity, slmoEntry, slmoEntry, "Slo-Mo Short");
							order.isLong = false;
							order.stopPrice = slmoEntry - 2 * TickSize;
							order.limitPrice = slmoEntry - 2 * TickSize;
							order.type = "sl";
							order.signalName = "Slo-Mo Short";
						//}
						Alert(CurrentBar.ToString(),  Priority.High, "Look for seller and go short", "", 5, Color.Transparent, Color.Green);
					} 
					break;
				}
				case 3: {
					sloMoFlag = 0;
					break;
				}
				case -3: {
					sloMoFlag = 0;
					break;
				}
				
			}; // end switch
		} // end if isFlat
			
		if (enableDebug) {
			if (sloMoFlag > 0)
				DrawText(CurrentBar.ToString()+"ESM", sloMoFlag+"", 0, DonchianRick(DonchianPeriod).Lower[disp] + 5 * TickSize, Color.Black);
			if (sloMoFlag < 0)
				DrawText(CurrentBar.ToString()+"ESM", sloMoFlag+"", 0, DonchianRick(DonchianPeriod).Upper[disp] - 5 * TickSize, Color.Black);
		}
	} // end if EnableSloMo		
		#endregion			
			
		#region RTM
		// ===========================================================================
		// Condition Algo RTM - Algo Reversion To The Mean Entry
		// ===========================================================================
		if (enableRtm) 
		{
			if (isFlat())
			{
				switch (rtmFlag) {
					// Check for channel extension
					case 0: {
						if (High[0] > DonchianRick(DonchianPeriod).Upper[disp]
							&& Low[0] > DonchianRick(DonchianPeriod).Mean[disp]
							&& enableLong
							) 
						{
							if (enableDebug) BackColorAll = Color.Cyan;
							rtmFlag = 1;
						}
						
						if (Low[0] < DonchianRick(DonchianPeriod).Lower[disp]
							&& High[0] < DonchianRick(DonchianPeriod).Mean[disp]
							&& enableShort
							) 
						{
							if (enableDebug) BackColorAll = Color.Fuchsia;
							rtmFlag = -1;
						}
						break;
					}
					// check for touch of mean
					case 1: {
						//MinFlatMeanBars
						if (High[0] < DonchianRick(DonchianPeriod).Mean[disp])
						{							
							// it went to far, it's not a RTM reversal 
							if (enableDebug)
								DrawText(CurrentBar.ToString()+"DIS1", "DIS1", 0, High[0] + 15 * TickSize, Color.Red);
							rtmFlag = 0;
							break;
						}
							
						// if the mean has moved up to meet price, it's not a RTM reversal
						// so don't do it if it's lower
						if (DonchianRick(DonchianPeriod).Mean[disp + minFlatMeanBars] >= DonchianRick(DonchianPeriod).Mean[disp]
							&& isAlgoBarPos()
							&& isAlgoStopPos()
							&& isMacdPos()
							)
						{
							// can't place an order if other stategies have an order placed
							if (momoFlag == 0 && sloMoFlag == 0) {
								rtmEntry = DonchianRick(DonchianPeriod).Mean[disp];
								//EnterLongLimit(DefaultQuantity, rtmEntry, "RTM Limit Long");
								order.isLong = true;
								order.limitPrice = rtmEntry;
								order.type = "l";
								order.signalName = "RTM Limit Long";
								if (enableDebug) DrawArrowUp(CurrentBar.ToString()+"rtm", 0, DonchianRick(DonchianPeriod).Mean[disp] - 10 * TickSize, Color.Green);
							} else if (Low[0] < DonchianRick(DonchianPeriod).Mean[disp]) {
								// price is at target, enter order
								//EnterLong(DefaultQuantity, "RTM Long");
								order.isLong = true;
								order.type = "m";
								order.signalName = "RTM Long";

							}
						}
						break;
					}
					case -1: {
						if (Low[0] > DonchianRick(DonchianPeriod).Mean[disp])
						{							
							// it went to far, it's not a RTM reversal 
							if (enableDebug)
								DrawText(CurrentBar.ToString()+"DIS1", "DIS1", 0, Low[0] - 20 * TickSize, Color.Red);
							rtmFlag = 0;
							break;
						}
						
						// if the mean has moved down to meet price, it's not a RTM reversal 
						if (DonchianRick(DonchianPeriod).Mean[disp + minFlatMeanBars] <= DonchianRick(DonchianPeriod).Mean[disp]
							&& isAlgoBarNeg()
							&& isAlgoStopNeg()
							&& isMacdNeg()
							) 
						{
							rtmEntry = DonchianRick(DonchianPeriod).Mean[disp];

							if (High[0] >= rtmEntry) {
								// price is at target, enter order
								//EnterShortLimit(DefaultQuantity, rtmEntry, "RTM xxx Limit Short");									
								//EnterShort(DefaultQuantity, "RTM Short");

								order.isLong = false;
								order.type = "m";
								order.signalName = "RTM Short";

								//Print(this.Time + " Entered RTM Short @ " + rtmEntry);
								if (enableDebug) DrawArrowDown(CurrentBar.ToString()+"rtm", 0, rtmEntry + 10 * TickSize, Color.Green);
							} else if (momoFlag == 0 && sloMoFlag == 0) {
								// can't place an order if other stategies have an order placed
								//EnterShortLimit(DefaultQuantity, rtmEntry, "RTM Limit Short");

								order.isLong = false;
								//order.stopPrice = slmoEntry;
								order.limitPrice = rtmEntry;
								order.type = "l";
								order.signalName = "RTM Limit Short";

							}
						}
						break;
					}
				};
			}
			
			if (enableDebug) {
				if (rtmFlag > 0) 
					DrawText(CurrentBar.ToString()+ "1",  "1", 0, DonchianRick(DonchianPeriod).Mean[disp] + 5 * TickSize, Color.Blue);
				if (rtmFlag < 0)					
					DrawText(CurrentBar.ToString()+"-1", "-1", 0, DonchianRick(DonchianPeriod).Mean[disp] - 5 * TickSize, Color.Red);
			}
		}			
		#endregion
			
			closeLong();
			closeShort();
			
			placeOrder(order.type, order.limitPrice, order.stopPrice, order.signalName);
		} // end OnBarUpdate

		
		#region utils
		protected void closeLong() {
			
			if (Position.MarketPosition != MarketPosition.Long) 
				return;
			
			rtmFlag = 0;
			sloMoFlag = 0;
			
			double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			
			double highSinceOpen = High[HighestBar(High, BarsSinceEntry())];
			double tStopPrice = highSinceOpen - (ssAtProfitTrigger * TickSize);

			// set breakeven if triggered										
			if (highSinceOpen >= (approxEntry + (ssAbeProfitTrigger * TickSize))) {
				stopPrice = approxEntry;
				//Print("Set to B/E: " + stopPrice);
			} else { 
				if (highSinceOpen >= (approxEntry + (ssAbeProfitTrigger1 * TickSize))) {
					stopPrice = approxEntry - (ssAbePartialAdjustment1 * TickSize);
					//Print("Moved up to just below B/E: " + stopPrice);
				} else {
					stopPrice = (approxEntry - (ssAtStopLoss * TickSize));
					//Print("Moved up to cover stop loss: " + stopPrice);
				}
			}
				
			stopPrice = Math.Max(tStopPrice, stopPrice);
			stopPrice = Math.Max(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0], stopPrice);
			stopPrice = Math.Max(stopPrice, (approxEntry - (ssAtStopLoss * TickSize)));		

			if (enableDebug) {
				DrawDiamond(CurrentBar.ToString()+"Stop", true, 0, stopPrice, Color.Green);

				if (BarsSinceEntry() == -10) {
					Print("----------LONGSTOP------------");
					Print(BarsSinceEntry() + ": approxEntry@" + approxEntry);
					Print("highSinceOpen: " + highSinceOpen);
					Print("ssAtProfitTrigger: " + ssAtProfitTrigger + ", TickSize: " + TickSize + " *= " + (ssAtProfitTrigger * TickSize) + " --> tStopPrice: " + tStopPrice);
					Print("(approxEntry - (ssAtStopLoss * TickSize)" + approxEntry + "-" + ssAtStopLoss + "*" + TickSize + "=" + (approxEntry - (ssAtStopLoss * TickSize)));
					Print("Calculated stopPrice: " + stopPrice); 
				}				
			}

			double slope = Slope(HMA(hullPeriod),1,0);
			if (Math.Abs(slope) < 0.005 && stopPrice >= approxEntry) {
				ExitLong();		
			} else {				
				if (Close[0] <= stopPrice) {
					ExitLong();
				} else {
					ExitLongStop(stopPrice);
				}
			} /*
				if (Close[0] <= stopPrice) {
					ExitLong();
				} else {
					ExitLongStop(stopPrice);
				}
			*/
		}

		protected void closeShort() {

			if (Position.MarketPosition != MarketPosition.Short) 
				return;
			
			rtmFlag = 0;
			sloMoFlag = 0;
			
			double approxEntry = Instrument.MasterInstrument.Round2TickSize(Position.AvgPrice);
			
			double lowSinceOpen = Low[LowestBar(Low, BarsSinceEntry())];
			double tStopPrice = lowSinceOpen + (ssAtProfitTrigger * TickSize);			
			
			// set breakeven if triggered			
			if (lowSinceOpen <= (approxEntry - (ssAbeProfitTrigger * TickSize))) {
				stopPrice = approxEntry;
				//if (enableDebug) Print("Moving stopPrice to B/E, hit auto b/e profit trigger: " + stopPrice);
			} else { 
				// check for partial move to b/e
				if (lowSinceOpen <= (approxEntry - (ssAbeProfitTrigger1 * TickSize))) {
					stopPrice = approxEntry + (ssAbePartialAdjustment1 * TickSize);
					//if (enableDebug) Print("Moving stopPrice closer to B/E, hit auto b/e partial profit trigger: " + stopPrice);
				} else {
					// worse case, starting stop loss setting
					stopPrice = (approxEntry + (ssAtStopLoss * TickSize));
					//if (enableDebug) Print("StopPrice set at stopLoss value: " + stopPrice);
				}
			}
			
			stopPrice = Math.Min(tStopPrice, stopPrice);
			stopPrice = Math.Min(ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0], stopPrice);
			stopPrice = Math.Min(stopPrice, (approxEntry + (ssAtStopLoss * TickSize)));		
			
			if (enableDebug) {
				DrawDiamond(CurrentBar.ToString()+"sdia", true, 0, stopPrice, Color.Yellow);
				//Print (Time[0] + " --PROCESS-- " + BarsSinceEntry() + "/" + BarsSinceExit() + " ----" + stopPrice);
				
				if (BarsSinceEntry() < -3) {
					Print(BarsSinceEntry() + ": approxEntry@" + approxEntry);
					Print("LOW SINCE OPEN: " + lowSinceOpen);
					Print("TRAILING STOP PRICE: " + tStopPrice + " = ssAtProfitTrigger: " + ssAtProfitTrigger + ",  ssAbeProfitTrigger*TickSize= " + (ssAbeProfitTrigger * TickSize));
					Print("(approxEntry + (ssAtStopLoss * TickSize) -> " + approxEntry + " + " + ssAtStopLoss + " * " + TickSize + " = " + (approxEntry + (ssAtStopLoss * TickSize)));
					Print("Calculated stopPrice: " + stopPrice); 
					Print(Time[0] + " ------Info End-----------");
				}				
			}	
			
			double slope = Slope(HMA(hullPeriod),1,0);
			if (Math.Abs(slope) < 0.005 && stopPrice <= approxEntry) {
				ExitShort();		
			} else {				
				if (Close[0] >= stopPrice) {
					ExitShort();
				} else {
					ExitShortStop(stopPrice);
				}
			}
		}
		
		/// <summary>
		/// Sends the order that was queued up on the bar update.
		/// </summary>
		/// <param name="orderType">[type|direction] - 
		/// 	types - m - Market
		/// 			l - LimitOrder
		/// 			s - Stop
		///             sl - stoplimit			
		/// </param>
		/// <param name="limitPrice"></param>
		/// <param name="stopPrice"></param>
		/// <param name="signalName"></param>
		protected void placeOrder(string orderType, double limitPrice, double stopPrice, string signalName) {

			if (order.type == "") 
				return;
			
			if (order.isLong) {
				// buy stop or buy stop limit orders can't be placed below the market
				if (order.type == "sl" && (stopPrice <= Close[0] || limitPrice <= Close[0]))
					order.type = "l";
				switch (order.type) {
					// The most I'll pay for it
					case "l":
						EnterLongLimit(DefaultQuantity, limitPrice, signalName);
						break;					
					case "s":
						EnterLongStop(DefaultQuantity, stopPrice, signalName);
						break;
					// I'll buy if my stop price is hit for no more than my limit price
					case "sl":
						EnterLongStopLimit(DefaultQuantity, limitPrice, stopPrice, signalName);
						break;
					default:
						EnterLong(DefaultQuantity, signalName);
						break;
				}
			} else {
				// Sell stop or sell stop limit orders can't be placed above the market
				if (order.type == "sl" && (Close[0] <= limitPrice || Close[0] <= stopPrice))
					order.type = "l";
				switch (order.type) {
					case "l":
						EnterShortLimit(DefaultQuantity, limitPrice, signalName);
						break;
					case "s":
						EnterShortStop(DefaultQuantity, stopPrice, signalName);
						break;
					case "sl":
						EnterShortStopLimit(DefaultQuantity, limitPrice, stopPrice, signalName);
						break;
					default:
						EnterShort(DefaultQuantity, signalName);
						break;
				}			
				
			}
		}
		
		
		private void StopStrategy()
		{
			// If our Long Limit order is still active we will need to cancel it.
			//CancelOrder(myEntryOrder);
			
			// If we have a position we will need to close the position
			if (Position.MarketPosition == MarketPosition.Long)
				ExitLong();			
			else if (Position.MarketPosition == MarketPosition.Short)
				ExitShort();
		}

		
			// the following is used for short stops
			//DrawDiamond("myDot" + Low[0], true, 0, 
			//	ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] - 2 * TickSize, Color.Yellow);
			// the following is used for long stops
			//DrawDiamond("otherDot" + Low[0], true, 0, 
			//	ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] + 2 * TickSize, Color.Pink);

		protected override void OnExecution(IExecution execution)		
		{
			if (execution.Order != null && execution.Order.OrderState == OrderState.Filled) {
				
				//rtmFlag = 0;
				//sloMoFlag = 0;
		    	//Print(execution.ToString());
				//execution.Name; -- orderEntryName
				//if (enableDebug) Print(execution.Time + "," + execution.Name + "," + execution.Quantity + "," + execution.Price);
			}
			
			if (Performance.RealtimeTrades.Count > 0)
			{
				// Get the last completed real-time trade (at index 0)
				Trade lastTrade = Performance.RealtimeTrades[Performance.RealtimeTrades.Count - 1];
		
				// Calculate the PnL for the last completed real-time trade
				double lastProfit = Instrument.MasterInstrument.Round2TickSize(lastTrade.ProfitCurrency * lastTrade.Quantity);
		
				// Pring the PnL to the Output window
				if (enableDebug) Print(Time + ", profit, " + lastProfit + ", " + execution.Order.OrderState);
			}
		}
		
		protected Boolean isAlgoStopPos() {
			return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Upper[0] < Low[0];	
		}

		protected Boolean isAlgoStopNeg() {
			return ATRTrailing(ATRTimes, ATRPeriod, ATRRatched).Lower[0] > High[0];	
		}

		protected Boolean isAlgoBarPos() {
			return Rising(HMARick(HullPeriod, 30));	
		}
		
		protected Boolean isAlgoBarNeg() {
			return Falling(HMARick(HullPeriod, 30));	
		}
		
		protected Boolean isMacdPos() {
			return MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] > 0;	
		}
		
		protected Boolean isMacdNeg() {
			return MACDrick(mACDFast, mACDSlow, mACDSmooth).Diff[0] <= 0;	
		}
		
		protected Boolean isFlat() {
			return Position.MarketPosition == MarketPosition.Flat;
		}

		protected Boolean isLong() {
			return Position.MarketPosition == MarketPosition.Long;
		}

		protected Boolean isShort() {
			return Position.MarketPosition == MarketPosition.Short;
		}

		protected void colorBars() 
		{
			//double slope = Slope(HMA(hullPeriod),1,0);
			//if (Math.Abs(slope) < 0.005) {
			//	BarColor = Color.Black;
			//} else {
				if (Rising(HMARick(HullPeriod, 30)) == true)
				{
					BarColor = Color.Blue;
				}
				if (Falling(HMARick(HullPeriod, 30)) == true)
				{
					BarColor = Color.Red;
				}
			//}
		}
		#endregion


		#region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public double ATRTimes
        {
            get { return aTRTimes; }
            set { aTRTimes = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ATRPeriod
        {
            get { return aTRPeriod; }
            set { aTRPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double ATRRatched
        {
            get { return aTRRatched; }
            set { aTRRatched = Math.Max(0.000, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int MinFlatMeanBars		
        {
            get { return minFlatMeanBars; }
            set { minFlatMeanBars  = Math.Min(20, value); }
        }

//		[Description("")]
//        [GridCategory("Parameters")]
//        public int MaxRiskRTM		
//        {
//            get { return maxRiskRTM; }
//            set { maxRiskRTM  = Math.Min(60, value); }
//        }

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
		
		[Description("")]
        [GridCategory("Parameters")]
        public bool EnableMomo
        {
            get { return enableMomo; }
            set { enableMomo = value; }
        }

		[Description("Enables or disables short trades")]
        [GridCategory("Parameters")]
        public bool EnableShort
        {
            get { return enableShort; }
            set { enableShort = value; }
        }

		[Description("Enables Slow Momentum (Slo-MOMO) trades")]
        [GridCategory("Parameters")]
        public bool EnableSloMo
        {
            get { return enableSloMo; }
            set { enableSloMo = value; }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public bool EnableRtm
        {
            get { return enableRtm; }
            set { enableRtm = value; }
        }

		[Description("Enables tracing of all orders into the output window")]
        [GridCategory("Parameters")]
        public bool EnableTraceOrders
        {
            get { return enableTraceOrders; }
            set { enableTraceOrders = value; }
        }


//		[Description("")]
//        [GridCategory("Parameters")]
//        public int TrailingStop
//        {
//            get { return trailingStop; }
//            set { trailingStop = Math.Max(1, value); }
//        }

//		[Description("")]
//        [GridCategory("Parameters")]
//        public int ProfitTarget
//        {
//            get { return profitTarget; }
//            set { profitTarget = Math.Max(1, value); }
//        }

//		[Description("")]
//        [GridCategory("Parameters")]
//        public int StopLoss
//        {
//            get { return stopLoss; }
//            set { stopLoss = Math.Max(1, value); }
//        }

//		[Description("")]
//        [GridCategory("Parameters")]
//        public int StopLossShort
//        {
//            get { return stopLossShort; }
//            set { stopLossShort = Math.Max(1, value); }
//        }

        [Description("")]
        [GridCategory("Parameters")]
        public int HullPeriod
        {
            get { return hullPeriod; }
            set { hullPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDFast
        {
            get { return mACDFast; }
            set { mACDFast = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDSlow
        {
            get { return mACDSlow; }
            set { mACDSlow = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDSmooth
        {
            get { return mACDSmooth; }
            set { mACDSmooth = Math.Max(1, value); }
        }

		[Description("Price target of when to move stop ssAbePartialAdjustment1 ticks away from open")]
        [GridCategory("Parameters")]
        public int SsAbeProfitTrigger1
        {
            get { return ssAbeProfitTrigger1; }
            set { ssAbeProfitTrigger1  = Math.Max(1, value); }
        }

		[Description("Number of ticks from open to move on partial adjustment to b/e")]
        [GridCategory("Parameters")]
        public int SsAbePartialAdjustment1
        {
            get { return ssAbePartialAdjustment1; }
            set { ssAbePartialAdjustment1  = Math.Max(1, value); }
        }

		[Description("Stop Strategy, Auto breakeven (ticks)")]
        [GridCategory("Parameters")]
        public int SsAbeProfitTrigger		
        {
            get { return ssAbeProfitTrigger; }
            set { ssAbeProfitTrigger  = Math.Max(1, value); }
        }

		[Description("Trailing stop, start auto adjusting our Stop Loss if this value is reached")]
        [GridCategory("Parameters")]
        public int SsAtProfitTrigger
        {
            get { return ssAtProfitTrigger; }
            set { ssAtProfitTrigger = Math.Max(1, value); }
        }

		[Description("Stop loss setting")]
        [GridCategory("Parameters")]
        public int SsAtStopLoss
        {
            get { return ssAtStopLoss; }
            set { ssAtStopLoss  = Math.Max(1, value); }
        }
        #endregion

    }
}
