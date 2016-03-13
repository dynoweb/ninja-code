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
    /// Ran this on /CL for daily from 2008-2010 daily, 4.79 PF with 55,700 profit, -7055 Max DD
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class rcTTMSqueeze : Strategy
    {
        #region Variables
        // Wizard generated variables
		private int aTRPeriod = 1;
        private double aTRRatched = 0.000; 
        private double aTRTimes = 4;

        private int aPeriod = 1;
        private double atrX = 4.5;
        private int bPeriod = 20;
        private double bStdX = 1.500;
        private int kPeriod = 20;
        private double kStdX = 1.5;
        private int mPeriod = 15;
        private int mSmooth = 3;
        // User defined variables (add any user defined variables below)
		
		int periodD = 3;
		int periodK = 5;
		int smooth = 2;
				
		bool squeeze = false;
		
		ATRTrailing atr;
		Bollinger bollinger;
		KeltnerChannel keltnerChannel;
		SMA sMomentum;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			
			atr = ATRTrailing(ATRTimes, ATRPeriod, ATRRatched);
			bollinger = Bollinger(BStdX, BPeriod);
			keltnerChannel = KeltnerChannel(KStdX, KPeriod);
			sMomentum = SMA(Momentum(MPeriod), MSmooth);
			//Add(Stochastics(periodD, periodK, smooth));
			Add(RCTTMSqueeze(1,2,2,20));
			
			Add(atr);
			//Add(bollinger);
			//Add(keltnerChannel);
			//Add(sMomentum);
			
			atr.AutoScale = false;
			atr.PaintPriceMarkers = false;
			
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				if (squeeze) 
				{
					if (!RCTTMSqueeze(1,2,2,20).InSqueeze[0])
					{
						squeeze = false;
						if (RCTTMSqueeze(1,2,2,20).Histogram[0] > 0 &&
							Stochastics(periodD, periodK, smooth).K[0] > 40) 
						{
							EnterLong();
						}
						if (RCTTMSqueeze(1,2,2,20).Histogram[0] < 0 &&
							Stochastics(periodD, periodK, smooth).K[0] < 60) 
						{
							EnterShort();
						}
					}
				}
				
				if (RCTTMSqueeze(1,2,2,20).InSqueeze[0])
				{
					squeeze = true;	
				}
			}
			else
			{
				squeeze = false;
				
				if (Position.MarketPosition == MarketPosition.Long &&
					//Stochastics(periodD, periodK, smooth).K[0] < 40 &&
					Low[0] < atr.Upper[0]) 
				{
					ExitLong();
				}
				if (Position.MarketPosition == MarketPosition.Short && 
					Stochastics(periodD, periodK, smooth).K[0] > 60 &&
					High[0] > atr.Lower[0]) 
				{
					ExitShort();
				}
			}				
        }

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
        public int BPeriod
        {
            get { return bPeriod; }
            set { bPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double BStdX
        {
            get { return bStdX; }
            set { bStdX = Math.Max(1, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int KPeriod
        {
            get { return kPeriod; }
            set { kPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double KStdX
        {
            get { return kStdX; }
            set { kStdX = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MPeriod
        {
            get { return mPeriod; }
            set { mPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MSmooth
        {
            get { return mSmooth; }
            set { mSmooth = Math.Max(1, value); }
        }


        #endregion
    }
}
