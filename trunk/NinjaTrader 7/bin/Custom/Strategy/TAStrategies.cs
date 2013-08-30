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
    /// This set of strategies tries to include as many as possible trade triggers or notifications. This uses the ES and will present itself similiar to Dan's VOL Algo chart.  The settings may be change to increase performance.
	/// Data Range Jan 1st, 2012 - Sept 3th, 2012, Test Date 9/4/12
	/// 150T: -119,635
	/// 333T:  -65,377
	/// 512T:  -37,367
	/// 1024T: -21,510
	/// 
	/// I don't remember what settings I had this on when I was creating it.
	/// 
    /// </summary>
    [Description("This set of strategies tries to include as many as possible trade triggers or notifications. This uses the ES and will present itself similiar to Dan's VOL Algo chart.  The settings may be change to increase performance.")]
    public class TAStrategies : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int hMAPeriod = 100; // Default setting for HMAPeriod
        private int donchianPeriod = 40; // Default setting for DonchianPeriod
        private int mACDFast = 35; // Default setting for MACDFast
        private int mACDSlow = 70; // Default setting for MACDSlow
        private int mACDSmooth = 35; // Default setting for MACDSmooth
        private double mACDPosMin = 0.02; 
        private double mACDNegMax = -0.02;
		private int macdBarsBack = 1;
		private double mACDMinDelta = 0.001;
        // User defined variables (add any user defined variables below)
	
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(HMA(HMAPeriod));
			//Add(HMA(HMAPeriod/2));
            Add(MACD(MACDFast, MACDSlow, MACDSmooth));
            Add(DonchianChannel(DonchianPeriod));

            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {

			// Condition set 1
			double currMacd = MACD(MACDFast, MACDSlow, MACDSmooth).Diff[0];
			double prevMacd = MACD(MACDFast, MACDSlow, MACDSmooth).Diff[MacdBarsBack];
			double macdDiff = Math.Abs(currMacd) - Math.Abs(prevMacd);
			int pTarget = 4;
			int stop = 3;
			
			if (MACD(MACDFast, MACDSlow, MACDSmooth).Diff[0] > 0 
				&& MACD(MACDFast, MACDSlow, MACDSmooth).Diff[MacdBarsBack] < 0
				//&& macdDiff > mACDMinDelta
				//&& Rising(HMA(HMAPeriod/2)) == true
				//&& currMacd > mACDPosMin
				//&& (High[0] + pTarget * TickSize) < DonchianChannel(DonchianPeriod).Upper[0]
				) 
			{					
				//Alert("MyAlert0", Priority.High, "My Alert", @"C:\Program Files (x86)\NinjaTrader 7\sounds\Alert1.wav", 60, Color.White, Color.Black);
				//Print("Rasing Hull, Current MACD " + currMacd + " prevMacd " + prevMacd);
				DrawArrowUp("SecretUp" + MACD(MACDFast, MACDSlow, MACDSmooth).Diff[0], true, 0, Low[0] - 2*TickSize, Color.Blue);
				EnterLong("SS Long");
				SetProfitTarget(CalculationMode.Ticks, pTarget);
				SetStopLoss(CalculationMode.Ticks, stop);
			} 
			
			if (MACD(MACDFast, MACDSlow, MACDSmooth).Diff[0] < 0 
				&& MACD(MACDFast, MACDSlow, MACDSmooth).Diff[MacdBarsBack] > 0
				//&& macdDiff > mACDMinDelta
				//&& Rising(HMA(HMAPeriod/2)) == false
				//&& currMacd < mACDNegMax 
				//&& (Low[0] - pTarget * TickSize) > DonchianChannel(DonchianPeriod).Lower[0]
				) 
			{					
				//Alert("MyAlert0", Priority.High, "My Alert", @"C:\Program Files (x86)\NinjaTrader 7\sounds\Alert1.wav", 60, Color.White, Color.Black);
				DrawArrowDown("SecretUp" + MACD(MACDFast, MACDSlow, MACDSmooth).Diff[0], true, 0, High[0] + 2*TickSize, Color.Red);
				EnterShort("SS Short");
				SetProfitTarget(CalculationMode.Ticks, pTarget);
				SetStopLoss(CalculationMode.Ticks, stop);
			}

			if (Rising(HMA(HMAPeriod)) == true)
            {
				//DrawDot("", false, 0, Low[0] - 1 * TickSize, Color.Coral);
				//DrawDot("", false, 0, HMA(HMAPeriod)[0], Color.Green);
			} 
			if (Falling(HMA(HMAPeriod)) == true)
			{
				DrawDiamond("", true, 0, High[0] + 2 * TickSize, Color.Blue);
			}

            // Condition set 2
            if (Rising(MACD(MACDFast, MACDSlow, MACDSmooth).Avg) == true
                && MACD(MACDFast, MACDSlow, MACDSmooth).Avg[2] == Variable9)
            {
            }
			
			

            // Condition set 3
            if (Rising(DonchianChannel(DonchianPeriod).Lower) == true)
            {
            }
        }

		#region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public double MACDMinDelta
        {
            get { return mACDMinDelta; }
            set { mACDMinDelta = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double MACDPosMin
        {
            get { return mACDPosMin; }
            set { mACDPosMin = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double MACDNegMax
        {
            get { return mACDNegMax; }
            set { mACDNegMax = value; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MacdBarsBack
        {
            get { return macdBarsBack; }
            set { macdBarsBack = Math.Min(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int HMAPeriod
        {
            get { return hMAPeriod; }
            set { hMAPeriod = Math.Max(1, value); }
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
        #endregion
    }
}
