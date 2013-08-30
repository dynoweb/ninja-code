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
    /// Sets the background color in a time range
    /// </summary>
    [Description("Sets the background color in a time range")]
    public class PitColor : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int endTime = 83000; // Default setting for EndTime
            private int startTime = 161500; // Default setting for StartTime
			private Color colorBackground = Color.Black;
			private int opacity		= 25;
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
			CalculateOnBarClose = true;
			DrawOnPricePanel = false;
			AutoScale = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			//DateTime exp = new DateTime(2013, 3, 15);
			if ((startTime > endTime && (ToTime(Time[0]) >= startTime || ToTime(Time[0]) <= endTime))
				|| (startTime < endTime && (ToTime(Time[0]) >= startTime && ToTime(Time[0]) <= endTime)))
			//if (ToTime(Time[0]) >= startTime)
			{	
				//BackColor = colorBackground;;
				BackColorAll = Color.FromArgb(Opacity, ColorBackground);
				//BackColorAll = colorBackground;  
			}
        }

        #region Properties

        [Description("Start of color HHMMSS")]
        [GridCategory("Parameters")]
        public int StartTime
        {
            get { return startTime; }
            set { startTime = Math.Max(0, value); }
        }

        [Description("End of color HHMMSS")]
        [GridCategory("Parameters")]
        public int EndTime
        {
            get { return endTime; }
            set { endTime = Math.Max(1, value); }
        }
		
        [Description("Opacity range 0-100, lower numbers for more transparency")]
        [GridCategory("Parameters")]
        public int Opacity
        {
            get { return opacity; }
            set { opacity = Math.Max(0, value); }
        }
		
        [Description("Color for Back Ground")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayNameAttribute("Background Color")]
        public Color ColorBackground
        {
            get { return colorBackground; }
            set { colorBackground = value; }
        }
		
        [Browsable(false)]
        public string bgColorDownSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(colorBackground); }
            set { colorBackground = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private PitColor[] cachePitColor = null;

        private static PitColor checkPitColor = new PitColor();

        /// <summary>
        /// Sets the background color in a time range
        /// </summary>
        /// <returns></returns>
        public PitColor PitColor(Color colorBackground, int endTime, int opacity, int startTime)
        {
            return PitColor(Input, colorBackground, endTime, opacity, startTime);
        }

        /// <summary>
        /// Sets the background color in a time range
        /// </summary>
        /// <returns></returns>
        public PitColor PitColor(Data.IDataSeries input, Color colorBackground, int endTime, int opacity, int startTime)
        {
            if (cachePitColor != null)
                for (int idx = 0; idx < cachePitColor.Length; idx++)
                    if (cachePitColor[idx].ColorBackground == colorBackground && cachePitColor[idx].EndTime == endTime && cachePitColor[idx].Opacity == opacity && cachePitColor[idx].StartTime == startTime && cachePitColor[idx].EqualsInput(input))
                        return cachePitColor[idx];

            lock (checkPitColor)
            {
                checkPitColor.ColorBackground = colorBackground;
                colorBackground = checkPitColor.ColorBackground;
                checkPitColor.EndTime = endTime;
                endTime = checkPitColor.EndTime;
                checkPitColor.Opacity = opacity;
                opacity = checkPitColor.Opacity;
                checkPitColor.StartTime = startTime;
                startTime = checkPitColor.StartTime;

                if (cachePitColor != null)
                    for (int idx = 0; idx < cachePitColor.Length; idx++)
                        if (cachePitColor[idx].ColorBackground == colorBackground && cachePitColor[idx].EndTime == endTime && cachePitColor[idx].Opacity == opacity && cachePitColor[idx].StartTime == startTime && cachePitColor[idx].EqualsInput(input))
                            return cachePitColor[idx];

                PitColor indicator = new PitColor();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.ColorBackground = colorBackground;
                indicator.EndTime = endTime;
                indicator.Opacity = opacity;
                indicator.StartTime = startTime;
                Indicators.Add(indicator);
                indicator.SetUp();

                PitColor[] tmp = new PitColor[cachePitColor == null ? 1 : cachePitColor.Length + 1];
                if (cachePitColor != null)
                    cachePitColor.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePitColor = tmp;
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
        /// Sets the background color in a time range
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PitColor PitColor(Color colorBackground, int endTime, int opacity, int startTime)
        {
            return _indicator.PitColor(Input, colorBackground, endTime, opacity, startTime);
        }

        /// <summary>
        /// Sets the background color in a time range
        /// </summary>
        /// <returns></returns>
        public Indicator.PitColor PitColor(Data.IDataSeries input, Color colorBackground, int endTime, int opacity, int startTime)
        {
            return _indicator.PitColor(input, colorBackground, endTime, opacity, startTime);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Sets the background color in a time range
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PitColor PitColor(Color colorBackground, int endTime, int opacity, int startTime)
        {
            return _indicator.PitColor(Input, colorBackground, endTime, opacity, startTime);
        }

        /// <summary>
        /// Sets the background color in a time range
        /// </summary>
        /// <returns></returns>
        public Indicator.PitColor PitColor(Data.IDataSeries input, Color colorBackground, int endTime, int opacity, int startTime)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PitColor(input, colorBackground, endTime, opacity, startTime);
        }
    }
}
#endregion
