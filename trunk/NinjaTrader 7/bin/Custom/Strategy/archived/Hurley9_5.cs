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
    /// [big mike] jan 30 2010: v1.00, initial release
	/// [big mike] jan 31 2010: v1.01, fixed position sizing
	/// 						made universal 6.5 and 7 compatible code
	/// [Nano] Feb 22 2010		Added modified code by sflanagan98 in section starting: "if (CMOabovebelow )"
	/// 							and also deleted the APZ gap rule to allow trades to be taken ofter the open
	/// [Nano] Feb 23 2010		Added strategy start and finish times as variables		
	/// [Nano] Feb 26 2010		Added _ADXVMA_Alertsv01_5_1_Hurley (same as _ADXVMA_Alerts_v01_5_1 with some parameter changed) indicator as a filter for taking long and short trades
	/// [Nano] Feb 27 2010		Added exit statements for target1, target2 & target3 if low/high crosses below/above _ADXVMA_Alertsv01_5_1_Hurley(Main) for longs/shorts.
	/// [tellyTub] Feb 27 2010	Made the code easier to read, and changed the Timestart/Timefinish to string
	/// [Nano] Mar 12 2010		Change includes a second variable for _ADXVMA_Alertsv01_5_1_Hurley period length to act as an additional filter.
	/// 							SbsRenko10 data series appears to work OK.
	/// 							For long trades to initiate price must be above _ADXVMA_Alertsv01_5_1_Hurley at periods of both adxvmalength1 and adxvmalength2.
	/// 							For short trades to initiate price must be below _ADXVMA_Alertsv01_5_1_Hurley at periods of both adxvmalength1 and adxvmalength2.
	/// [Nano] Mar 25 2010		Deleted be1, be2 and be3. Deleted BM's exit on reversal as the exit crossing the ADXVMA line1 works better. This is Nano's attempt to simplify the code.
	/// 							Also deleted the complex trailing stoplosses as runners work better without them.
	/// [Nano] Mar 29 2010		NT7b12 version. To make this NT6.5 compatable delete the line "Enabled				= true;"
	/// [Nano} Mar 30 2010		Added APZ as a filter for entries to reduce stopouts
	/// [Nano] Mar 31 2010		Added tstop as a variable to allow a choice for the move in stoploss for Target2-Target5 after Target1 is hit. Must be less than your Target1 value of course
	/// 						Added a filter that the ADXVMA length1 line must be above the ADXVMA length2 line for longs and vice-versa.
	/// [sflanagan98] Mar 31 2010	Changes made under ManageOrders() including defaulting cmoabovebelow to false
	/// [Nano] Apr 2 2010		Altered the ADXVMA exit signal to include crossing below/above adxvmalength1 OR adxvmalength2
	/// [sam028] Apr 7 2010		Includes code to allow strategy to work in NT6.5 and NT7
	/// [Nano] Apr 8 2010		Added a variable that when changed to "true" allows the strategy to start and finish at selected times
	/// [nano] Apr 8 2010		Added another exit signal: for longs exit when close is lower than open, & exit shorts when close is higher than open
    /// [aknip] Apr 10 2010		Added fix to  to start and finish at selected times: Supports now crossing over midnight
	/// [Nano] Apr 18 2010		Ceased using cmorisingfalling - now take longs when CMO above zero and shorts when CMO below zero & cmoabovebelow now defaults to true
	/// [aknip] Apr 21			Added fix to cumprofittarget/cumlosstarget => not fully tested!
	/// [Nano] Apr 22 2010		Added a true/false variable "NoHistorical" to enable only real-time trades to display
	/// [Nano] Apr 30 2010		Modified the exit rules to simplify exits: only exit rule is price crossing adxvmalength2 (slower of the two)
	/// [aknip] May 1 2010		optimized order of parameter list, integrated parameters for exit usage and technique
	/// [Nano] May 5 2010		Added a filter for the CMO: only enter longs when less than cmolimit and only enter shorts when greater than -cmolimit
	/// [Nano] May 8 2010		Added BarsSinceEntry() >= 1 to the Exit Conditions to avoid exits triggered by the entry bar crossing the adxvmalength2
	/// [Nano] May 9 2010		Modified the trailing stoploss control to allow Hurley to trade OK with 1 Target and 1 contract on T1size. Default target is 100 ticks with trailing stoplosses
	/// 							to capture profits
	/// [aknip] May 9 2010		Complete refactoring of order management, works now with IOrders
	/// 							Exits are now executed by first cancelling the stop orders, after execution of cancelment the exit is executed, should avoid phantom positions now
	/// 							New proprerty 'trailactive' controls moving/trailing of stops, including Nano's 'HurleyOne' approach (if set to 0)
	/// [aknip] May 13 2010		Modified trailing-stop('HurleyOne'): Monitoring of trailing now intra-bar; Stop-modifications(=trails) are now fired not only once but everytime the cond.; 
	/// 							is met check for target the criteria are met; target criteria not measured by High[0]/Low[0] but by GetCurrentBid()/GetCurrentAsk()
	/// 						Modified Exit-conditions: Conditions are now checked not only once but everytime the condition is met
	/// [Nano] May 16 2010		Further modifications to 1-target trading to tighten up dynamics SL movements
	/// [Nano] May 21 2010		Added a second version of _ADXVMA_Alertsv01_5_1_Hurley called _ADXVMA_Alertsv01_5_1_Hurley2 - has different colours for Rising and Falling
	/// [Nano] May 24 2010		Minor changes to Target1 and Target 2 (set to 10) and Target5 (set to 20) and One Contract dynamic trail (1st SL change to 5 ticks).
	/// [aknip]May 26 2010		Modified trailing-stop('HurleyOne'): Targets now configurable (Target1,2,3,4,5). SL movement as in mulit-contract version
	/// 						Minor enhancements in logging (new detail logging-level 2) and display of daily PnL in chart
	/// [Nano] 7 June 2010		Changed Exit Rules: trades exit after two reversal bars, and Exit Rules=1 is now the default.
	/// 						An additional filter added for entries to avoid longs/shorts when adxvma is also showing Neutral (gregid's suggestion)
	/// 						Target values adjusted based on back-testing data
	/// [Nano] 7 June 2010		Deleted ADXVMA indicator
	/// / 						Defaulted to Exit Rules=0
	/// [Nano] 8 June 2010		Added d9ParticleOscillator_V2 indicator as a filter
	///// 						Defaulted to Exit Rules=1
	/// [Nano] 9 June 2010		Exit rules changed: only apply when longs are less than and when shorts are greater than 1/4 of target1. Does not apply to runners.
	/// 
	/// </summary>
    [Description("Modified Hurley with cumulative profit and stop targets; use of APZ as  filter; use of d9ParticleOscillatorWVertLineR as a filter")]
    public class Hurley9_5 : Strategy
    {		
        private int		emalength			= 134;
		private bool	emarisingfalling	= true;
		private bool	emaabovebelow		= true;
		
		private int		cmolength			= 8;
		private bool	cmorisingfalling	= false; // changed by Nano
		private bool	cmoabovebelow		= true; // changed by Nano
		
		private double	apzstddev			= 3;
		private int		apzlength			= 22;
		
		private int		target1				= 5;
		private int		target2				= 5;
		private int		target3				= 20;
		private int		target4				= 20;
		private int		target5				= 50;
		
		private double	cumprofittarget		= 1000; // [Nano]
		private double	cumlosstarget		= 1000; // [Nano]
		
		private int		stop				= 10;
		private int		tstop				= -3;
		private int		trailactive			= 5;
		
		private int		t1size				= 1;
		private int		t2size				= 1;
		private int		t3size				= 1;
		private int		t4size				= 1;
		private int		t5size				= 1;
		
		private bool	timeframe			= true; // Added by Nano - turns off the select time frame you want to trade
		
		
		// [tellyTub] Changed ints to string 
		private string 	startTime = @"18:00";
		private string 	endTime = @"00:00";		
		private string 	closingTime = @"00:00";

		// [aknip] Added to fix cumprofittarget/cumlosstarget
		private int		priorTradesCount		= 0;
		private double	priorTradesCumProfit	= 0;
		private bool 	firstBarFlag 			= false;
		private int		maxTradesNumber			= 0;
		
		// [Nano] when true will not display historical trades
		private bool	noHistorical			= true;
		
		private int		exitCondition		= 0;
		private int		closingFlag 		= 0;
		
		
		// [aknip] added for display of performance info in chart
		public enum displaytype
			{
				ProfitLoss,
				Off
			}
		private displaytype showPerformanceData = displaytype.ProfitLoss;
		private double	actprice			= 0;
		
		// [aknip] added for detailed logging
		private int		logLevel			= 0;
		
		private int		exitRules			= 0;
			
		private int 	exitTechnique		= 1;
			
		private double  cmolimit			= 50;
			
		private int		d9length			= 14;
			
		
		// [aknip] added for IOrder based order management	
		private string 	direction = "";
		private IOrder 	entryOrder1 		= null; // These variables hold objects representing our entry orders
		private IOrder 	entryOrder2 		= null; 
		private IOrder 	entryOrder3 		= null; 
		private IOrder 	entryOrder4 		= null; 
		private IOrder 	entryOrder5 		= null; 
		private IOrder 	targetOrder1 		= null; // These variables hold objects representing our target orders
		private IOrder 	targetOrder2 		= null; 
		private IOrder 	targetOrder3 		= null; 
		private IOrder 	targetOrder4 		= null; 
		private IOrder 	targetOrder5 		= null; 
		private IOrder 	stopOrder1 			= null; // These variables hold objects representing our stop orders
		private IOrder 	stopOrder2		 	= null; 
		private IOrder 	stopOrder3 			= null; 
		private IOrder 	stopOrder4 			= null; 
		private IOrder 	stopOrder5 			= null; 
		private IOrder 	exitOrder1 			= null; // These variables hold objects representing our exit orders
		private IOrder 	exitOrder2 			= null;
		private IOrder 	exitOrder3 			= null;
		private IOrder 	exitOrder4 			= null;
		private IOrder 	exitOrder5 			= null;
		private int		dyntrailflag		= 0;
		private int 	closingHrs 			= 0;
		private int 	closingMin 			= 0;
		private bool	OrderStateFlag		= false;
		
		#region DebugFunctions
		private void debug (String message, int detaillevel)
		{
			// Added by Nanop modified by aknip, Note: BID=Sell=StopLong  ASK=Buy=StopShort
			// 
			if (detaillevel == 0)
			{
				Print (this.Name + "; " + message);
			}
			if (detaillevel == 1)
			{
				Print (this.Name + "; "+Time[0]+"; " + message + "; ASK=" + GetCurrentAsk().ToString("F2") + "; BID=" + GetCurrentBid().ToString("F2") + 
				"; CLOSE=" + Close[0].ToString("F2") + "; exitCondition=" + exitCondition + "; dyntrailflag" + dyntrailflag);
			}
			
		}
		
		private void PrintUpdateOrderstate (String orderName, IOrder order)
		{
			// Added by aknip
			//
			
			OrderStateFlag=false;
				
			if (orderName=="entry1") OrderStateFlag=(entryOrder1 != null && entryOrder1.Token == order.Token);
			if (orderName=="entry2") OrderStateFlag=(entryOrder2 != null && entryOrder2.Token == order.Token);
			if (orderName=="entry3") OrderStateFlag=(entryOrder3 != null && entryOrder3.Token == order.Token);
			if (orderName=="entry4") OrderStateFlag=(entryOrder4 != null && entryOrder4.Token == order.Token);
			if (orderName=="entry5") OrderStateFlag=(entryOrder5 != null && entryOrder5.Token == order.Token);
			if (orderName=="target1") OrderStateFlag=(targetOrder1 != null && targetOrder1.Token == order.Token);
			if (orderName=="target2") OrderStateFlag=(targetOrder2 != null && targetOrder2.Token == order.Token);
			if (orderName=="target3") OrderStateFlag=(targetOrder3 != null && targetOrder3.Token == order.Token);
			if (orderName=="target4") OrderStateFlag=(targetOrder4 != null && targetOrder4.Token == order.Token);
			if (orderName=="target5") OrderStateFlag=(targetOrder5 != null && targetOrder5.Token == order.Token);
			if (orderName=="stop1") OrderStateFlag=(stopOrder1 != null && stopOrder1.Token == order.Token);
			if (orderName=="stop2") OrderStateFlag=(stopOrder2 != null && stopOrder2.Token == order.Token);
			if (orderName=="stop3") OrderStateFlag=(stopOrder3 != null && stopOrder3.Token == order.Token);
			if (orderName=="stop4") OrderStateFlag=(stopOrder4 != null && stopOrder4.Token == order.Token);
			if (orderName=="stop5") OrderStateFlag=(stopOrder5 != null && stopOrder5.Token == order.Token);
			if (orderName=="exit1") OrderStateFlag=(exitOrder1 != null && exitOrder1.Token == order.Token);
			if (orderName=="exit2") OrderStateFlag=(exitOrder2 != null && exitOrder2.Token == order.Token);
			if (orderName=="exit3") OrderStateFlag=(exitOrder3 != null && exitOrder3.Token == order.Token);
			if (orderName=="exit4") OrderStateFlag=(exitOrder4 != null && exitOrder4.Token == order.Token);
			if (orderName=="exit5") OrderStateFlag=(exitOrder5 != null && exitOrder5.Token == order.Token);
	
			if (OrderStateFlag)
			{	
				if (order.OrderState == OrderState.Accepted) debug("OrderUpdate: " + orderName + " Accepted", 1);
				if (order.OrderState == OrderState.Cancelled) debug("OrderUpdate: " + orderName + " Cancelled", 1);
				if (order.OrderState == OrderState.Filled) debug("OrderUpdate: " + orderName + " Filled", 1);
				if (order.OrderState == OrderState.PartFilled) debug("OrderUpdate: " + orderName + " PartFilled", 1);
				if (order.OrderState == OrderState.PendingCancel) debug("OrderUpdate: " + orderName + " PendingCancel", 1);
				if (order.OrderState == OrderState.PendingChange) debug("OrderUpdate: " + orderName + " PendingChange", 1);
				if (order.OrderState == OrderState.PendingSubmit) debug("OrderUpdate: " + orderName + " PendingSubmit", 1);
				if (order.OrderState == OrderState.Rejected) debug("OrderUpdate: " + orderName + " Rejected", 1);
				if (order.OrderState == OrderState.Working) debug("OrderUpdate: " + orderName + " Working", 1);
				if (order.OrderState == OrderState.Unknown) debug("OrderUpdate: " + orderName + " Unknown", 1);
			}
		}
		
		
			
		private void PrintExecutionOrderstate (String orderName, IExecution execution)
		{
			// Added by aknip
			//
			
			OrderStateFlag=false;
				
			if (orderName=="entry1") OrderStateFlag=(entryOrder1 != null && entryOrder1.Token == execution.Order.Token);
			if (orderName=="entry2") OrderStateFlag=(entryOrder2 != null && entryOrder2.Token == execution.Order.Token);
			if (orderName=="entry3") OrderStateFlag=(entryOrder3 != null && entryOrder3.Token == execution.Order.Token);
			if (orderName=="entry4") OrderStateFlag=(entryOrder4 != null && entryOrder4.Token == execution.Order.Token);
			if (orderName=="entry5") OrderStateFlag=(entryOrder5 != null && entryOrder5.Token == execution.Order.Token);
			if (orderName=="target1") OrderStateFlag=(targetOrder1 != null && targetOrder1.Token == execution.Order.Token);
			if (orderName=="target2") OrderStateFlag=(targetOrder2 != null && targetOrder2.Token == execution.Order.Token);
			if (orderName=="target3") OrderStateFlag=(targetOrder3 != null && targetOrder3.Token == execution.Order.Token);
			if (orderName=="target4") OrderStateFlag=(targetOrder4 != null && targetOrder4.Token == execution.Order.Token);
			if (orderName=="target5") OrderStateFlag=(targetOrder5 != null && targetOrder5.Token == execution.Order.Token);
			if (orderName=="stop1") OrderStateFlag=(stopOrder1 != null && stopOrder1.Token == execution.Order.Token);
			if (orderName=="stop2") OrderStateFlag=(stopOrder2 != null && stopOrder2.Token == execution.Order.Token);
			if (orderName=="stop3") OrderStateFlag=(stopOrder3 != null && stopOrder3.Token == execution.Order.Token);
			if (orderName=="stop4") OrderStateFlag=(stopOrder4 != null && stopOrder4.Token == execution.Order.Token);
			if (orderName=="stop5") OrderStateFlag=(stopOrder5 != null && stopOrder5.Token == execution.Order.Token);
			if (orderName=="exit1") OrderStateFlag=(exitOrder1 != null && exitOrder1.Token == execution.Order.Token);
			if (orderName=="exit2") OrderStateFlag=(exitOrder2 != null && exitOrder2.Token == execution.Order.Token);
			if (orderName=="exit3") OrderStateFlag=(exitOrder3 != null && exitOrder3.Token == execution.Order.Token);
			if (orderName=="exit4") OrderStateFlag=(exitOrder4 != null && exitOrder4.Token == execution.Order.Token);
			if (orderName=="exit5") OrderStateFlag=(exitOrder5 != null && exitOrder5.Token == execution.Order.Token);
	
	
			if (OrderStateFlag)
			{	
				if (execution.Order.OrderState == OrderState.Accepted) debug("OrderExecution: " + orderName + " Accepted", 1);
				if (execution.Order.OrderState == OrderState.Cancelled) debug("OrderExecution: " + orderName + " Cancelled", 1);
				if (execution.Order.OrderState == OrderState.Filled) debug("OrderExecution: " + orderName + " Filled", 1);
				if (execution.Order.OrderState == OrderState.PartFilled) debug("OrderExecution: " + orderName + " PartFilled", 1);
				if (execution.Order.OrderState == OrderState.PendingCancel) debug("OrderExecution: " + orderName + " PendingCancel", 1);
				if (execution.Order.OrderState == OrderState.PendingChange) debug("OrderExecution: " + orderName + " PendingChange", 1);
				if (execution.Order.OrderState == OrderState.PendingSubmit) debug("OrderExecution: " + orderName + " PendingSubmit", 1);
				if (execution.Order.OrderState == OrderState.Rejected) debug("OrderExecution: " + orderName + " Rejected", 1);
				if (execution.Order.OrderState == OrderState.Working) debug("OrderExecution: " + orderName + " Working", 1);
				if (execution.Order.OrderState == OrderState.Unknown) debug("OrderExecution: " + orderName + " Unknown", 1);
			}
		}
		
		#endregion
		
		
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(d9ParticleOscillator_V2(D9length, 0));// Added by Nano to show d9ParticleOscillator_V2 on chart
			CalculateOnBarClose = true; 
			ExitOnClose			= false;
			// Added by sam028			
			#if NT7
				Enabled				= true;
			#endif
			EntryHandling		= EntryHandling.UniqueEntries;
			
			closingHrs = 0;
            closingMin = 0;
            string[] closingT = closingTime.Split(':');
            if (closingT.Length > 1)
            {
                int.TryParse(closingT[0], out closingHrs);
                int.TryParse(closingT[1], out closingMin);
            }
           
        }
		
        private void GoLong()
		{
			#region Entry Long
	
			direction = "long";
			exitCondition = 0;
			dyntrailflag  = 0;
			exitOrder1 = null;
			exitOrder2 = null;
			exitOrder3 = null;
			exitOrder4 = null;
			exitOrder5 = null;
			
			if (entryOrder1 == null) entryOrder1 = EnterLong(t1size, "entry1");
			
            if (t2size > 0)
            {
                if (entryOrder2 == null) entryOrder2 = EnterLong(T2Size, "entry2");
            }
            if (t3size > 0)
            {
                if (entryOrder3 == null) entryOrder3 = EnterLong(T3Size, "entry3");
            }
			if (t4size > 0)
            {
                if (entryOrder4 == null) entryOrder4 = EnterLong(t4size, "entry4");
            }
            if (t5size > 0)
            {
                if (entryOrder5 == null) entryOrder5 = EnterLong(t5size, "entry5");
            }
			
			#endregion
		}
		
		private void GoShort()
		{
			#region Entry Short
			
			direction = "short";
			exitCondition = 0;
			dyntrailflag = 0;
			exitOrder1 = null;
			exitOrder2 = null;
			exitOrder3 = null;
			exitOrder4 = null;
			exitOrder5 = null;
			
			if (entryOrder1 == null) entryOrder1 = EnterShort(t1size, "entry1");
			
            if (t2size > 0)
            {
                if (entryOrder2 == null) entryOrder2 = EnterShort(t2size, "entry2");
            }
            if (t3size > 0)
            {
                if (entryOrder3 == null) entryOrder3 = EnterShort(t3size, "entry3");
            }
			if (t4size > 0)
            {
                if (entryOrder4 == null) entryOrder4 = EnterShort(t4size, "entry4");
            }
            if (t5size > 0)
            {
                if (entryOrder5 == null) entryOrder5 = EnterShort(t5size, "entry5");
            }
			
			#endregion
		}
		
		private void ExitLong()
		{
			#region Exit Long
			
			if (exitTechnique==1)
			{
				if (stopOrder1 != null) CancelOrder(stopOrder1);
				if (stopOrder2 != null) CancelOrder(stopOrder2);
				if (stopOrder3 != null) CancelOrder(stopOrder3);
				if (stopOrder4 != null) CancelOrder(stopOrder4);
				if (stopOrder5 != null) CancelOrder(stopOrder5);
				
			}
			if (exitTechnique==2)
			{
				if (stopOrder1 != null) stopOrder1 = ExitLongStop(0, true, stopOrder1.Quantity, GetCurrentBid() - 2 * TickSize, "stop1", "entry1");
				if (stopOrder2 != null) stopOrder2 = ExitLongStop(0, true, stopOrder2.Quantity, GetCurrentBid() - 2 * TickSize, "stop2", "entry2");
				if (stopOrder3 != null) stopOrder3 = ExitLongStop(0, true, stopOrder3.Quantity, GetCurrentBid() - 2 * TickSize, "stop3", "entry3");
				if (stopOrder4 != null) stopOrder4 = ExitLongStop(0, true, stopOrder4.Quantity, GetCurrentBid() - 2 * TickSize, "stop4", "entry4");
				if (stopOrder5 != null) stopOrder5 = ExitLongStop(0, true, stopOrder5.Quantity, GetCurrentBid() - 2 * TickSize, "stop5", "entry5");
			}
			if (logLevel >= 1) debug("Exit condtion " + exitCondition + " triggered", 1);
			
			#endregion
		}
		
		
		private void ExitShort()
		{
			#region Exit Short
			
			if (exitTechnique==1)
			{
				if (stopOrder1 != null) CancelOrder(stopOrder1);
				if (stopOrder2 != null) CancelOrder(stopOrder2);
				if (stopOrder3 != null) CancelOrder(stopOrder3);
				if (stopOrder4 != null) CancelOrder(stopOrder4);
				if (stopOrder5 != null) CancelOrder(stopOrder5);
				
			}
			if (exitTechnique==2)
			{
				if (stopOrder1 != null) stopOrder1 = ExitShortStop(0, true, stopOrder1.Quantity, GetCurrentAsk() + 2 * TickSize, "stop1", "entry1");
				if (stopOrder2 != null) stopOrder2 = ExitShortStop(0, true, stopOrder2.Quantity, GetCurrentAsk() + 2 * TickSize, "stop2", "entry2");
				if (stopOrder3 != null) stopOrder3 = ExitShortStop(0, true, stopOrder3.Quantity, GetCurrentAsk() + 2 * TickSize, "stop3", "entry3");
				if (stopOrder4 != null) stopOrder4 = ExitShortStop(0, true, stopOrder4.Quantity, GetCurrentAsk() + 2 * TickSize, "stop4", "entry4");
				if (stopOrder5 != null) stopOrder5 = ExitShortStop(0, true, stopOrder5.Quantity, GetCurrentAsk() + 2 * TickSize, "stop5", "entry5");
			}
			if (logLevel >= 1) debug("Exit condtion " + exitCondition + " triggered", 1);
			
			#endregion
		}
		
		
		
		private void CheckForExits()
		{
			
			// Added by Nano modified by aknip
			// Further modified by Nano 30Apr2010 to decrease complexity
			
			#region Exit rules
			
			if (exitRules == 1)
			{
				if ((Position.MarketPosition == MarketPosition.Long) 
					&& (GetCurrentBid() < Position.AvgPrice + target1/4 * TickSize) // Added by Nano
					&& Close[0] < Open[0] // Modified by Nano
                	&& Close[1] < Open[1]) // Modified by Nano
					
				{
					exitCondition = 1;
				}	
				
				if ((Position.MarketPosition == MarketPosition.Short)  
					&& (GetCurrentAsk() > Position.AvgPrice - target1/4 * TickSize) // Added by Nano
					&& Close[0] > Open[0] // Modified by Nano
                	&& Close[1] > Open[1]) // Modified by Nano
				{
					exitCondition = 2;
				}	
			}
			
			if (exitCondition == 1 && BarsSinceEntry() >= 1) // Modified by Nano
			{
				ExitLong();
			}
			
			if (exitCondition == 2 && BarsSinceEntry() >= 1) // Modified by Nano
			{
				ExitShort();
			}
			
			#endregion
					
		}
		
		
		private void DynamicTrail()
		{
			// This is Nano's "HurleyOne" stop strategy
			// Added by Nanop modified by aknip, Note: BID=Sell=StopLong  ASK=Buy=StopShort
			
			#region HurleyOne Dynamic Trail for one contract
	
			if (Position.MarketPosition == MarketPosition.Long)
			{
                if ((dyntrailflag <=1 ) && (GetCurrentBid() < Position.AvgPrice + ((target1 + target2) * TickSize)) && (GetCurrentBid() >= Position.AvgPrice + (target1 * TickSize)))
                {
                	if (stopOrder1 != null) stopOrder1 = ExitLongStop(0, true, stopOrder1.Quantity, Math.Min(GetCurrentBid(), Position.AvgPrice + tstop * TickSize), "stop1", "entry1");
					dyntrailflag = 1;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 1. trailing (LONG)", 1);
				}
				
                if ((dyntrailflag >=2 ) && (dyntrailflag <=3 ) && (GetCurrentBid() < Position.AvgPrice + ((target1 + target2 + target3) * TickSize)) && (GetCurrentBid() >= Position.AvgPrice + ((target1 + target2) * TickSize)))
                {
                    if (stopOrder1 != null) stopOrder1 = ExitLongStop(0, true, stopOrder1.Quantity, Math.Min(GetCurrentBid(), Position.AvgPrice + target1 * TickSize), "stop1", "entry1");
                	dyntrailflag = 3;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 2. trailing (LONG)", 1);
				}
                
				if ((dyntrailflag >=4 ) && (dyntrailflag <=5 )&& (GetCurrentBid() < Position.AvgPrice + ((target1 + target2 + target3 + target4) * TickSize)) && (GetCurrentBid() >= Position.AvgPrice + ((target1 + target2 + target3) * TickSize)))
                {
                   	if (stopOrder1 != null) stopOrder1 = ExitLongStop(0, true, stopOrder1.Quantity, Math.Min(GetCurrentBid(), Position.AvgPrice + (target1 + target2) * TickSize), "stop1", "entry1");
                	dyntrailflag = 5;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 3. trailing (LONG)", 1);
				}
                
				if ((dyntrailflag >=6 ) && (dyntrailflag <=7 )&& (GetCurrentBid() < Position.AvgPrice + ((target1 + target2 + target3 + target4 + target5) * TickSize)) && (GetCurrentBid() >= Position.AvgPrice + ((target1 + target2 + target3 + target4) * TickSize)))
                {
                    if (stopOrder1 != null) stopOrder1 = ExitLongStop(0, true, stopOrder1.Quantity, Math.Min(GetCurrentBid(), Position.AvgPrice + (target1 + target2 + target3) * TickSize), "stop1", "entry1");
                	dyntrailflag = 7;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 4. trailing (LONG)", 1);
				}	
                
			}
			
			if (Position.MarketPosition == MarketPosition.Short)
			{
                if ((dyntrailflag <=1 ) && (GetCurrentAsk() > Position.AvgPrice - ((target1 + target2) * TickSize)) && (GetCurrentAsk() <= Position.AvgPrice - (target1 * TickSize)))
                {
					if (stopOrder1 != null) stopOrder1 = ExitShortStop(0, true, stopOrder1.Quantity, Math.Max(GetCurrentAsk(), Position.AvgPrice - tstop * TickSize), "stop1", "entry1");
                	dyntrailflag = 1;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 1. trailing (SHORT)", 1);
				}
				
                if ((dyntrailflag >=2 ) && (dyntrailflag <=3 ) &&(GetCurrentAsk() > Position.AvgPrice - ((target1 + target2 + target3) * TickSize)) && (GetCurrentAsk() <= Position.AvgPrice - ((target1 + target2) * TickSize)))
                {
                    if (stopOrder1 != null) stopOrder1 = ExitShortStop(0, true, stopOrder1.Quantity, Math.Max(GetCurrentAsk(), Position.AvgPrice - target1 * TickSize), "stop1", "entry1");
                	dyntrailflag = 3;
					if (logLevel == 3) debug("DynamicTrail 'HurleyOne' 2. trailing (SHORT)", 1);
				}
                
				if ((dyntrailflag >=4 ) && (dyntrailflag <=5 ) &&(GetCurrentAsk() > Position.AvgPrice - ((target1 + target2 + target3 + target4) * TickSize)) && (GetCurrentAsk() <= Position.AvgPrice - ((target1 + target2 + target3) * TickSize)))
                {
                    if (stopOrder1 != null) stopOrder1 = ExitShortStop(0, true, stopOrder1.Quantity, Math.Max(GetCurrentAsk(), Position.AvgPrice - (target1 + target2) * TickSize), "stop1", "entry1");
                	dyntrailflag = 5;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 3. trailing (SHORT)", 1);
				}
                
				if ((dyntrailflag >=6 ) && (dyntrailflag <=7 ) &&(GetCurrentAsk() > Position.AvgPrice - ((target1 + target2 + target3 + target4 + target5) * TickSize)) && (GetCurrentAsk() <= Position.AvgPrice - ((target1 + target2 + target3 + target4) * TickSize)))
                {
                    if (stopOrder1 != null) stopOrder1 = ExitShortStop(0, true, stopOrder1.Quantity, Math.Max(GetCurrentAsk(), Position.AvgPrice - (target1 + target2 + target3) * TickSize), "stop1", "entry1");
                	dyntrailflag = 7;
					if (logLevel >= 1) debug("DynamicTrail 'HurleyOne' 4. trailing (SHORT)", 1);
				}
			}
			#endregion			
		}
		
		
        protected override void OnBarUpdate()
        {
			
			
			#region Check for timeframe and calculate daily P&L
			
			// Added by tellyTub & modified by Nano / aknip
			int startHrs = 18;
            int startMin = 00;
            int endHour = 00;
            int endMin = 00;

            string[] start = startTime.Split(':');
            string[] finish = endTime.Split(':');

            if (start.Length > 1)
            {
                int.TryParse(start[0], out startHrs);
                int.TryParse(start[1], out startMin);
            }
            
            if (finish.Length > 1)
            {
                int.TryParse(finish[0], out endHour);
                int.TryParse(finish[1], out endMin);
            }
			
			if (CurrentBar < 1) return;
			
			
			// This section added by Nano + modified by aknip
			/* Prevents further trading if the current session's cumulative profit or loss targets are realised. */
			
			// At the start of a new session
			// Store the strategy's prior cumulated realized profit and number of trades
			// includes  historical virtual trades AND real-time trades!
			// change to Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit; if only realtime trades should be used!
			if (timeframe)
			{
				if (ToTime(startHrs, startMin, 0) < ToTime(endHour, endMin, 0))
				{
					if ((ToTime(Time[0]) >= ToTime(startHrs, startMin, 0) && ToTime(Time[0]) <= ToTime(endHour, endMin, 0))) 
					{
						if (firstBarFlag == false)
						{
							priorTradesCount = Performance.AllTrades.Count;
							priorTradesCumProfit = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
							firstBarFlag = true;
							closingFlag = 0;
						}
					}
				}
				else
				{
					if ((ToTime(Time[0]) >= ToTime(startHrs, startMin, 0)  || ToTime(Time[0]) <= ToTime(endHour, endMin, 0))) 
					{
						if (firstBarFlag == false)
						{
							priorTradesCount = Performance.AllTrades.Count;
							priorTradesCumProfit = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
							firstBarFlag = true;
							closingFlag = 0;
						}
					}
				} 
				
			}
				else
			{	
				if (Bars.FirstBarOfSession)
				{	
					priorTradesCount = Performance.AllTrades.Count;
					priorTradesCumProfit = Performance.AllTrades.TradesPerformance.Currency.CumProfit;
					closingFlag = 0;
				}
			}
			// Reset FirstBarFlag after session
			if (timeframe)
			{
				if (ToTime(startHrs, startMin, 0) < ToTime(endHour, endMin, 0))
				{
					if ((ToTime(Time[0]) <= ToTime(startHrs, startMin, 0) || ToTime(Time[0]) >= ToTime(endHour, endMin, 0))) firstBarFlag = false;
				}
				else
				{
					if ((ToTime(Time[0]) <= ToTime(startHrs, startMin, 0) && ToTime(Time[0]) >= ToTime(endHour, endMin, 0))) firstBarFlag = false;
				} 
			}
			
			/* Prevents further trading if the current session's cumulative profit or loss targets are realised. */			
			if (Performance.AllTrades.TradesPerformance.Currency.CumProfit - priorTradesCumProfit >= cumprofittarget || 
				Performance.AllTrades.TradesPerformance.Currency.CumProfit - priorTradesCumProfit <= -cumlosstarget)
			{
				return;
			}
			
			 
			// * Prevents further trading if "maxTradesNumber" trades have already been made in this session.	
			if (maxTradesNumber > 0)
			{
				if (Performance.AllTrades.Count - priorTradesCount >= maxTradesNumber)
				{
					return;
				}
			}
			
			
			
			if (NoHistorical && Historical) return; // [Nano] only takes real time trades, and does not display historical trades if NoHistorical is true
			
			// Added by tellyTub & modified by Nano / aknip
			if (timeframe)
			{
				if (ToTime(startHrs, startMin, 0) < ToTime(endHour, endMin, 0))
				{
					if ((ToTime(Time[0]) <= ToTime(startHrs, startMin, 0) || ToTime(Time[0]) >= ToTime(endHour, endMin, 0))) return;
				}
				else
				{
					if ((ToTime(Time[0]) <= ToTime(startHrs, startMin, 0) && ToTime(Time[0]) >= ToTime(endHour, endMin, 0))) return;
				} 
			}		
			
			#endregion
			
			if (Position.MarketPosition != MarketPosition.Flat) CheckForExits(); // Call Exit condition check
			
			// [aknip] Check for trailing stopps 
			// HERE: ONLY FOR BACKTESTING (HISTORICAL), REALTIME TRAILING in OnMarketData() EVENT !!!
			if (Historical && (exitCondition == 0) && (trailactive == 0) && (Position.MarketPosition != MarketPosition.Flat)) DynamicTrail(); // "HurleyOne"
			
			if (Position.MarketPosition != MarketPosition.Flat) return;
			
			#region Calculate entry condtions
			
			bool	_cmolong	= false;
			bool	_cmoshort	= false;
			bool	_emalong	= false;
			bool	_emashort	= false;
			
			EMA		emav		= EMA(EMAlength);
			CMO		cmov		= CMO(CMOlength);
			APZ		apzuv		= APZ(High, APZstddev, APZlength);
			APZ		apzlv		= APZ(Low, APZstddev, APZlength);
			
			// changed by sflanagan98 from H E R E  2010Mar31
				if (cmoabovebelow && cmorisingfalling)
			{
							
				if (cmov[0] > 0 && Rising(cmov)
					&& (CMO(cmolength)[0] < cmolimit)) // mod by Nano
			    {		
                    _cmolong = true;
				}
			   
				 if (cmov[0] < 0 && Falling(cmov)
					&& (CMO(cmolength)[0] > -cmolimit)) // mod by Nano
			        { 		
            	    _cmoshort = true;
			        }
			}
			else if (cmoabovebelow )
			
			{
                if (cmov[0] > 0
					&& (CMO(cmolength)[0] < cmolimit)) // mod by Nano
                {
                    _cmolong = true;
                }
                if (cmov[0] < 0
					&& (CMO(cmolength)[0] > -cmolimit)) // mod by Nano
                {
                    _cmoshort = true;
                }
			}

			else if (cmorisingfalling)
			{
                if (Rising(cmov)
					&& (CMO(cmolength)[0] < cmolimit)) // mod by Nano
                {
                    _cmolong = true;
                }
                if (Falling(cmov)
					&& (CMO(cmolength)[0] > -cmolimit)) // mod by Nano
                {
                    _cmoshort = true;
                }
			}

	
			if (emaabovebelow && emarisingfalling)
			{
				
			if (Close[0] > emav[0] && Rising(emav))
				{
					_emalong = true;
				}
			
			if (Close[0] < emav[0] && Falling(emav))
				{
					_emashort = true;
				}
			}	
			else if (emaabovebelow)
			{
				if (Close[0] > emav[0])
				{
					_emalong = true;
				}
				
			    if (Close[0] < emav[0])
				{
					_emashort = true;
				}
				
			}

			else if (emarisingfalling)
			{
				if (Rising(emav))
				{
					_emalong = true;
				}
				if (Falling(emav))				
				{
					_emashort = true;
				}
			}
			
			#endregion
			
			// changed by sflanagan98 T H R O U G H    H E R E  2010Mar31
			// Added by Nano & modified by Nano 12 March 2010	
			
			if (exitTechnique == 3)
				{
					// aknip: Experimental SIMPLE mode, for testing only
					if (d9ParticleOscillator_V2(d9length, 0).DotU[0] == 0)
					{
						if (logLevel >= 1)
						{		
							debug("", 0);
							debug("Entry LONG triggered", 1);
							debug("                     Entry conditions:" , 0);
							debug("                        _cmolong : " + _cmolong , 0);
							debug("                        _emalong : " + _emalong , 0);
							debug("                        Low[0] / apzlv.Upper[1] : " + Low[0] + " / " + apzlv.Upper[1] , 0);
							debug("                        APZ().Upper[0] / APZ().Upper[1] : " + APZ(apzstddev, apzlength).Upper[0] + " / " +  APZ(apzstddev, apzlength).Upper[1] , 0);
							debug("                        APZ().Lower[0] / APZ().Lower[1] : " + APZ(apzstddev, apzlength).Lower[0] + " / " +  APZ(apzstddev, apzlength).Lower[1] , 0);
						}
						GoLong();
					}	
					else if (d9ParticleOscillator_V2(d9length, 0).DotL[0] == 0)
					{
						if (logLevel >= 1)
						{		
							debug("",0);
							debug("Entry SHORT triggered", 1);
							debug("                     Entry conditions:" , 0);
							debug("                        _cmolong : " + _cmolong , 0);
							debug("                        _emalong : " + _emalong , 0);
							debug("                        Low[0] / apzlv.Upper[1] : " + Low[0] + " / " + apzlv.Upper[1] , 0);
							debug("                        APZ().Upper[0] / APZ().Upper[1] : " + APZ(apzstddev, apzlength).Upper[0] + " / " +  APZ(apzstddev, apzlength).Upper[1] , 0);
							debug("                        APZ().Lower[0] / APZ().Lower[1] : " + APZ(apzstddev, apzlength).Lower[0] + " / " +  APZ(apzstddev, apzlength).Lower[1] , 0);
						}
						
						GoShort();
					}
				}
			else
				{
					if (_cmolong && _emalong && Low[0] <= apzlv.Upper[1]
						&& d9ParticleOscillator_V2(D9length, 0).DotU[0] == 0
						&& d9ParticleOscillator_V2(D9length, 0).DotWarn[0] != 0
						&& d9ParticleOscillator_V2(D9length, 0).Prediction[0] > d9ParticleOscillator_V2(D9length, 0).Prediction[1]
						&& (APZ(apzstddev, apzlength).Upper[0] > APZ(apzstddev, apzlength).Upper[1])
						&& (APZ(apzstddev, apzlength).Lower[0] > APZ(apzstddev, apzlength).Lower[1]))
						
					{
						if (logLevel >= 1)
						{		
							debug("", 0);
							debug("Entry LONG triggered", 1);
							debug("                     Entry conditions:" , 0);
							debug("                        _cmolong : " + _cmolong , 0);
							debug("                        _emalong : " + _emalong , 0);
							debug("                        Low[0] / apzlv.Upper[1] : " + Low[0] + " / " + apzlv.Upper[1] , 0);
							debug("                        APZ().Upper[0] / APZ().Upper[1] : " + APZ(apzstddev, apzlength).Upper[0] + " / " +  APZ(apzstddev, apzlength).Upper[1] , 0);
							debug("                        APZ().Lower[0] / APZ().Lower[1] : " + APZ(apzstddev, apzlength).Lower[0] + " / " +  APZ(apzstddev, apzlength).Lower[1] , 0);
						}
						GoLong();
					}
					else if (_cmoshort && _emashort && High[0] >= apzuv.Lower[1]
						&& d9ParticleOscillator_V2(D9length, 0).DotL[0] == 0
						&& d9ParticleOscillator_V2(D9length, 0).DotWarn[0] != 0
						&& d9ParticleOscillator_V2(D9length, 0).Prediction[0] < d9ParticleOscillator_V2(D9length, 0).Prediction[1]
						&& (APZ(apzstddev, apzlength).Upper[0] < APZ(apzstddev, apzlength).Upper[1])
						&& (APZ(apzstddev, apzlength).Lower[0] < APZ(apzstddev, apzlength).Lower[1]))
					{
						if (logLevel >= 1)
						{		
							debug("",0);
							debug("Entry SHORT triggered", 1);
							debug("                     Entry conditions:" , 0);
							debug("                        _cmolong : " + _cmolong , 0);
							debug("                        _emalong : " + _emalong , 0);
							debug("                        Low[0] / apzlv.Upper[1] : " + Low[0] + " / " + apzlv.Upper[1] , 0);
							debug("                        APZ().Upper[0] / APZ().Upper[1] : " + APZ(apzstddev, apzlength).Upper[0] + " / " +  APZ(apzstddev, apzlength).Upper[1] , 0);
							debug("                        APZ().Lower[0] / APZ().Lower[1] : " + APZ(apzstddev, apzlength).Lower[0] + " / " +  APZ(apzstddev, apzlength).Lower[1] , 0);
						}
						GoShort();
					}
				}
			}

		
		//added by aknip
        protected override void OnOrderUpdate(IOrder order)
        {
			// Handle entry orders. The entryOrder object allows us to identify 
			// that the order that is calling the OnOrderUpdate() method 
			// is the entry order
			
			if (logLevel >= 3)
			{
				PrintUpdateOrderstate("entry1", order);
				PrintUpdateOrderstate("entry2", order);
				PrintUpdateOrderstate("entry3", order);
				PrintUpdateOrderstate("entry4", order);
				PrintUpdateOrderstate("entry5", order);
				PrintUpdateOrderstate("target1", order);
				PrintUpdateOrderstate("target2", order);
				PrintUpdateOrderstate("target3", order);
				PrintUpdateOrderstate("target4", order);
				PrintUpdateOrderstate("target5", order);
				PrintUpdateOrderstate("stop1", order);
				PrintUpdateOrderstate("stop2", order);
				PrintUpdateOrderstate("stop3", order);
				PrintUpdateOrderstate("stop4", order);
				PrintUpdateOrderstate("stop5", order);
				PrintUpdateOrderstate("exit1", order);
				PrintUpdateOrderstate("exit2", order);
				PrintUpdateOrderstate("exit3", order);
				PrintUpdateOrderstate("exit4", order);
				PrintUpdateOrderstate("exit5", order);
				
			}

			
			// [aknip] Check for trailing stopp movement execution : INTRABAR
			if ((dyntrailflag==1) && (trailactive == 0) && (Position.MarketPosition != MarketPosition.Flat)) 
			{	
				if (stopOrder1 != null && stopOrder1.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Accepted) 
					{
						debug("DynamicTrail 'HurleyOne' 1. trailing : stop movement Accepted", 1);
						dyntrailflag=2;
					}	
				}	
			}
			if ((dyntrailflag==3) && (trailactive == 0) && (Position.MarketPosition != MarketPosition.Flat)) 
			{	
				if (stopOrder1 != null && stopOrder1.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Accepted) 
					{
						debug("DynamicTrail 'HurleyOne' 2. trailing : stop movement Accepted", 1);
						dyntrailflag=4;
					}	
				}	
			}
			if ((dyntrailflag==5) && (trailactive == 0) && (Position.MarketPosition != MarketPosition.Flat)) 
			{	
				if (stopOrder1 != null && stopOrder1.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Accepted) 
					{
						debug("DynamicTrail 'HurleyOne' 3. trailing : stop movement Accepted", 1);
						dyntrailflag=6;
					}	
				}	
			}
			if ((dyntrailflag==7) && (trailactive == 0) && (Position.MarketPosition != MarketPosition.Flat)) 
			{	
				if (stopOrder1 != null && stopOrder1.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Accepted) 
					{
						debug("DynamicTrail 'HurleyOne' 4. trailing : stop movement Accepted", 1);
						dyntrailflag=8;
					}	
				}	
			}
			

			
			#region Check for Stop-Cancellation and Call Exits
			if (exitCondition != 0)
			{
				if (stopOrder1 != null && stopOrder1.Token == order.Token)
				{	
					// Check
					if (order.OrderState == OrderState.Cancelled)
					{
						if (entryOrder1 == null) 
						{
							if (logLevel >= 1) debug("stop1 cancelled", 1);
							if (direction=="long") 
							{
								exitOrder1 = ExitLong("entry1"); 
								if (logLevel >= 1) debug("Exit-LONG contract 1 called after Stop cancellation", 1);
							}
							else 
							{
								exitOrder1 = ExitShort("entry1");
								if (logLevel >= 1) debug("Exit-SHORT contract 1 called after Stop cancellation", 1);
							}
						}
					}
				}
				
				if (t2size > 0)
				{
					if (stopOrder2 != null && stopOrder2.Token == order.Token)
					{	
						// Check
						if (order.OrderState == OrderState.Cancelled)
						{
							if (entryOrder2 == null) 
							{
								if (logLevel >= 1) debug("stop2 cancelled", 1);
								if (direction=="long") 
								{
									ExitLong("entry2"); 
									if (logLevel >= 1) debug("Exit-LONG contract 2 called after Stop cancellation", 1);
								}
								else 
								{
									ExitShort("entry2");
									if (logLevel >= 1) debug("Exit-SHORT contract 2 called after Stop cancellation", 1);
								}
							}
						}
					}
				}
				
				if (t3size > 0)
				{
					if (stopOrder3 != null && stopOrder3.Token == order.Token)
					{	
						// Check
						if (order.OrderState == OrderState.Cancelled)
						{
							if (entryOrder3 == null) 
							{
								if (logLevel >= 1) debug("stop3 cancelled", 1);
								if (direction=="long") 
								{
									ExitLong("entry3"); 
									if (logLevel >= 1) debug("Exit-LONG contract 3 called after Stop cancellation", 1);
								}
								else 
								{
									ExitShort("entry3");
									if (logLevel >= 1) debug("Exit-SHORT contract 3 called after Stop cancellation", 1);
								}
							}
						}
					}
				}
				
				if (t4size > 0)
				{
					if (stopOrder4 != null && stopOrder4.Token == order.Token)
					{	
						// Check
						if (order.OrderState == OrderState.Cancelled)
						{
							if (entryOrder4 == null) 
							{
								if (logLevel >= 1) debug("stop4 cancelled", 1);
								if (direction=="long") 
								{
									ExitLong("entry4"); 
									if (logLevel >= 1) debug("Exit-LONG contract 4 called after Stop cancellation", 1);
								}
								else 
								{
									ExitShort("entry4");
									if (logLevel >= 1) debug("Exit-SHORT contract 4 called after Stop cancellation", 1);
								}
							}
						}
					}
				}
				
				if (t5size > 0)
				{
					if (stopOrder5 != null && stopOrder5.Token == order.Token)
					{	
						// Check
						if (order.OrderState == OrderState.Cancelled)
						{
							if (entryOrder5 == null) 
							{
								if (logLevel >= 1) debug("stop5 cancelled", 1);
								if (direction=="long") 
								{
									ExitLong("entry5"); 
									if (logLevel >= 1) debug("Exit-LONG contract 5 called after Stop cancellation", 1);
								}
								else 
								{
									ExitShort("entry5");
									if (logLevel >= 1) debug("Exit-SHORT contract 5 called after Stop cancellation", 1);
								}
							}
						}
					}
				}
			}
			
			
			#endregion
			
			#region Reset the entryOrder objects if order was cancelled 
			
			// Reset the entryOrder object to null if order was cancelled 
			// without any fill
			if (entryOrder1 != null && entryOrder1.Token == order.Token)
			{	
				if (order.OrderState == OrderState.Cancelled && order.Filled == 0) entryOrder1 = null;
			}
			
			if (t2size > 0)
			{
				if (entryOrder2 != null && entryOrder2.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Cancelled && order.Filled == 0) entryOrder2 = null;
				}
			}
			
			if (t3size > 0)
			{
				if (entryOrder3 != null && entryOrder3.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Cancelled && order.Filled == 0) entryOrder3 = null;
				}
			}
			
			if (t4size > 0)
			{
				if (entryOrder4 != null && entryOrder4.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Cancelled && order.Filled == 0) entryOrder4 = null;
				}
			}
			
			if (t5size > 0)
			{
				if (entryOrder5 != null && entryOrder5.Token == order.Token)
				{	
					if (order.OrderState == OrderState.Cancelled && order.Filled == 0) entryOrder5 = null;
				}
			}
			
			#endregion
		}
		
		
		//added by aknip
		protected override void OnExecution(IExecution execution)
        {
			// We use monitoring OnExecution to trigger submission of stop/target orders 
			// instead of OnOrderUpdate() since OnExecution() is called after 
			// OnOrderUpdate() which ensures our strategy has received the execution 
			// which is used for internal signal tracking. 
			
			if (logLevel >= 2)
			{
				PrintExecutionOrderstate("entry1", execution);
				PrintExecutionOrderstate("entry2", execution);
				PrintExecutionOrderstate("entry3", execution);
				PrintExecutionOrderstate("entry4", execution);
				PrintExecutionOrderstate("entry5", execution);
				PrintExecutionOrderstate("target1", execution);
				PrintExecutionOrderstate("target2", execution);
				PrintExecutionOrderstate("target3", execution);
				PrintExecutionOrderstate("target4", execution);
				PrintExecutionOrderstate("target5", execution);
				PrintExecutionOrderstate("stop1", execution);
				PrintExecutionOrderstate("stop2", execution);
				PrintExecutionOrderstate("stop3", execution);
				PrintExecutionOrderstate("stop4", execution);
				PrintExecutionOrderstate("stop5", execution);
				PrintExecutionOrderstate("exit1", execution);
				PrintExecutionOrderstate("exit2", execution);
				PrintExecutionOrderstate("exit3", execution);
				PrintExecutionOrderstate("exit4", execution);
				PrintExecutionOrderstate("exit5", execution);
				
			}
			
			
			#region Place initial stop / target orders for contracts 1-5 (on entry/fill)
			
			// Place stop / target orders for contract 1
			if (entryOrder1 != null && entryOrder1.Token == execution.Order.Token)
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
				{							
					if (direction=="long")
					{
						stopOrder1 	= ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - (stop * TickSize), "stop1", "entry1");
						if (trailactive == 0) 
								targetOrder1 = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + ((target1+target2+target3+target4+target5)*TickSize), "target1", "entry1");	
							else
								targetOrder1 = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + (target1*TickSize), "target1", "entry1");	
						if (logLevel >= 1) debug("target1/stop1 order placed (LONG)", 1);
					}
					else
					{
						stopOrder1 	= ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + (stop * TickSize), "stop1", "entry1");
						if (trailactive == 0)
								targetOrder1 = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - ((target1+target2+target3+target4+target5)*TickSize), "target1", "entry1");	
							else
								targetOrder1 = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - (target1*TickSize), "target1", "entry1");	
						if (logLevel >= 1) debug("target1/stop1 order placed (SHORT)", 1);
					}	
					
					// Resets the entryOrder object to null after the order has been filled or partially filled
					if (execution.Order.OrderState != OrderState.PartFilled)
					{
						entryOrder1	= null;
					}
				}
			}
			
			// Place stop / target orders for contract 2
			if (t2size > 0)
			{
				if (entryOrder2 != null && entryOrder2.Token == execution.Order.Token)
				{
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
					{							
						if (direction=="long")
						{
							stopOrder2 	= ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - (stop * TickSize), "stop2", "entry2");
							targetOrder2 = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + ((target1 + target2) * TickSize), "target2", "entry2");	
							if (logLevel >= 1) debug("target2/stop2 order placed (LONG)", 1);
						}
						else
						{
							stopOrder2 	= ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + (stop * TickSize), "stop2", "entry2");
							targetOrder2 = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - ((target1 + target2) * TickSize), "target2", "entry2");	
							if (logLevel >= 1) debug("target2/stop2 order placed (SHORT)", 1);
						}	
						
						// Resets the entryOrder object to null after the order has been filled or partially filled
						if (execution.Order.OrderState != OrderState.PartFilled)
						{
							entryOrder2	= null;
						}
					}
				}	
			}
			
			// Place stop / target orders for contract 3
			if (t3size > 0)
			{
				if (entryOrder3 != null && entryOrder3.Token == execution.Order.Token)
				{
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
					{							
						if (direction=="long")
						{
							stopOrder3 	= ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - (stop * TickSize), "stop3", "entry3");
							targetOrder3 = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + ((target1 + target2 + target3) * TickSize), "target3", "entry3");	
							if (logLevel >= 1) debug("target3/stop3 order placed (LONG)", 1);
						}
						else
						{
							stopOrder3 	= ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + (stop * TickSize), "stop3", "entry3");
							targetOrder3 = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - ((target1 + target2 + target3) * TickSize), "target3", "entry3");	
							if (logLevel >= 1) debug("target3/stop3 order placed (SHORT)", 1);
						}	
						
						// Resets the entryOrder object to null after the order has been filled or partially filled
						if (execution.Order.OrderState != OrderState.PartFilled)
						{
							entryOrder3	= null;
						}
					}
				}	
			}
			
			// Place stop / target orders for contract 4
			if (t4size > 0)
			{
				if (entryOrder4 != null && entryOrder4.Token == execution.Order.Token)
				{
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
					{							
						if (direction=="long")
						{
							stopOrder4 	= ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - (stop * TickSize), "stop4", "entry4");
							targetOrder4 = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + ((target1 + target2 + target3 + target4) * TickSize), "target4", "entry4");	
							if (logLevel >= 1) debug("target4/stop4 order placed (LONG)", 1);
						}
						else
						{
							stopOrder4 	= ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + (stop * TickSize), "stop4", "entry4");
							targetOrder4 = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - ((target1 + target2 + target3 + target4) * TickSize), "target4", "entry4");	
							if (logLevel >= 1) debug("target4/stop4 order placed (SHORT)", 1);
						}	
						
						// Resets the entryOrder object to null after the order has been filled or partially filled
						if (execution.Order.OrderState != OrderState.PartFilled)
						{
							entryOrder4	= null;
						}
					}
				}	
			}
			
			// Place stop / target orders for contract 5
			if (t5size > 0)
			{
				if (entryOrder5 != null && entryOrder5.Token == execution.Order.Token)
				{
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
					{							
						if (direction=="long")
						{
							stopOrder5 	= ExitLongStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - (stop * TickSize), "stop5", "entry5");
							targetOrder5 = ExitLongLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + ((target1 + target2 + target3 + target4 + target5) * TickSize), "target5", "entry5");	
							if (logLevel >= 1) debug("target5/stop5 order placed (LONG)", 1);
						}
						else
						{
							stopOrder5 	= ExitShortStop(0, true, execution.Order.Filled, execution.Order.AvgFillPrice + (stop * TickSize), "stop5", "entry5");
							targetOrder5 = ExitShortLimit(0, true, execution.Order.Filled, execution.Order.AvgFillPrice - ((target1 + target2 + target3 + target4 + target5) * TickSize), "target5", "entry5");	
							if (logLevel >= 1) debug("target5/stop5 order placed (SHORT)", 1);
						}	
						
						// Resets the entryOrder object to null after the order has been filled or partially filled
						if (execution.Order.OrderState != OrderState.PartFilled)
						{
							entryOrder5	= null;
						}
					}
				}	
			}
			
			#endregion
			
			#region Trailing Stop of contracts 2-5
			
			
			// Target 1 hit
			// Checks to see if Target1 has been reached and our Stop Order has been submitted already
			if ((trailactive >= 1) && (targetOrder1 != null && targetOrder1.Token == execution.Order.Token))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					
					if (logLevel >= 1) debug("target1 hit & filled", 1);
					if (direction=="long")
					{
						if (t2size > 0)
						{
							if (stopOrder2 != null && stopOrder2.StopPrice < Position.AvgPrice + tstop * TickSize)
							{
								stopOrder2 = ExitLongStop(0, true, stopOrder2.Quantity, Position.AvgPrice + tstop * TickSize, "stop2", "entry2");
								if (logLevel >= 1) debug("stop2 1. trailing (LONG)", 1);
							}
						}
						if (t3size > 0)
						{
							if (stopOrder3 != null && stopOrder3.StopPrice < Position.AvgPrice + tstop * TickSize)
							{
								stopOrder3 = ExitLongStop(0, true, stopOrder3.Quantity, Position.AvgPrice + tstop * TickSize, "stop3", "entry3");
								if (logLevel >= 1) debug("stop3 1. trailing (LONG)", 1);
							}	
						}
						if (t4size > 0)
						{
							if (stopOrder4 != null && stopOrder4.StopPrice < Position.AvgPrice + tstop * TickSize)
							{
								stopOrder4 = ExitLongStop(0, true, stopOrder4.Quantity, Position.AvgPrice + tstop * TickSize, "stop4", "entry4");
								if (logLevel >= 1) debug("stop4 1. trailing (LONG)", 1);
							}	
						}
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice < Position.AvgPrice + tstop * TickSize)
							{
								stopOrder5 = ExitLongStop(0, true, stopOrder5.Quantity, Position.AvgPrice + tstop * TickSize, "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 1. trailing (LONG)", 1);
							}	
						}
						
					}
					else
					{
						if (t2size > 0)
						{
							if (stopOrder2 != null && stopOrder2.StopPrice > Position.AvgPrice - tstop * TickSize)
							{
								stopOrder2 = ExitShortStop(0, true, stopOrder2.Quantity, Position.AvgPrice - tstop * TickSize, "stop2", "entry2");
								if (logLevel >= 1) debug("stop2 1. trailing (SHORT)", 1);
							}
						}
						if (t3size > 0)
						{
							if (stopOrder3 != null && stopOrder3.StopPrice > Position.AvgPrice - tstop * TickSize)
							{
								stopOrder3 = ExitShortStop(0, true, stopOrder3.Quantity, Position.AvgPrice - tstop * TickSize, "stop3", "entry3");
								if (logLevel >= 1) debug("stop3 1. trailing (SHORT)", 1);
							}
						}
						if (t4size > 0)
						{
							if (stopOrder4 != null && stopOrder4.StopPrice > Position.AvgPrice - tstop * TickSize)
							{
								stopOrder4 = ExitShortStop(0, true, stopOrder4.Quantity, Position.AvgPrice - tstop * TickSize, "stop4", "entry4");
								if (logLevel >= 1) debug("stop4 1. trailing (SHORT)", 1);
							}
						}
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice > Position.AvgPrice  - tstop * TickSize)
							{
								stopOrder5 = ExitShortStop(0, true, stopOrder5.Quantity, Position.AvgPrice - tstop * TickSize, "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 1. trailing (SHORT)", 1);
							}
						}
						
					}
					
				}
			}
			
			// Target 2 hit
			// Checks to see if Target2 has been reached and our Stop Order has been submitted already
			if ((trailactive >= 2) && (targetOrder2 != null && targetOrder2.Token == execution.Order.Token))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					if (logLevel >= 1) debug("target2 hit & filled", 1);
					if (direction=="long")
					{
						if (t3size > 0)
						{
							if (stopOrder3 != null && stopOrder3.StopPrice < Position.AvgPrice + (target1*TickSize))
							{
								stopOrder3 = ExitLongStop(0, true, stopOrder3.Quantity, Position.AvgPrice + (target1*TickSize), "stop3", "entry3");
								if (logLevel >= 1) debug("stop3 2. trailing (LONG)", 1);
							}	
						}
						if (t4size > 0)
						{
							if (stopOrder4 != null && stopOrder4.StopPrice < Position.AvgPrice + (target1*TickSize))
							{
								stopOrder4 = ExitLongStop(0, true, stopOrder4.Quantity, Position.AvgPrice + (target1*TickSize), "stop4", "entry4");
								if (logLevel >= 1) debug("stop4 2. trailing (LONG)", 1);
							}	
						}
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice < Position.AvgPrice + (target1*TickSize))
							{
								stopOrder5 = ExitLongStop(0, true, stopOrder5.Quantity, Position.AvgPrice + (target1*TickSize), "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 2. trailing (LONG)", 1);
							}	
						}
						
					}
					else
					{
						if (t3size > 0)
						{
							if (stopOrder3 != null && stopOrder3.StopPrice > Position.AvgPrice - (target1*TickSize))
							{
								stopOrder3 = ExitShortStop(0, true, stopOrder3.Quantity, Position.AvgPrice - (target1*TickSize), "stop3", "entry3");
								if (logLevel >= 1) debug("stop3 2. trailing (SHORT)", 1);
							}
						}
						if (t4size > 0)
						{
							if (stopOrder4 != null && stopOrder4.StopPrice > Position.AvgPrice - (target1*TickSize))
							{
								stopOrder4 = ExitShortStop(0, true, stopOrder4.Quantity, Position.AvgPrice - (target1*TickSize), "stop4", "entry4");
								if (logLevel >= 1) debug("stop4 2. trailing (SHORT)", 1);
							}
						}
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice > Position.AvgPrice - (target1*TickSize))
							{
								stopOrder5 = ExitShortStop(0, true, stopOrder5.Quantity, Position.AvgPrice - (target1*TickSize), "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 2. trailing (SHORT)", 1);
							}
						}
						
					}
					
				}
			}
			
			
			
			// Target 3 hit
			// Checks to see if Target3 has been reached and our Stop Order has been submitted already
			if ((trailactive >= 3) && (targetOrder3 != null && targetOrder3.Token == execution.Order.Token))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					if (logLevel >= 1) debug("target3 hit & filled", 1);
					if (direction=="long")
					{
						if (t4size > 0)
						{
							if (stopOrder4 != null && stopOrder4.StopPrice < Position.AvgPrice + ((target1+target2)*TickSize))
							{
								stopOrder4 = ExitLongStop(0, true, stopOrder4.Quantity, Position.AvgPrice + ((target1+target2)*TickSize), "stop4", "entry4");
								if (logLevel >= 1) debug("stop4 3. trailing (LONG)", 1);
							}	
						}
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice < Position.AvgPrice + ((target1+target2)*TickSize))
							{
								stopOrder5 = ExitLongStop(0, true, stopOrder5.Quantity, Position.AvgPrice + ((target1+target2)*TickSize), "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 3. trailing (LONG)", 1);
							}	
						}
						
					}
					else
					{
						if (t4size > 0)
						{
							if (stopOrder4 != null && stopOrder4.StopPrice > Position.AvgPrice - ((target1+target2)*TickSize))
							{
								stopOrder4 = ExitShortStop(0, true, stopOrder4.Quantity, Position.AvgPrice - ((target1+target2)*TickSize), "stop4", "entry4");
								if (logLevel >= 1) debug("stop4 3. trailing (SHORT)", 1);
							}
						}
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice > Position.AvgPrice - ((target1+target2)*TickSize))
							{
								stopOrder5 = ExitShortStop(0, true, stopOrder5.Quantity, Position.AvgPrice - ((target1+target2)*TickSize), "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 3. trailing (SHORT)", 1);
							}
						}
						
					}
					
				}
			}
			
			// Target 4 hit
			// Checks to see if Target4 has been reached and our Stop Order has been submitted already
			if ((trailactive >= 4) && (targetOrder4 != null && targetOrder4.Token == execution.Order.Token))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					if (logLevel >= 1) debug("target4 hit & filled", 1);
					if (direction=="long")
					{
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice < Position.AvgPrice + ((target1+target2+target3)*TickSize))
							{
								stopOrder5 = ExitLongStop(0, true, stopOrder5.Quantity, Position.AvgPrice + ((target1+target2+target3)*TickSize), "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 4. trailing (LONG)", 1);
							}	
						}
						
					}
					else
					{
						if (t5size > 0)
						{
							if (stopOrder5 != null && stopOrder5.StopPrice > Position.AvgPrice - ((target1+target2+target3)*TickSize))
							{
								stopOrder5 = ExitShortStop(0, true, stopOrder5.Quantity, Position.AvgPrice - ((target1+target2+target3)*TickSize), "stop5", "entry5");
								if (logLevel >= 1) debug("stop5 4. trailing (SHORT)", 1);
							}
						}
						
					}
					
				}
			}
			
			// Reset our stop order and target orders' IOrder objects after our position is closed.
			if ((stopOrder1 != null && stopOrder1.Token == execution.Order.Token))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					if (logLevel >= 1) debug("stop1 hit & filled", 1);
					stopOrder1 = null;
					targetOrder1 = null;
				}
			}
			if ((targetOrder1 != null && targetOrder1.Token == execution.Order.Token))
			{
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
				{
					if (logLevel >= 1) debug("target1 hit & filled", 1);
					stopOrder1 = null;
					targetOrder1 = null;
				}
			}
			
			if (t2size > 0)
			{	
				if ((stopOrder2 != null && stopOrder2.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("stop2 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder2 = null;
						targetOrder2 = null;	
					}
				}
				if ((targetOrder2 != null && targetOrder2.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("target2 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder2 = null;
						targetOrder2 = null;	
					}
				}
			}
			if (t3size > 0)
			{
				if ((stopOrder3 != null && stopOrder3.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("stop3 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder3 = null;
						targetOrder3 = null;	
					}
				}
				if ((targetOrder3 != null && targetOrder3.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("target3 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder3 = null;
						targetOrder3 = null;	
					}
				}
			}
			if (t4size > 0)
			{
				if ((stopOrder4 != null && stopOrder4.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("stop4 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder4 = null;
						targetOrder4 = null;	
					}
				}
				if ((targetOrder4 != null && targetOrder4.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("target4 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder4 = null;
						targetOrder4 = null;	
					}
				}
			}
			if (t5size > 0)
			{
				if ((stopOrder5 != null && stopOrder5.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("stop5 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder5 = null;
						targetOrder5 = null;	
					}
				}
				if ((targetOrder5 != null && targetOrder5.Token == execution.Order.Token))
				{
					if (logLevel >= 1) debug("target5 hit & filled", 1);
					if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled)
					{
						stopOrder5 = null;
						targetOrder5 = null;	
					}
				}
			}
			
			#endregion
			
		}
		
		
		//added by aknip
		protected override void OnMarketData(MarketDataEventArgs e) 
        {
			
			#region Auto Close all positions at defined time
			
			if ((timeframe) && (closingHrs != 0) && (closingMin != 0))
			{
				if ((ToTime(Time[0]) >= ToTime(closingHrs, closingMin, 0)) && (closingFlag == 0))
				{
					closingFlag = 1;
					// close all
					debug("*** Auto-closing: Close all positions", 1);
					if (direction == "long")
					{
						if (entryOrder1 == null) 
						{
							exitOrder1 = ExitLong("entry1"); 
						}
						else 
						{
							CancelOrder(entryOrder1);
						}
						
						if (t2size > 0)
						{
							if (entryOrder2 == null) 
							{
								exitOrder2 = ExitLong("entry2"); ;
							}
							else 
							{
								CancelOrder(entryOrder2);
							}
						}
						
						if (t3size > 0)
						{
							if (entryOrder3 == null) 
							{
								exitOrder3 = ExitLong("entry3"); 
							}
							else 
							{
								CancelOrder(entryOrder3);
							}
						}
						
						if (t4size > 0)
						{
							if (entryOrder4 == null) 
							{
								exitOrder4 = ExitLong("entry4"); 
							}
							else 
							{
								CancelOrder(entryOrder4);
							}
						}
						
						if (t5size > 0)
						{
							if (entryOrder5 == null) 
							{
								exitOrder5 = ExitLong("entry5"); 
							}
							else 
							{
								CancelOrder(entryOrder5);
							}
						}
					}
					else
					{
						if (entryOrder1 == null) 
						{
							exitOrder5 = ExitShort("entry1"); 
						}
						else 
						{
							CancelOrder(entryOrder1);
						}
						
						if (t2size > 0)
						{
							if (entryOrder2 == null) 
							{
								exitOrder2 = ExitShort("entry2"); 
							}
							else 
							{
								CancelOrder(entryOrder2);
							}
						}
						
						if (t3size > 0)
						{
							if (entryOrder3 == null) 
							{
								exitOrder3 = ExitShort("entry3"); 
							}
							else 
							{
								CancelOrder(entryOrder3);
							}
						}
						
						if (t4size > 0)
						{
							if (entryOrder4 == null) 
							{
								exitOrder4 = ExitShort("entry4"); 
							}
							else 
							{
								CancelOrder(entryOrder4);
							}
						}
						
						if (t5size > 0)
						{
							if (entryOrder5 == null) 
							{
								exitOrder5 = ExitShort("entry5"); 
							}
							else 
							{
								CancelOrder(entryOrder5);
							}
						}
					}
				}
				
			}	
			#endregion 
			
			// [aknip] Check for trailing stopps : INTRABAR
			if ((e.MarketDataType == MarketDataType.Ask) && (exitCondition == 0) && (trailactive == 0) && (Position.MarketPosition != MarketPosition.Flat)) 
			{	
				DynamicTrail(); // "HurleyOne"
			}
			
						
			// Print our current P&L to the upper right hand corner of the chart
			if (showPerformanceData == displaytype.ProfitLoss)
			{	
				string pnl_display = "PnL Realized: " + Performance.RealtimeTrades.TradesPerformance.Currency.CumProfit.ToString("C");
				if (Position.MarketPosition != MarketPosition.Flat) 
				{
					//
					//if (e.MarketDataType == MarketDataType.Ask) e.Price;
					if (direction == "long") actprice = GetCurrentBid(); else actprice = GetCurrentAsk();
					pnl_display = pnl_display + "\nPnL Unrealized: " + Position.GetProfitLoss(actprice, PerformanceUnit.Currency).ToString("C"); 
				}
				pnl_display = pnl_display + "\nPnL for this day: " + (Performance.AllTrades.TradesPerformance.Currency.CumProfit - priorTradesCumProfit).ToString("C");
				DrawTextFixed("PnL", pnl_display, TextPosition.TopLeft,Color.Red, new Font("Arial", 8), Color.Black, Color.LightGray, 100);
			}
			
				
        }
	
        #region Properties
		// 
		//Gui.Design.DisplayName : Defines Label in properties-dialog
		//    Sting pattern "\t" defines order of properties: the more "\t" in the DisplayName, the more on top of the list it is positioned 
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tSignal Parameters")]
        public String A1
        {
			get { return ""; }
        }	
		
		[Description("STd deviation selected for APZ")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tAPZ stddev")]
        public double APZstddev
        {
            get { return apzstddev; }
            set { apzstddev = Math.Max(0.1, value); }
        }
		[Description("Period length selected for APZ")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tAPZ length")]
        public int APZlength
        {
            get { return apzlength; }
            set { apzlength = Math.Max(1, value); }
        }
		
        [Description("Period selected for CMO")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tCMO length")]
        public int CMOlength
        {
            get { return cmolength; }
            set { cmolength = Math.Max(1, value); }
        }
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tCMO rising/falling")]
        public bool CMOrisingfalling
        {
            get { return cmorisingfalling; }
            set { cmorisingfalling = value; }
        }
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tCMO above/below")]
        public bool CMOabovebelow
        {
            get { return cmoabovebelow; }
            set { cmoabovebelow = value; }
        }
		
		[Description("Enter longs when CMO is less than this value, and enter shorts when CMO is greater than this value")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tCmolimit")]
        public double Cmolimit
        {
            get { return cmolimit; }
            set { cmolimit = Math.Max(0.1, value); }
        }
		[Description("period length for d9ParticleOscillator_V2")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\td9 Length")]
        public int D9length
        {
            get { return d9length; }
            set { d9length = value; }
        }
		
		[Description("Period selected for EMA")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tEMA length")]
        public int EMAlength
        {
            get { return emalength; }
            set { emalength = Math.Max(1, value); }
        }
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tEMA rising/falling")]
        public bool EMArisingfalling
        {
            get { return emarisingfalling; }
            set { emarisingfalling = value; }
        }
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tEMA above/below")]
        public bool EMAabovebelow
        {
            get { return emaabovebelow; }
            set { emaabovebelow = value; }
        }
		[Description("0: No exit rules (only stops), 1: exit if price reverses 2 bars")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tExit Rules")]
        public int ExitRules
        {
            get { return exitRules; }
            set { exitRules = value; }
        }
		[Description("1: use Ninja IOrder events to cancel stopps and then close position,  2: modify stop to exit")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tExit Technique")]
        public int ExitTechnique
        {
            get { return exitTechnique; }
            set { exitTechnique = value; }
        }
		
		//
		//
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tMoney Management")]
        public String A2
        {
			get { return ""; }
        }	
		
		[Description("Lot size for target 1.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tT1 Size")]
        public int T1Size
        {
            get { return t1size; }
            set { t1size = Math.Max(1, value); }
        }
		[Description("Lot size for target 2.  0 disables target 2")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tT2 Size")]
        public int T2Size
        {
            get { return t2size; }
            set { t2size = Math.Max(0, value); }
        }
		[Description("Lot size for target 3.  0 disables target 3")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tT3 Size")]
        public int T3Size
        {
            get { return t3size; }
            set { t3size = Math.Max(0, value); }
        }
		[Description("Lot size for target 4.  0 disables target 4")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tT4 Size")]
        public int T4Size
        {
            get { return t4size; }
            set { t4size = Math.Max(0, value); }
        }
		[Description("Lot size for target 5.  0 disables target 5")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tT5 Size")]
        public int T5Size
        {
            get { return t5size; }
            set { t5size = Math.Max(0, value); }
        }
		
		[Description("Value selected for Target1")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tTarget1")]
        public int Target1
        {
            get { return target1; }
            set { target1 = Math.Max(1, value); }
        }
		[Description("Value selected for Target2")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tTarget2")]
        public int Target2
        {
            get { return target2; }
            set { target2 = Math.Max(1, value); }
        }
		[Description("Value selected for Target3")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tTarget3")]
        public int Target3
        {
            get { return target3; }
            set { target3 = Math.Max(1, value); }
        }
		[Description("Value selected for Target4")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tTarget4")]
        public int Target4
        {
            get { return target4; }
            set { target4 = Math.Max(1, value); }
        }
		[Description("Value selected for Target5")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tTarget5")]
        public int Target5
        {
            get { return target5; }
            set { target5 = Math.Max(1, value); }
        }
		[Description("Initial stoploss setting")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\t\tStop Loss")]
        public int Stop
        {
            get { return stop; }
            set { stop = Math.Max(1, value); }
        }
		
		[Description("Set trailing active: 0='HurleyOne' Trailing for one contract; 1= trail stops to (entry+TStop)after target1 is hit; 2= trail stops to target1 after target2 is hit; 3= trail stops to target2 after target3 is hit etc.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\t\tTrailing Strategy")]
        public int TrailActive
        {
            get { return trailactive; }
            set { trailactive = value; }
        }
		
		[Description("A variable to allow a choice for the move in stoploss for Target2-Target5 after Target1 is hit. Must be less than your Target1 value of course")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\t\tT Stop")]
        public int Tstop
        {
            get { return tstop; }
            set { tstop = Math.Max(-10, value); }
        }
		
		// these nextt 2 added by Nano 8Feb10
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\t\tTimeframe / P&L")]
        public String A3
        {
			get { return ""; }
        }	
		
		
		[Description("Time to start strategy - make sure you seperate it by a : i.e. 09:00, N.B. use 24 hr")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\t\tStart Time")]
        public string StartTime
         {
            get { return startTime; }
            set { startTime = value; }
        }
		
		[Description("Time to end strategy - make sure you seperate it by a : i.e. 21:00, N.B. use 24 hr")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\tStop Time")]
        public string EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }	
		
		[Description("Time to close all positions (00:00 = NO automatic closing) - make sure you seperate it by a : i.e. 21:00, N.B. use 24 hr")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\t\tClosing time")]
        public string ClosingTime
        {
            get { return closingTime; }
            set { closingTime = value; }
        }
		
		[Description("A variable that when changed to true allows a start and finish time for the strategy to operate")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\t\tTimeframe Active")]
        public bool Timeframe
        {
            get { return timeframe; }
            set { timeframe = value; }
        }
		[Description("The cumulative total of profits in $ after which the strategy will cease trading")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\t\tCum Profittarget")]
        public double Cumprofittarget
        {
            get { return cumprofittarget; }
            set { cumprofittarget = Math.Max(0.1, value); }
        }
		[Description("The cumulative total of losses in $ after which the strategy will cease trading")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\t\tCum Losstarget")]
        public double Cumlosstarget
        {
            get { return cumlosstarget; }
            set { cumlosstarget = Math.Max(0.1, value); }
        }
		[Description("Maximum number of trades per session. Set to zero if not used.")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\t\tMax TradesNumber")]
        public int MaxTradesNumber
        {
            get { return maxTradesNumber; }
            set { maxTradesNumber = value; }
        }
		
		[Description("")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\t\tVisuals")]
        public String A4
        {
			get { return ""; }
        }	
		[Description("NoHistorical set to true will only displays real-time trades, set to false will display historical trades")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\t\t\tNo Historical")]
        public bool NoHistorical
        {
            get { return noHistorical; }
            set { noHistorical = value; }
        }
		[Description("Display performance data of strategy in chart.")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tShow Performance Data")]
		public displaytype ShowPerformanceData
		{
			get { return showPerformanceData; }
			set { showPerformanceData = value; }
		}
		[Description("Log-Level: 0=off; 1=general debug-info; 2=including order execution logging; 3=including order update logging")]
        [Category("Parameters")]
		[Gui.Design.DisplayName("\tLog-Level")]
        public int LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }
		
        #endregion
    }
}
