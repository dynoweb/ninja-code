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
using System.IO;
using System.Collections;
using System.Windows.Forms;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    #region Revisions
	/*
	Revisor:	Rev date:	Zip version:			Changes:				
	[Ducman]	21Feb11:	Myst_FDAX_2Target		First release of Myst_FDAX_2Target.
	[Ducman]	27Feb11:	Myst_FDAX_2Target_1		Disappearing graph text problem corrected (thanks gulabv); Parameters adjusted to new version MFISMA2 (MFISMA2_1).
	[Ducman]	06Mar11:	Myst_FDAX_2Target_2		Changed to latest version PriceLineIndicator that works on both NT6.5 and NT7; Removed jtEconNews2 indicator;
													Display # of False Moves; Target P1 changed from 8 to 6 ticks to reduce number of double losses.
	[Ducman] 	07Mar11:	Myst_FDAX_2Target_2_1	Print section added to False Move; cancelBars default changed from 1 to 2.
	[Ducman]	09Mar11:	Myst_FDAX_2Target_3		Stop Loss enabled for contract 2 when at initial BE; Trail method contract 1 and 2 now the same.
	[Ducman]	19Mar11:	Myst_FDAX_2Target_3_1	Changed to CalcOnBarClose = False; Removed Backtesting regions; Fixed trail sound issue.
	[Ducman]	28Mar11:	Myst_FDAX_2Target_4		Dynamic trail bahavior corrected; Calculate todays P&L; Changed to TraceOrders = false;
													Added Write Report section (report location UserDataDir/tmp); Some small code clean up modifications.
	[Ducman]	03Apr11:	Myst_FDAX_2Target_5 	Calculate todays P&L corrected for after hour postions;	Sharkychop indicator added as chop killer.												
	[Ducman]	07Apr11:	Myst_FDAX_2Target_6		Sharkyhop lookback period parameter added, default = 25 bars; Multiple signals per bar disabled by adding FirstTickOfBar;
													Some small corrections made in line with FirstTickOfBar; 'Call Statusbox' added in P&L section
	[Ducman]	10Apr11:	Myst_FDAX_2Target_6_1	FirstTickOfBar approach removed again (not successful); Trailing secion moved to OnMarketData;
													Sharkychop lookback period default changed from 25 to 50;
	[Ducman]	20Apr11:	Myst_FDAX_2Target_6_2	Small correction in False move detection; Exclude Fridays from trading;
	[Ducman]	7Jul11:		Myst_FDAX_2Target_7 	Turn Position Condition added (not activated); Exit rules section added; FlattenAll section added; 
													P&L limit section improved; outsideBar added in entry rules (not activated); ToolBarButtons added; 
													oneTarget option added; Tick counter added in Status Box; Trailing section improved;
													Extra rule added in enter section; Lunchtime break added; Cleaned up some parameters from UI
	[Ducman]	12Jul11:	Myst_FDAX_2Target_7_1	Daily profit corrected; Cleaned up Trail StopLoss; Issue with Daily P&L reset solved; Sound link changed;
													Change Background color when strategy on halt; added sound when in entry signal = true (long.wav, short.wav)
	[Ducman]	23Sep11:	Myst_FDAX_2Target_7_2	Reduced trading time to first and last 1.5h; Included the Friday as trading day again.									
	[Ducman]	27Dec11:	Myst_FDAX_2Target_8		Profit 1 from 6 to 24; Stop loss from 16 to 72; BE Trigger from 6 to 20; BE Plus from -6 to -30; Exit Trigger from 24 to 20 
													ADXVMA/SMA Exit added; Tail Bar entry Exit added; Dynamic trail starts only when in winning position; 
													Modified Bar reversal Exit; Further optimized trading time; maxProfit changed from 2000 to 2500;
													trailLookBackBars changed from 3 to 4. Changes made Myst more aggressive without losing control!
	[Ducman]	15Jan12:	Myst_FDAX_2Target_8_1	Target and stop based on theoretical entry instead of fill price to eliminate slippage effects;
													Three trading periods created.
	[Ducman]	22Mar12:	Myst_FDAX_2Target_8_2	enterprice method changed; BE Plus from -30 to -24; cancelBars from 2 to 3
													Halt strategy until # bars after start session > aDXVMAperiod; insidebar outsidebar method removed;
													Timing halt rules changed slightly; Removed Turn Position section; Removed COBC sections;
	[Ducman]	31Mar12:	Myst_FDAX_2Target_8_3	Reversal bar Exit 2 removed; Reversal bar Exit 1 exit ticks changed from 2 to 3; All Print actions disabled; SL changed from 72 to 64.
	[Ducman]	12Apr12:	Myst_FDAX_2Target_8_4	Trend Reversal Exit removed (stpped out too many good trades).
	*/
	#endregion
	
	/// <summary>
	/// March 2011, by Ducman. 
	/// This strategy is started from the 'SampleAdvancedAutomatedStratv1.1' strategy (thanks 'dsraider').
	/// It's my first strategy and I called it Myst, the famous adventure game from the nineties.
	/// My goal is to create a strategy that rides the waves but avoids the false moves.
	/// Enter conditions are based on the trend indicator '_ADXVMA_Alerts_v01_5_1', the 'CCI' and a new indicator I called 'MFISMA2'.
	/// This 'MFISMA2' is my version of the Tick Volume Oscillator from Bill Blau. It's a moving average of the 'MFI(14)'.
	/// The simulated orders (located in OnMarketData) are created to prevent you from getting into false moves.
	/// The strategy is optimized to run with CalculateOnBarClose = FALSE. It's been optimized for FDAX on 6 BetterRenko timeframe.
	/// Since switching from MedianRenko to BetterRenko both NT6.5 and NT7 generate similar results.
	/// Although it performs quite well, suggestions for improvement are always welcome.
	/// </summary>
	
    [Description("My first strategy is one big adventure")]
    public class Myst_FDAX_2Target_8_4 : Strategy
    {
        #region Variables        	
		private bool	aDXVMA_ConservativeMode	= true;	// when true signal ADXVMA indicator can go neutral
		private bool	enterLong			= false;	// Variable used to enter position
		private bool	enterShort			= false;	// Variable used to enter position		
		private bool	noHistorical		= true; 	// Historical data included when false (used for Backtesting)
		private bool	writeToFile			= false; 	// Write results to text file
		private bool	dynamicTrail		= true;		// Variable to set dynamic trailing on or off		
		private bool 	tradeOnFriday		= true;		// Variable used to set trade on Fridays
		private bool   	manualTrading		= false;	// Variable to enable manual entries
		private bool   	oneTarget			= false;	// Variable to trade only 1 position
		private bool	trailBars			= true;		// Variable used to switch between trail bars and trail ticks
		private bool  	toolBarButtons		= false;	// Enable toolBarButtons
		private bool    strategyHalt 		= true;     // Variable used to check strategy status
						
		private int		breakEvenTicks		= 20;		// Default amount of Ticks to activate break even
		private int		breakEvenTicksPlus	= -24;		// Default offset to break even
		private int		cancelBars			= 3; 		// Amount of Bars to cancel order when not filled
		private int		maxprofit			= 2500;		// Max profit before halt strategy
		private int		maxloss				= 500; 		// Max loss before halt strategy
		private int 	profit1 			= 24; 		// Default setting for Profit1 (FDAX: 2 Tick = 1 point = €25)
		private int 	profit2 			= 0; 		// Default setting for Profit2 (Runner when 0)
		private int 	stoploss 			= 64;		// Default setting for Stoploss
        private int 	enterTicks 			= 4; 		// Amount of Ticks from Close[0] to enter position
		private int		exitTrigger			= 20;		// Default value when to activate Exit section rules
		private int 	aDXVMAperiod 		= 14; 		// Default setting for aDXVMAperiod
		private int		trailLookBackBars	= 4;		// Number of Bars back to use for Trail Stop
		private int		submitOrderBar		= 0;		// Variable used to count bars after submit order
		// Original trading hours 
		/*
		private int		timeStart			= 090500;	// Start Trading Time		
		private int		breakStart1			= 093000;	// Break Start Trading Time
		private int		breakStop1			= 160000;	// Break Stop Trading Time
		private int		breakStart2			= 170000;	// Break Start Trading Time
		private int		breakStop2			= 170000;	// Break Stop Trading Time
		private int		timeStop			= 170000;	// Stop Trading Time
		private int 	timeEnd  			= 215500;	// Halt Strategy
		*/ // Adjusted for CST 
		private int		timeStart			= 020500;	// Start Trading Time		
		private int		breakStart1			= 023000;	// Break Start Trading Time
		private int		breakStop1			= 090000;	// Break Stop Trading Time
		private int		breakStart2			= 100000;	// Break Start Trading Time
		private int		breakStop2			= 100000;	// Break Stop Trading Time
		private int		timeStop			= 100000;	// Stop Trading Time
		private int 	timeEnd  			= 145500;	// Halt Strategy
		private int		cCIsignalLevel		= -200;		// The level at which the CCI is compared. (-200)agressive, (0)offensive, (100)conservative
		private int		cCIexitLevel		= 200;		// The level at which the CCI is considered overbought (200)agressive, (150)offensive, (100)conservative
		private int		mFIvolumeDelta		= 10;		// Delta Volume Filter (only enter volume supported signals)
		private int 	mFIperiod 			= 6; 		// Default setting for MFIperiod    
		private int 	mFI_SMAperiod 		= 3; 		// Default setting for mFI_SMAperiod
        private int	 	mFI_EMAperiod 		= 3; 		// Default setting for mFI_EMAperiod
        private int 	mFI_TEMAperiod 		= 6; 		// Default setting for TEMAperiod
        private int 	mFI_VMAperiod 		= 3; 		// Default setting for mFI_VMAperiod
		private int 	mFI_VMAvolatility 	= 6; 		// Default setting for mFI_VMAvolatility
		private int		mFIaverageType		= 3;		// Type of Average for MFISMA2. 1=SMA, 2=EMA 3=TEMA, 4=VMA
		private int		countFalseMoves		= 0;		// Variable to count the number of False Moves
		private int 	writeLineType		= 0; 		// Variable to determine what line to write to report
		private int		sharkyChop_LookBack	= 50;		// Maximum number of lookback bars for Sharkychop indicator		
		private int 	lastPositionBar; 				// Variable to indicate last bar with postion	
		private int 	firstPositionBar; 				// Variable to indicate first possible bar with postion
				
		private double	initialBreakEven	= 0;		// Variable used to calculate initial break even
		private double 	previousPrice 		= 0;		// Variable used to capture previous price 
		private double 	newPrice 			= 0;		// Variable used to capture new price
		private double	stopLossPrice		= 0;		// Variable used to calculate stop loss price
		private double	signalprice			= 0;		// Variable used to capture signal price
		private double  enterprice			= 0;		// Variable used to define enter price		
		private double	vDelta				= 0;		// Variable used to calculate volume delta		
		private double	cumprofit			= 0;		// Variable used to calculate cumprofit of previous days
		private double	todaysPL			= 0;		// Variable used to calculate todays P&L
		private double	sharkyChop_Trigger	= 0.02;		// Default setting for SharkyChopTrigger  
		private double 	range_expansion		= 1.2;		// VolumeStop parameter
		
		private IOrder 	entryOrder1 		= null;
		private IOrder 	entryOrder2 		= null;
		private IOrder	limitPrice			= null;		
		
		// Parameters used for ToolBarButtons
		private System.Windows.Forms.ToolStrip strip 					= null;
		private System.Windows.Forms.Control[] controls    				= null;
		private System.Windows.Forms.ToolStripButton btnManual 			= null;
		private System.Windows.Forms.ToolStripButton btnLongEntry 		= null;
		private System.Windows.Forms.ToolStripButton btnShortEntry 		= null;
		private System.Windows.Forms.ToolStripButton btnManualLongEntry = null;
		private System.Windows.Forms.ToolStripButton btnManualShortEntry= null;
		private System.Windows.Forms.ToolStripButton btnFlatten		 	= null;
		private System.Windows.Forms.ToolStripSeparator sepL 			= null;
		private System.Windows.Forms.ToolStripSeparator sep1 			= null;
		private System.Windows.Forms.ToolStripSeparator sep2 			= null;
		private System.Windows.Forms.ToolStripSeparator sep3 			= null;
		private System.Windows.Forms.ToolStripSeparator sep4 			= null;
		private System.Windows.Forms.ToolStripSeparator sep5 			= null;
		private System.Windows.Forms.ToolStripSeparator sepR 			= null;
		
		private Font boldFont = new Font("Arial", 8,FontStyle.Bold);
		private Font regularFont = new Font("Arial", 8);
		private bool buttonsloaded = false;
		
		// Creates a StreamWriter and a StreamReader object
		private System.IO.StreamWriter sw;	
        #endregion		
		
        #region Initialize
		protected override void Initialize()
        {
          	Add(_ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode));			
			Add(CCI(14));
			Add(MFISMA2(mFI_EMAperiod, mFIaverageType, mFIperiod, mFI_SMAperiod, mFI_TEMAperiod, mFI_VMAperiod, mFI_VMAvolatility));
			Add(DH_HorizPriceLine());
			Add(Sharkchop_v01_1(13, 1, 1, 16));			
						
			#if NT7
				Enabled = true;		
			#endif
									
			EntriesPerDirection = 2; // Maximum number of entires per direction
			EntryHandling = EntryHandling.UniqueEntries; // Every entry is identified by a name
									
            CalculateOnBarClose = false; 			
			TraceOrders = false; // Will send info on orders method (stop, fill, cancel etc.) to Output window
			IncludeCommission = true;			
        }		
		#endregion	
		        
		#region OnBarUpdate
        protected override void OnBarUpdate()
        {								
			#region Trading Time and P&L
			if (Position.MarketPosition == MarketPosition.Flat) // Only calculate when position is flat
			{				
				if (Bars.FirstBarOfSession)
				{
					countFalseMoves = 0; // Reset countFalseMoves
					DrawTextFixed("False Moves", "# False Moves: " + countFalseMoves.ToString(), TextPosition.BottomRight);	
					cumprofit = Performance.AllTrades.TradesPerformance.Currency.CumProfit; // Calculate the cumprofit all strategy days
					todaysPL = Performance.AllTrades.TradesPerformance.Currency.CumProfit - cumprofit; // Reset todays P&L
				//	PrintWithTimeStamp("FirstBarOfSession!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				}	
				
				// Calculate todays P&L
				if (todaysPL != Performance.AllTrades.TradesPerformance.Currency.CumProfit - cumprofit)
				{
					todaysPL = Performance.AllTrades.TradesPerformance.Currency.CumProfit - cumprofit;
					writeLineType = 1;
					WriteTextToFile();
				}
				
				// Halts strategy when todays P&L exceeds limits
				if (!Bars.FirstBarOfSession && (todaysPL > maxprofit || todaysPL < - maxloss))				
				{						
				//	PrintWithTimeStamp("TODAYS P&L HAS BEEN REACHED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");					
					if (!strategyHalt) DrawText("P&L" + CurrentBar, "TODAYS P&L REACHED", -8, Median[0], Color.Black);
					// Call StatusBox
					StatusBox();
					enterLong = false;
					enterShort = false;
					strategyHalt = true;
					BackColorAll = Color.Gray;					
					return;
				}								
				
				// Checks to see if the day of the week is Friday. Exclude Fridays from trading
				if (Time[0].DayOfWeek == DayOfWeek.Friday && !tradeOnFriday) 
				{
					strategyHalt = true;
					BackColorAll = Color.Gray;
					return;
				}
				
				// Halts strategy when we are outside trading hours but finishes any running positions
				if ((ToTime(Time[0]) <= timeStart || ToTime(Time[0]) >= timeStop) 
					|| (ToTime(Time[0]) >= breakStart1 && ToTime(Time[0]) <= breakStop1)
					|| (ToTime(Time[0]) >= breakStart2 && ToTime(Time[0]) <= breakStop2)
					&& (!enterShort && !enterLong) // Do not halt when enter signal
					)	
				{
					// Call CancelAll() when open orders
					if ((entryOrder1 != null && entryOrder1.LimitPrice != null) || (entryOrder2 != null && entryOrder2.LimitPrice != null))
					{
						CancelAll();
					//	PrintWithTimeStamp("CancelAll HAS BEEN CALLED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					}				
								
					// Call StatusBox
					StatusBox();
					
				//	PrintWithTimeStamp("OUTSIDE STRATEGY TRADING HOURS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");					
					strategyHalt = true;
					BackColorAll = Color.Gray;
					return;
				}				
			}				
			
			else if (Position.MarketPosition != MarketPosition.Flat) // Only calculate when position is not flat
			{				
				// Halts strategy when we are at Market closure and closes all open positions
				if (ToTime(Time[0]) >= timeEnd)
				{
					// Close all open positions and cancel all working orders
					FlattenAll();		
				//	PrintWithTimeStamp("FlattenAll HAS BEEN CALLED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					return;
				}
			}	
			
			// Call StatusBox
			StatusBox();								
			#endregion
			
			#region Strategy Halt Conditions
			strategyHalt = false;		
			
			// When active it will not trade on historical  data
			if (Historical && noHistorical)	
			{
				strategyHalt = true;
				BackColorAll = Color.Gray;
				return; 
			}
			
			// Halt strategy until # bars is > aDXVMAperiod			
			if (Bars.BarsSinceSession < aDXVMAperiod) 
			{
				strategyHalt = true;
				BackColorAll = Color.Gray;
				return; 
			}		
			
			// Halt when volume = 0 (gap filling range or renko bars)
			if (Volume[0] == 0 || Volume[1] == 0) 
			{
				strategyHalt = true;
				BackColorAll = Color.Gray;
				return; 
			}
			
			// Halt when working in wrong timeframe   
			if (BarsInProgress != 0) 
			{
				strategyHalt = true;
				BackColorAll = Color.Gray;
				return; 
			}
			
			// Halt when manualTrading is true
			if (manualTrading) 
			{
				enterLong = false;
				enterShort = false;	
			//	PrintWithTimeStamp("MANUAL TRADING ACTIVATED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				strategyHalt = true;
				BackColorAll = Color.Gray;
				return; 
			}
			#endregion
			
			#region Debug and Print Section			
			if (FirstTickOfBar)
			{
				//Print("_ADXVMA_Alerts_v01_5_1 [0]: " + _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[0]); 
				//Print("enterLong is: " + enterLong);
				//Print("enterShort is: " + enterShort);
				//PrintWithTimeStamp("CurrentBar is: " + CurrentBar);
				//PrintWithTimeStamp("submitOrderBar is: " + submitOrderBar);
				//Print("Today's P&L is: " + (Performance.AllTrades.TradesPerformance.Currency.CumProfit - cumprofit));
				//PrintWithTimeStamp("ADXVMA Rising is: " + _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Rising);
				//PrintWithTimeStamp("ADXVMA Falling is: " + _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Falling);
				//PrintWithTimeStamp("ADXVMA Main is: " + _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Main);
				//PrintWithTimeStamp("Slope is: " + Slope(_ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Main, 1, 0));				
				//PrintWithTimeStamp("signalprice is: " + signalprice);
				//PrintWithTimeStamp("enterprice is: " + enterprice);
			}
			#endregion							
							
			#region Enter Position Conditions			
			// Calculate MFISMA2 volume delta based on TEMA			
			vDelta = MFISMA2(mFI_EMAperiod, mFIaverageType, mFIperiod, mFI_SMAperiod, mFI_TEMAperiod, mFI_VMAperiod, mFI_VMAvolatility).PlotTEMA[0] 
					 - MFISMA2(mFI_EMAperiod, mFIaverageType, mFIperiod, mFI_SMAperiod, mFI_TEMAperiod, mFI_VMAperiod, mFI_VMAvolatility).PlotTEMA[1];
					
			if (Position.MarketPosition == MarketPosition.Flat)	// True when no positions					
			{					
				// GO LONG CONDITIONS
				if (entryOrder1 == null && entryOrder2 == null					
					&& _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[0] == 1 // Rising trend
					&& _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[1] == 1 // Rising trend
					&& _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[0] != 0 // No trend, Neutral
					&& MAX(Sharkchop_v01_1(13, 1, 1, 16).Long, Math.Min(Bars.BarsSinceSession, sharkyChop_LookBack))[0] >= sharkyChop_Trigger					
					&& vDelta > mFIvolumeDelta // Volume confirms direction
					&& Close[0] > Median[0] && Close[0] > High[1] // Protect from false move											
					&& CCI(14).Value[0] >= cCIsignalLevel // (-200) agressive, (0) offensive, (100) conservative
					&& CCI(14).Value[0] <= cCIexitLevel // Overbought
					&& CCI(14).Value[0] >= CCI(14).Value[1] // CCI confirms direction				
					)
				{				
					enterLong = true;
					enterShort = false;					
					submitOrderBar = CurrentBar;
					DrawArrowUp("LONG" + CurrentBar, true, 0, Low[0] - 12 * TickSize, Color.Green);
					DrawText("SignalLong" + CurrentBar, "Signal Long", 0, Low[0] - 8 * TickSize, Color.Blue);					
				}	
					
				// GO SHORT CONDITIONS
				if (entryOrder1 == null && entryOrder2 == null					
					&& _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[0] == -1 // Falling trend
					&& _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[1] == -1 // Falling trend
					&& _ADXVMA_Alerts_v01_5_1(aDXVMAperiod, aDXVMA_ConservativeMode).Signal[0] != 0 // No trend, Neutral
					&& MAX(Sharkchop_v01_1(13, 1, 1, 16).Short, Math.Min(Bars.BarsSinceSession, sharkyChop_LookBack))[0] >= sharkyChop_Trigger					
					&& -vDelta > mFIvolumeDelta // Volume confirms direction
					&& Close[0] < Median[0] && Close[0] < Low[1] // Protect from false move					
					&& CCI(14).Value[0] <= -cCIsignalLevel // (-200) agressive, (0) offensive, (100) conservative
					&& CCI(14).Value[0] >= -cCIexitLevel // Oversold
					&& CCI(14).Value[0] <= CCI(14).Value[1] // CCI confirms direction				
					)					 
				{				
					enterShort = true;
					enterLong = false;						
					submitOrderBar = CurrentBar;
					DrawArrowDown("SHORT" + CurrentBar, true, 0, High[0] + 12 * TickSize, Color.Red);
					DrawText("SignalShort" + CurrentBar, "Signal Short", 0, High[0] + 8 * TickSize, Color.Red);						
				}				
				
				//ENTER SIGNAL ALERT
				if (FirstTickOfBar && enterLong) PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\Long.wav");
				else if (FirstTickOfBar && enterShort) PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\Short.wav");
							
				// CANCEL ORDER CONDITIONS
				// Call CancelAll() when not filled after 'cancelBars' bars
				if (CurrentBar >= submitOrderBar + cancelBars)
				{
					if ((entryOrder1 != null && entryOrder1.LimitPrice != null) || (entryOrder2 != null && entryOrder2.LimitPrice != null))					
					CancelAll();			
				}					
			}			
			#endregion				
        }
		#endregion		
		
		#region OnOrderUpdate
		protected override void OnOrderUpdate(IOrder order)
		{
			// Checks for all updates to entryOrder.
			if (entryOrder1 != null && entryOrder1.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					entryOrder1 = null;	// Reset entryOrder back to null					
				}						
			}	
			// Checks for all updates to entryOrder.
			if (entryOrder2 != null && entryOrder2.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					entryOrder2 = null; // Reset entryOrder back to null						
				}					
			}			
		}			
		#endregion
		
		#region OnMarketData
		protected override void OnMarketData(MarketDataEventArgs e)
		{			
			if (e.MarketDataType == MarketDataType.Last)
			{				
				#region Enter Long
				// Enter Long			
				if (enterLong && btnLongEntry.Text == "Allow Long" && Position.MarketPosition == MarketPosition.Flat && CurrentBar != submitOrderBar) // For conditions see Enter Conditions region
				{				
					signalprice = High[CurrentBar - submitOrderBar];
					enterprice = signalprice + enterTicks * TickSize;				 
					
					if (e.Price >= enterprice && CurrentBar < submitOrderBar + cancelBars)
					{						
						entryOrder1 = EnterLong(0, DefaultQuantity, "Long 1a");
						if(!oneTarget) entryOrder2 = EnterLong(0, DefaultQuantity, "Long 1b");							
						enterLong = false;						
					} 
					
					else if (CurrentBar >= submitOrderBar + cancelBars)
					{
						countFalseMoves = countFalseMoves + 1;
						DrawTextFixed("False Moves", "# False Moves: " + countFalseMoves.ToString(), TextPosition.BottomRight);
						DrawText("FalseMove" + CurrentBar, "False Move", 0, High[0] + 8 * TickSize, Color.Blue);
						DrawSquare("NoLong" + CurrentBar, true, 0, High[0] + 12 * TickSize, Color.Blue);						
						enterLong = false;					
						writeLineType = 2;
						WriteTextToFile();		
					}				
				}
				#endregion
			
				#region Enter Short
				// Enter Short
				else if (enterShort && btnShortEntry.Text == "Allow Short" && Position.MarketPosition == MarketPosition.Flat && CurrentBar != submitOrderBar) // For conditions see Enter Conditions region
				{
					signalprice = Low[CurrentBar - submitOrderBar];
					enterprice = signalprice - enterTicks * TickSize;				
					
					if (e.Price <= enterprice && CurrentBar < submitOrderBar + cancelBars)
					{							
						entryOrder1 = EnterShort(0, DefaultQuantity, "Short 1a");
						if(!oneTarget) entryOrder2 = EnterShort(0, DefaultQuantity, "Short 1b");						
						enterShort = false;						
					} 
					
					else if (CurrentBar >= submitOrderBar + cancelBars)
					{
						countFalseMoves = countFalseMoves + 1;
						DrawTextFixed("False Moves", "# False Moves: " + countFalseMoves.ToString(), TextPosition.BottomRight);
						DrawText("FalseMove" + CurrentBar, "False Move", 0, Low[0] - 8 * TickSize, Color.Red);
						DrawSquare("NOSHORT" + CurrentBar, true, 0, Low[0] - 12 * TickSize, Color.Red);								
						enterShort = false;					
						writeLineType = 2;
						WriteTextToFile();					
					}
				}
				#endregion																				
			
				#region Break Even		
					switch (Position.MarketPosition)
					{
						case MarketPosition.Flat:
						// Only allow entries if we have no current positions open	
						SetStopLoss("Long 1a", CalculationMode.Price, enterprice - stoploss * TickSize, false);
						SetStopLoss("Short 1a", CalculationMode.Price, enterprice + stoploss * TickSize, false);
						if(!oneTarget)
						{
							SetStopLoss("Long 1b", CalculationMode.Price, enterprice - stoploss * TickSize, false);						
							SetStopLoss("Short 1b", CalculationMode.Price, enterprice + stoploss * TickSize, false);				
						}									
						SetProfitTarget("Long 1a", CalculationMode.Price, enterprice + profit1 * TickSize);
						SetProfitTarget("Short 1a", CalculationMode.Price, enterprice - profit1 * TickSize);			
						// profit2 = 0 will make it a Runner
						if (profit2 != 0)
						{
							SetProfitTarget("Long 1b", CalculationMode.Price, enterprice + profit2 * TickSize);
							SetProfitTarget("Short 1b", CalculationMode.Price, enterprice - profit2 * TickSize);			
						}
						previousPrice = 0;
						stopLossPrice = 0;
						trailBars = true;
						firstPositionBar = CurrentBar; // First possible bar with position
						RemoveDrawObject("stop");
						RemoveDrawObject("target1");
						RemoveDrawObject("target2");
						break;
									
						case MarketPosition.Long:						 
						// Once the price is greater than entry price+ breakEvenTicks ticks, set stop loss of first order to initialBreakEven
						if (e.Price >= enterprice + breakEvenTicks * TickSize && previousPrice == 0)
						{						
							initialBreakEven = enterprice + breakEvenTicksPlus * TickSize;
							SetStopLoss("Long 1a", CalculationMode.Price, initialBreakEven, false);
							if(!oneTarget) SetStopLoss("Long 1b", CalculationMode.Price, initialBreakEven, false); // Enable when you want both positions to be stopped
							previousPrice = enterprice;
							DrawExtendedLine("stop", false, 50, initialBreakEven, 0, initialBreakEven, Color.Red, DashStyle.DashDot, 2);							
							PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\AutoTrail.wav");							
						}	
						
						else if (e.Price < enterprice + breakEvenTicks * TickSize && previousPrice == 0)
						{
							DrawExtendedLine("stop", false, 50, enterprice - stoploss * TickSize, 0, enterprice - stoploss * TickSize, Color.Red, DashStyle.DashDot, 2);							
						}
						
						// Check if theoretical price is best price
						if (Position.AvgPrice < enterprice)
						{
							enterprice = Position.AvgPrice;
							SetProfitTarget("Long 1a", CalculationMode.Price, enterprice + profit1 * TickSize);									
								// profit2 = 0 will make it a Runner
								if (profit2 != 0)
								{
									SetProfitTarget("Long 1b", CalculationMode.Price, enterprice + profit2 * TickSize);												
								}
						}
						
						DrawExtendedLine("target1", false, 50, enterprice + profit1 * TickSize, 0, enterprice + profit1 * TickSize, Color.Green, DashStyle.DashDot, 2);
						if (profit2 != 0) DrawExtendedLine("target2", false, 50, enterprice + profit2 * TickSize, 0, enterprice + profit2 * TickSize, Color.Green, DashStyle.DashDot, 2);
						break;
						
						case MarketPosition.Short:						 
						// Once the price is Less than entry price - breakEvenTicks ticks, set stop loss of first order to initialBreakEven
						if (e.Price <= enterprice - breakEvenTicks * TickSize && previousPrice == 0)
						{
							initialBreakEven = enterprice - breakEvenTicksPlus * TickSize;
							SetStopLoss("Short 1a", CalculationMode.Price, initialBreakEven, false);
							if(!oneTarget) SetStopLoss("Short 1b", CalculationMode.Price, initialBreakEven, false); // Enable when you want both positions to be stopped
							previousPrice = enterprice;	
							DrawExtendedLine("stop", false, 50, initialBreakEven, 0, initialBreakEven, Color.Red, DashStyle.DashDot, 2);
							PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\AutoTrail.wav");
						}	
						
						else if (e.Price > enterprice - breakEvenTicks * TickSize && previousPrice == 0)
						{
							DrawExtendedLine("stop", false, 50, enterprice + stoploss * TickSize, 0, enterprice + stoploss * TickSize, Color.Red, DashStyle.DashDot, 2);							
						}						
						
						// Check if theoretical price is best price
						if (Position.AvgPrice > enterprice)
						{
							enterprice = Position.AvgPrice;							
							SetProfitTarget("Short 1a", CalculationMode.Price, enterprice - profit1 * TickSize);			
								// profit2 = 0 will make it a Runner
								if (profit2 != 0)
								{
									SetProfitTarget("Short 1b", CalculationMode.Price, enterprice - profit2 * TickSize);			
								}
						}
							
						DrawExtendedLine("target1", false, 50, enterprice - profit1 * TickSize, 0,enterprice - profit1 * TickSize, Color.Green, DashStyle.DashDot, 2);
						if (profit2 != 0) DrawExtendedLine("target2", false, 50, enterprice - profit2 * TickSize, 0, enterprice - profit2 * TickSize, Color.Green, DashStyle.DashDot, 2);
						break;
					}			
				#endregion	
				
				#region Exit			
				switch (Position.MarketPosition)
				{						
					case MarketPosition.Long:
					int lookBackLong = Math.Max(0, CurrentBar - submitOrderBar + 1);
					lastPositionBar = CurrentBar;						
					
					// Close when position is taken in tail of bar
					if(CurrentBar == firstPositionBar + 1 && enterprice > Open[1] && Open[1] > Close[1] && Close[0] < Low[1] - 2 * TickSize)
						{
							ExitLong();	
							DrawText("TailBar Entry Exit" + CurrentBar, "TailBar Entry Exit", 0, High[0] - 8 * TickSize, Color.Red);
						}							
					// Close on break of trend							
					if (MAX(High, lookBackLong)[0] > enterprice + exitTrigger * TickSize)
					{
						if (High[1] > High[2]   
							&& High[2] >= High[3]  
							&& Close[1] < Open[1]
							&& Close[0] < Low[1] - 3 * TickSize						
							)					
						{
							ExitLong();									
							DrawText("BarReversal" + CurrentBar, "Reversal Bar Exit 1", 0, High[0] - 8 * TickSize, Color.Red);
						}						
					}					
					break;
					
					case MarketPosition.Short:	
					int lookBackShort = Math.Max(0, CurrentBar - submitOrderBar + 1);
					lastPositionBar = CurrentBar;						
					
					// Close when position is taken in tail of bar
					if(CurrentBar == firstPositionBar + 1 && enterprice < Open[1] && Open[1] < Close[1] && Close[0] > High[1] + 2 * TickSize)
						{
							ExitShort();									
							DrawText("TailBar Entry Exit" + CurrentBar, "TailBar Entry Exit", 0, Low[0] + 8 * TickSize, Color.Blue);
						}					
					// Close on break of trend				
					if (MIN(Low, lookBackShort)[0] < enterprice - exitTrigger * TickSize)
					{							
						if (Low[1] < Low[2]
							&& Low[2] <= Low[3]  
							&& Close[1] > Open[1]
							&& Close[0] > High[1] + 3 * TickSize					
							)
						{
							ExitShort();							
							DrawText("BarReversal" + CurrentBar, "Reversal Bar Exit 1", 0, Low[0] + 8 * TickSize, Color.Blue);
						}							
					}			
					break;
				}				
				#endregion	
			
				#region Dynamic Trail 				
				if (dynamicTrail)
				{					
					if (trailBars)
					{
						switch (Position.MarketPosition)
						{								
							case MarketPosition.Long:									
							// Once at breakeven and winning, start trailing
							if (previousPrice	!= 0 //StopLoss is at breakeven
							&& e.Price > enterprice //Winning position 
							&& e.Price > MIN(Open, trailLookBackBars)[1]) //Minimum Open during lookBackPeriod						
							{
								newPrice = Math.Max(initialBreakEven, MIN(Open, trailLookBackBars)[1] - 2 * TickSize);
								if (previousPrice != newPrice) 
								{
									SetStopLoss("Long 1a", CalculationMode.Price, newPrice, false);
									if(!oneTarget) SetStopLoss("Long 1b", CalculationMode.Price, newPrice, false);										
									PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\AutoTrail.wav");
								}
								previousPrice = newPrice;
								DrawExtendedLine("stop", false, 50, newPrice, 0, newPrice, Color.Red, DashStyle.DashDot, 2);
							}												
							break;
							
							case MarketPosition.Short:					
							// Once at breakeven and winning, start trailing
							if (previousPrice	!= 0 //StopLoss is at breakeven
							&& e.Price < enterprice //Winning position
							&& e.Price < MAX(Open, trailLookBackBars)[1]) // Maximum Open during trailLookBackBars														
							{
								newPrice = Math.Min(initialBreakEven, MAX(Open, trailLookBackBars)[1] + 2 * TickSize);
								if (previousPrice != newPrice) 
								{
									SetStopLoss("Short 1a", CalculationMode.Price, newPrice, false);
									if(!oneTarget) SetStopLoss("Short 1b", CalculationMode.Price, newPrice, false);										
									PlaySound(@"C:\Program Files (x86)\NinjaTrader 7\sounds\AutoTrail.wav");
								}
								previousPrice = newPrice;
								DrawExtendedLine("stop", false, 50, newPrice, 0, newPrice, Color.Red, DashStyle.DashDot, 2);
							}						
							break;
						}	
					}			
				}
				#endregion				
			}
		}		
		#endregion	
		
		#region CancelAll
		private void CancelAll()
		{
			// Cancel orders when still active
			enterLong = false;
			enterShort = false;
			CancelOrder(entryOrder1);
			if(!oneTarget) CancelOrder(entryOrder2);			
					
		//	PrintWithTimeStamp("ALL OPEN ORDERS HAVE BEEN CANCELLED!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			DrawText("CancelAll" + CurrentBar, "All Orders Cancelled", 0, Median[0], Color.Red); 
 		}
		#endregion
			
		#region FlattenAll
		private void FlattenAll()
		{
			// Close positions when still open
			if (Position.MarketPosition == MarketPosition.Long)
			{
				ExitLong();	
			}
			
			else if (Position.MarketPosition == MarketPosition.Short)
			{
				ExitShort();			
			}
			
			// Cancel orders when still active
			if ((entryOrder1 != null && entryOrder1.LimitPrice != null) || (entryOrder2 != null && entryOrder2.LimitPrice != null))					
			{
				CancelAll();			
			}
 		}
		#endregion
		
		#region Write Report
		public void WriteTextToFile()
        {
			if (writeToFile)
			{
				string currentTime = Time[0].ToString("HH:mm:ss"); // Generate the string with the date and time		        
				string currentDate = Time[0].Date.ToString("dd-MM-yy"); // Generate the string with the date           
				string filePath = string.Format(NinjaTrader.Cbi.Core.UserDataDir + @"\tmp\{0}\{1}\", Time[0].Date.ToString("MMMM-yy"), Name.ToString()); // Generate the target directory path
				string fileName = string.Format("{0}_{1}.txt", Instrument.FullName.ToString(), currentDate); // Generate the file name
	            string fileLocation = System.IO.Path.Combine(filePath, fileName); // Combine both strings in the complete path   
            
			//	Print("Current file location: " + fileLocation);
   	
    	        // Make the directory if it's not yet existing
        	    if (!System.IO.Directory.Exists(filePath))
           		{
                	System.IO.Directory.CreateDirectory(filePath);
           		}
   
           		// Start writing to file
            	try
            	{
                	sw = System.IO.File.AppendText(fileLocation);
					switch (writeLineType) // (1)todays P&L,(2)False Move
					{
						case (1):
						sw.WriteLine("Time " + currentTime + " Todays P&L " + todaysPL);
						break;
						case (2):
						sw.WriteLine("Time " + currentTime + " False Move " + countFalseMoves);
						break;
					}				
            	}
            	// Catch the exceptions
            	catch (Exception e)
            	{
               		// Outputs the error to the log
                	Log("You cannot write and read from the same file at the same time. Please remove SampleStreamReader.", NinjaTrader.Cbi.LogLevel.Error);
                	throw;
            	}
            	// Close SteamWriter
				writeLineType = 0;
            	sw.Close();
        	}
		}
		#endregion
		
		#region Status Box
		private void StatusBox()
		{
			// Print our current P&L to the upper right hand corner of the chart
				double actprice;
				string pnl_display = "P&L Realized: " + Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit.ToString("C");
				if (Position.MarketPosition != MarketPosition.Flat) 
				{
					if (Position.MarketPosition == MarketPosition.Long) actprice = GetCurrentBid(); 
					else actprice = GetCurrentAsk();
					pnl_display = pnl_display + "\nP&L Unrealized: " + Position.GetProfitLoss(actprice, PerformanceUnit.Currency).ToString("C"); 
				}
				pnl_display = pnl_display + "\nP&L this Day: " + (Performance.AllTrades.TradesPerformance.Currency.CumProfit - cumprofit).ToString("C");				
				pnl_display = pnl_display + "\n# of Trades: " + Performance.AllTrades.Count.ToString();
				pnl_display = pnl_display + "\nTick Counter: " + Bars.TickCount.ToString(); 
				pnl_display = pnl_display + "\nLong Signal: " + enterLong; 
				pnl_display = pnl_display + "\nShort Signal: " + enterShort; 
				pnl_display = pnl_display + "\nStrategy on Halt: " + strategyHalt;
				DrawTextFixed("PnL", pnl_display, TextPosition.TopLeft,Color.Red, new Font("Arial", 8), Color.Black, Color.LightGray, 100);
		}
		#endregion
		
		#region OnTermination
		// Necessary to call in order to clean up resources used by the StreamWriter object
		protected override void OnTermination() 
		{
			// Disposes resources used by the StreamWriter
			if (sw != null)
			{
				sw.Dispose();
				sw = null;
			}
		}
		#endregion
		
		#region ToolBarButtons		
		#region OnStartUp
		protected override void OnStartUp()
		{
			System.Windows.Forms.Control[] controls = ChartControl.Controls.Find("tsrTool", false);

			if (controls.Length > 0) 
			{
				
				ToolStripButton btnTemp = new System.Windows.Forms.ToolStripButton("temp");
				btnTemp = null;

				strip = (System.Windows.Forms.ToolStrip)controls[0];

				sepL = new ToolStripSeparator();
				strip.Items.Add(sepL);				

				btnLongEntry = new System.Windows.Forms.ToolStripButton("btnLongEntry");
				btnLongEntry.Font = boldFont ;
				btnLongEntry.ForeColor = Color.Gainsboro;
				btnLongEntry.BackColor=Color.Green;
				btnLongEntry.Text = "Allow Long";
				btnLongEntry.ToolTipText = "Permits Long Entries";

				strip.Items.Add(btnLongEntry);
				btnLongEntry.Click += btnLongEntry_Click;
				
				sep2 = new ToolStripSeparator();
				strip.Items.Add(sep2);

				btnShortEntry = new System.Windows.Forms.ToolStripButton("btnShortEntry");
				btnShortEntry.Font = boldFont ;
				btnShortEntry.ForeColor = Color.Gainsboro;
				btnShortEntry.BackColor=Color.Green;
				btnShortEntry.Text = "Allow Short";
				btnShortEntry.ToolTipText = "Permits Short Entries";

				strip.Items.Add(btnShortEntry);
				btnShortEntry.Click += btnShortEntry_Click;
				
				sep3 = new ToolStripSeparator();
				strip.Items.Add(sep3);
				
				if (toolBarButtons)
				{
					btnManual = new System.Windows.Forms.ToolStripButton("btnManual");
					btnManual.Font = regularFont ;    
					btnManual.Font = boldFont ;
					btnManual.ForeColor = Color.Gainsboro;
					btnManual.BackColor= Color.Green;    
					btnManual.Text = "Auto";
					btnManual.ToolTipText = "Set to Auto for auto trade entry, Manual for manual trade entry";

					strip.Items.Add(btnManual);
					btnManual.Click += btnManual_Click;
					
					sep1 = new ToolStripSeparator();
					strip.Items.Add(sep1);
					
					btnManualLongEntry = new System.Windows.Forms.ToolStripButton("btnManualLongEntry");
					btnManualLongEntry.Font = boldFont ;
					btnManualLongEntry.ForeColor = Color.Gainsboro;
					btnManualLongEntry.BackColor=Color.Green;
					btnManualLongEntry.Text = "Enter Long";
					btnManualLongEntry.ToolTipText = "With the Auto/Manual button set to Manual and Allow Long enabled, will enter a Long Market Order";
				
					strip.Items.Add(btnManualLongEntry);
					btnManualLongEntry.Click += btnManualLongEntry_Click;
					
					sep4 = new ToolStripSeparator();
					strip.Items.Add(sep4);

					btnManualShortEntry = new System.Windows.Forms.ToolStripButton("btnManualShortEntry");
					btnManualShortEntry.Font = boldFont ;
					btnManualShortEntry.ForeColor = Color.Gainsboro;
					btnManualShortEntry.BackColor=Color.Green;
					btnManualShortEntry.Text = "Enter Short";
					btnManualShortEntry.ToolTipText = "With the Auto/Manual button set to Manual and Sell Short enabled, will enter a Short Market Order";

					strip.Items.Add(btnManualShortEntry);
					btnManualShortEntry.Click += btnManualShortEntry_Click;
					
					sep5 = new ToolStripSeparator();
					strip.Items.Add(sep5);
				}

				btnFlatten = new System.Windows.Forms.ToolStripButton("btnFlatten");
				btnFlatten.Font = boldFont ;
				btnFlatten.ForeColor = Color.Gainsboro;
				btnFlatten.BackColor=Color.DarkGoldenrod;
				btnFlatten.Text = "Flatten";
				btnFlatten.ToolTipText = "Will Flatten your position for this Strategy only";

				strip.Items.Add(btnFlatten);
				btnFlatten.Click += btnFlatten_Click;
				
				sepR = new ToolStripSeparator();
				strip.Items.Add(sepR);

				buttonsloaded=true;				
			}
		}
		#endregion		
		
		#region Dispose
			public override void Dispose() 
			{
				if (buttonsloaded==true)
				{
					strip.Items.Remove(btnManual);
					strip.Items.Remove(btnLongEntry);
					strip.Items.Remove(btnShortEntry);
					strip.Items.Remove(btnManualLongEntry);
					strip.Items.Remove(btnManualShortEntry);
					strip.Items.Remove(btnFlatten);
					strip.Items.Remove(sepL);
					strip.Items.Remove(sep1);
					strip.Items.Remove(sep2);
					strip.Items.Remove(sep3);
					strip.Items.Remove(sep4);
					strip.Items.Remove(sep5);
					strip.Items.Remove(sepR);
					RemoveDrawObjects();
				}
			}
			#endregion
			
		#region Button Click
			private void btnManual_Click(object sender, EventArgs e)
			{			
				if (btnManual.Text == "Manual")
				{
					if (Position.MarketPosition != MarketPosition.Flat)
					{
						if(MessageBox.Show("Do you want to close open positions and switch to Auto Trading?",
							"Auto Trade",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question) == DialogResult.Yes)
						{
							FlattenAll();
							manualTrading = false; //Auto trading
							btnManual.Text = "Auto";
							btnManual.Font =boldFont;
							btnManual.ForeColor = Color.Gainsboro;
							btnManual.BackColor=Color.Green;
						}
					}
					else if(Position.MarketPosition == MarketPosition.Flat) 
					{
							
						if(MessageBox.Show("Are you sure you want to switch to Auto Trading?",
							"Auto Trade",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question) == DialogResult.Yes)
						{
							manualTrading = false; //Auto trading
							btnManual.Text = "Auto";
							btnManual.Font =boldFont;
							btnManual.ForeColor = Color.Gainsboro;
							btnManual.BackColor=Color.Green;
						}
					}
				}
				else if(btnManual.Text == "Auto")
					if (Position.MarketPosition != MarketPosition.Flat)
					{				
						if(MessageBox.Show("Do you want to close open positions and switch to Manual Trading?",
								"Manual Trade",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Question) == DialogResult.Yes)
						{
							FlattenAll();
							manualTrading = true; //Manual trading
							btnManual.Text = "Manual";
							btnManual.Font =regularFont ;
							btnManual.ForeColor = Color.Gainsboro;
							btnManual.BackColor=Color.Red;
						}
					}
					else if(Position.MarketPosition == MarketPosition.Flat) 
					{
						if(MessageBox.Show("Are you sure you want to switch to Manual Trading?",
							"Manual Trade",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question) == DialogResult.Yes)
						{
							manualTrading = true; //Manual trading
							btnManual.Text = "Manual";
							btnManual.Font =regularFont ;
							btnManual.ForeColor = Color.Gainsboro;
							btnManual.BackColor=Color.Red;
						}
					}				
				btnManual.Enabled = true;
			}

			private void btnLongEntry_Click(object sender, EventArgs e)
			{
				if (btnLongEntry.Text == "No Longs")
				{
					btnLongEntry.Text = "Allow Long";
					btnLongEntry.Font = boldFont ;
					btnLongEntry.ForeColor = Color.Gainsboro;
					btnLongEntry.BackColor=Color.Green;
				}
				else
				{
					btnLongEntry.Text = "No Longs";
					btnLongEntry.Font = regularFont ;
					btnLongEntry.ForeColor = Color.Gainsboro;
					btnLongEntry.BackColor=Color.DarkRed;
				}

				btnLongEntry.Enabled = true;
			}

			private void btnShortEntry_Click(object sender, EventArgs e)
			{
				if (btnShortEntry.Text == "No Shorts")
				{
					btnShortEntry.Text = "Allow Short";
					btnShortEntry.Font = boldFont ;
					btnShortEntry.ForeColor = Color.Gainsboro;
					btnShortEntry.BackColor=Color.Green;
				}
				else
				{
					btnShortEntry.Text = "No Shorts";
					btnShortEntry.Font = regularFont ;
					btnShortEntry.ForeColor = Color.Gainsboro;
					btnShortEntry.BackColor=Color.DarkRed;
				}

				btnLongEntry.Enabled = true;
			}
			#endregion
		
		#region Manual Trading		
		private void btnManualLongEntry_Click(object sender, EventArgs e)
		{
			if (Position.MarketPosition == MarketPosition.Flat && btnManual.Text == "Manual" && btnLongEntry.Text == "Allow Long")
			{
				EnterLong("Enter Long");
				DrawArrowUp("LONG" + CurrentBar, true, 0, Low[0] - 12 * TickSize, Color.Green);
				DrawText("SignalLong" + CurrentBar, "Enter Long", 0, Low[0] - 8 * TickSize, Color.Blue);
			}
			
			else if (Position.MarketPosition == MarketPosition.Flat && btnManual.Text == "Auto" && btnLongEntry.Text == "Allow Long")
			{				
				signalprice = Close[0];
				enterLong = true;
				enterShort = false;			
				submitOrderBar = CurrentBar;
				DrawArrowUp("LONG" + CurrentBar, true, 0, Low[0] - 12 * TickSize, Color.Green);
				DrawText("SignalLong" + CurrentBar, "Auto Long", 0, Low[0] - 8 * TickSize, Color.Blue);
			}
			btnManualLongEntry.Enabled = true;
		}

		private void btnManualShortEntry_Click(object sender, EventArgs e)
		{
			if (Position.MarketPosition == MarketPosition.Flat && btnManual.Text == "Manual" && btnShortEntry.Text == "Allow Short")
			{
				EnterShort("Enter Short");
				DrawArrowDown("SHORT" + CurrentBar, true, 0, High[0] + 12 * TickSize, Color.Red);
				DrawText("SignalShort" + CurrentBar, "Enter Short", 0, High[0] + 8 * TickSize, Color.Red);
			}
			
			else if (Position.MarketPosition == MarketPosition.Flat && btnManual.Text == "Auto" && btnShortEntry.Text == "Allow Short")
			{
				signalprice = Close[0];						
				enterShort = true;
				enterLong = false;					
				submitOrderBar = CurrentBar;
				DrawArrowDown("SHORT" + CurrentBar, true, 0, High[0] + 12 * TickSize, Color.Red);
				DrawText("SignalShort" + CurrentBar, "Auto Short", 0, High[0] + 8 * TickSize, Color.Red);
			}
			btnManualShortEntry.Enabled = true;
		}

		private void btnFlatten_Click(object sender, EventArgs e)
		{
			FlattenAll();
			btnFlatten.Enabled = true;
		}
		#endregion
		#endregion		
		
		#region Properties
		#region General Settings
		[Description("")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t1) GENERAL")]
		public String A1
		{
			get { return ""; }
		}	
		[Description("Trade only 1 position?")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:One Position?")]
        public bool OneTarget
        {
            get { return oneTarget; }
            set { oneTarget = value; }
        }
		
		[Description("Trade on Fridays?")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:Trade on Fridays?")]
        public bool TradeOnFriday
        {
            get { return tradeOnFriday; }
            set { tradeOnFriday = value; }
        }
		
		[Description("Exclude historical data?")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:No Historical?")]
        public bool NoHistorical
        {
            get { return noHistorical; }
            set { noHistorical = value; }
        }		
		
		[Description("When enabled, ADXVMA will go into Neutral when no trend.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:ADXVMA Conservative?")]
        public bool ADXVMA_ConservativeMode
        {
            get { return aDXVMA_ConservativeMode; }
            set { aDXVMA_ConservativeMode = value; }
        }		
		
		[Description("When true: Dynamic trail. When false: Reversal bar exit only")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:Trail Dynamic?")]
        public bool DynamicTrail
        {          
			get { return dynamicTrail; }
            set { dynamicTrail =value; }
        }
		
        [Description("Start Trading Time. Format is HHMMSS.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime1: Start Strategy")]
        public int TimeStart
        {
            get { return timeStart; }
            set { timeStart = Math.Max(000000, value); }
        }		
		
		[Description("Halt Trading Time. Format is HHMMSS.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime2: Halt Strategy")]
        public int BreakStart1
        {
            get { return breakStart1; }
            set { breakStart1 = Math.Max(timeStart, value); }
        }
		
		[Description("Resume Trading Time. Format is HHMMSS.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime3: Resume Strategy")]
        public int BreakStop1
        {
            get { return breakStop1; }
            set { breakStop1 = Math.Max(breakStart1, value); }
        }
		
		[Description("Halt Trading Time. Format is HHMMSS.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime4: Halt Strategy")]
        public int BreakStart2
        {
            get { return breakStart2; }
            set { breakStart2 = Math.Max(breakStop1, value); }
        }
		
		[Description("Resume Trading Time. Format is HHMMSS.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime5: Resume Strategy")]
        public int BreakStop2
        {
            get { return breakStop2; }
            set { breakStop2 = Math.Max(breakStart2, value); }
        }
		
		[Description("Stop Trading Time. Format is HHMMSS. Open orders will be cancelled.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime6: Stop Strategy")]
        public int TimeStop
        {
            get { return timeStop; }
            set { timeStop = Math.Max(breakStop2, value); }
        }
		
		[Description("End Strategy. Format is HHMMSS. Open positions will be closed.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tTime7: End Strategy")]
        public int TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = Math.Max(timeStop, value); }
        }	
		
		[Description("Write results to text file?")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:Write Report?")]
        public bool WriteToFile
        {
            get { return writeToFile; }
            set { writeToFile = value; }
        }	
		
		[Description("Display ToolBar Buttons for Manual Entries?")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tQ:Manual Entry Buttons?")]
        public bool ToolBarButtons
        {
            get { return toolBarButtons; }
            set { toolBarButtons = value; }
        }
		#endregion
				
		#region Target & Stop
		[Description("")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\t2) TARGET & STOP")]
		public String A2
		{
			get { return ""; }
		}	
		
		[Description("Maximum profit.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tP&L: Max Profit")]
        public int MaxProfit
        {
            get { return maxprofit; }
            set { maxprofit = Math.Max(1, value); }
        }
		
		[Description("Maximum loss (enter positive value!).")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tP&L: Max Loss")]
        public int MaxLoss
        {
            get { return maxloss; }
            set { maxloss = Math.Max(1, value); }
        }
		
		[Description("Target for first order.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTarget: Profit 1")]
        public int Profit1
        {
            get { return profit1; }
            set { profit1 = Math.Max(1, value); } // Minimum value = 1 Tick
        }
		
		[Description("Target for second order. When '0' it will treated as Runner.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTarget: Profit 2")]
        public int Profit2
        {
            get { return profit2; }
            set { profit2 = Math.Max(0, value); }
        }	
		
		[Description("Trigger to activate Break Even (must be <= Profit1.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tStop: BE Trigger")]
        public int BreakEvenTicks
        {
            get { return breakEvenTicks; }
            set { breakEvenTicks = Math.Min(profit1, value); }
        }
		
		[Description("Offset to average enter price (must be <= BE Trigger.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tStop: BE Plus")]
        public int BreakEvenTicksPlus
        {
            get { return breakEvenTicksPlus; }
            set { breakEvenTicksPlus = Math.Min(breakEvenTicks, value); }
        }
		
		[Description("Number of positive Ticks when go into Reversal Bar Exit.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tStop: Exit Trigger")]
        public int ExitTrigger
        {
            get { return exitTrigger; }
            set { exitTrigger = Math.Max(0, value); }
        }
				
		[Description("Stoploss.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\tStop: Stop Loss")]
        public int Stoploss
        {
            get { return stoploss; }
            set { stoploss = Math.Max(0, value); }
        }			
		#endregion
		
		#region Indicator settings
		[Description("")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t3) INDICATOR")]
		public String A3
		{
			get { return ""; }
		}	

		[Description("Period for ADXVMA indicator (14).")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tADXVMA Period")]
        public int ADXVMAperiod
        {
            get { return aDXVMAperiod; }
            set { aDXVMAperiod = Math.Max(1, value); }
        }		
		
		[Description("SharkyChop trigger level.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tSharkyChop Trigger")]
        public double SharkyChop_Trigger
        {
            get { return sharkyChop_Trigger; }
            set { sharkyChop_Trigger = Math.Max(0, value); }
        }	
		
		[Description("SharkyChop lookback period.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tSharkyChop LookBack")]
        public int SharkyChop_LookBack
        {
            get { return sharkyChop_LookBack; }
            set { sharkyChop_LookBack = Math.Max(0, value); }
        }	
				
		[Description("The level at which the CCI is triggered (-200)Agressive, (0)Offensive, (100)Conservative.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tCCI Signal Level")]
        public int CCIsignalLevel
        {
            get { return cCIsignalLevel; }
            set { cCIsignalLevel = Math.Max(-200, value); }
        }	
		
		[Description("The level at which CCI is in overbought (200)Agressive, (150)Offensive, (100)Conservative.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tCCI Overbought")]
        public int CCIexitLevel
        {
            get { return cCIexitLevel; }
            set { cCIexitLevel = Math.Max(100, value); }
        }	
		
		[Description("The BarVolume delta volume filter (0 - 50).")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tMFI Volume Delta")]
        public int MFIvolumeDelta
        {
            get { return mFIvolumeDelta; }
            set { mFIvolumeDelta = Math.Max(0, value); }
        }
		
		[Description("MFI period")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tMFI Period")]
        public int MFIperiod
        {
            get { return mFIperiod; }
            set { mFIperiod = Math.Max(1, value); }
        }
		
		[Description("0=Limit order set to Close[0]. x=Limit order set to Close[0] + x (long), Close[0] - x (short).")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tOrder Enter Ticks")]
        public int EnterTicks
        {
            get { return enterTicks; }
            set { enterTicks = Math.Max(0, value); }
        }
		
		[Description("Amount of Bars to cancel order when not filled.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tOrder Cancel Bars")]
        public int CancelBars
        {
            get { return cancelBars; }
            set { cancelBars = Math.Max(1, value); }
        }		
		
		[Description("Number of Bars back to use for Trail Stop.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tTrail Lookback Bars")]
        public int TrailLookBackBars
        {
            get { return trailLookBackBars; }
            set { trailLookBackBars = Math.Max(0, value); }
        }		
		#endregion
		#endregion		
    }
}
