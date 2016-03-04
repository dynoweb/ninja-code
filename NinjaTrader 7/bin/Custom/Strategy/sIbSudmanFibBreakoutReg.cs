#region Using declarations
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
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

namespace NinjaTrader.Strategy
{
    public class sIbSudmanFibBreakoutReg : Strategy
    {	
		/*
		#region parameters
					
		private string stratNamePrefix 				= "(FibR)_";
		private int numTrianglePoints				= 2; //not shown to user.  Changing would alter rules.
		
		private int ticksFromPullbackToEnter		= 4; 
		
		private FibLevels fibLevels_Entry       	= FibLevels._500;
		private FibLevels fibLevels_TakeProfit		= FibLevels._382;
		private FibLevels fibLevels_StopLoss		= FibLevels._618;
		
		private int swingStrength					= 5; 
		private int dtbStrength 					= 10;
		private bool useCloseValues				    = false; 
		private PriceActionSwing.Base.SwingStyle swingStyle 		= PriceActionSwing.Base.SwingStyle.Standard;
		
		#endregion //closes parameters
			
		#region globalVariables
		
		private IOrder entryOrder = null, basicExitOrder = null, rejectionExitOrder = null; 
		
		private Indicator.PriceActionSwingPro pas; 
		
		private double fibLevelEntry, fibLevelTarget, fibLevelStop, distanceBtwSwings;
		
		private int triNumberAtEntry; 
		
		private Color segmentColor;	
			
		private readonly object _object = new object();  

		#endregion //globalVariables
		
		#region userMethods_TradingTimes
		
		private bool isBlockedTime(DateTime dt)
		{
			return(false); 
		}
		
		#endregion //closes userMethods_TradingTimes
		
		#region orderProcessing
		
		public void processEntryFills(IExecution ex, ref IOrder orderToEnter)
		{
			if (this._FillCount == 0)
			{
				this._TakeProfit = distanceBtwSwings * (this.fibLevelEntry - this.fibLevelTarget); 
				this._TakeProfit = Instrument.MasterInstrument.Round2TickSize(Math.Max((this._MinTarget_InTicks * TickSize), this._TakeProfit)); 
				this._StopLoss 	 = distanceBtwSwings * (this.fibLevelStop - this.fibLevelEntry);
				this._StopLoss   = Instrument.MasterInstrument.Round2TickSize(Math.Max((this._MinTarget_InTicks * TickSize), this._StopLoss));
			}
			
			this._handleEntryExecutions(ex, ref orderToEnter); 
			
			this._setTrailExitLevels(); 
			this._updateExitLevels(ex); 
		}
		
		public void processExitFills(IExecution ex, ref IOrder orderToClose)
		{
			this._handleExitFills(ex, ref orderToClose);
			if (this._IsPositionClosed) 
			{
				this._checkForConsecutiveWinsOrLosses(); 
				if (this._IsTradingHalted) 
				{
					if (entryOrder != null)  this._processCanceledOrder(ref entryOrder, CancelReason.TradingHalt); 
					
					Print(String.Format("{0} {1} trading is halted at {2:T}. Consecutive winning trades: {3}. Consecutive losing trades: {4}.", 
						this._StratName, this._FuturesCode, this._EventTime, this._ConsecutiveWinners, this._ConsecutiveLosers));
					Print(String.Empty);
				}
			}
		}
		
		#endregion //closes orderProcessing
		
		#region dataIntegrity
		
		private void setParameterDefaults()
		{
			FirstSessionStartTime  = new TimeSpan (09,01,00);
			SecondSessionEndTime   = new TimeSpan (14,30,00);
			PrdBeforeEndToFlatten  = new TimeSpan (00,15,00); 
			ExitAtMarketClose 	   = true;
			
			fibLevelEntry   	   = this._getFibLevelFromEnum(this.fibLevels_Entry);
			fibLevelTarget         = this._getFibLevelFromEnum(this.fibLevels_TakeProfit);
			fibLevelStop           = this._getFibLevelFromEnum(this.fibLevels_StopLoss);
						
			_TriNumberOfPoints     = numTrianglePoints; 
			MaxTicksFromSrLine     = 5; 
		}
		
		private void validateParameters()
		{
			this._validateHistoricalDataInputs(); 	
			this._validateExitParameters();
			this._validateTimeParameters(); 
			
			if (this.fibLevelEntry <= this.fibLevelTarget)
				Print(String.Format("{0} fibLevelEntry must be greater than or equal to fibLevelTarget.", this._StratName));
			
			if (this.fibLevelEntry >= this.fibLevelStop)
				Print(String.Format("{0} fibLevelEntry must be greater than or equal to fibLevelStop.", this._StratName));		
		}
		
		#endregion //closes dataIntegrity
		
		#region entryRules
		
// RICK Commented this out to get it to compile		
//		public PendingEntryType getEntryType()
//		{
//			if (   (_TriSwingClassList[0] == SwingClass.DoubleBottom) 
//				|| (_TriSwingClassList[0] == SwingClass.HigherLow) 
//				|| (_TriSwingClassList[0] == SwingClass.LowerLow) )  return(PendingEntryType.Long); 
//			
//			if (   (_TriSwingClassList[0] == SwingClass.DoubleTop) 
//				|| (_TriSwingClassList[0] == SwingClass.HigherHigh) 
//				|| (_TriSwingClassList[0] == SwingClass.LowerHigh) ) return(PendingEntryType.Short); 
//			
//			return(PendingEntryType.None);
//		}
		
		public double getEntryOrderPrice(OrderAction oa) 
		{
			double pullBackLevel = 0.0;
			double entryAdjust   = this.ticksFromPullbackToEnter * TickSize;
			
			if (oa == OrderAction.Buy)
			{
				pullBackLevel = this._TriSwingPriceList[1] - (this.fibLevelEntry * distanceBtwSwings); 
				return(pullBackLevel + entryAdjust); 
			}
			if (oa == OrderAction.Sell)
			{
				pullBackLevel = this._TriSwingPriceList[1] + (this.fibLevelEntry * distanceBtwSwings); 
				return(pullBackLevel - entryAdjust); 
			}
			
			return(pullBackLevel); 
		}
		
		public void sendNewOrder()
		{
			if (entryOrder == null)
			{
				if ((!isBlockedTime(this._EventTime))
					&& (((this._EventTime >= this._Session1StartTime) && (this._EventTime < this._Session1EndTime))
						|| ((this._EventTime >= this._Session2StartTime) && (this._EventTime <= this._LastNewEntryTime))) )	
				{
					if ((!this._IsTradingHalted) && (this._TriHasFirstTriangle))
					{
						if ((this._CanSendNewEntry) && (Position.MarketPosition == MarketPosition.Flat) )
						{
							this._OrderPrice         = 0.0d; 
							OrderAction orderAction	 = OrderAction.Buy; 
									
							if ( this._PendingEntry == PendingEntryType.Long) 
							{
								this._OrderPrice     = getEntryOrderPrice(orderAction);  
								segmentColor         = this._TriColorLong;
								
								if ( (Close[0] >= _HistData[HistoricalData.YdyLow]) && (Close[0] <= _HistData[HistoricalData.YdyHigh]) )
								{
									if ((this._OrderPrice < _HistData[HistoricalData.YdyLow]) 
										|| (this._OrderPrice > _HistData[HistoricalData.YdyHigh])
										|| (!this._isEntryPriceNearSRLine(this._OrderPrice)))    this._PendingEntry = PendingEntryType.None;
								}
								
								if (Close[0] > _HistData[HistoricalData.YdyHigh]) 
								{
									if ( ((this._OrderPrice < _HistData[HistoricalData.YdyClose]) 
										&& (this._OrderPrice < _ValAreaData[ValueAreaData.Y_VAL]))
										|| (!this._isEntryPriceNearSRLine(this._OrderPrice)) )  this._PendingEntry = PendingEntryType.None;
								}
								if (Close[0] < _HistData[HistoricalData.YdyLow])                this._PendingEntry = PendingEntryType.None;
							}
							
							if ( this._PendingEntry == PendingEntryType.Short) 
							{
								orderAction          = OrderAction.Sell;
								this._OrderPrice     = getEntryOrderPrice(orderAction);
								segmentColor         = this._TriColorShort;
								
								if ( (Close[0] >= _HistData[HistoricalData.YdyLow]) && (Close[0] <= _HistData[HistoricalData.YdyHigh]) )
								{
									if ((this._OrderPrice < _HistData[HistoricalData.YdyLow]) 
										|| (this._OrderPrice > _HistData[HistoricalData.YdyHigh])
										|| (!this._isEntryPriceNearSRLine(this._OrderPrice)))    this._PendingEntry = PendingEntryType.None;
								}
								if (Close[0] < _HistData[HistoricalData.YdyLow]) 
								{
									if ( ((this._OrderPrice > _HistData[HistoricalData.YdyClose]) 
										&& (this._OrderPrice > _ValAreaData[ValueAreaData.Y_VAH]))
										|| (!this._isEntryPriceNearSRLine(this._OrderPrice)) )  this._PendingEntry = PendingEntryType.None;
								}
								if (Close[0] > _HistData[HistoricalData.YdyHigh])               this._PendingEntry = PendingEntryType.None;
							}
							
							if ( this._PendingEntry != PendingEntryType.None) 
							{
								this._TriangleCount++;
								triNumberAtEntry = this._TriangleCount; 
								
								this._submitNewEntryOrder(ref entryOrder, orderAction, this._OrderPrice);
								this._OrderSubmissionTime  = this._EventTime; 
								
								Print(String.Format("{0} {1} order to {2} will be placed at {3:N2}. Time: {4:T}.", 
									this._StratName, this._EntryOrderType, orderAction, this._OrderPrice, this._EventTime));
								
								this._EntryPointLine	 = DrawLine("entryPointLine", this._AutoScale, this._EventTime.Subtract(this._TimeZoneOffset), this._OrderPrice, 
									this._EventTime.Subtract(this._TimeZoneOffset).Add(this._LineDuration), this._OrderPrice, Color.Blue, DashStyle.Solid, 2);
								
//								DrawLine("L1_" + this._TriangleCount, this._AutoScale, this._BarCount - this._TriBarCountAtSwingList[0], this._TriSwingPriceList[0], 
//									this._BarCount - this._TriBarCountAtSwingList[1], this._TriSwingPriceList[1], segmentColor, DashStyle.Solid, 2);   			//connects P1 to P2
							}
						}
					}
				}
			}	
		}
		
		#endregion //closes entryRules
				
        protected override void Initialize()
        {
			EntriesPerDirection 	= 1;
    		EntryHandling 			= EntryHandling.AllEntries;
			Unmanaged 				= true; 
			CalculateOnBarClose 	= true; 
			ExitOnClose 			= this._ExitAtMarketClose;
			
			DisconnectDelaySeconds 	= 20;
			ConnectionLossHandling 	= ConnectionLossHandling.KeepRunning;
			MaxRestartMinutes 		= 2;
			MaxRestartAttempts 		= 500; 
			
			Add(PeriodType.Tick, 1);
			
			this.setParameterDefaults(); 
			
			pas = PriceActionSwingPro(Convert.ToInt32(DtbStrength), Convert.ToInt32(SwingStrength), SwingStyle, UseCloseValues);
			this._pasSetIndicatorSettings(pas); 
		}
		
		protected override void OnStartUp()
		{
			if (this._UseCustomChart) this._setChartControl(); 	
		}
		
		protected override void OnBarUpdate()
        {	
			this._EventTime = DateTime.Now.Add(this._TimeZoneOffset);
			if (Historical) this._EventTime = Time[0].Add(this._TimeZoneOffset);
			
			try
			{					
				if (Time[0].Date == DateTime.Today.Date)
				{
					#region firstTickCode
					if (this._IsFirstTick)
					{
						this._IsFirstTick         = false; 
						
						this._setFuturesCode(); 
						this._setStrategyName(stratNamePrefix);
						
						this.validateParameters();
						this._setLocalTimeZoneOffset(); 
						this._setValidTradingTimes(this._EventTime);
						this._setHistoricalData(); 
									
						double clsPx = Close[0]; 
						if (this._Session1EndTime != this._Session2StartTime)
							DrawRectangle("blockedTimes", this._AutoScale, this._Session1EndTime.Subtract(this._TimeZoneOffset), clsPx - (clsPx/2) , 
								this._Session2StartTime.Subtract(this._TimeZoneOffset), clsPx + (clsPx/2), Color.Black, Color.LightGray, 6);
					}
					#endregion //closes firstTickCode	
		
					if (!this._IsFirstTick)
					{
						#region tickCode
						if (BarsInProgress == 1)
						{	
							if (!Historical) 
							{
								if (this._EventTime >= this._Session1StartTime)
								{
									#region exitTests
									if (Position.MarketPosition != MarketPosition.Flat)
									{
										lock(_object)
										{
											this._setExitReason(Position);  
											if ((this._ExitReason != ExitReason.None) && (!this._ExitOrderSent)) 
											{	
												this._ExitOrderSent = true; 
												
												OrderAction oa = OrderAction.Sell;
												if (Position.MarketPosition == MarketPosition.Short) oa = OrderAction.Buy; 
												
												string msg = this._createOrderString(this._StratName, OrderPurpose.Flatten, oa, this._TradeNumber);
												basicExitOrder = SubmitOrder(0, oa, OrderType.Market, Position.Quantity, 0, 0, String.Empty, msg); 	
											}
										}	
									}
									#endregion //closes exitTests
									
									#region cancelOrders_ExceedsEntryTime							
									if ((entryOrder != null)  && (entryOrder.OrderState != OrderState.PartFilled) && (entryOrder.OrderState != OrderState.Filled))
									{
										if (this._EventTime >= this._OrderSubmissionTime.Add(this._MaxTimeToEnterTrade)) 
										{
											this._processCanceledOrder(ref entryOrder, CancelReason.MaxTime); 
											this.RemoveDrawObject(this._EntryPointLine);
											this.RemoveDrawObject("L1_" + triNumberAtEntry); 
										}	
									}				
									#endregion //closes cancelOrders_ExceedsEntryTime	
								}
							}
						}
						#endregion //closes tickCode
						
						#region primaryBarCode
						
						if (BarsInProgress == 0) //executed on the primary bar
						{
							#region historicalPlots
							this._recalculateHistoricalData(); //can be done at every tick if needed.  Updates O/N H/L and IB H/L.  
							if ( (this._ShowHistoricalPlot) && (!this._IsFirstTick) && (!Historical)) 
							{
								//no calculations are done here.  Solely updates the display so that the lines remain at the right-most edge of the chart.
								this._plotHistoricalLines();
								this._updateValueArea();
							}
							#endregion //closes historicalPlots
							
							#region duringTimeMarketIsOpen
									
							if ((this._EventTime >= this._Session1StartTime) && (this._EventTime <= this._Session2EndTime) ) //(!Historical) && 
							{								
								this._BarCount++; 
								
								#region createTriangles
								//get swing and price for current bar
								SwingClass swingClass  = this._pasGetSwingClass(pas, 0);  
								double swingPrice      = this._pasGetSwingPrice(pas, swingClass, 0);
								
								#region firstTriangle
								if (!this._TriHasFirstTriangle)
								{
									this._testForFirstTriangle(pas, swingClass, swingPrice);  
										
									if (this._TriSwingClassList.Count == numTrianglePoints)
									{
										this._TriHasFirstTriangle = true; 	
										this._PendingEntry 	      = getEntryType();
										distanceBtwSwings 	      = Math.Abs(_TriSwingPriceList[1] - _TriSwingPriceList[0]);
										if (!Historical) this.sendNewOrder();
									}
								}
								#endregion //closes firstTriangle
								
								#region otherTriangles
								else
								{
									//if first triangle is formed and a NEW swing occurs, shift points
									if ((pas.HasNewSwingLow[0]) && (!pas.HasUpdatedSwingLow[0])
									|| (pas.HasNewSwingHigh[0]) && (!pas.HasUpdatedSwingHigh[0]))
									{
										this._shiftTrianglePoints(swingClass, swingPrice); 
										
										this._PendingEntry = getEntryType();
										distanceBtwSwings  = Math.Abs(_TriSwingPriceList[1] - _TriSwingPriceList[0]);
											
										if (!Historical)
										{		
											if ( (entryOrder != null)  && (entryOrder.OrderState != OrderState.PartFilled) && (entryOrder.OrderState != OrderState.Filled))
											{
												this._processCanceledOrder(ref entryOrder, CancelReason.OppSideSwing); 
												this.RemoveDrawObject(this._EntryPointLine);
												this.RemoveDrawObject("L1_" + triNumberAtEntry); 
											}
											
											if (entryOrder == null) this.sendNewOrder();
										}
									}
									
									//if first triangle is formed and a swing UPDATE occurs, update last triangle point
									if (pas.HasUpdatedSwingLow[0] || pas.HasUpdatedSwingHigh[0])	
									{
										this._updateLastTrianglePoint(0, swingClass, swingPrice); 
										distanceBtwSwings  = Math.Abs(_TriSwingPriceList[1] - _TriSwingPriceList[0]);
										this._PendingEntry = getEntryType();
										
										if (!Historical)
										{
											if (entryOrder != null)
											{
												double newPx = getEntryOrderPrice(entryOrder.OrderAction);
												Print(String.Format("{0} Changed {1} price of order to {2}. Old price {3:N2}. New price {4:N2}. Time: {5:T}.", 
													this._StratName, this._EntryOrderType, entryOrder.OrderAction, entryOrder.LimitPrice, newPx, this._EventTime));
								
												this.RemoveDrawObject(this._EntryPointLine);
												this.RemoveDrawObject("L1_" + triNumberAtEntry); 
												ChangeOrder(entryOrder, entryOrder.Quantity, newPx, 0); 
												
												this._OrderSubmissionTime = this._EventTime; 
												this._EntryPointLine	  = DrawLine("entryPointLine", this._AutoScale, this._EventTime.Subtract(this._TimeZoneOffset), newPx, 
													this._EventTime.Subtract(this._TimeZoneOffset).Add(this._LineDuration), newPx, Color.Blue, DashStyle.Solid, 2);
								
//												Color sColor = this._TriColorLong;
//												if (entryOrder.OrderAction == OrderAction.Sell) sColor = this._TriColorShort;
//												DrawLine("L1_" + this._TriangleCount, this._AutoScale, _BarCount - _TriBarCountAtSwingList[0], _TriSwingPriceList[0], 
//													_BarCount - _TriBarCountAtSwingList[1], _TriSwingPriceList[1], sColor, DashStyle.Solid, 2);   
											}
											else
											{
												this.sendNewOrder();
											}
										}
									}								
								}	
								#endregion //closes otherTriangles
		
								#endregion //closes createTriangles 
							}
							#endregion //closes duringTimeMarketIsOpen
						}
						#endregion //closes primaryBarCode
					}
				}
			}
			catch(Exception e)
			{
				Print(String.Format("{0} Exception occurred: {1}", this._StratName, e));
			}
		}
		
		protected override void OnExecution(IExecution execution)
        {
			if (execution.Order != null) 
			{	 				
				#region unFilledOrders
				
				#region rejectedOrders
				if (execution.Order.OrderState == OrderState.Rejected)
				{
					this._CanSendNewEntry = true;
					this._printOrderStateString(execution); 
					
					if ((this._OpenContracts != 0) || (Position.MarketPosition != MarketPosition.Flat))
					{
						OrderAction flattenAction = OrderAction.Buy;
						if (this._OpenContracts < 0) flattenAction = OrderAction.Sell;

						string msg = this._createOrderString(this._StratName, OrderPurpose.HandleRejection, flattenAction, this._TradeNumber); 
						rejectionExitOrder = SubmitOrder(0, flattenAction, OrderType.Market, Math.Abs(this._OpenContracts), 0, 0, String.Empty, msg); 
					}
				}	
				#endregion //closes rejectedOrders
				
				if (execution.Order.OrderState == OrderState.Cancelled) this._printOrderStateString(execution); 
				
				#endregion //closes unFilledOrders	
				
				#region filledOrders
				if ((execution.Order.OrderState == OrderState.Filled) || (execution.Order.OrderState == OrderState.PartFilled))
				{	
					//if the filled order is an entry order
					if ((entryOrder != null) &&  (execution.Order == entryOrder))         this.processEntryFills(execution, ref entryOrder); 
					//if the filled order is an exit order
					if ((basicExitOrder != null) && (execution.Order == basicExitOrder))  this.processExitFills(execution, ref basicExitOrder); 
				}
				#endregion //closes filledOrders	
			}
		}    
		
		protected override void OnTermination()
		{
			if (!this._IsStrategyTerminated )
			{
				this._IsStrategyTerminated = true; 
				
				Print(String.Empty); 
				Print(String.Format("{0} Strategy has been stopped at: {1:T}. Closing open positions and canceling unfilled orders.", this._StratName, DateTime.Now.Add(this._TimeZoneOffset))); 
				if (entryOrder != null) CancelOrder(entryOrder);
				
				this._flattenPositions("StrategyStopped"); 
			}
		}

		#region properties_Custom
		
		[Description("Fib Levels - Entry")]
		[GridCategory("Entry Parameters")]
		public FibLevels FibLevels_Entry
		{
			get { return fibLevels_Entry; }
			set { fibLevels_Entry = value; }
		}
		
		[Description("Ticks from Pullback to Enter")]
		[GridCategory("Entry Parameters")]
		public int TicksFromPullbackToEnter
		{
			get { return ticksFromPullbackToEnter; }
			set { ticksFromPullbackToEnter = Math.Max(0,value); }
		}

		[Description("Swing Strength1")]
		[GridCategory("Price Action Parameters")]
		public int SwingStrength
		{
			get { return swingStrength; }
			set { swingStrength = Math.Max(1,value); }
		}
		
		[Description("PAS - Double Top/Bottom Strength1")]
		[GridCategory("Price Action Parameters")]
		public int DtbStrength
		{
			get { return dtbStrength; }
			set { dtbStrength = Math.Max(1,value); }
		}
		
		[Description("PAS - Swing Style1")]
		[GridCategory("Price Action Parameters")]
		public PriceActionSwing.Base.SwingStyle SwingStyle
		{
			get { return swingStyle; }
			set { swingStyle = value; }
		}
		
		[Description("PAS - Use Close Value1")]
		[GridCategory("Price Action Parameters")]
		public bool UseCloseValues
		{
			get { return useCloseValues; }
			set { useCloseValues = value; }
		}
		
		[Description("Fib Levels - Take Profit")]
		[GridCategory("Exit Parameters")]
		public FibLevels FibLevels_TakeProfit
		{
			get { return fibLevels_TakeProfit; }
			set { fibLevels_TakeProfit = value; }
		}
		
		[Description("Fib Levels - Stop Loss")]
		[GridCategory("Exit Parameters")]
		public FibLevels FibLevels_StopLoss
		{
			get { return fibLevels_StopLoss; }
			set { fibLevels_StopLoss = value; }
		}
		
		#endregion //closes properties_Custom
		
        #region properties_Standard
		
		[Description("Use Custom Chart?")]
		[GridCategory("Display Parameters")]
		public bool UseCustomChart
		{
			get { return this._UseCustomChart; }
			set { this._UseCustomChart = value; }
		}
		
		[Description("Plot Historical Data")]
		[GridCategory("Display Parameters")]
		public bool ShowHistoricalPlot
		{
			get { return this._ShowHistoricalPlot; }
			set { this._ShowHistoricalPlot = value; }
		}
		
		[Description("Auto Scale")]
		[GridCategory("Display Parameters")]
		public bool AutoScale
		{
			get { return this._AutoScale; }
			set { this._AutoScale = value; }
		}
		
		[XmlIgnore()]
		[Description("Pit Session Open")]
		[GridCategory("Historical Data")]
		public TimeSpan PitSessionOpenTime
		{
			get { return this._PitSessionOpenTime; }
			set { this._PitSessionOpenTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Pit Session Close")]
		[GridCategory("Historical Data")]
		public TimeSpan PitSessionCloseTime
		{
			get { return this._PitSessionCloseTime; }
			set { this._PitSessionCloseTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Pit Session Early Close")]
		[GridCategory("Historical Data")]
		public TimeSpan PitSessionEarlyCloseTime
		{
			get { return this._PitSessionEarlyCloseTime; }
			set { this._PitSessionEarlyCloseTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Initial Balance Start Time")]
		[GridCategory("Historical Data")]
		public TimeSpan InitialBalanceStartTime
		{
			get { return this._InitialBalanceStartTime; }
			set { this._InitialBalanceStartTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Initial Balance End Time")]
		[GridCategory("Historical Data")]
		public TimeSpan InitialBalanceEndTime
		{
			get { return this._InitialBalanceEndTime; }
			set { this._InitialBalanceEndTime = value; }
		}

		[XmlIgnore()]
		[Description("Strategy Time Zone (associated with Trading Hours)")]
		[GridCategory("Time parameters")]
		public StrategyTimeZone StratTimeZone
		{
			get { return this._StrategyTimeZone; }
			set { this._StrategyTimeZone = value; }
		}
		
		[XmlIgnore()]
		[Description("First Session Start Time")]
		[GridCategory("Time parameters")]
		public TimeSpan FirstSessionStartTime
		{
			get { return this._FirstSessionStartTime; }
			set { this._FirstSessionStartTime = value; }
		}
		
		[XmlIgnore()]
		[Description("First Session End Time")]
		[GridCategory("Time parameters")]
		public TimeSpan FirstSessionEndTime
		{
			get { return this._FirstSessionEndTime; }
			set { this._FirstSessionEndTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Second Session Start Time")]
		[GridCategory("Time parameters")]
		public TimeSpan SecondSessionStartTime
		{
			get { return this._SecondSessionStartTime; }
			set { this._SecondSessionStartTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Second Session End Time")]
		[GridCategory("Time parameters")]
		public TimeSpan SecondSessionEndTime
		{
			get { return this._SecondSessionEndTime; }
			set { this._SecondSessionEndTime = value; }
		}
		
		[XmlIgnore()]
		[Description("Time before End of Session to Stop New Entries")]
		[GridCategory("Time parameters")]
		public TimeSpan PrdBeforeEndToFlatten
		{
			get { return this._PrdBeforeEndToFlatten; }
			set { this._PrdBeforeEndToFlatten = value; }
		}
		
		[XmlIgnore()]
		[Description("Max Time Allowed for Entry")]
		[GridCategory("Time parameters")]
		public TimeSpan MaxTimeToEnterTrade
		{
			get { return this._MaxTimeToEnterTrade; }
			set { this._MaxTimeToEnterTrade = value; }
		}

		[Description("Exit at End of Trading Hours")]
		[GridCategory("Time parameters")]
		public bool ExitAtMarketClose
		{
			get { return this._ExitAtMarketClose; }
			set { this._ExitAtMarketClose = value; }
		}
		
		[Description("Strategy Name Prefix")]
		[GridCategory("Strategy Parameters")]
		public string StratNamePrefix
		{
			get { return stratNamePrefix; }
			set { stratNamePrefix = value; }
		}	
		
		[Description("Number of Contracts/Trade Size")]
		[GridCategory("Strategy Parameters")]
		public int NumberOfContracts
		{
			get { return this._NumContracts; }
			set { this._NumContracts = Math.Max(1,value); }
		}	
		
		[Description("Entry Order Type")]
		[GridCategory("Strategy Parameters")]
		public OrderType EntryOrderType
		{
			get { return this._EntryOrderType; }
			set { this._EntryOrderType = value; }
		}
		
		[Description("Number of Days before Expiry to Roll Futures")]
		[GridCategory("Strategy Parameters")]
		public int NumBizDaysToRollBeforeExpiry
		{
			get { return this._NumBizDaysToRollBeforeExpiry; }
			set { this._NumBizDaysToRollBeforeExpiry = Math.Max(0,value); }
		}
		
		[Description("Value Area Percent")]
		[GridCategory("Historical Data")]
		public double ValueAreaPercent
		{
			get { return this._ValueAreaPercent; }
			set { this._ValueAreaPercent = value; }
		}
			
		[Description("Percent of TakeProfit - Trigger 1")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Trigger1
		{
			get { return this._PercentTakeProfit_Trigger1; }
			set { this._PercentTakeProfit_Trigger1 = value; }
		}
				
		[Description("Percent of TakeProfit - Trigger 2")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Trigger2
		{
			get { return this._PercentTakeProfit_Trigger2; }
			set { this._PercentTakeProfit_Trigger2 = value; }
		}
		
		[Description("Percent of TakeProfit - Trigger 3 ")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Trigger3
		{
			get { return this._PercentTakeProfit_Trigger3; }
			set { this._PercentTakeProfit_Trigger3 = value; }
		}
	
		[Description("Percent of TakeProfit - Exit 1")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Exit1
		{
			get { return this._PercentTakeProfit_Exit1; }
			set { this._PercentTakeProfit_Exit1 = value; }
		}
		
		[Description("Percent of TakeProfit - Exit 2")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Exit2
		{
			get { return this._PercentTakeProfit_Exit2; }
			set { this._PercentTakeProfit_Exit2 = value; }
		}
		
		[Description("Percent of TakeProfit - Exit 3")]
		[GridCategory("Exit Parameters")]
		public double PercentTakeProfit_Exit3
		{
			get { return this._PercentTakeProfit_Exit3; }
			set { this._PercentTakeProfit_Exit3 = value; }
		}
		
		[Description("Trail Stop - Step Interval")]
		[GridCategory("Exit Parameters")]
		public double StepInterval
		{
			get { return this._StepInterval; }
			set { this._StepInterval = value; }
		}
		
		[Description("Minimum Target (Ticks)")]
		[GridCategory("Exit Parameters")]
		public int MinTarget_InTicks
		{
			get { return this._MinTarget_InTicks; }
			set { this._MinTarget_InTicks = Math.Max(0,value); }
		}	
				
		[Description("Use BE/BEPlus Stops")]
		[GridCategory("Exit Parameters")]
		public bool UseTrailingStops
		{
			get { return this._UseTrailingStops; }
			set { this._UseTrailingStops = value; }
		}
		
		[Description("Consecutive Wins/Losses Before Trading Halt")]
		[GridCategory("Strategy Parameters")]
		public int ConsecutiveWLForTradingHalt
		{
			get { return this._ConsecutiveWLForTradingHalt; }
			set { this._ConsecutiveWLForTradingHalt = Math.Max(1,value); }
		}
		
		[Description("Maximum Ticks from SR Line")]
		[GridCategory("Entry Parameters")]
		public int MaxTicksFromSrLine
		{
			get { return this._MaxTicksFromSrLine; }
			set { this._MaxTicksFromSrLine = Math.Max(0,value); }
		}
		
		#endregion //closes properties_Standard
		
		#region timeSpanSerialization
	
		[Browsable(false)]
		public string srl_PitSessionOpenTime
		{
			get {return this._PitSessionOpenTime.ToString();}	
			set {this._PitSessionOpenTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_PitSessionCloseTime
		{
			get {return this._PitSessionCloseTime.ToString();}	
			set {this._PitSessionCloseTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_PitSessionEarlyCloseTime
		{
			get {return this._PitSessionEarlyCloseTime.ToString();}	
			set {this._PitSessionEarlyCloseTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_InitialBalanceStartTime
		{
			get {return this._InitialBalanceStartTime.ToString();}	
			set {this._InitialBalanceStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_InitialBalanceEndTime
		{
			get {return this._InitialBalanceEndTime.ToString();}	
			set {this._InitialBalanceEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_FirstSessionStartTime
		{
			get {return this._FirstSessionStartTime.ToString();}	
			set {this._FirstSessionStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}

		[Browsable(false)]
		public string srl_FirstSessionEndTime
		{
			get {return this._FirstSessionEndTime.ToString();}	
			set {this._FirstSessionEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_SecondSessionStartTime
		{
			get {return this._SecondSessionStartTime.ToString();}	
			set {this._SecondSessionStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_SecondSessionEndTime
		{
			get {return this._SecondSessionEndTime.ToString();}	
			set {this._SecondSessionEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}

		[Browsable(false)]
		public string srl_PrdBeforeEndToFlatten
		{
			get {return this._PrdBeforeEndToFlatten.ToString();}	
			set {this._PrdBeforeEndToFlatten = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string srl_MaxTimeToEnterTrade
		{
			get {return this._MaxTimeToEnterTrade.ToString();}	
			set {this._MaxTimeToEnterTrade = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
	
		#endregion //closes timeSpanSerialization
		*/
    }
}