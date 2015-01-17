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
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class AACandlePatterns : Indicator
    {
        #region Variables
        // Wizard generated variables
            private bool showEveningStar = true; // Default setting for ShowEveningStar
            private bool showEngulfing = true; // Default setting for ShowEngulfing
		    private bool showHarami = true;
			private bool showPiercing = true;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 10)
				return;

			if (showPiercing)
			{
				if (Close[0] > Open[0] && Close[1] < Open[1] && Close[2] < Open[2] && Close[3] < Open[3])  
				{
					if (Open[0] < Close[1] && Close[0] > Close[1] + (Open[1] - Close[1])/2)
					{
						DrawDiamond(CurrentBar + " Piercing ling", false, 0, Low[0]*0.99, Color.LimeGreen); 
					}
				}
				if (Close[0] < Open[0] && Close[1] > Open[1] && Close[2] > Open[2] && Close[3] > Open[3])  
				{
					if (Open[0] > Close[1] && Close[0] < Open[1] + (Close[1] - Open[1])/2)
					{
						DrawDiamond(CurrentBar + " Piercing ling", false, 0, High[0]*1.01, Color.Pink); 
					}
				}
			}
			
			if (showEngulfing)
			{
				// bullish engulfing pattern
				if (Close[0] > Open[0] && Close[1] < Open[1] && Open[0] < Low[1] && Close[0] > High[1]) 
				{
					DrawDiamond(CurrentBar + " Bullish engulfing", false, 0, Low[0]*0.99, Color.Green); 
				}
				// bearish engulfing pattern
				if (Close[0] < Open[0] && Close[1] > Open[1] && Open[0] > High[1] && Close[0] < Low[1]) 
				{
					DrawDiamond(CurrentBar + " Bearish engulfing", false, 0, High[0]*1.01, Color.Red); 
				}
			}
			
			if (showHarami)
			{
				// Harami
				if (Close[0] > Open[0] && Close[1] < Open[1] && Low[0] > Close[1] && High[0] < Open[1]) 
				{
					DrawDot(CurrentBar + " Bullish Harami", false, 0, Low[0]*0.99, Color.Green); 
				}
				if (Close[0] < Open[0] && Close[1] > Open[1] && Low[0] > Open[1] && High[0] < Close[1]) 
				{
					DrawDot(CurrentBar + " Bearish Harami", false, 0, High[0]*1.01, Color.Red); 
				}
			}
			
			// look for Evening Star reversal pattern
			// 3 candles, bull to bear
			// 1st candle is a long bull bodied candle
			// 2nd gaps higher on open, small real body (red or green), completely above candle 1
			// 3rd candle, real red body which closes well into 1st body, the longer, the better
			//     higher volume adds confirmation to this candle
			if (showEveningStar)
			{
				if (Close[0] < Open[0] && Close[2] > Open[2] && Close[3] > Open[3])
				{
					if (Close[0] < Close[2] && Close[0] < Close[1] && Close[2] > Close[3])
					{
						if (Open[1] > Close[2] && Close[1] > Close[2])
						{
							double barSize = 0;
							int bars = 8;
							for (int i = 0; i < bars; i++) 
							{
								barSize = High[i]-Low[i];
							}
							double avgBarSize = barSize/bars;
							// was going to use avgBarSize to see if 1sr candle ([2]) has a long body
							DrawTriangleDown(CurrentBar + " Bullish Evening Star", false, 1, High[1]*1.01, Color.Red); 
						}
					}
				}
				// look for Evening Star reversal pattern
				// 3 candles, bear to bull
				if (Close[0] > Open[0] && Close[2] < Open[2] && Close[3] < Open[3])
				{
					if (Close[0] > Close[2] && Close[0] > Close[1] && Close[2] < Close[3])
					{
						if (Open[1] < Close[2] && Close[1] < Close[2])
						{
							double barSize = 0;
							int bars = 8;
							for (int i = 0; i < bars; i++) 
							{
								barSize = High[i]-Low[i];
							}
							double avgBarSize = barSize/bars;
							DrawTriangleUp(CurrentBar + " Bearish Evening Star", false, 1, Low[1]*0.99, Color.Green); 
						}
					}
				}
			}
        }

        #region Properties

        [Description("Show Evening Start reversal patterns")]
        [GridCategory("Parameters")]
        public bool ShowEveningStar
        {
            get { return showEveningStar; }
            set { showEveningStar = value; }
        }

        [Description("Show Engulfing patterns, reversal pattern, new bar is engulfed by prior bar")]
        [GridCategory("Parameters")]
        public bool ShowEngulfing
        {
            get { return showEngulfing; }
            set { showEngulfing = value; }
        }

		[Description("Show Harami patterns, reversal pattern, new bar engulfs prior bar")]
        [GridCategory("Parameters")]
        public bool ShowHarami
        {
            get { return showHarami; }
            set { showHarami = value; }
        }

		[Description("Show Piercing line/dark cloud cover patterns")]
        [GridCategory("Parameters")]
        public bool ShowPiercing
        {
            get { return showPiercing; }
            set { showPiercing = value; }
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
        private AACandlePatterns[] cacheAACandlePatterns = null;

        private static AACandlePatterns checkAACandlePatterns = new AACandlePatterns();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AACandlePatterns AACandlePatterns(bool showEngulfing, bool showEveningStar, bool showHarami, bool showPiercing)
        {
            return AACandlePatterns(Input, showEngulfing, showEveningStar, showHarami, showPiercing);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public AACandlePatterns AACandlePatterns(Data.IDataSeries input, bool showEngulfing, bool showEveningStar, bool showHarami, bool showPiercing)
        {
            if (cacheAACandlePatterns != null)
                for (int idx = 0; idx < cacheAACandlePatterns.Length; idx++)
                    if (cacheAACandlePatterns[idx].ShowEngulfing == showEngulfing && cacheAACandlePatterns[idx].ShowEveningStar == showEveningStar && cacheAACandlePatterns[idx].ShowHarami == showHarami && cacheAACandlePatterns[idx].ShowPiercing == showPiercing && cacheAACandlePatterns[idx].EqualsInput(input))
                        return cacheAACandlePatterns[idx];

            lock (checkAACandlePatterns)
            {
                checkAACandlePatterns.ShowEngulfing = showEngulfing;
                showEngulfing = checkAACandlePatterns.ShowEngulfing;
                checkAACandlePatterns.ShowEveningStar = showEveningStar;
                showEveningStar = checkAACandlePatterns.ShowEveningStar;
                checkAACandlePatterns.ShowHarami = showHarami;
                showHarami = checkAACandlePatterns.ShowHarami;
                checkAACandlePatterns.ShowPiercing = showPiercing;
                showPiercing = checkAACandlePatterns.ShowPiercing;

                if (cacheAACandlePatterns != null)
                    for (int idx = 0; idx < cacheAACandlePatterns.Length; idx++)
                        if (cacheAACandlePatterns[idx].ShowEngulfing == showEngulfing && cacheAACandlePatterns[idx].ShowEveningStar == showEveningStar && cacheAACandlePatterns[idx].ShowHarami == showHarami && cacheAACandlePatterns[idx].ShowPiercing == showPiercing && cacheAACandlePatterns[idx].EqualsInput(input))
                            return cacheAACandlePatterns[idx];

                AACandlePatterns indicator = new AACandlePatterns();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ShowEngulfing = showEngulfing;
                indicator.ShowEveningStar = showEveningStar;
                indicator.ShowHarami = showHarami;
                indicator.ShowPiercing = showPiercing;
                Indicators.Add(indicator);
                indicator.SetUp();

                AACandlePatterns[] tmp = new AACandlePatterns[cacheAACandlePatterns == null ? 1 : cacheAACandlePatterns.Length + 1];
                if (cacheAACandlePatterns != null)
                    cacheAACandlePatterns.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAACandlePatterns = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AACandlePatterns AACandlePatterns(bool showEngulfing, bool showEveningStar, bool showHarami, bool showPiercing)
        {
            return _indicator.AACandlePatterns(Input, showEngulfing, showEveningStar, showHarami, showPiercing);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AACandlePatterns AACandlePatterns(Data.IDataSeries input, bool showEngulfing, bool showEveningStar, bool showHarami, bool showPiercing)
        {
            return _indicator.AACandlePatterns(input, showEngulfing, showEveningStar, showHarami, showPiercing);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AACandlePatterns AACandlePatterns(bool showEngulfing, bool showEveningStar, bool showHarami, bool showPiercing)
        {
            return _indicator.AACandlePatterns(Input, showEngulfing, showEveningStar, showHarami, showPiercing);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.AACandlePatterns AACandlePatterns(Data.IDataSeries input, bool showEngulfing, bool showEveningStar, bool showHarami, bool showPiercing)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AACandlePatterns(input, showEngulfing, showEveningStar, showHarami, showPiercing);
        }
    }
}
#endregion
