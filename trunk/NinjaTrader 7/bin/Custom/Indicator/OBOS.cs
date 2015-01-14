#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Over Bought, Over Sold
    /// </summary>
    [Description("Over Bought, Over Sold")]
    public class OBOS : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool rSIRequired = true; // Default setting for RSIRequired
            private bool stochasticsRequired = false; // Default setting for StochasticsRequired
            private Color overBoughtColor = Color.Cyan; // Default setting for OverBoughtColor
            private Color overSoldColor = Color.Fuchsia; // Default setting for OverSoldColor
        // User defined variables (add any user defined variables below)
		
		
		// Stochastics vars
		private int				periodD	= 3; //7;	// SlowDperiod
		private int				periodK	= 5; //14;	// Kperiod
		private int				smooth	= 2; //3;	// SlowKperiod
		private DataSeries		den;
		private DataSeries		nom;
        private DataSeries      fastK;
		private DataSeries		K;	// slow stochastics value
		private DataSeries		D;	// fast stochastics value
		private int				stochUpper = 80;
		private int				stochLower = 20;

		// RSI vars
		private DataSeries		avgUp;
		private DataSeries		avgDown;
		private DataSeries		down;
		private int				rsiPeriod	= 14;
		private int				rsiSmooth	= 3;
		private DataSeries		up;
		private DataSeries		Rsi;
		private DataSeries		Avg;
		private int				rsiUpper = 70;
		private int				rsiLower = 30;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
			
			// used for Stochastics calcuations
			den			= new DataSeries(this);
			nom			= new DataSeries(this);
            fastK       = new DataSeries(this);
			K       	= new DataSeries(this);
			D       	= new DataSeries(this);
			
			// used for RSI calculation
			avgUp		= new DataSeries(this);
			avgDown		= new DataSeries(this);
			down		= new DataSeries(this);
			up			= new DataSeries(this);
			Rsi			= new DataSeries(this);
			Avg			= new DataSeries(this);

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			bool stochOverBought = false;
			bool stochOverSold = false;
			bool rsiOverBought = false;
			bool rsiOverSold = false;
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			
			if (StochasticsRequired) 
			{
				calcStochastics();
				if ((D[0] > stochUpper)) // && D[0] < D[1])) // || (D[1] > stochUpper && D[0] < D[1]))
					stochOverBought = true;
				if ((D[0] < stochLower)) // && D[0] > D[1])) // || (D[1] < stochLower && D[0] > D[1]))
					stochOverSold = true;
			}
			
			if (RSIRequired)
			{
				calcRsi();
				if (Rsi[0] > rsiUpper)
					rsiOverBought = true;
				if (Rsi[0] < rsiLower)
					rsiOverSold = true;
			}
			
			if (((StochasticsRequired && stochOverBought) && (RSIRequired && rsiOverBought))
				|| ((StochasticsRequired && stochOverBought) && (!RSIRequired))
				|| ((RSIRequired && rsiOverBought) && (!StochasticsRequired)))
			{
				CandleOutlineColor = OverBoughtColor;
				//CandleOutlineColorSeries[0] = OverBoughtColor;
			} 
			else 
				if (((StochasticsRequired && stochOverSold) && (RSIRequired && rsiOverSold))
				|| ((StochasticsRequired && stochOverSold) && (!RSIRequired))
				|| ((RSIRequired && rsiOverSold) && (!StochasticsRequired)))
				{
					CandleOutlineColor = OverSoldColor;
					//CandleOutlineColorSeries[0] = OverSoldColor;
				}
				else
				{
					//CandleOutlineColorSeries[0] = Color.Black;	
					CandleOutlineColor = Color.Black;
					BarColor = Color.Empty;
				}
				
        }
		
		private void calcStochastics()
		{
            nom.Set(Close[0] - MIN(Low, PeriodK)[0]);
            den.Set(MAX(High, PeriodK)[0] - MIN(Low, PeriodK)[0]);

            if (den[0].Compare(0, 0.000000000001) == 0)
                fastK.Set(CurrentBar == 0 ? 50 : fastK[1]);
            else
                fastK.Set(Math.Min(100, Math.Max(0, 100 * nom[0] / den[0])));

            // Slow %K == Fast %D
            K.Set(SMA(fastK, Smooth)[0]);
            D.Set(SMA(K, PeriodD)[0]);
		}
		
		private void calcRsi()
		{
			if (CurrentBar == 0)
			{
				down.Set(0);
				up.Set(0);

                if (RsiPeriod < 3)
                    Avg.Set(50);
				return;
			}

			down.Set(Math.Max(Input[1] - Input[0], 0));
			up.Set(Math.Max(Input[0] - Input[1], 0));

			if ((CurrentBar + 1) < RsiPeriod) 
			{
				if ((CurrentBar + 1) == (RsiPeriod - 1))
					Avg.Set(50);
				return;
			}

			if ((CurrentBar + 1) == RsiPeriod) 
			{
				// First averages 
				avgDown.Set(SMA(down, RsiPeriod)[0]);
				avgUp.Set(SMA(up, RsiPeriod)[0]);
			}  
			else 
			{
				// Rest of averages are smoothed
				avgDown.Set((avgDown[1] * (RsiPeriod - 1) + down[0]) / RsiPeriod);
				avgUp.Set((avgUp[1] * (RsiPeriod - 1) + up[0]) / RsiPeriod);
			}

			double rsi	  = avgDown[0] == 0 ? 100 : 100 - 100 / (1 + avgUp[0] / avgDown[0]);
			double rsiAvg = (2.0 / (1 + RsiSmooth)) * rsi + (1 - (2.0 / (1 + RsiSmooth))) * Avg[1];

			Avg.Set(rsiAvg);
			Rsi.Set(rsi);
		}			

        #region Properties

        [Description("Requires RSI Overbought/Oversold, false - RSI not used")]
        [GridCategory("Parameters")]
        public bool RSIRequired
        {
            get { return rSIRequired; }
            set { rSIRequired = value; }
        }

        [Description("Requires Stochastics Overbought/Oversold, false - Stochastics not used")]
        [GridCategory("Parameters")]
        public bool StochasticsRequired
        {
            get { return stochasticsRequired; }
            set { stochasticsRequired = value; }
        }

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for moving average over K values")]
		[GridCategory("Parameters")]
		public int PeriodD
		{
			get { return periodD; }
			set { periodD = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculating the K values")]
		[GridCategory("Parameters")]
		public int PeriodK
		{
			get { return periodK; }
			set { periodK = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing the slow K values")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int RsiSmooth
		{
			get { return rsiSmooth; }
			set { rsiSmooth = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int RsiPeriod
		{
			get { return rsiPeriod; }
			set { rsiPeriod = Math.Max(1, value); }
		}

		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color OverSoldColor
		{
			get { return overSoldColor; }
			set { overSoldColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string OverSoldColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(overSoldColor); }
			set { overSoldColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Select color for downtrend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color OverBoughtColor
		{
			get { return overBoughtColor; }
			set { overBoughtColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string OverBoughtColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(overBoughtColor); }
			set { overBoughtColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		
		
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private OBOS[] cacheOBOS = null;

        private static OBOS checkOBOS = new OBOS();

        /// <summary>
        /// Over Bought, Over Sold
        /// </summary>
        /// <returns></returns>
        public OBOS OBOS(int periodD, int periodK, int rsiPeriod, bool rSIRequired, int rsiSmooth, int smooth, bool stochasticsRequired)
        {
            return OBOS(Input, periodD, periodK, rsiPeriod, rSIRequired, rsiSmooth, smooth, stochasticsRequired);
        }

        /// <summary>
        /// Over Bought, Over Sold
        /// </summary>
        /// <returns></returns>
        public OBOS OBOS(Data.IDataSeries input, int periodD, int periodK, int rsiPeriod, bool rSIRequired, int rsiSmooth, int smooth, bool stochasticsRequired)
        {
            if (cacheOBOS != null)
                for (int idx = 0; idx < cacheOBOS.Length; idx++)
                    if (cacheOBOS[idx].PeriodD == periodD && cacheOBOS[idx].PeriodK == periodK && cacheOBOS[idx].RsiPeriod == rsiPeriod && cacheOBOS[idx].RSIRequired == rSIRequired && cacheOBOS[idx].RsiSmooth == rsiSmooth && cacheOBOS[idx].Smooth == smooth && cacheOBOS[idx].StochasticsRequired == stochasticsRequired && cacheOBOS[idx].EqualsInput(input))
                        return cacheOBOS[idx];

            lock (checkOBOS)
            {
                checkOBOS.PeriodD = periodD;
                periodD = checkOBOS.PeriodD;
                checkOBOS.PeriodK = periodK;
                periodK = checkOBOS.PeriodK;
                checkOBOS.RsiPeriod = rsiPeriod;
                rsiPeriod = checkOBOS.RsiPeriod;
                checkOBOS.RSIRequired = rSIRequired;
                rSIRequired = checkOBOS.RSIRequired;
                checkOBOS.RsiSmooth = rsiSmooth;
                rsiSmooth = checkOBOS.RsiSmooth;
                checkOBOS.Smooth = smooth;
                smooth = checkOBOS.Smooth;
                checkOBOS.StochasticsRequired = stochasticsRequired;
                stochasticsRequired = checkOBOS.StochasticsRequired;

                if (cacheOBOS != null)
                    for (int idx = 0; idx < cacheOBOS.Length; idx++)
                        if (cacheOBOS[idx].PeriodD == periodD && cacheOBOS[idx].PeriodK == periodK && cacheOBOS[idx].RsiPeriod == rsiPeriod && cacheOBOS[idx].RSIRequired == rSIRequired && cacheOBOS[idx].RsiSmooth == rsiSmooth && cacheOBOS[idx].Smooth == smooth && cacheOBOS[idx].StochasticsRequired == stochasticsRequired && cacheOBOS[idx].EqualsInput(input))
                            return cacheOBOS[idx];

                OBOS indicator = new OBOS();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.PeriodD = periodD;
                indicator.PeriodK = periodK;
                indicator.RsiPeriod = rsiPeriod;
                indicator.RSIRequired = rSIRequired;
                indicator.RsiSmooth = rsiSmooth;
                indicator.Smooth = smooth;
                indicator.StochasticsRequired = stochasticsRequired;
                Indicators.Add(indicator);
                indicator.SetUp();

                OBOS[] tmp = new OBOS[cacheOBOS == null ? 1 : cacheOBOS.Length + 1];
                if (cacheOBOS != null)
                    cacheOBOS.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheOBOS = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Over Bought, Over Sold
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.OBOS OBOS(int periodD, int periodK, int rsiPeriod, bool rSIRequired, int rsiSmooth, int smooth, bool stochasticsRequired)
        {
            return _indicator.OBOS(Input, periodD, periodK, rsiPeriod, rSIRequired, rsiSmooth, smooth, stochasticsRequired);
        }

        /// <summary>
        /// Over Bought, Over Sold
        /// </summary>
        /// <returns></returns>
        public Indicator.OBOS OBOS(Data.IDataSeries input, int periodD, int periodK, int rsiPeriod, bool rSIRequired, int rsiSmooth, int smooth, bool stochasticsRequired)
        {
            return _indicator.OBOS(input, periodD, periodK, rsiPeriod, rSIRequired, rsiSmooth, smooth, stochasticsRequired);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Over Bought, Over Sold
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.OBOS OBOS(int periodD, int periodK, int rsiPeriod, bool rSIRequired, int rsiSmooth, int smooth, bool stochasticsRequired)
        {
            return _indicator.OBOS(Input, periodD, periodK, rsiPeriod, rSIRequired, rsiSmooth, smooth, stochasticsRequired);
        }

        /// <summary>
        /// Over Bought, Over Sold
        /// </summary>
        /// <returns></returns>
        public Indicator.OBOS OBOS(Data.IDataSeries input, int periodD, int periodK, int rsiPeriod, bool rSIRequired, int rsiSmooth, int smooth, bool stochasticsRequired)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.OBOS(input, periodD, periodK, rsiPeriod, rSIRequired, rsiSmooth, smooth, stochasticsRequired);
        }
    }
}
#endregion
