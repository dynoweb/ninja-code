// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
	/// </summary>
	[Description("The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.")]
	public class MACDRick2 : Indicator
	{
		#region Variables
		private int					fast	= 5;
		private int					slow	= 20;
		private int					smooth	= 30;
		private	DataSeries		fastEma;
		private	DataSeries		slowEma;
		private int upDown = 0;
		
		Color backgroundShortColor = Color.Red;
		Color backgroundLongColor = Color.Green;
		int opacity	= 25;
		bool enableBackgroundColor = true;
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			/*
			Add(new Plot(Color.Green, "Macd"));
			Add(new Plot(Color.DarkViolet, "Avg"));
			Add(new Plot(new Pen(Color.Navy, 2), PlotStyle.Bar, "Diff"));
			*/
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Black),2), PlotStyle.Bar, "Macd"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Blue), 3), PlotStyle.Line, "Avg"));
            Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Transparent), 1), PlotStyle.Line, "Diff"));
			Add(new Plot(new Pen(Color.FromKnownColor(KnownColor.Black), 1), PlotStyle.Line, "Mom"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));

			fastEma	= new DataSeries(this);
			slowEma	= new DataSeries(this);
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				fastEma.Set(Input[0]);
				slowEma.Set(Input[0]);
				Value.Set(0);
				Avg.Set(0);
				Diff.Set(0);
			}
			else
			{
				upDown = 0;
				fastEma.Set((2.0 / (1 + Fast)) * Input[0] + (1 - (2.0 / (1 + Fast))) * fastEma[1]);
				slowEma.Set((2.0 / (1 + Slow)) * Input[0] + (1 - (2.0 / (1 + Slow))) * slowEma[1]);

				double macd		= fastEma[0] - slowEma[0];
				double macdAvg	= (2.0 / (1 + Smooth)) * macd + (1 - (2.0 / (1 + Smooth))) * Avg[1];
				
				Value.Set(macd);
				Avg.Set(macdAvg);
				//Diff.Set(macd - macdAvg);
				Diff.Set(macd);
				
				if (Value[0] > Value[1])
				{
					PlotColors[0][0] = Color.Green;
					upDown++;
				} else {
					PlotColors[0][0] = Color.Red;
					upDown--;
				}
				
				if (Rising(Avg)) 
				{
					PlotColors[1][0] = Color.Green;
					upDown++;
				} else {
					PlotColors[1][0] = Color.Red;
					upDown--;
				}
				
				Mom.Set(macd);
				
				// dad = macdAvg
				// mom = macd
				if (macd > 0 && macdAvg > 0)
					BackColorAll = Color.FromArgb(Opacity, BackgroundLongColor);
				else if (macd < 0 && macdAvg < 0)
					BackColorAll = Color.FromArgb(Opacity, BackgroundShortColor);
			}
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Avg
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Default
		{
			get { return Values[0]; }
		}
		
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Diff
		{
			get { return Values[2]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Mom
		{
			get { return Values[3]; }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for fast EMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for slow EMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}

        [Description("Opacity range 0-100, lower numbers for more transparency")]
        [GridCategory("Background")]
        public int Opacity
        {
            get { return opacity; }
            set { opacity = Math.Max(0, value); }
        }
		
        [Description("Color for background MACD long")]
        [GridCategory("Background")]
        [Gui.Design.DisplayNameAttribute("Background Long Color")]
        public Color BackgroundLongColor
        {
            get { return backgroundLongColor; }
            set { backgroundLongColor = value; }
        }
		
        [Description("Color for background MACD short")]
        [GridCategory("Background")]
        [Gui.Design.DisplayNameAttribute("Background Short Color")]
        public Color BackgroundShortColor
        {
            get { return backgroundShortColor; }
            set { backgroundShortColor = value; }
        }
		
        [Description("Turn on background trend colors")]
        [GridCategory("Background")]
        [Gui.Design.DisplayNameAttribute("Background Colors")]
        public bool EnableBackgroundColor
        {
            get { return enableBackgroundColor; }
            set { enableBackgroundColor = value; }
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
        private MACDRick2[] cacheMACDRick2 = null;

        private static MACDRick2 checkMACDRick2 = new MACDRick2();

        /// <summary>
        /// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public MACDRick2 MACDRick2(Color backgroundLongColor, Color backgroundShortColor, bool enableBackgroundColor, int fast, int opacity, int slow, int smooth)
        {
            return MACDRick2(Input, backgroundLongColor, backgroundShortColor, enableBackgroundColor, fast, opacity, slow, smooth);
        }

        /// <summary>
        /// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public MACDRick2 MACDRick2(Data.IDataSeries input, Color backgroundLongColor, Color backgroundShortColor, bool enableBackgroundColor, int fast, int opacity, int slow, int smooth)
        {
            if (cacheMACDRick2 != null)
                for (int idx = 0; idx < cacheMACDRick2.Length; idx++)
                    if (cacheMACDRick2[idx].BackgroundLongColor == backgroundLongColor && cacheMACDRick2[idx].BackgroundShortColor == backgroundShortColor && cacheMACDRick2[idx].EnableBackgroundColor == enableBackgroundColor && cacheMACDRick2[idx].Fast == fast && cacheMACDRick2[idx].Opacity == opacity && cacheMACDRick2[idx].Slow == slow && cacheMACDRick2[idx].Smooth == smooth && cacheMACDRick2[idx].EqualsInput(input))
                        return cacheMACDRick2[idx];

            lock (checkMACDRick2)
            {
                checkMACDRick2.BackgroundLongColor = backgroundLongColor;
                backgroundLongColor = checkMACDRick2.BackgroundLongColor;
                checkMACDRick2.BackgroundShortColor = backgroundShortColor;
                backgroundShortColor = checkMACDRick2.BackgroundShortColor;
                checkMACDRick2.EnableBackgroundColor = enableBackgroundColor;
                enableBackgroundColor = checkMACDRick2.EnableBackgroundColor;
                checkMACDRick2.Fast = fast;
                fast = checkMACDRick2.Fast;
                checkMACDRick2.Opacity = opacity;
                opacity = checkMACDRick2.Opacity;
                checkMACDRick2.Slow = slow;
                slow = checkMACDRick2.Slow;
                checkMACDRick2.Smooth = smooth;
                smooth = checkMACDRick2.Smooth;

                if (cacheMACDRick2 != null)
                    for (int idx = 0; idx < cacheMACDRick2.Length; idx++)
                        if (cacheMACDRick2[idx].BackgroundLongColor == backgroundLongColor && cacheMACDRick2[idx].BackgroundShortColor == backgroundShortColor && cacheMACDRick2[idx].EnableBackgroundColor == enableBackgroundColor && cacheMACDRick2[idx].Fast == fast && cacheMACDRick2[idx].Opacity == opacity && cacheMACDRick2[idx].Slow == slow && cacheMACDRick2[idx].Smooth == smooth && cacheMACDRick2[idx].EqualsInput(input))
                            return cacheMACDRick2[idx];

                MACDRick2 indicator = new MACDRick2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BackgroundLongColor = backgroundLongColor;
                indicator.BackgroundShortColor = backgroundShortColor;
                indicator.EnableBackgroundColor = enableBackgroundColor;
                indicator.Fast = fast;
                indicator.Opacity = opacity;
                indicator.Slow = slow;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                MACDRick2[] tmp = new MACDRick2[cacheMACDRick2 == null ? 1 : cacheMACDRick2.Length + 1];
                if (cacheMACDRick2 != null)
                    cacheMACDRick2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMACDRick2 = tmp;
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
        /// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACDRick2 MACDRick2(Color backgroundLongColor, Color backgroundShortColor, bool enableBackgroundColor, int fast, int opacity, int slow, int smooth)
        {
            return _indicator.MACDRick2(Input, backgroundLongColor, backgroundShortColor, enableBackgroundColor, fast, opacity, slow, smooth);
        }

        /// <summary>
        /// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.MACDRick2 MACDRick2(Data.IDataSeries input, Color backgroundLongColor, Color backgroundShortColor, bool enableBackgroundColor, int fast, int opacity, int slow, int smooth)
        {
            return _indicator.MACDRick2(input, backgroundLongColor, backgroundShortColor, enableBackgroundColor, fast, opacity, slow, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACDRick2 MACDRick2(Color backgroundLongColor, Color backgroundShortColor, bool enableBackgroundColor, int fast, int opacity, int slow, int smooth)
        {
            return _indicator.MACDRick2(Input, backgroundLongColor, backgroundShortColor, enableBackgroundColor, fast, opacity, slow, smooth);
        }

        /// <summary>
        /// The MACD (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.MACDRick2 MACDRick2(Data.IDataSeries input, Color backgroundLongColor, Color backgroundShortColor, bool enableBackgroundColor, int fast, int opacity, int slow, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MACDRick2(input, backgroundLongColor, backgroundShortColor, enableBackgroundColor, fast, opacity, slow, smooth);
        }
    }
}
#endregion
