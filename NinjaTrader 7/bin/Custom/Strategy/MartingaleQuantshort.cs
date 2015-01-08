#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Collections.Generic;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion


namespace NinjaTrader.Strategy
{
    
        [Description("Uses 6E 300 Ticks - Martingale framework created by Trader PARDS with entry exit signals created by Quantismo.")]
        public class MartingaleQuantshort : Strategy
        {
// VARIABLES $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$        
		
		
		private bool                        bForex	= false;//if using FOREX LOTS
		private int                         myTarg = 100; 
        private int                         myStop = 20; 
		private double                      bolDev = 2.000; 
        private int                         bolPer = 20; 
		// Double up on losing trades
        private enum eTradeResult { WINNER, LOSER, BREAK_EVEN };
		private List<eTradeResult> listWinLosses; 		// Each element indicates whether a trade finished a winner or a loser.
		private bool bDoubleQtyLoss = true; //use false for no martingale
		public enum MaxDoubleUp { One, Two, Four, Eight };
		private MaxDoubleUp maxExp		= MaxDoubleUp.Eight;
//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$4		
		
		        

       
                protected override void Initialize()
                {
				//$$$$$$$$$$$$$$$$$$$$show signals$$$$$$$$$$$	
				Add(Bollinger(BolDev, BolPer));
                Add(anaRegressionChannel(300, 3.5));
				//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$	
			    SetProfitTarget("", CalculationMode.Ticks, MyTarg);
                SetStopLoss("", CalculationMode.Ticks, MyStop, false);
                Enabled = true;      //new ADD
			    ExitOnClose = false; //new ADD
			    CalculateOnBarClose	= true;//was set to true
			
                listWinLosses = new List<eTradeResult>();
			
                }

        
                protected override void OnBarUpdate()
		        {
					
				//////////////////////////////////////////////////////////////	
				//////////////////////////////////////////////////////////////	
			    //$$$$$$$$$$$$$$$ ENNTRY CONDITIONS$$$$$$$$$$$$$$$$$$$$$$$$$$$
			    if (High[1] > Open[1]
                && Low[1] < Open[1]
                && High[1] > Bollinger(BolDev, BolPer).Upper[0]
                && High[0] > anaRegressionChannel(300, 3.5).Upper[0])
			
			
			    //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
				//////////////////////////////////////////////////////////////	
				//////////////////////////////////////////////////////////////	
			    {
				int qty = getQty();
				if( Position.MarketPosition == MarketPosition.Long ) 
				{
				if( DoubleQuantityLoss )
				{
				if( Position.MarketPosition != MarketPosition.Flat
				&& Position.GetProfitLoss(Close[0], PerformanceUnit.Currency) < 0 ) 
				{
				qty = getTradeCount( getConsecutiveLosses()+1 );
                //DrawText( "consecLosses"+CurrentBar, (getConsecutiveLosses()+1).ToString()+", "+qty.ToString(), 0, High[0]+3*TickSize, Color.Black);
				}
				else if( Position.MarketPosition != MarketPosition.Flat
				&& Position.GetProfitLoss(Close[0], PerformanceUnit.Currency) > 0 )  {
				qty = 1;
				}
				}
				}
 
				
				
				
				
				if( IsForex ) {
					qty *= 10000;
				}
				EnterShort(qty);
				
				
				
				
				
 
			    }
			    //$$$$$$$$$$$$$$$$$$$$$$ EXIT SHORT IF CONDITIONS $$$$$$$$$$$$$$$$$$$
			 if (CrossAbove(Low, anaRegressionChannel(300, 3.5).Lower, 1))
            {
                ExitShort("", "");
            }

            // Condition set 3
            if (CrossAbove(Low, anaRegressionChannel(300, 3.5).Middle, 1))
            {
                ExitShort("", "");
            }
			    
                }

		#region OnExecution
       
        /// In this handler, we fill the list listWinLosses so we can tell what quantity we want to use for 
		/// the losers.
       
        protected override void OnExecution(IExecution execution)
        {
            Trade lastTrade = null;
            try
            {
                lastTrade = Performance.AllTrades[Performance.AllTrades.Count - 1];
				if( lastTrade != null
					&& lastTrade.ExitExecution == execution )
                {
					// Fill listWinLosses
					if (lastTrade.ProfitCurrency > 0)
					{
						listWinLosses.Add(eTradeResult.WINNER);
					}
					else if (lastTrade.ProfitCurrency < 0)
					{
						listWinLosses.Add(eTradeResult.LOSER);
					}
                }				
            }
            catch
            {
//				DrawText( "OnExe"+CurrentBar, listWinLosses[listWinLosses.Count-1].ToString(), 0, High[0]+9*TickSize, Color.Black );
            }
        }	
        #endregion		
		
		/// <summary>
		/// This method is used to determine what quantity we'll trade with.  We're going to trade one contract normally.
		/// But the user may want to elect to double-up on the losers.  If they elect to go that then if we get a loser, 
		/// then we want to double up on the number of trades that will be taken.  That means if we get one loser, we'll 
		/// trade two contracts on the next trade.  If we get two losers, we'll trade four contracts on the next trade 
		/// and so on, up to a maximum of eight contracts.
		
