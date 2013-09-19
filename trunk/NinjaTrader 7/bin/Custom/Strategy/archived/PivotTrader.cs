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
/* 
4/5/13 - ValutaTrader - Battle of the Bots

Reversal strat
Ok, I would like to try something simple. 


Can you add the attached 'bot to the following instruments
CL 05-13
GC 06-13
Both charts being Point & Figure (Period 15 Minute, Box Size 10, Reversal 2, Price: HighAndLows) 
24/7 sessions

The strategy is quite simple and reverses positions on Pivot bars. The important thing is to use Point&Figure to get less noise and false signals as in regular charts. 


PS Please note that the strategy does not produce valid backtest results on the mentioned chart type, but may be tested on replay data and forward testing.
*/
	
    /// <summary>
    /// This strategy puts on reversal trades on pivot points (with N bars to the left and right of the pivot bar
	/// 
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class PivotTrader : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int sl = 45; // Default setting for STOP LOSS TICKS
		private int leftPivotStrength=2;
		private int rightPivotStrength=2;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        { 
            EntriesPerDirection = 1;
            EntryHandling = EntryHandling.AllEntries;
            CalculateOnBarClose = true;
            IncludeCommission = true;
            Slippage = 1;
            ExitOnClose = false;				
			SetStopLoss(CalculationMode.Ticks, sl);	
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (PivotLowDetected(PivotStrengthLeft,PivotStrengthRight)){
					EnterLong();
			}
			if ( PivotHighDetected(PivotStrengthLeft,PivotStrengthRight)){
					EnterShort();				
			}				
        }
		
		#region Functions

		public bool PivotLowDetected(int leftStrength, int rightStrength){		
			bool flag=true;
			for (int i=(rightStrength-1);i>=0;i--){
				if (Low[i]<Low[rightStrength])
					flag=false;
			}
			for (int i=(rightStrength+1);i<=(leftStrength+rightStrength);i++){
				if (Low[i]<Low[rightStrength])
					flag=false;
			}	
			return flag;
		}

		public bool PivotHighDetected(int leftStrength, int rightStrength){
			bool flag=true;

			for (int i=(rightStrength-1);i>=0;i--){
				if (High[i]>High[rightStrength])
					flag=false;
			}
			for (int i=(rightStrength+1);i<=(leftStrength+rightStrength);i++){
				if (High[i]>High[rightStrength])
					flag=false;
			}
			return flag;
		}
#endregion
		
        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int SL
        {
            get { return sl; }
            set { sl = Math.Max(0, value); }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int PivotStrengthRight
        {
            get { return rightPivotStrength; }
            set { rightPivotStrength = Math.Max(1, value); }
        }
        [Description("")]
        [GridCategory("Parameters")]
        public int PivotStrengthLeft
        {
            get { return leftPivotStrength; }
            set { leftPivotStrength = Math.Max(1, value); }
        }		

        #endregion
    }
}
