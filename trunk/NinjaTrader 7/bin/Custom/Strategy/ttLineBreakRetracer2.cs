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
using System.Collections.Generic;
#endregion

#region spec
/*	for Tapas Tejura, September 2012
•       Utilising LineBreak (or LineBreakWicked) charts
•       Takes retracement entries at Limit upon reversals of trend after x bars in one direction
•       Can trade multiple entries (up to 4) from the one bar, based on which entry options are switched on:
o   Automatic, 3 inputs for % retracement (all configurable x percent)
o   Manual, 1 input for x Ticks retracement
•       Target and stoploss
o   Automatic mode, based on trigger bar range with inputs for x ticks beyond the range
o   Manual mode, of x ticks
o   Auto and Manual switchable separately for the target and for the stoploss
•       Minimum size and maximum size filter for the trigger bar range
•       Code written to handle partial fills. (of all 4 entries, stoploss and target)
•       Adjustable time frames (a window, or zone) for when the automated strategy is enabled (ie; only morning or afternoon session) e.g. 9:30 to 12:00, trade within those times.
*/
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// By TradingCoders.com - trades retracements from reversals on LineBreak charts.
    /// </summary>
    [Description("By TradingCoders.com - trades retracements from reversals on LineBreak charts.")]
    public class ttLineBreakRetracer2 : Strategy
    {
		public enum ModeType
		{
			Auto,
			Manual
		}
		
		public enum ModeTypeTarget
		{
			Auto,
			Manual
			//,BarColorChange
		}
		
		public enum LineBreakPeriodType
		{
			Minute,
			Tick,
			Volume
		}
		
        #region Variables
        // USER INPUT PARAMETERS
		private bool  standardEntries = true;
		private bool  sandwichEntries = false;
		
        private double autoRetracePercent_1 = 25; // Default setting for AutoRetracePercent_1
        private double autoRetracePercent_2 = 50; // Default setting for AutoRetracePercent_2
        private double autoRetracePercent_3 = 100; // Default setting for AutoRetracePercent_3
        private int manualRetraceTicks = 5; // Default setting for ManualRetraceTicks
        private int autoTargetTicks = 3; // Default setting for AutoTargetTicks
        private int manualTargetTicks = 5; // Default setting for ManualTargetTicks
        private int autoStoplossTicks = 1; // Default setting for AutoStoplossTicks
        private int manualStoplossTicks = 5; // Default setting for ManualStoplossTicks
        private int barsInOneDirectionRequired = 3;
		private bool barColorChangeExit = true;
		private bool barColorChangeExitAlways = true; // takes any color change bar, not just a signal bar (that has sufficient barsinOneDirection behind it)
		
		private bool allow_LongEntries = true;
		private bool allow_ShortEntries = true;
		
		private ModeTypeTarget targetMode = ModeTypeTarget.Auto;
		private ModeType stopLossMode = ModeType.Auto;
		private int barsUntilCancelOrders = 1;
		
		private bool autoRetraceEnabled_1 = true;
		private bool autoRetraceEnabled_2 = true;
		private bool autoRetraceEnabled_3 = true;
		private bool manualRetraceEnabled = true;
		
		private int triggerBarMinimumTicks = 4;
		private int triggerBarMaximumTicks = 30;
		
		private int beginTime = 930;
		private int endTime = 1600;
		
		private bool paintTriggerBars = true;
		//private int positionSize = 1;
		private int minimumExitTicks = 3; // ticks
		
		private bool superTrendFilter_Enabled = false;
		
		// daily pnl
		private int profitTargetDaily = 300; // Default setting for ProfitTargetDaily
		private int stopLossDaily = 300;
		private bool dailyStopAndTargetEnabled = false;
		private bool includeUnrealizedPnL = false; // not implemented to not conflict with the high gran trade DataSeries tbip
				// internal
				private double dailyTally = 0;	// daily PnL, and also used for AtmStrategy
				private double totalTally = 0;	// multi-day accrued PnL for internal trading mode
				private bool noTrading = false;
				private double unrealized = 0;		
				string lastDailyNoTradingTag = ""; // used to re-draw the tag with the true closed PnL
				bool drawADailyPnLMessage = false; // used within OnPositionUpdate()
				// end internal
		
		// theoretical break bar trailing stop
		private bool breakBarTrailingStop = true;
		private int  breakBarTrailingStopBars = 3;
		
		// complex position sizing
		private bool 	leadupSizing = false;
		private bool 	unfilledSizing = false;
		private int 	fixedSizeAuto1 = 1;
		private int		fixedSizeAuto2 = 1;
		private int		fixedSizeAuto3 = 1;
		private int		fixedSizeManual = 1;
		private int[]	leadupBars = new int[]{3,5};
		private int[]	leadupSizes= new int[]{1,2,3};
		private int[]	unfilledBars = new int[]{3,5};
		private int[]	unfilledSizes = new int[]{2,1};
		
		// additional supertrend filtering timeframes 
		private bool 	extraTimeFrame1 = false;
		private bool 	extraTimeFrame2 = false;
		private bool 	extraTimeFrame3 = false;
		private bool 	extraTimeFrame4 = false;
		private LineBreakPeriodType 	extraPeriodType1 = LineBreakPeriodType.Minute;
		private LineBreakPeriodType 	extraPeriodType2 = LineBreakPeriodType.Minute;
		private LineBreakPeriodType 	extraPeriodType3 = LineBreakPeriodType.Minute;
		private LineBreakPeriodType 	extraPeriodType4 = LineBreakPeriodType.Minute;
		private int 	extraPeriodValue1 = 2;
		private int 	extraPeriodValue2 = 5;
		private int 	extraPeriodValue3 = 15;
		private int 	extraPeriodValue4 = 60;
		private int 	extraLineBreak1 = 3;
		private int 	extraLineBreak2 = 3;
		private int 	extraLineBreak3 = 3;
		private int 	extraLineBreak4 = 3;
		
		
		#region variables for anaSuperTrendU11
		
		private int 						basePeriod			= 3; // Default setting for Median Period
            private int 						rangePeriod			= 15; // Default setting for Range Period
			private double 						multiplier 			= 2.5; // Default setting for Multiplier
			private anaSuperTrendU11BaseType 	thisBaseType		= anaSuperTrendU11BaseType.Median; 
			private anaSuperTrendU11OffsetType 	thisOffsetType		= anaSuperTrendU11OffsetType.Default; 
			private anaSuperTrendU11VolaType 	thisVolaType		= anaSuperTrendU11VolaType.True_Range; 
			private bool						candles				= false;
			private bool 						gap					= false;
			private bool						reverseIntraBar		= false;
			private bool 						showArrows 			= false;
			private bool 						showPaintBars 		= false;
			private bool 						showStopLine		= true;
			private bool						soundAlert			= false;
			private bool 						currentUpTrend		= true;
			private bool 						priorUpTrend		= true;
			private bool						stoppedOut			= false;
			private double						movingBase			= 0.0;
			private double 						offset				= 0.0;
			private double						trailingAmount		= 0.0;
			private double						currentStopLong		= 0.0;
			private double						currentStopShort	= 0.0;
			private double						margin				= 0.0;
			private int							displacement		= 0;
			private int							opacity				= 3;
			private int							alpha				= 0;
			private int 						plot0Width 			= 1;
			private PlotStyle 					plot0Style			= PlotStyle.Dot;
			private DashStyle					dash0Style			= DashStyle.Dot;
			private int 						plot1Width 			= 1;
			private PlotStyle 					plot1Style			= PlotStyle.Line;
			private DashStyle 					dash1Style			= DashStyle.Solid;
			private Color 						upColorST			= Color.DodgerBlue;
			private Color 						downColorST			= Color.Red;
			private Color						trendColor			= Color.Empty;
			private int 						rearmTime			= 30;
			private string 						confirmedUpTrend	= "newuptrend.wav";
			private string 						confirmedDownTrend	= "newdowntrend.wav";
			private string 						potentialUpTrend	= "potentialuptrend.wav";
			private string 						potentialDownTrend	= "potentialdowntrend.wav";
//			private DataSeries					reverseDot;			
//			private BoolSeries 					upTrend;
//			private IDataSeries		 			baseline;
//			private IDataSeries					rangeSeries;
//			private IDataSeries 				offsetSeries;
//			private ATR							volatility;
		
		#endregion
		
		// INTERNAL VARIABLES
		bool bInit = false;
		bool myHistorical = true;
		int myHistoricalBar = -10;
		int latestTriggerBar = 0;
		int realtimeBar = 0;
		int AZ = 0;
		bool isOvernight = false;
		/// <summary>
		/// set to true when all orders are cancelled ; new orders are allowed to go on same bar
		/// </summary>
		bool freshCancelOfOrders = false;
		
		/// <summary>
		/// set to true when all orders are cancelled and a position is exiting; new orders are allowed to go on same bar
		/// </summary>
		bool exitDone = false;
		/// <summary>
		/// auto entry or manual entry IOrder
		/// </summary>
		IOrder[] longEntryOrders = new IOrder[4], shortEntryOrders = new IOrder[4];
		IOrder[] longTargetOrders = new IOrder[4], shortTargetOrders = new IOrder[4];
		IOrder[] longStopLossOrders = new IOrder[4], shortStopLossOrders = new IOrder[4];
		IOrder exitOrder;
		
		bool waitUntilNewSession = false;	// set to True when an ExitOnClose happens, to avoid entering AFTER that still in the expiring session
		
		bool developer = false;
		
		anaSuperTrendU11 supert, supertExtra1, supertExtra2, supertExtra3, supertExtra4;
		
		TCStrategyPlotMulti tradePlots,supertPlots; 
		int[] supertValues= new int[4];
		
		// PRESETS
		Color upColor = Color.SkyBlue;
		Color downColor = Color.Magenta;
		int tbip = 1;
		int e1bip = -1, e2bip = -1, e3bip = -1, e4bip = -1;
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
			TraceOrders = true;
			Unmanaged = true;
			//ExitOnClose = false;
			
			// add high granularity timeframe for accurate backtesting
			Add(PeriodType.Range,1);
			tbip = BarsPeriods.Length-1;
			
			// possibly add other supertrend timeframes
			if (extraTimeFrame1)
			{
				if (BarsPeriods[0].Id == PeriodType.LineBreak)
					AddLineBreak(Instrument.FullName,PeriodTypeConverter(extraPeriodType1),extraPeriodValue1,extraLineBreak1,MarketDataType.Last);
				else
				{
					Add(PeriodType.Custom8,extraLineBreak1);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodType = PeriodTypeConverter(extraPeriodType1);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodValue = extraPeriodValue1;
				}
				e1bip = BarsPeriods.Length-1;
			}
			if (extraTimeFrame2)
			{
				if (BarsPeriods[0].Id == PeriodType.LineBreak)
					AddLineBreak(Instrument.FullName,PeriodTypeConverter(extraPeriodType2),extraPeriodValue2,extraLineBreak2,MarketDataType.Last);
				else
				{	// Custom8 is LineBreakWicked
					Add(PeriodType.Custom8,extraLineBreak2);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodType = PeriodTypeConverter(extraPeriodType2);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodValue = extraPeriodValue2;
				}
				e2bip = BarsPeriods.Length-1;
			}
			if (extraTimeFrame3)
			{
				if (BarsPeriods[0].Id == PeriodType.LineBreak)
					AddLineBreak(Instrument.FullName,PeriodTypeConverter(extraPeriodType3),extraPeriodValue3,extraLineBreak3,MarketDataType.Last);
				else
				{
					Add(PeriodType.Custom8,extraLineBreak3);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodType = PeriodTypeConverter(extraPeriodType3);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodValue = extraPeriodValue3;
				}
				e3bip = BarsPeriods.Length-1;
			}
			if (extraTimeFrame4)
			{
				if (BarsPeriods[0].Id == PeriodType.LineBreak)
					AddLineBreak(Instrument.FullName,PeriodTypeConverter(extraPeriodType4),extraPeriodValue4,extraLineBreak4,MarketDataType.Last);
				else
				{
					Add(PeriodType.Custom8,extraLineBreak4);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodType = PeriodTypeConverter(extraPeriodType4);
					BarsPeriods[BarsPeriods.Length-1].BasePeriodValue = extraPeriodValue4;
				}
				e4bip = BarsPeriods.Length-1;
			}
			
			supertPlots = TCStrategyPlotMulti(1,4,false);
			supertPlots.Name = "ExtraTimeFrameSuperTrends";
			supertPlots.Plots[0].PlotStyle = PlotStyle.Block;
			supertPlots.Plots[0].Pen.Width = 5;
			supertPlots.Plots[0].Name = "ExtraSuperTrend 1";
			supertPlots.Plots[1].PlotStyle = PlotStyle.Block;
			supertPlots.Plots[1].Pen.Width = 5;
			supertPlots.Plots[1].Name = "ExtraSuperTrend 2";
			supertPlots.Plots[2].PlotStyle = PlotStyle.Block;
			supertPlots.Plots[2].Pen.Width = 5;
			supertPlots.Plots[2].Name = "ExtraSuperTrend 3";
			supertPlots.Plots[3].PlotStyle = PlotStyle.Block;
			supertPlots.Plots[3].Pen.Width = 5;
			supertPlots.Plots[3].Name = "ExtraSuperTrend 4";
			Add(supertPlots);
			
			
			developer = (  NinjaTrader.Cbi.License.MachineId == "770F07788EAE7D89468263CFE8ED7ECE"  // JR win7
						||  NinjaTrader.Cbi.License.MachineId == "2C3520AC55DFD1980BCAD65C8ABD0637" // JR virtual xp
						);
			
			supert = anaSuperTrendU11(basePeriod,confirmedDownTrend,confirmedUpTrend,dash0Style,dash1Style,downColorST,multiplier,opacity,plot0Style,plot0Width
					,plot1Style,plot1Width,potentialDownTrend,potentialUpTrend,rangePeriod,rearmTime,reverseIntraBar,showArrows,showPaintBars,showStopLine,soundAlert
					,thisBaseType,thisOffsetType,thisVolaType,upColorST);
			if (superTrendFilter_Enabled)
				Add(supert);
			
			tradePlots = TCStrategyPlotMulti(2,8,true);
			tradePlots.Name = "Stops and Targets";
			tradePlots.Plots[0].Name = "Stop1";
			tradePlots.Plots[0].Pen.Color = Color.Red;
			tradePlots.Plots[0].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[1].Name = "Stop2";
			tradePlots.Plots[1].Pen.Color = Color.Red;
			tradePlots.Plots[1].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[2].Name = "Stop3";
			tradePlots.Plots[2].Pen.Color = Color.Red;
			tradePlots.Plots[2].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[3].Name = "Stop4";
			tradePlots.Plots[3].Pen.Color = Color.Red;
			tradePlots.Plots[3].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[4].Name = "Target1";
			tradePlots.Plots[4].Pen.Color = Color.Blue;
			tradePlots.Plots[4].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[5].Name = "Target2";
			tradePlots.Plots[5].Pen.Color = Color.Blue;
			tradePlots.Plots[5].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[6].Name = "Target3";
			tradePlots.Plots[6].Pen.Color = Color.Blue;
			tradePlots.Plots[6].PlotStyle = PlotStyle.Hash;
			tradePlots.Plots[7].Name = "Target4";
			tradePlots.Plots[7].Pen.Color = Color.Blue;
			tradePlots.Plots[7].PlotStyle = PlotStyle.Hash;
			Add(tradePlots);
        }

		private PeriodType PeriodTypeConverter(LineBreakPeriodType type)
		{
			switch (type) 
			{
				case LineBreakPeriodType.Minute:
					return PeriodType.Minute;
					break;
				case LineBreakPeriodType.Tick:
					return PeriodType.Tick;
					break;
				case LineBreakPeriodType.Volume:
					return PeriodType.Volume;
					break;
				default:
					return PeriodType.Minute;
					break;
			}
		}
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (!bInit)
			{
				// do-once initialise
				ClearOutputWindow();
				for (int a = 0; a<BarsArray.Length; a++)
					Print("BarsArray["+a+"] is "+BarsPeriods[a].ToString()+", bar count "+BarsArray[a].Count+", first bar at "+BarsArray[a].Get(0).Time);
				
				Print("Version is from February 2nd, 2013, trading BarsInProgress is "+tbip);
				
				supert.Update(); // kick tyres
				
				if (e1bip > 0)
					supertExtra1 = anaSuperTrendU11(BarsArray[e1bip],
													basePeriod,confirmedDownTrend,confirmedUpTrend,dash0Style,dash1Style,downColorST,multiplier,opacity,plot0Style,plot0Width
					,plot1Style,plot1Width,potentialDownTrend,potentialUpTrend,rangePeriod,rearmTime,reverseIntraBar,showArrows,showPaintBars,showStopLine,soundAlert
					,thisBaseType,thisOffsetType,thisVolaType,upColorST);
				
				if (e2bip > 0)
					supertExtra2 = anaSuperTrendU11(BarsArray[e2bip],
													basePeriod,confirmedDownTrend,confirmedUpTrend,dash0Style,dash1Style,downColorST,multiplier,opacity,plot0Style,plot0Width
					,plot1Style,plot1Width,potentialDownTrend,potentialUpTrend,rangePeriod,rearmTime,reverseIntraBar,showArrows,showPaintBars,showStopLine,soundAlert
					,thisBaseType,thisOffsetType,thisVolaType,upColorST);
				
				if (e3bip > 0)
					supertExtra3 = anaSuperTrendU11(BarsArray[e3bip],
													basePeriod,confirmedDownTrend,confirmedUpTrend,dash0Style,dash1Style,downColorST,multiplier,opacity,plot0Style,plot0Width
					,plot1Style,plot1Width,potentialDownTrend,potentialUpTrend,rangePeriod,rearmTime,reverseIntraBar,showArrows,showPaintBars,showStopLine,soundAlert
					,thisBaseType,thisOffsetType,thisVolaType,upColorST);
				
				if (e4bip > 0)
					supertExtra4 = anaSuperTrendU11(BarsArray[e4bip],
													basePeriod,confirmedDownTrend,confirmedUpTrend,dash0Style,dash1Style,downColorST,multiplier,opacity,plot0Style,plot0Width
					,plot1Style,plot1Width,potentialDownTrend,potentialUpTrend,rangePeriod,rearmTime,reverseIntraBar,showArrows,showPaintBars,showStopLine,soundAlert
					,thisBaseType,thisOffsetType,thisVolaType,upColorST);
				
				
				bInit = true;
			}
			if (BarsInProgress > 0)
				return;
			
			if (CurrentBar <= barsInOneDirectionRequired)
				return;
			
			if (myHistorical)
			{
				if (myHistoricalBar == CurrentBar || !Historical)
				{
					myHistorical = false;
					Print(Now+" myHistorical hits realtime");
					realtimeBar = CurrentBar;
				}
				myHistoricalBar = CurrentBar;
			}
			AZ = (myHistorical || CalculateOnBarClose) ? 0 : 1;
			
			bool isOvernightMem = isOvernight;			
			isOvernight = !CheckTime(beginTime,endTime);
			if (	(	isOvernightMem && !isOvernight)
				||	(beginTime==endTime && Bars.FirstBarOfSession && FirstTickOfBar)
				)
			{
				// first bar of trading session, reset things
				
				totalTally += dailyTally;
				dailyTally = 0;
				noTrading = false;
				lastDailyNoTradingTag = "";
				Print("FirstBar of session is at "+Time[0]+" _____________________________________________ ");
				BackColor = Color.Azure;
			}
			if (isOvernight)
				BackColor = Color.FromArgb(80,Color.Plum);	// show the out-of-session times
			
			if (waitUntilNewSession)
			{
				if (Bars.FirstBarOfSession && FirstTickOfBar)
					waitUntilNewSession = false;
			}
			
			// additional supertrends
			PlotExtraSuperTrends();
			
			// daily
			PerformDailyPnLCheck(); // set globals
			
			// ________________________________________________
			
			freshCancelOfOrders = FirstTickOfBar ? CancelEntryOrdersWhenNeeded() : false;
			if (freshCancelOfOrders //&& developer
				)	// show on chart an order cancellation
				BackColorSeries[AZ] = Color.FromArgb(70,Color.Silver);
			
			exitDone = TradeManagement();
			
			if (	FirstTickOfBar && realtimeBar != CurrentBar && (!(myHistorical && CurrentBar == Count-1))
				&&	(!waitUntilNewSession)
				)
			{
				if (!myHistorical)
					Print("FirstTickOfBar, seeking entry, AZ="+AZ+", lastCloseBar="+(CurrentBar-AZ)+", "+Time[AZ]);
				
				SeekTriggerBar(); // find a trigger bar and if it is one place the entry orders
			}
			
			DoTradePlots();
			
        }

		#region daily pnl
		
		// avoid the next trade entry but know to close the open one
		private bool CheckDailyPnLOnTheQT(bool showMessage, out double nowPnL)
		{
			bool _noTrading = noTrading;
			double _dailyTally = Performance.AllTrades.TradesPerformance.Currency.CumProfit - totalTally;
			double _unrealized = Position.MarketPosition!=MarketPosition.Flat? Position.GetProfitLoss(Closes[tbip][0],PerformanceUnit.Currency) : 0;
			double checkTally = _dailyTally + _unrealized;
			nowPnL = checkTally;
			if ( dailyStopAndTargetEnabled && Math.Round(checkTally) >= profitTargetDaily)
			{	 // hit Daily Profit Target
				if (showMessage == true)
				{
					Print(Now+" Day Profit Target! +"+Math.Round(checkTally));
					IText t = DrawText("DayTarget"+CurrentBar,true,"Day Profit Target! +"+Math.Round(checkTally),0,Median[0],0,Color.DarkGreen,new Font("Arial",14),StringAlignment.Center,Color.Transparent,Color.LightGreen,4);
					Print("\t"+t.ToString());
					lastDailyNoTradingTag = "DayTarget"+CurrentBar;
				}
				_noTrading = true;	
			}
			if ( dailyStopAndTargetEnabled && Math.Round(checkTally) <= -stopLossDaily)
			{	// hit Daily Stoploss
				if (showMessage == true)
				{
					Print(Now+" Day StopLoss! "+Math.Round(checkTally));
					IText t = DrawText("DayStopLoss"+CurrentBar,true,"Day StopLoss! "+Math.Round(checkTally),0,Median[0],0,Color.Maroon,new Font("Arial",14),StringAlignment.Center,Color.Transparent,Color.Pink,4);
					Print("\t"+t.ToString());
					lastDailyNoTradingTag = "DayStopLoss"+CurrentBar;
				}
				_noTrading = true;
			}
			
			return _noTrading;
		}
		
		// general purpose check from OnBarsUpdate()
		private void PerformDailyPnLCheck()
		{			
			// Max/Min daily PnL code:
			// calculate Daily Profit Limit and Daily Stoploss Limit
			dailyTally = Performance.AllTrades.TradesPerformance.Currency.CumProfit - totalTally;
			unrealized = Position.MarketPosition!=MarketPosition.Flat? Position.GetProfitLoss(Closes[tbip][0],PerformanceUnit.Currency) : 0;
			double checkTally = dailyTally + (includeUnrealizedPnL ? unrealized : 0);
			
			if ( dailyStopAndTargetEnabled && Math.Round(checkTally) >= profitTargetDaily)
			{	 // hit Daily Profit Target
				if (noTrading == false)
				{
					Print(Now+" Day Profit Target! +"+Math.Round(checkTally));
					IText t = DrawText("DayTarget"+CurrentBar,true,"Day Profit Target! +"+Math.Round(checkTally),0,Median[0],0,Color.DarkGreen,new Font("Arial",14),StringAlignment.Center,Color.Transparent,Color.LightGreen,4);
					Print("\t"+t.ToString());
					lastDailyNoTradingTag = "DayTarget"+CurrentBar;
				}
				noTrading = true;	
			}
			if ( dailyStopAndTargetEnabled && Math.Round(checkTally) <= -stopLossDaily)
			{	// hit Daily Stoploss
				if (noTrading == false)
				{
					Print(Now+" Day StopLoss! "+Math.Round(checkTally));
					IText t = DrawText("DayStopLoss"+CurrentBar,true,"Day StopLoss! "+Math.Round(checkTally),0,Median[0],0,Color.Maroon,new Font("Arial",14),StringAlignment.Center,Color.Transparent,Color.Pink,4);
					Print("\t"+t.ToString());
					lastDailyNoTradingTag = "DayStopLoss"+CurrentBar;
				}
				noTrading = true;
			}
			
		}
		
		/// <summary>
		///  a way to force a daily pnl message (called from within OnPositionUpdate once flat)
		/// </summary>
		private void ShowDailyPnLMessage()
		{			
			RemoveDrawObject(lastDailyNoTradingTag);  // get rid of one that may be not yet calculated on the Closed PnL
			
			// Max/Min daily PnL code:
			
			// calculate Daily Profit Limit and Daily Stoploss Limit
			dailyTally = Performance.AllTrades.TradesPerformance.Currency.CumProfit - totalTally;
			if ( dailyStopAndTargetEnabled && Math.Round(dailyTally) >=0)
			{	 // hit Daily Profit Target
				{
					Print(Now+" Day Profit Target! +"+Math.Round(dailyTally));
					IText t = DrawText("DayTarget"+CurrentBar,true,"Day Profit Target! +"+Math.Round(dailyTally),0,Median[0],0,Color.DarkGreen,new Font("Arial",14),StringAlignment.Center,Color.Transparent,Color.LightGreen,4);
					Print("\t"+t.ToString());
					lastDailyNoTradingTag = "DayTarget"+CurrentBar;
				}	
			}
			if ( dailyStopAndTargetEnabled && Math.Round(dailyTally) <0)
			{	// hit Daily Stoploss
				{
					Print(Now+" Day StopLoss! "+Math.Round(dailyTally));
					IText t = DrawText("DayStopLoss"+CurrentBar,true,"Day StopLoss! "+Math.Round(dailyTally),0,Median[0],0,Color.Maroon,new Font("Arial",14),StringAlignment.Center,Color.Transparent,Color.Pink,4);
					Print("\t"+t.ToString());
					lastDailyNoTradingTag = "DayStopLoss"+CurrentBar;
				}
			}
			
		}
		
		#endregion
		
		#region Entries
		
		private void SeekTriggerBar()
		{			
			// GET SOME SIGNALS (if possible)
			int nowBarSign = 0;
			if (standardEntries)
				nowBarSign = SeekStandardEntry();
			bool standardSignal = nowBarSign != 0;
			if (sandwichEntries && nowBarSign==0)
				nowBarSign = SeekSandwichEntry();
			bool sandwichSignal = (!standardSignal) && nowBarSign != 0;
			
			if (nowBarSign == 0)
				return;	// nothing doing
			
			Print(SmartTime+" found a trigger bar "+(nowBarSign>0?"Long":"Short")+" at bar "+(CurrentBar-AZ)+", "+Time[AZ]+(isOvernight?" but currently out of session hours":""));
			// mark the bar
			if (paintTriggerBars)
			{
				if (nowBarSign > 0)
					BarColorSeries[AZ] = standardSignal ? Color.SkyBlue : Color.DeepSkyBlue;
				else if (nowBarSign < 0)
					BarColorSeries[AZ] = standardSignal ? Color.Magenta : Color.DarkMagenta;
			}
			
			// inserted for version 2, to allow reversals
			if 	(	(!freshCancelOfOrders) && (!AllEntryOrdersInactive())	)
			{
				Print("New signal causes existing unfilled entry orders to be cancelled.");
				freshCancelOfOrders = CancelEntryOrders();
			}			
			if (	(	(	Position.MarketPosition == MarketPosition.Long && nowBarSign == -1 && sandwichSignal	)
					||	(	Position.MarketPosition == MarketPosition.Short && nowBarSign == 1 && sandwichSignal	)
					)
				&&	(!exitDone)
				)
			{
				Print("New sandwich signal causes existing position to be reversed. Closing "+Position.MarketPosition);
				exitDone = 	GetFlat("Reversal");
			}
			// end insertion <<
			// version 1			
			if( !(		(freshCancelOfOrders || AllEntryOrdersInactive())
					&& 	(exitDone || Position.MarketPosition == MarketPosition.Flat)
				)
			  )
				return;	// just quit, we don't take positions while in a position or got unfilled orders open
			
			
			if (isOvernight)
				return;
			
			// check for bar ranges that are invalid
			double range = Math.Abs(High[AZ] - Low[AZ]);
			int rangeTicks = (int)(range/TickSize);
			if (rangeTicks < triggerBarMinimumTicks || rangeTicks > triggerBarMaximumTicks)
			{
				Print("\tcannot trigger trades as the bar range is "+rangeTicks+" ticks, which is outside the allowed sizes");
				return;
			}
			
			// FILTERS:  ------>>>>>>>>>>>
			
			// SuperTrend
			if (superTrendFilter_Enabled)
			{
				if (	(supert.UpTrend[AZ] && nowBarSign < 0 && Close[AZ] >= supert.StopLine[AZ])
					||	(supert.UpTrend[AZ]==false && nowBarSign > 0 && Close[AZ] <= supert.StopLine[AZ])
					)
				{
					Print(Time[AZ]+" not taking "+(nowBarSign<0?"Short":"Long")+" entry as SuperTrend is against it");	
					return;
				}				
			}
			if (extraTimeFrame1)
			{
				if (	Math.Sign(supertValues[0]) != nowBarSign	)
				{
					Print(Time[AZ]+" not taking "+(nowBarSign<0?"Short":"Long")+" entry as SuperTrend EXTRATIMEFRAME 1 is against it");	
					return;
				}
			}
			if (extraTimeFrame2)
			{
				if (	Math.Sign(supertValues[1]) != nowBarSign	)
				{
					Print(Time[AZ]+" not taking "+(nowBarSign<0?"Short":"Long")+" entry as SuperTrend EXTRATIMEFRAME 2 is against it");	
					return;
				}
			}
			if (extraTimeFrame3)
			{
				if (	Math.Sign(supertValues[2]) != nowBarSign	)
				{
					Print(Time[AZ]+" not taking "+(nowBarSign<0?"Short":"Long")+" entry as SuperTrend EXTRATIMEFRAME 3 is against it");	
					return;
				}
			}
			if (extraTimeFrame4)
			{
				if (	Math.Sign(supertValues[3]) != nowBarSign	)
				{
					Print(Time[AZ]+" not taking "+(nowBarSign<0?"Short":"Long")+" entry as SuperTrend EXTRATIMEFRAME 4 is against it");	
					return;
				}
			}
			
			// daily pnl
			if (noTrading)
				return;
			double nowPnL = 0;
			bool dontTakeAnotherEntry = CheckDailyPnLOnTheQT(false, out nowPnL);
			if (dontTakeAnotherEntry)
			{
				Print(Time[AZ]+" not taking "+(nowBarSign<0?"Short":"Long")+" entry as DailyPnL limit is reached");	
				noTrading = true;					
				if (Position.MarketPosition != MarketPosition.Flat)
					drawADailyPnLMessage = true;		// show a message once closed
				else
					ShowDailyPnLMessage();
				return;
			}
			
			// GOOD TO GO!
			// place entry orders
			latestTriggerBar = CurrentBar-AZ;
			double body = Math.Abs(Close[AZ]-Open[AZ]); // the candle body range
			if (nowBarSign > 0 && allow_LongEntries)
			{
				// LONGS
				double auto1Price = rnd(Close[AZ] - (0.01*autoRetracePercent_1 * body));
				double auto2Price = rnd(Close[AZ] - (0.01*autoRetracePercent_2 * body));
				double auto3Price = rnd(Close[AZ] - (0.01*autoRetracePercent_3 * body));	
				double manualPrice = rnd(Close[AZ] - (manualRetraceTicks * TickSize));
				
				if (autoRetraceEnabled_1)
					longEntryOrders[1] = SubmitOrder(tbip,OrderAction.Buy,OrderType.Limit,GetPositionSize(+1, CurrentBar-AZ, 1),auto1Price,auto1Price,GetAtmStrategyUniqueId(),"LAE1");
				if (autoRetraceEnabled_2)
					longEntryOrders[2] = SubmitOrder(tbip,OrderAction.Buy,OrderType.Limit,GetPositionSize(+1, CurrentBar-AZ, 2),auto2Price,auto2Price,GetAtmStrategyUniqueId(),"LAE2");
				if (autoRetraceEnabled_3)
					longEntryOrders[3] = SubmitOrder(tbip,OrderAction.Buy,OrderType.Limit,GetPositionSize(+1, CurrentBar-AZ, 3),auto3Price,auto3Price,GetAtmStrategyUniqueId(),"LAE3");
				if (manualRetraceEnabled)
					longEntryOrders[0] = SubmitOrder(tbip,OrderAction.Buy,OrderType.Limit,GetPositionSize(+1, CurrentBar-AZ, 4),manualPrice,manualPrice,GetAtmStrategyUniqueId(),"LME");				
			}
			if (nowBarSign < 0 && allow_ShortEntries)
			{
				// SHORTS
				double auto1Price = rnd(Close[AZ] + (0.01*autoRetracePercent_1 * body));
				double auto2Price = rnd(Close[AZ] + (0.01*autoRetracePercent_2 * body));
				double auto3Price = rnd(Close[AZ] + (0.01*autoRetracePercent_3 * body));	
				double manualPrice = rnd(Close[AZ] + (manualRetraceTicks * TickSize));
				
				if (autoRetraceEnabled_1)
					shortEntryOrders[1] = SubmitOrder(tbip,OrderAction.SellShort,OrderType.Limit,GetPositionSize(-1, CurrentBar-AZ, 1),auto1Price,auto1Price,GetAtmStrategyUniqueId(),"SAE1");
				if (autoRetraceEnabled_2)
					shortEntryOrders[2] = SubmitOrder(tbip,OrderAction.SellShort,OrderType.Limit,GetPositionSize(-1, CurrentBar-AZ, 2),auto2Price,auto2Price,GetAtmStrategyUniqueId(),"SAE2");
				if (autoRetraceEnabled_3)
					shortEntryOrders[3] = SubmitOrder(tbip,OrderAction.SellShort,OrderType.Limit,GetPositionSize(-1, CurrentBar-AZ, 3),auto3Price,auto3Price,GetAtmStrategyUniqueId(),"SAE3");
				if (manualRetraceEnabled)
					shortEntryOrders[0] = SubmitOrder(tbip,OrderAction.SellShort,OrderType.Limit,GetPositionSize(-1, CurrentBar-AZ, 4),manualPrice,manualPrice,GetAtmStrategyUniqueId(),"SME");				
			}
		}
		
		private int SeekStandardEntry()
		{
			// look back x bars to see if they all closed in the same direction, and this latest bar is opposite to that
			// and is within the required range
			int nowBarSign = Math.Sign(Close[AZ] - Open[AZ]);
			if (nowBarSign == 0)
				return 0;	// just quit; not sensible
			
			for (int a = AZ+1; a<AZ+1+barsInOneDirectionRequired; a++)
			{
				if (Math.Sign(Close[a] - Open[a]) == nowBarSign)
					return 0;	// just quit, insufficient bars in a row detected
			}
			
			// got a signal
			return nowBarSign;
		}
		
		private int SeekSandwichEntry()
		{
			// a bar that is the same as 2 bars ago, with the 1 bar ago being different (the sandwich)
			int nowBarSign = 	Math.Sign(Close[AZ] 	- Open[AZ]);
			int prev1BarSign = 	Math.Sign(Close[AZ+1] 	- Open[AZ+1]);
			int prev2BarSign = 	Math.Sign(Close[AZ+2] 	- Open[AZ+2]);
			
			if (	nowBarSign == prev2BarSign
				&&	nowBarSign != 0
				&&	nowBarSign == - prev1BarSign
				)
				return nowBarSign;	// got a signal
			
			return 0; // fail
		}
		
		#endregion
		
		#region Order management
		
		protected override void OnTermination()
		{
			CancelAllOrders(true,true);
			Position.Close();
			base.OnTermination();
		}
		
		protected override void OnPositionUpdate(IPosition pos)
		{
			// try and avoid an ExitOnClose bug
			if (	pos != null && pos.MarketPosition == MarketPosition.Flat 
				&& ExitOnClose
				&& (	Performance.AllTrades.Count > 0
					&&	Performance.AllTrades[Performance.AllTrades.Count-1].Exit.Name == "Exit on close"
					)
				)
			{
				// ensure all cancelled
				Print(SmartTime+" Cancelling all orders at Position.Flat after an ExitOnClose");
				//CancelAllOrders(true,true);
				CancelEveryOrder();
				waitUntilNewSession = true;
			}
			
			// show a DailyPnLMessage once closed
			if (drawADailyPnLMessage && pos.MarketPosition == MarketPosition.Flat)
			{
				Print(Now+" Showing DailyPnLMessage");
				drawADailyPnLMessage = false;
				ShowDailyPnLMessage(); // show a message this time
			}
		}
		
		protected override void OnExecution(IExecution e)
		{
			// set stops and targets on an entry execution
			for (int a = 0; a<longEntryOrders.Length; a++)
			{
				if (longEntryOrders[a] != null && e.Order == longEntryOrders[a])	
					SetOrAdjustStopsAndTargets(ref longEntryOrders[a],a,MarketPosition.Long);
			}
			for (int a = 0; a<shortEntryOrders.Length; a++)
			{
				if (shortEntryOrders[a] != null && e.Order == shortEntryOrders[a])	
					SetOrAdjustStopsAndTargets(ref shortEntryOrders[a],a,MarketPosition.Short);
			}
		}
		
		private void SetOrAdjustStopsAndTargets(ref IOrder o, int idx, MarketPosition dir)
		{
			string oco = GetAtmStrategyUniqueId();
			if (dir == MarketPosition.Long)
			{
				double targetPrice = targetMode == ModeTypeTarget.Auto ? 
						rnd(Close[CurrentBar-latestTriggerBar] + (autoTargetTicks*TickSize))
					:	(targetMode == ModeTypeTarget.Manual ? rnd(o.AvgFillPrice + (manualTargetTicks * TickSize))
					: 0);
				targetPrice = targetPrice > 0 ? Math.Max(targetPrice,rnd(o.AvgFillPrice + (minimumExitTicks*TickSize))) : 0;
				
				double stopLossPrice = stopLossMode == ModeType.Auto ?
						rnd(Low[CurrentBar-latestTriggerBar] - (autoStoplossTicks*TickSize))
					:	rnd(o.AvgFillPrice - (manualStoplossTicks * TickSize));
				stopLossPrice = Math.Min(stopLossPrice,rnd(o.AvgFillPrice - (minimumExitTicks*TickSize)));
				
				
				// target
				if (OrderIsActive(longTargetOrders[idx]) && targetPrice > 0)
				{
					Print(SmartTime+" updating target order "+o.Name);
					ChangeOrder(longTargetOrders[idx],o.Filled,targetPrice,targetPrice);
					oco = longTargetOrders[idx].Oco;
				}
				else if (targetPrice > 0)
				{
					string name = o.Name.Replace("E","")+"Target"+(stopLossMode == ModeType.Auto ? "A":"M");
					Print(SmartTime+" creating target order "+name);
					longTargetOrders[idx] = SubmitOrder(tbip,OrderAction.Sell,OrderType.Limit,o.Filled,targetPrice,targetPrice,oco,name);
				}
				
				// stoploss
				if (OrderIsActive(longStopLossOrders[idx]))
				{
					Print(SmartTime+" updating stoploss order "+o.Name);
					ChangeOrder(longStopLossOrders[idx],o.Filled,stopLossPrice,stopLossPrice);
				}
				else
				{
					string name = o.Name.Replace("E","")+"Stop"+(stopLossMode == ModeType.Auto ? "A":"M");
					Print(SmartTime+" creating stoploss order "+name);
					longStopLossOrders[idx] = SubmitOrder(tbip,OrderAction.Sell,OrderType.Stop,o.Filled,stopLossPrice,stopLossPrice,oco,name);
				}
			}
			
			if (dir == MarketPosition.Short)
			{
				double targetPrice = targetMode == ModeTypeTarget.Auto ? 
						rnd(Close[CurrentBar-latestTriggerBar] - (autoTargetTicks*TickSize))
					:	(targetMode == ModeTypeTarget.Manual ? rnd(o.AvgFillPrice - (manualTargetTicks * TickSize))
					: 0 );
				targetPrice = Math.Min(targetPrice,rnd(o.AvgFillPrice - (minimumExitTicks*TickSize)));
				
				double stopLossPrice = stopLossMode == ModeType.Auto ?
						rnd(High[CurrentBar-latestTriggerBar] + (autoStoplossTicks*TickSize))
					:	rnd(o.AvgFillPrice + (manualStoplossTicks * TickSize));
				stopLossPrice = Math.Max(stopLossPrice,rnd(o.AvgFillPrice + (minimumExitTicks*TickSize)));
				
				
				// target
				if (OrderIsActive(shortTargetOrders[idx]) && targetPrice > 0)
				{
					Print(SmartTime+" updating target order "+o.Name);
					ChangeOrder(shortTargetOrders[idx],o.Filled,targetPrice,targetPrice);
					oco = shortTargetOrders[idx].Oco;
				}
				else if (targetPrice > 0)
				{
					string name = o.Name.Replace("E","")+"Target"+(stopLossMode == ModeType.Auto ? "A":"M");
					Print(SmartTime+" creating target order "+name);
					shortTargetOrders[idx] = SubmitOrder(tbip,OrderAction.BuyToCover,OrderType.Limit,o.Filled,targetPrice,targetPrice,oco,name);
				}
				
				// stoploss
				if (OrderIsActive(shortStopLossOrders[idx]))
				{
					Print(SmartTime+" updating stoploss order "+o.Name);
					ChangeOrder(shortStopLossOrders[idx],o.Filled,stopLossPrice,stopLossPrice);
				}
				else
				{
					string name = o.Name.Replace("E","")+"Stop"+(stopLossMode == ModeType.Auto ? "A":"M");
					Print(SmartTime+" creating stoploss order "+name);
					shortStopLossOrders[idx] = SubmitOrder(tbip,OrderAction.BuyToCover,OrderType.Stop,o.Filled,stopLossPrice,stopLossPrice,oco,name);
				}
			}
		}
		
		private bool OrderIsActive(IOrder o)
		{
			if (	o == null
				|| 	o.OrderState == OrderState.Cancelled
				||	o.OrderState == OrderState.Filled
				||	o.OrderState == OrderState.Rejected
				||	o.OrderState == OrderState.PendingCancel
				)
				return false;
			else
				return true;			
		}
		
		private bool AllEntryOrdersInactive()
		{
			for (int a = 0; a<longEntryOrders.Length; a++)
			{
				if (OrderIsActive(longEntryOrders[a]))
					return false;
			}
			for (int a = 0; a<shortEntryOrders.Length; a++)
			{
				if (OrderIsActive(shortEntryOrders[a]))
					return false;
			}			
			
			return true;
		}
		
		/// <summary>
		/// returns TRUE if something had to be cancelled
		/// </summary>
		/// <returns></returns>
		private bool CancelEntryOrdersWhenNeeded()
		{
			
			bool tookAction = false;
			if (latestTriggerBar <= CurrentBar -AZ- barsUntilCancelOrders)
			{
				if (latestTriggerBar == CurrentBar -AZ- barsUntilCancelOrders)
					Print(SmartTime+" doing CancelEntryOrdersWhenNeeded(), latestTriggerBar="+latestTriggerBar+", CurrentBar-AZ="+(CurrentBar-AZ)+", barsUntilCancelOrders="+barsUntilCancelOrders);
			
				for (int a = 0; a<longEntryOrders.Length; a++)
				{
					if (OrderIsActive(longEntryOrders[a]))
					{
						tookAction = true;
						Print("\t"+SmartTime+" cancelling unfilled entry order "+longEntryOrders[a].Name);
						CancelOrder(longEntryOrders[a]);
					}
				}
				for (int a = 0; a<shortEntryOrders.Length; a++)
				{
					if (OrderIsActive(shortEntryOrders[a]))
					{
						tookAction = true;
						Print("\t"+SmartTime+" cancelling unfilled entry order "+shortEntryOrders[a].Name);
						CancelOrder(shortEntryOrders[a]);
					}
				}
			}
			
			return tookAction;
		}
		
		/// <summary>
		/// returns TRUE if something had to be cancelled
		/// </summary>
		/// <returns></returns>
		private bool CancelEntryOrders()
		{
			
			bool tookAction = false;
			
			{
				
				for (int a = 0; a<longEntryOrders.Length; a++)
				{
					if (OrderIsActive(longEntryOrders[a]))
					{
						tookAction = true;
						Print("\t"+SmartTime+" cancelling unfilled entry order "+longEntryOrders[a].Name);
						CancelOrder(longEntryOrders[a]);
					}
				}
				for (int a = 0; a<shortEntryOrders.Length; a++)
				{
					if (OrderIsActive(shortEntryOrders[a]))
					{
						tookAction = true;
						Print("\t"+SmartTime+" cancelling unfilled entry order "+shortEntryOrders[a].Name);
						CancelOrder(shortEntryOrders[a]);
					}
				}
			}
			
			return tookAction;
		}
		
		protected override void OnOrderUpdate(IOrder o)
		{
			if (o != null && (!o.OrderState.ToString().Contains("Pending")))
				Print("OnOrderUpdate(): "+SmartTime+" "+this.Name+" "+BarsPeriods[0].ToString()+" "+o.ToString());			
		}
		
		private void CancelEveryOrder()
		{
			foreach(IOrder o in longEntryOrders)
				if (OrderIsActive(o))
					CancelOrder(o);
				
			foreach(IOrder o in shortEntryOrders)
				if (OrderIsActive(o))
					CancelOrder(o);
					
			foreach(IOrder o in longTargetOrders)
				if (OrderIsActive(o))
					CancelOrder(o);
				
			foreach(IOrder o in shortTargetOrders)
				if (OrderIsActive(o))
					CancelOrder(o);
			
			foreach(IOrder o in longStopLossOrders)
				if (OrderIsActive(o))
					CancelOrder(o);
				
			foreach(IOrder o in shortStopLossOrders)
				if (OrderIsActive(o))
					CancelOrder(o);					
		}
		
		#endregion
		
		#region trade management (exits, order resizing after placement)
		
		
		
		/// <summary>
		///  returns True if took action to exit position (and cancelled all orders as well)
		/// </summary>
		/// <returns></returns>
		private bool TradeManagement()
		{
			// adjust order sizes if we are using unfilledSizing
			if (unfilledSizing 
				&& !freshCancelOfOrders 
				&& !exitDone
				&& FirstTickOfBar
				)
				AdjustUnfilledOrdersSizing();
			
			
			if (Position.MarketPosition == MarketPosition.Flat)
				return false;
			
			
			if (	barColorChangeExit//targetMode == ModeTypeTarget.BarColorChange
				&&	FirstTickOfBar 
				&&  realtimeBar != CurrentBar && (!(myHistorical && CurrentBar == Count-1))
				)
			{
				// look back x bars to see if they all closed in the same direction, and this latest bar is opposite to that
				// and is within the required range
				int nowBarSign = Math.Sign(Close[AZ] - Open[AZ]);
				bool ccexit = true;
				if (nowBarSign != 0)
				{				
					// the following changed by client request for version 2, Feb 2013
					if (!barColorChangeExitAlways)
					{
						for (int a = AZ+1; a<AZ+1+barsInOneDirectionRequired; a++)
						{
							if (Math.Sign(Close[a] - Open[a]) == nowBarSign)
								ccexit = false; // insufficient bars in a row detected
						}
					}
				}
				//Print(Time[AZ]+" ccexit="+ccexit+" barSign="+nowBarSign+" position is "+Position.MarketPosition); // debugging
				
				if ( ccexit && Position.MarketPosition == MarketPosition.Long && nowBarSign < 0)
				{
					Print(Time[AZ]+" exiting Long at bar color change");
					CancelAllOrders(false,true);
					exitOrder = SubmitOrder(tbip,OrderAction.Sell,OrderType.Market,Position.Quantity,0,0,GetAtmStrategyUniqueId(),"ColorChange");
					return true;
				}
				else if (ccexit && Position.MarketPosition == MarketPosition.Short && nowBarSign > 0)
				{
					Print(Time[AZ]+" exiting Short at bar color change");
					CancelAllOrders(false,true);
					exitOrder = SubmitOrder(tbip,OrderAction.BuyToCover,OrderType.Market,Position.Quantity,0,0,GetAtmStrategyUniqueId(),"ColorChange");
					return true;
				}				
				
			}
			
		
			// do an exit if daily profit targets or daily stoploss says NO TRADING)
			if (	Position.MarketPosition == MarketPosition.Long 
				&&	noTrading
				)
			{
				Print(Now+" exiting Long position from daily target/stoploss");
				CancelAllOrders(false,true);
				exitOrder = SubmitOrder(tbip,OrderAction.Sell,OrderType.Market,Position.Quantity,0,0,GetAtmStrategyUniqueId(),"DailyPnL");
				drawADailyPnLMessage = true;
				return true;
			}
			if (	Position.MarketPosition == MarketPosition.Short 
				&&	noTrading
				)
			{
				Print(Now+" exiting Short position from daily target/stoploss");
				CancelAllOrders(false,true);
				exitOrder = SubmitOrder(tbip,OrderAction.BuyToCover,OrderType.Market,Position.Quantity,0,0,GetAtmStrategyUniqueId(),"DailyPnL");
				drawADailyPnLMessage = true;
				return true;
			}
			
			// Theoretical Break Bar trailing stop
			if (breakBarTrailingStop && FirstTickOfBar)
			{
				bool notValid = false;
				if (Position.MarketPosition == MarketPosition.Long)
				{
					for (int a = AZ; a<AZ+breakBarTrailingStopBars; a++)
					{
						if (Close[a] < Open[a])
							notValid = true;
					}
					if (!notValid)
					{
						double bb = rnd(MIN(Low,breakBarTrailingStopBars)[AZ]);
						foreach (IOrder o in longStopLossOrders)
						{
							if (	OrderIsActive(o)	
								&&	rnd(o.StopPrice) < bb
								&&	bb < GetCurrentBid()-TickSize
								)
							{
								Print(Time[AZ]+" BreakBarTrailingStop : moving stoploss in order "+o.Name+" from "+o.StopPrice+" to "+bb);
								ChangeOrder(o,o.Quantity,o.LimitPrice,bb);
							}
						}
					}
				}
				if (Position.MarketPosition == MarketPosition.Short)
				{
					for (int a = AZ; a<AZ+breakBarTrailingStopBars; a++)
					{
						if (Close[a] > Open[a])
							notValid = true;
					}
					if (!notValid)
					{
						double bb = rnd(MAX(High,breakBarTrailingStopBars)[AZ]);
						foreach (IOrder o in shortStopLossOrders)
						{
							if (	OrderIsActive(o)	
								&&	rnd(o.StopPrice) > bb
								&&	bb > GetCurrentAsk()+TickSize
								)
							{
								Print(Time[AZ]+" BreakBarTrailingStop : moving stoploss in order "+o.Name+" from "+o.StopPrice+" to "+bb);
								ChangeOrder(o,o.Quantity,o.LimitPrice,bb);
							}
						}
					}
				}
			
			}
			
			return false;
		}
		
		private bool GetFlat(string reason)
		{
			if (Position.MarketPosition == MarketPosition.Long)
			{
				Print(Time[AZ]+" exiting Long :"+reason);
				CancelAllOrders(false,true);
				exitOrder = SubmitOrder(tbip,OrderAction.Sell,OrderType.Market,Position.Quantity,0,0,GetAtmStrategyUniqueId(),reason);
				return true;
			}
			else if (Position.MarketPosition == MarketPosition.Short)
			{
				Print(Time[AZ]+" exiting Short : "+reason);
				CancelAllOrders(false,true);
				exitOrder = SubmitOrder(tbip,OrderAction.BuyToCover,OrderType.Market,Position.Quantity,0,0,GetAtmStrategyUniqueId(),reason);
				return true;
			}	
			return false;
		}
		
		#endregion
		
		#region Times, extra supert, misc
		
		private void DoTradePlots()
		{
			for (int a = 0; a<=3; a++)
			{
				if (OrderIsActive(longStopLossOrders[a])	)
					tradePlots.Values[0+a].Set(longStopLossOrders[a].StopPrice);
				if (OrderIsActive(shortStopLossOrders[a])	)
					tradePlots.Values[0+a].Set(shortStopLossOrders[a].StopPrice);
				
				if (OrderIsActive(longTargetOrders[a])	)
					tradePlots.Values[4+a].Set(longTargetOrders[a].LimitPrice);
				if (OrderIsActive(shortTargetOrders[a])	)
					tradePlots.Values[4+a].Set(shortTargetOrders[a].LimitPrice);
			}
		}
		
		private double rnd(double price)
		{
			return Instrument.MasterInstrument.Round2TickSize(price);
		}
		
		protected DateTime Now
		{
			get {
				DateTime now;
				if (Bars != null && Bars.MarketData != null &&
						Bars.MarketData.Connection.Options.Provider == Cbi.Provider.Replay) {
					now = Bars.MarketData.Connection.Now;
				}
				else {
					if (!myHistorical)
						now = DateTime.Now;
					else
						now = Times[tbip][0];
				}
				return now;
			}
		}
		
		protected DateTime SmartTime
		{
			get {
					DateTime now;
					if (myHistorical)
						now = Time[0];
					else
						now = Now;
				
				return now;
			}
		}
		
		/// <summary>
		/// True = In Session, False = Out Of Session, this method checks the open and closing times (USER INPUT) for the session.
		/// </summary>
		/// <param name="tradingSessionOpen"></param>
		/// <param name="tradingSessionClose"></param>
		/// <returns></returns>
		private bool CheckTime(int tradingSessionOpen, int tradingSessionClose)
		{
			
			int lOpen =  tradingSessionOpen * 100;
			int lClose = tradingSessionClose * 100;
			
			bool isOutOfSession = false;		// initialise to false

			if (lOpen == lClose)
				return true;	// nothing to do if both times the same, is always in session.
			
			if (lOpen<lClose)
			{	// a "normal" session, that does not span midnight

				if ( (ToTime(Time[0]) > lOpen) && (ToTime(Time[0]) <= lClose)
					//&& (Time[0].DayOfWeek !=DayOfWeek.Saturday) && (Time[0].DayOfWeek != DayOfWeek.Sunday) )
					)
				{
					// It is a valid session time, do stuff here if required
				}
				else
				{
					isOutOfSession = true;
				}
			}
			else
			{	// a global session which DOES span midnight.

				if ( ((ToTime(Time[0]) > lOpen) || (ToTime(Time[0]) <= lClose))
					//&& (Time[0].DayOfWeek !=DayOfWeek.Saturday) && (Time[0].DayOfWeek != DayOfWeek.Sunday) )
					)
				{
					// it is a valid session time, do stuff here if required
				}
				else
				{
					isOutOfSession = true;
				}
				
			}
			
			return !isOutOfSession; 

		}
		
		// **************************************************************************************************************
		
		private void PlotExtraSuperTrends()
		{
			int p = 0;
			supertValues.Initialize();
			
			if (e1bip > 0)
			{
				if (supertExtra1.UpTrend.ContainsValue(0))
				{
					int dir = Math.Sign(Closes[e1bip][0] - supertExtra1.StopLine[0]);
					Color col = dir > 0 ? upColor : downColor;
					supertPlots.Values[p].Set(p+1);
					supertPlots.PlotColors[p][0] = col;
					supertValues[p] = dir > 0 ? +1 : -1;
				}
			}
			else
				supertPlots.Values[p].Reset();
			p++;
			
			if (e2bip > 0)
			{
				if (supertExtra2.UpTrend.ContainsValue(0))
				{
					int dir = Math.Sign(Closes[e2bip][0] - supertExtra2.StopLine[0]);
					Color col = dir > 0 ? upColor : downColor;
					supertPlots.Values[p].Set(p+1);
					supertPlots.PlotColors[p][0] = col;
					supertValues[p] = dir > 0 ? +1 : -1;
				}
			}
			else
				supertPlots.Values[p].Reset();
			p++;
			
			if (e3bip > 0)
			{
				if (supertExtra3.UpTrend.ContainsValue(0))
				{
					int dir = Math.Sign(Closes[e3bip][0] - supertExtra3.StopLine[0]);
					Color col = dir > 0 ? upColor : downColor;
					supertPlots.Values[p].Set(p+1);
					supertPlots.PlotColors[p][0] = col;
					supertValues[p] = dir > 0 ? +1 : -1;
				}
			}
			else
				supertPlots.Values[p].Reset();
			p++;
			
			if (e4bip > 0)
			{
				if (supertExtra4.UpTrend.ContainsValue(0))
				{
					int dir = Math.Sign(Closes[e4bip][0] - supertExtra4.StopLine[0]);
					Color col = dir > 0 ? upColor : downColor;
					supertPlots.Values[p].Set(p+1);
					supertPlots.PlotColors[p][0] = col;
					supertValues[p] = dir > 0 ? +1 : -1;
				}
			}
			else
				supertPlots.Values[p].Reset();
			p++;
			
			
		}
		
		#endregion
		
		#region Position Sizing
		
		private int GetPositionSize(int direction, int signalBar, int entryIndex)
		{
			int size = 1; // start with default
			
			int barsSinceSignal = CurrentBar - AZ - signalBar;
			int leadUpCount = 0;
			int p = CurrentBar - signalBar + 1;
			while (	p < CurrentBar 
					&& Math.Sign(Close[p] - Open[p]) == -direction)
			{
				leadUpCount++;
				p++;
			}
			
			
			// start with the fixed
			switch (entryIndex) {
				case 1:
					size = fixedSizeAuto1;
					break;
				case 2:
					size = fixedSizeAuto2;
					break;
				case 3:
					size = fixedSizeAuto3;
					break;
				case 4:
					size = fixedSizeManual;
					break;
				default:
					
					break;
			}
			
			// leadup sizing (BarsInOneDirection
			if (leadupSizing)
			{
				size = leadupSizes[leadupSizes.Length-1]; // start with the open-ended size
				for (int a = leadupBars.Length-1; a>=0; a--)
				{
					if (	leadUpCount <= 	leadupBars[a] 
						&& leadupSizes.Length > a
						)
						size = leadupSizes[a];
				}
				Print(Time[AZ]+" Calculating position size for order "+entryIndex+" with leadup bars counted as "+leadUpCount+" to produce size of "+size);
			}
			
			// unfilled sizing
			if (unfilledSizing)
			{
				for (int a = 0; a<unfilledBars.Length; a++)
				{
					if (barsSinceSignal >= 	unfilledBars[a] 
						&& unfilledSizes.Length > a
						)
						size = unfilledSizes[a];
				}
				Print(Time[AZ]+" Calculating position size for order "+entryIndex+" with unfilled bars counted as "+barsSinceSignal+" to produce size of "+size);
			}
			
			return size;
		}
		
		private void AdjustUnfilledOrdersSizing()
		{
			for (int a = 0; a<longEntryOrders.Length; a++)
			{
				if (	OrderIsActive(longEntryOrders[a])
					&&	longEntryOrders[a].Filled == 0
					)
				{
					int s = GetPositionSize(+1, latestTriggerBar, a+1);
					if (s != longEntryOrders[a].Quantity)
					{
						Print(Time[AZ]+" UnfilledSizing: adjusting position size on order "+longEntryOrders[a].Name+" from "+longEntryOrders[a].Quantity+" to "+s);
						ChangeOrder(longEntryOrders[a],s,longEntryOrders[a].LimitPrice,longEntryOrders[a].StopPrice);
					}
				}	
			}
			for (int a = 0; a<shortEntryOrders.Length; a++)
			{
				if (	OrderIsActive(shortEntryOrders[a])
					&&	shortEntryOrders[a].Filled == 0
					)
				{
					int s = GetPositionSize(-1, latestTriggerBar, a+1);
					if (s != shortEntryOrders[a].Quantity)
					{
						Print(Time[AZ]+" UnfilledSizing: adjusting position size on order "+shortEntryOrders[a].Name+" from "+shortEntryOrders[a].Quantity+" to "+s);
						ChangeOrder(shortEntryOrders[a],s,shortEntryOrders[a].LimitPrice,shortEntryOrders[a].StopPrice);
					}
				}	
			}
		}
		
		#endregion
		
        #region Properties
		
		#region position sizing
		/*// complex position sizing
		private   = SizeType.Fixed;
		private bool 	 decreaseWhileUnfilled = false;
		private int 	fixedSizeAuto1 = 1;
		private int		fixedSizeAuto2 = 1;
		private int		fixedSizeAuto3 = 1;
		private int		fixedSizeManual = 1;
		private int[]	leadupBars = new int{}[3,5,8];
		private int[]	leadupSizes= new int{}[1,2,3];
		private int[]	decreaseBars = new int{}[2,3,4];
		private int[]	decreaseSizes = new int{}[3,2,1];*/
		[Description("Basic fixed size for position")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\t\t\tFixedSize Auto1")]
        public int FixedSizeAuto1
        {
            get { return fixedSizeAuto1; }
            set { fixedSizeAuto1 = Math.Max(1,value); }
        }
		
		[Description("Basic fixed size for position")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\t\t\tFixedSize Auto2")]
        public int FixedSizeAuto2
        {
            get { return fixedSizeAuto2; }
            set { fixedSizeAuto2 = Math.Max(1,value); }
        }
		
		[Description("Basic fixed size for position")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\t\t\tFixedSize Auto3")]
        public int FixedSizeAuto3
        {
            get { return fixedSizeAuto3; }
            set { fixedSizeAuto3 = Math.Max(1,value); }
        }
		
		[Description("Basic fixed size for position")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\t\t\tFixedSize Manual")]
        public int FixedSizeManual
        {
            get { return fixedSizeManual; }
            set { fixedSizeManual = Math.Max(1,value); }
        }
		
		[Description("Change the position size based on Bars In One Direction prior to signal")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\t\tLeadup Sizing Enabled")]
        public bool LeadupSizing
        {
            get { return leadupSizing; }
            set { leadupSizing = value; }
        }
		
		[Description("Change the position size based on UP TO THESE many Bars In One Direction prior to signal")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\tLeadup Sizing Bars")]
        public int[] LeadupBars
        {
            get { return leadupBars; }
            set { int[] temp=value; for (int a=0; a<temp.Length; a++) temp[a]=Math.Max(1,temp[a]); leadupBars = temp; }
        }
		
		[Description("Change the position size to THIS based on how many Bars In One Direction prior to signal")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\tLeadup Sizing Sizes")]
        public int[] LeadupSizes
        {
            get { return leadupSizes; }
            set { int[] temp=value; for (int a=0; a<temp.Length; a++) temp[a]=Math.Max(1,temp[a]); leadupSizes = temp; }
        }
		
		[Description("Change the position size based on bars completed while Unfilled since signal")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("\tUnfilled Sizing Enabled")]
        public bool UnfilledSizing
        {
            get { return unfilledSizing; }
            set { unfilledSizing = value; }
        }
		
		[Description("Change the position size based on THIS OR MORE bars completed while Unfilled since signal")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("Unfilled Sizing Bars")]
        public int[] UnfilledBars
        {
            get { return unfilledBars; }
            set { int[] temp=value; for (int a=0; a<temp.Length; a++) temp[a]=Math.Max(1,temp[a]); unfilledBars = temp; }
        }
		
		[Description("Change the position size to THIS based on bars completed while Unfilled since signal")]
        [GridCategory("Position Sizing")]
		[Gui.Design.DisplayName("Unfilled Sizing Sizes")]
        public int[] UnfilledSizes
        {
            get { return unfilledSizes; }
            set { int[] temp=value; for (int a=0; a<temp.Length; a++) temp[a]=Math.Max(1,temp[a]); unfilledSizes = temp; }
        }
		
		#endregion
		
		[Description("Take Standard (version 1) entries")]
        [GridCategory("Entries")]
		[Gui.Design.DisplayName("\t\tStandardEntries")]
        public bool StandardEntries
        {
            get { return standardEntries; }
            set { standardEntries = value; }
        }
		
		[Description("Take Sandwich entries")]
        [GridCategory("Entries")]
		[Gui.Design.DisplayName("\tSandwichEntries")]
        public bool SandwichEntries
        {
            get { return sandwichEntries; }
            set { sandwichEntries = value; }
        }
		
		
        [Description("Retracement of trigger bar for a Limit order")]
        [GridCategory("Entries")]
        public double AutoRetracePercent_1
        {
            get { return autoRetracePercent_1; }
            set { autoRetracePercent_1 = Math.Max(1, value); }
        }

        [Description("Retracement of trigger bar for a Limit order")]
        [GridCategory("Entries")]
        public double AutoRetracePercent_2
        {
            get { return autoRetracePercent_2; }
            set { autoRetracePercent_2 = Math.Max(1, value); }
        }

        [Description("Retracement of trigger bar for a Limit order")]
        [GridCategory("Entries")]
        public double AutoRetracePercent_3
        {
            get { return autoRetracePercent_3; }
            set { autoRetracePercent_3 = Math.Max(1, value); }
        }

        [Description("Retracement of trigger bar for a Limit order")]
        [GridCategory("Entries")]
        public int ManualRetraceTicks
        {
            get { return manualRetraceTicks; }
            set { manualRetraceTicks = Math.Max(0, value); }		// original delivery set to 1
        }

		[Description("Enable this entry?")]
        [GridCategory("Entries")]
        public bool AutoRetraceEnabled_1
        {
            get { return autoRetraceEnabled_1; }
            set { autoRetraceEnabled_1 = value; }
        }
		
		[Description("Enable this entry?")]
        [GridCategory("Entries")]
        public bool AutoRetraceEnabled_2
        {
            get { return autoRetraceEnabled_2; }
            set { autoRetraceEnabled_2 = value; }
        }
		
		[Description("Enable this entry?")]
        [GridCategory("Entries")]
        public bool AutoRetraceEnabled_3
        {
            get { return autoRetraceEnabled_3; }
            set { autoRetraceEnabled_3 = value; }
        }
		
		[Description("Enable this entry?")]
        [GridCategory("Entries")]
        public bool ManualRetraceEnabled
        {
            get { return manualRetraceEnabled; }
            set { manualRetraceEnabled = value; }
        }
		
		[Description("Minimum allowed range of trigger bar in TICKS")]
        [GridCategory("Entries")]
        public int TriggerBarMinimumTicks
        {
            get { return triggerBarMinimumTicks; }
            set { triggerBarMinimumTicks = value; }
        }
		
		[Description("Maximum allowed range of trigger bar in TICKS")]
        [GridCategory("Entries")]
        public int TriggerBarMaximumTicks
        {
            get { return triggerBarMaximumTicks; }
            set { triggerBarMaximumTicks = value; }
        }
		
		[Description("Minimum bars in one direction required for a setup")]
        [GridCategory("Entries")]
        public int BarsInOneDirectionRequired
        {
            get { return barsInOneDirectionRequired; }
            set { barsInOneDirectionRequired = Math.Max(1,value); }		// original delivery was set to 2
        }
		
		[Description("Bars that will pass before unfilled entry orders will be cancelled")]
        [GridCategory("Entries")]
        public int BarsUntilCancelOrders
        {
            get { return barsUntilCancelOrders; }
            set { barsUntilCancelOrders = Math.Max(1,value); }
        }
		
		[Description("Allow Long entries to be taken")]
        [GridCategory("Entries")]
		[Gui.Design.DisplayName("\t\t\tLong Entries")]
        public bool Allow_LongEntries
        {
            get { return allow_LongEntries; }
            set { allow_LongEntries = value; }
        }
		
		[Description("Allow Short entries to be taken")]
        [GridCategory("Entries")]
		[Gui.Design.DisplayName("\t\t\tShort Entries")]
        public bool Allow_ShortEntries
        {
            get { return allow_ShortEntries; }
            set { allow_ShortEntries = value; }
        }
		
        [Description("Automatic placement of target THIS many ticks beyond trigger bar Close")]
        [GridCategory("Stop & Target")]
        public int AutoTargetTicks
        {
            get { return autoTargetTicks; }
            set { autoTargetTicks = Math.Max(1, value); }
        }

        [Description("Target of THIS many ticks from entry")]
        [GridCategory("Stop & Target")]
        public int ManualTargetTicks
        {
            get { return manualTargetTicks; }
            set { manualTargetTicks = Math.Max(1, value); }
        }

        [Description("Automatic placement of target THIS many ticks beyond bar range")]
        [GridCategory("Stop & Target")]
        public int AutoStoplossTicks
        {
            get { return autoStoplossTicks; }
            set { autoStoplossTicks = Math.Max(1, value); }
        }

        [Description("Target of THIS many ticks behind entry")]
        [GridCategory("Stop & Target")]
        public int ManualStoplossTicks
        {
            get { return manualStoplossTicks; }
            set { manualStoplossTicks = Math.Max(1, value); }
        }
		
		[Description("Use an automatic or a fixed tick 'manual' mode")]
        [GridCategory("Stop & Target")]
        public ModeTypeTarget TargetMode
        {
            get { return targetMode; }
            set { targetMode = value; }
        }
				
		[Description("Use an automatic or a fixed tick 'manual' mode")]
        [GridCategory("Stop & Target")]
        public ModeType StopLossMode
        {
            get { return stopLossMode; }
            set { stopLossMode = value; }
        }
		
		[Description("Exit a trade if bar color changes to opposite")]
        [GridCategory("Stop & Target")]
        public bool	BarColorChangeExit
        {
            get { return barColorChangeExit; }
            set { barColorChangeExit = value; }
        }
		
		[Description("takes any color change bar, not just a signal bar (that has sufficient barsinOneDirection behind it)")]
        [GridCategory("Stop & Target")]
        public bool	BarColorChangeExitAlways
        {
            get { return barColorChangeExitAlways; }
            set { barColorChangeExitAlways = value; }
        }
		
		// --------------
		
		// -- daily targets
		
		[Description("$ of Profit Target per day. Trading will cease if achieved.")]
        [GridCategory("Daily Target/StopLoss")]
        public int ProfitTargetDaily
        {
            get { return profitTargetDaily; }
            set { profitTargetDaily = Math.Max(1, value); }
        }
		
		[Description("$ of StopLoss per day. Trading will cease if hit.")]
        [GridCategory("Daily Target/StopLoss")]
        public int StopLossDaily
        {
            get { return stopLossDaily; }
            set { stopLossDaily = Math.Max(1, value); }
        }
				
		[Description("Daily profit target and stop Enabled?")]
        [GridCategory("Daily Target/StopLoss")]
        public bool DailyStopAndTargetEnabled
        {
            get { return dailyStopAndTargetEnabled; }
            set { dailyStopAndTargetEnabled = value; }
        }
		
		// not implemented to not conflict with the high granularity DataSeries trading tbip
//		[Description("include unrealized profit and loss?")]
//        [GridCategory("Daily Target/StopLoss")]
//        public bool IncludeUnrealizedPnL
//        {
//            get { return includeUnrealizedPnL; }
//            set { includeUnrealizedPnL = value; }
//        }
		
		
		// --------------
		
		[Description("Begin time for trading time window")]
        [GridCategory("Trading Time")]
        public int BeginTime
        {
            get { return beginTime; }
            set { beginTime = Math.Max(0,Math.Min(2359,value)); }
        }
		
		[Description("End time for trading time window")]
        [GridCategory("Trading Time")]
        public int EndTime
        {
            get { return endTime; }
            set { endTime = Math.Max(0,Math.Min(2359,value)); }
        }
		
		// ------------
		
		[Description("Enable the BreakBar TrailingStop")]
        [GridCategory("Stop & Target")]
		[Gui.Design.DisplayName("BreakBar TrailingStop")]
        public bool BreakBarTrailingStop
        {
            get { return breakBarTrailingStop; }
            set { breakBarTrailingStop = value; }
        }
		
		[Description("Bars to use for the BreakBar TrailingStop")]
        [GridCategory("Stop & Target")]
		[Gui.Design.DisplayName("BreakBar Bars")]
        public int BreakBarTrailingStopBars
        {
            get { return breakBarTrailingStopBars; }
            set { breakBarTrailingStopBars = value; }
        }
		
		
		
		// -----------
		
		[Description("Highlight the trigger bars")]
        [GridCategory("Parameters")]
        public bool PaintTriggerBars
        {
            get { return paintTriggerBars; }
            set { paintTriggerBars = value; }
        }
		
//		[Description("quantity entered with each order")]
//        [GridCategory("Parameters")]
//        public int PositionSize
//        {
//            get { return positionSize; }
//            set { positionSize = Math.Max(1,value); }
//        }
		
		[Description("TICKS of minimum distance for a target or stoploss order from entry")]
        [GridCategory("Parameters")]
        public int MinimumExitTicks
        {
            get { return minimumExitTicks; }
            set { minimumExitTicks = Math.Max(2,value); }
        }
		
		#region Source indicator anaSuperTrendU11
		
		[Description("ENABLE the SuperTrend as an entry filter?")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("\t\tSuperTrendFilter ENABLED")]
        public bool SuperTrendFilter_Enabled
        {
            get { return superTrendFilter_Enabled; }
            set { superTrendFilter_Enabled = value; }
        }
		
		[Description("Median period")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("Baseline period")]
        public int BasePeriod
        {
            get { return basePeriod; }
            set { basePeriod = Math.Max(1, value); }
        }

		[Description("ATR period")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("Offset period")]
        public int RangePeriod
        {
            get { return rangePeriod; }
            set { rangePeriod = Math.Max(1, value); }
        }

       [Description("ATR multiplier")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("Offset multiplier")]
        public double Multiplier
        {
            get { return multiplier; }
            set { multiplier = Math.Max(0.0, value); }
        }
		
		/// <summary>
		/// </summary>
		[Description("Moving average type for baseline")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Baseline smoothing")]
		public anaSuperTrendU11BaseType ThisBaseType
		{
			get { return thisBaseType; }
			set { thisBaseType = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Moving average type for volatility estimator")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Offset smoothing")]
		public anaSuperTrendU11OffsetType ThisOffsetType
		{
			get { return thisOffsetType; }
			set { thisOffsetType = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Simple or True Range")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Offset type")]
		public anaSuperTrendU11VolaType ThisRangeType
		{
			get { return thisVolaType; }
			set { thisVolaType = value; }
		}

		[Description("Reverse intra-bar")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName ("Reverse intra-bar")]
        public bool ReverseIntraBar
        {
            get { return reverseIntraBar; }
            set { reverseIntraBar = value; }
        }

		[Description("Show arrows when trendline is violated?")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName ("Show arrows")]
        public bool ShowArrows
        {
            get { return showArrows; }
            set { showArrows = value; }
        }

		[Description("Color the bars in the direction of the trend?")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName ("Show paintbars")]
        public bool ShowPaintBars
        {
            get { return showPaintBars; }
            set { showPaintBars = value; }
        }

		[Description("Show stop line")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName ("Show stop line")]
        public bool ShowStopLine
        {
            get { return showStopLine; }
            set { showStopLine = value; }
        }

		[Description("Sound alerts activated")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("Sound alert active")]
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
        }
		
		/// <summary>
		/// </summary>
        [XmlIgnore()]		
		[Description("Select color for uptrend")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color UpColorST
		{
			get { return upColorST; }
			set { upColorST = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSTSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColorST); }
			set { upColorST = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
        [XmlIgnore()]		
		[Description("Select color for downtrend")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color DownColorST
		{
			get { return downColorST; }
			set { downColorST = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DownColorSTSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColorST); }
			set { downColorST = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Width for stop dots.")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Width stop dots")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for stop dots.")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Plot style stop dots")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for stop dots.")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Dash style stop dots")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 
		
		/// <summary>
		/// </summary>
		[Description("Width for stop line.")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Width stop line")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("PlotStyle for stop line.")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Plot style stop line")]
		public PlotStyle Plot1Style
		{
			get { return plot1Style; }
			set { plot1Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for stop line.")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Dash style stop line")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		} 

		[Description("When paint bars are activated, this parameter sets the opacity of the upclose bars")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Upclose opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}
		
		[Description("Sound file for confirmed new uptrend")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("New uptrend")]
		public string ConfirmedUpTrend
		{
			get { return confirmedUpTrend; }
			set { confirmedUpTrend = value; }
		}
		
		[Description("Sound file for confirmed new downtrend")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("New downtrend")]
		public string ConfirmedDownTrend
		{
			get { return confirmedDownTrend; }
			set { confirmedDownTrend = value; }
		}
		
		[Description("Sound file for potential new uptrend")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Potential uptrend")]
		public string PotentialUpTrend
		{
			get { return potentialUpTrend; }
			set { potentialUpTrend = value; }
		}
		
		[Description("Sound file for potential new downtrend")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Potential downtrend")]
		public string PotentialDownTrend
		{
			get { return potentialDownTrend; }
			set { potentialDownTrend = value; }
		}
		
		[Description("Rearm time for alert in seconds")]
		[GridCategory("SuperTrend")]
		[Gui.Design.DisplayNameAttribute("Rearm time (sec)")]
		public int RearmTime
		{
			get { return rearmTime; }
			set { rearmTime = value; }
		}
		
		
		#endregion
		
		#region extra timeframes of SuperTrend
		

		
		[Description("Enable this timeframe/supertrend filter?")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\t\tExtra TimeFrame 1")]
		public bool ExtraTimeFrame1
		{
			get { return extraTimeFrame1; }
			set { extraTimeFrame1 = value; }
		}
		
		[Description("Enable this timeframe/supertrend filter?")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\tExtra TimeFrame 2")]
		public bool ExtraTimeFrame2
		{
			get { return extraTimeFrame2; }
			set { extraTimeFrame2 = value; }
		}
		
		[Description("Enable this timeframe/supertrend filter?")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\tExtra TimeFrame 3")]
		public bool ExtraTimeFrame3
		{
			get { return extraTimeFrame3; }
			set { extraTimeFrame3 = value; }
		}
		
		[Description("Enable this timeframe/supertrend filter?")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("Extra TimeFrame 4")]
		public bool ExtraTimeFrame4
		{
			get { return extraTimeFrame4; }
			set { extraTimeFrame4 = value; }
		}
		
		
		[Description("LineBreak PeriodType for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\t\tLineBreak PeriodType 1")]
		public LineBreakPeriodType 	ExtraPeriodType1
		{
			get { return extraPeriodType1; }
			set { extraPeriodType1 = value; }
		}
		
		[Description("LineBreak PeriodType for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\tLineBreak PeriodType 2")]
		public LineBreakPeriodType 	ExtraPeriodType2
		{
			get { return extraPeriodType2; }
			set { extraPeriodType2 = value; }
		}
		
		[Description("LineBreak PeriodType for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\tLineBreak PeriodType 3")]
		public LineBreakPeriodType 	ExtraPeriodType3
		{
			get { return extraPeriodType3; }
			set { extraPeriodType3 = value; }
		}
		
		[Description("LineBreak PeriodType for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("LineBreak PeriodType 4")]
		public LineBreakPeriodType 	ExtraPeriodType4
		{
			get { return extraPeriodType4; }
			set { extraPeriodType4 = value; }
		}
		
		
		[Description("LineBreak PeriodValue for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\t\tLineBreak PeriodValue 1")]
		public int 	ExtraPeriodValue1
		{
			get { return extraPeriodValue1; }
			set { extraPeriodValue1 = Math.Max(1,value); }
		}
		
		[Description("LineBreak PeriodValue for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\tLineBreak PeriodValue 2")]
		public int 	ExtraPeriodValue2
		{
			get { return extraPeriodValue2; }
			set { extraPeriodValue2 = Math.Max(1,value); }
		}
		
		[Description("LineBreak PeriodValue for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\tLineBreak PeriodValue 3")]
		public int 	ExtraPeriodValue3
		{
			get { return extraPeriodValue3; }
			set { extraPeriodValue3 = Math.Max(1,value); }
		}
		
		[Description("LineBreak PeriodValue for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("LineBreak PeriodValue 4")]
		public int 	ExtraPeriodValue4
		{
			get { return extraPeriodValue4; }
			set { extraPeriodValue4 = Math.Max(1,value); }
		}
		
		
		[Description("LineBreak Bars for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\t\tLineBreak Bars 1")]
		public int 	ExtraLineBreak1
		{
			get { return extraLineBreak1; }
			set { extraLineBreak1 = Math.Max(1,value); }
		}
		
		[Description("LineBreak Bars for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\t\tLineBreak Bars 2")]
		public int 	ExtraLineBreak2
		{
			get { return extraLineBreak2; }
			set { extraLineBreak2 = Math.Max(1,value); }
		}
		
		[Description("LineBreak Bars for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("\tLineBreak Bars 3")]
		public int 	ExtraLineBreak3
		{
			get { return extraLineBreak3; }
			set { extraLineBreak3 = Math.Max(1,value); }
		}
		
		[Description("LineBreak Bars for this supertrend")]
		[GridCategory("Extra SuperTrend TimeFrames")]
		[Gui.Design.DisplayNameAttribute("LineBreak Bars 4")]
		public int 	ExtraLineBreak4
		{
			get { return extraLineBreak4; }
			set { extraLineBreak4 = Math.Max(1,value); }
		}
		
		
		#endregion
		
        #endregion
    }
}