		/// <returns>The number of contracts to trade</returns>
		        private int getQty()
		        {
			    // It's always qty = 1 if we're not doubling up
			    if( !DoubleQuantityLoss ) {
				return 1;
			    }

			    // It's also always 1 at the beginning of the list...
                if( listWinLosses.Count == 0 ) {
				return 1;
			    }
						
                int numbItems = listWinLosses.Count - 1;
			    if( listWinLosses[numbItems] == eTradeResult.WINNER )
			    {
				return 1;
			    }
			    else 
			    {
				return getTradeCount( getConsecutiveLosses() );
			    }
		        }
		
		
		        /// This method returns the number of consecutive losses we're showing.  This number WILL NOT include
		        /// the current trade for which the framework is responsible for closing, e.g. when it takes a trade in 
		        /// the opposite direction and has to first close the current trade.
		        /// Note: In OnBarUpdate(), I attempt to control for that.		
		        private int getConsecutiveLosses()
		        {
			    // Go through the list until we find a winner and the number of times we go through the loop 
			    // will tell us how many contracts we need to trade.
	            int iConsecutiveLossCount = 0;
			    int numbItems = listWinLosses.Count-1;
						
                for( int i = numbItems; i >= 0; --i )
			    {
				if( listWinLosses[i] == eTradeResult.LOSER )
				{
					++iConsecutiveLossCount;
				}
				else
				{
					break;
				}					
			    }
			    return iConsecutiveLossCount;	
		        }
			
		        /// This method is a companion to getQty() and is written so it can be called by itself.
		
		        /// <param name="inQty">Number of consecutive losses</param>
		        /// <returns>The number of contracts to trade based solely on the number of consecutive losses.</returns>
		        private int getTradeCount( int inQty )
		        {
			    switch( MaxExpValue )
			    {
			    case MaxDoubleUp.Eight:
			    {
			    switch( inQty )
					
			    {
						case 0:  // sanity check - should never get here
						case 4:
						case 8:
						case 12:
						case 16:
							return 1; 
							break;
						case 1:
						case 5:
						case 9:
						case 13:
						case 17:
							return 2;
							break;
						case 2:
						case 6:
						case 10:
						case 14:
						case 18:
							return 4;
							break;
						case 3:
						case 7:
						case 11:
						case 15:
						case 19:
							return 8;
							break;
						default:
							return 1;	
							break;		
					}
					break;
				}
				case MaxDoubleUp.Four:
				{
					switch( inQty )
					{
						case 0:		// sanity check - shouldn't ever get here
						case 3:
						case 6:
						case 9:
						case 12:
						case 15:
						case 18:
							return 1; 
							break;
						case 1:
						case 4:
						case 7:
						case 10:
						case 13:
						case 16:
						case 19:
							return 2;
							break;
						case 2:
						case 5:
						case 8:
						case 11:
						case 14:
						case 17:
						case 20:
							return 4;
							break;
						default:
							return 1;	
							break;		
					}						
					break;
				}
				case MaxDoubleUp.Two:
				{
					switch( inQty )
					{
						case 0:
						case 2:
						case 4:
						case 6:
						case 8:
						case 10:
						case 12:
						case 14:
						case 16:
						case 18:
						case 20:
							return 1; // sanity check - shouldn't ever get here
							break;
						case 1:
						case 3:
						case 5:
						case 7:
						case 9:
						case 11:
						case 13:
						case 15:
						case 17:
						case 19:
						case 21:
							return 2;
							break;
						default:
							return 1;	
							break;		
					}						
					break;
				}
				default:	// If MaxDoubleUp.One, for example
					return 1;
			}			
		}
		
        #region Properties
		//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
		[Description("")]
        [GridCategory("Parameters")]
        public int MyTarg
        {
            get { return myTarg; }
            set { myTarg = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MyStop
        {
            get { return myStop; }
            set { myStop = Math.Max(1, value); }
        }		
		[Description("True if trading the Forex market, false if trading futures")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("IsForex")]
		public bool IsForex
		{
			get { return bForex; }
			set { bForex = value; }
		}
		 [Description("")]
        [GridCategory("Parameters")]
        public double BolDev
        {
            get { return bolDev; }
            set { bolDev = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int BolPer
        {
            get { return bolPer; }
            set { bolPer = Math.Max(1, value); }
        }	
		
		
		//$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
		#region TradeParams
		[Description("True to double up on the losers, false otherwise.")]
		[Gui.Design.DisplayNameAttribute("\tDouble quantity on loss")]
		[GridCategory("TradeParams")]
		public bool DoubleQuantityLoss
		{
			get { return bDoubleQtyLoss; }
			set { bDoubleQtyLoss = value; }
		}
		
		[Description("Enter 1, 2, 4 or 8 as the maximum number of contracts the system will trade before it resets back to one.")]
		[Gui.Design.DisplayNameAttribute("Max value for doubling losers")]
		[GridCategory("TradeParams")]
		public MaxDoubleUp MaxExpValue
		{
			get { return maxExp; }
			set { maxExp = value; }
		}
		#endregion
		
		#endregion
    }
}
