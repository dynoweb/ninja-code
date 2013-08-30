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
    /// Battle of bots strategy for march 2013 by kk
    /// </summary>
    [Description("Battle of bots strategy for march 2013 by kk")]
    public class BattleKK : Strategy
     {
        #region Variables

			private int d9Period = 10; 
			private int d9Phase = 0;
		
         private int sessionStart = 170000;
         private int sessionEnd = 160000;

	    private string	 atmStrategyId		= string.Empty;	// Variable to hold the atmStrategyId 
		private string	orderId				= string.Empty; // Variable to hold the orderId
        private int entrybar ;

		private string aTMStrategy			= string.Empty; // select ATM strategy from your saved ones
  
        private OrderType typeEntry = OrderType.Limit;
        private int cancelOrderEntryBars =3;
        private int limitOffset = 1;
        private int stopOffset = 1;
		private bool timeok=false;
        

	 
        #endregion


        protected override void Initialize()
        {
			
			Enabled=true;
			EntryHandling = EntryHandling.UniqueEntries;
            TraceOrders = true;
		    CalculateOnBarClose = true;
			IncludeCommission=true;
			Slippage=1;
			ExitOnClose=true;
            
        }

        protected override void OnBarUpdate()
        {
			//ignore historical data
			  if(Historical )
				return;
		
			
			timeok=false;
			
			if(ToTime(Time[0]) >= sessionStart || ToTime(Time[0]) < sessionEnd) timeok=true;
			
                // Long conditions
                if (CrossAbove(d9ParticleOscillator_V2(Close,d9Period,d9Phase).Prediction, 0, 1)
                     && timeok
					 &&orderId.Length == 0 
				      && atmStrategyId.Length == 0
					 )
                    {
                          entrybar=CurrentBar;
					        atmStrategyId = GetAtmStrategyUniqueId();
					        orderId = GetAtmStrategyUniqueId();
					
                            AtmStrategyCreate(OrderAction.Buy, TypeEntry, (TypeEntry != OrderType.Market ? Close[0] - LimitOffset * TickSize : 0),
                            (TypeEntry != OrderType.StopLimit ? 0 : (Close[0] + StopOffset * TickSize)), TimeInForce.Day, orderId,
                            ATMStrategy, atmStrategyId);
						
						DrawText((CurrentBar-1).ToString(),"Buy",0,Low[0] - 7*TickSize,Color.White);
						DrawArrowUp(CurrentBar.ToString(),true,0,Low[0]-2*TickSize,Color.Navy);
                    }

                // Short conditions
                if (CrossBelow(d9ParticleOscillator_V2(Close,d9Period,d9Phase).Prediction, 0, 1)
                      && timeok
					  &&orderId.Length == 0 
				      && atmStrategyId.Length == 0
					)
                      {
                           entrybar=CurrentBar;
					        atmStrategyId = GetAtmStrategyUniqueId();
					        orderId = GetAtmStrategyUniqueId();
                            AtmStrategyCreate(OrderAction.SellShort, TypeEntry, (TypeEntry != OrderType.Market ? Close[0] + LimitOffset * TickSize : 0),
                            (TypeEntry != OrderType.StopLimit ? 0 : (Close[0] - StopOffset * TickSize)), TimeInForce.Day, orderId,
                            ATMStrategy, atmStrategyId);
						
						DrawText((CurrentBar-1).ToString(),"Sell",0,High[0] +7*TickSize,Color.White);	
						DrawArrowDown(CurrentBar.ToString(),true,0,High[0]+2*TickSize,Color.Red);
                      }
					 //Reenter SHORT conditions
                if (CrossBelow(d9ParticleOscillator_V2(Close,d9Period,d9Phase).Prediction, 0, 1)
                     && timeok
					 && GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Long
					 && atmStrategyId.Length >0
					)
                    { 
						    AtmStrategyClose(atmStrategyId);
						    DrawDiamond(CurrentBar.ToString(),true,0,High[0]+2*TickSize,Color.Yellow);
						
						    entrybar=CurrentBar;
					        atmStrategyId = GetAtmStrategyUniqueId();
					        orderId = GetAtmStrategyUniqueId();
					
                            AtmStrategyCreate(OrderAction.SellShort, TypeEntry, (TypeEntry != OrderType.Market ? Close[0] - LimitOffset * TickSize : 0),
                            (TypeEntry != OrderType.StopLimit ? 0 : (Close[0] + StopOffset * TickSize)), TimeInForce.Day, orderId,
                            ATMStrategy, atmStrategyId);
						
						DrawText((CurrentBar-1).ToString(),"Sell",0,High[0] +7*TickSize,Color.White);	
						DrawArrowDown(CurrentBar.ToString(),true,0,High[0]+2*TickSize,Color.Red);
                    }

                //Reenter LONG conditions
                if (CrossAbove(d9ParticleOscillator_V2(Close,d9Period,d9Phase).Prediction, 0, 1)
                      && timeok
					  && GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Short
					  && atmStrategyId.Length >0
					)
                      {
					        AtmStrategyClose(atmStrategyId);
							DrawDiamond(CurrentBar.ToString(),true,0,Low[0]-2*TickSize,Color.Yellow);
						
					     	 entrybar=CurrentBar;
					        atmStrategyId = GetAtmStrategyUniqueId();
					        orderId = GetAtmStrategyUniqueId();
                            AtmStrategyCreate(OrderAction.Buy, TypeEntry, (TypeEntry != OrderType.Market ? Close[0] + LimitOffset * TickSize : 0),
                            (TypeEntry != OrderType.StopLimit ? 0 : (Close[0] - StopOffset * TickSize)), TimeInForce.Day, orderId,
                            ATMStrategy, atmStrategyId);
						
						DrawText((CurrentBar-1).ToString(),"Buy",0,Low[0] - 7*TickSize,Color.White);
						DrawArrowUp(CurrentBar.ToString(),true,0,Low[0]-2*TickSize,Color.Navy);
					}
		
						// Check for a pending entry order
                    if (orderId.Length > 0)
                    {
                      string[] status = GetAtmStrategyEntryOrderStatus(orderId);

                      // If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
                      if (status.GetLength(0) > 0)
                            {
                                  //if the entry order is unfilled, cancel it after x bars

                                  if (CurrentBar== entrybar + cancelOrderEntryBars && (status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending"))
                                  {
                                    AtmStrategyCancelEntryOrder(orderId);
                                    orderId = string.Empty;
                                  }


                                  // If the order state is terminal, reset the order id value
                                  if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
                                  {
                                    orderId = string.Empty;
                                  }
                
                            }
                    }
                    // If the strategy has terminated reset the strategy id after doing some profit/loss calculations.
					
                    else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
                    {      
						    atmStrategyId=string.Empty;
                    }
                  
						
                  
				
		}
		
        #region Properties

        

        [Description("d9 period setting")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayName("1.1D9 period setting")]
        public int D9Period
        {
            get { return d9Period; }
            set { d9Period = Math.Max(1, value); }
        }

        [Description("d9 pahse setting")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayName("1.2D9 phase setting")]
        public int D9Phase
        {
            get { return d9Phase; }
            set { d9Phase = Math.Max(0, value); }
        }
		
    
        [Description("Session start time")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayName("2.1Session Start")]
        public int SessionStart
        {
          get { return sessionStart; }
          set { sessionStart = value; }
        }
        [Description("Session end time")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayName("2.2Session End")]
        public int SessionEnd
        {
          get { return sessionEnd; }
          set { sessionEnd = value; }
        }

     

        [Description("Write here ATM Strategy name from your  saved ones")]
         [GridCategory("Parameters")]
         [Gui.Design.DisplayName("3.1 ATM stratgey name ")]
        public string ATMStrategy
        {
            get { return aTMStrategy; }
            set { aTMStrategy = value; }
        }


        [Description("Type of order entry")]
        [GridCategory("Parameters")]
         [Gui.Design.DisplayName("3.2 Type of order entry ")]
        public OrderType TypeEntry
        {
          get { return typeEntry; }
          set { typeEntry = (value == OrderType.Market ? OrderType.Market : (value == OrderType.StopLimit ? OrderType.StopLimit : OrderType.Limit)); }
        }

        [Description("Number of ticks for Limit Order ")]
         [GridCategory("Parameters")]
         [Gui.Design.DisplayName("3.3 Limit order Offset ")]
        public int LimitOffset
        {
          get { return limitOffset; }
          set { limitOffset = value; }
        }
        [Description("Number of ticks for the StopLimit Order Stop price")]
         [GridCategory("Parameters")]
         [Gui.Design.DisplayName("3.4 Stop order Offset ")]
        public int StopOffset
        {
          get { return stopOffset; }
          set { stopOffset = value; }
        }

        [Description("Cancel the entry limit order after x bars")]
        [GridCategory("Parameters")]
         [Gui.Design.DisplayName("3.5 Bars after entry for cancel ")]
        public int CancelOrderEntryBars
        {
          get { return cancelOrderEntryBars; }
          set { cancelOrderEntryBars = value; }
        }
		

        #endregion
    }
}

