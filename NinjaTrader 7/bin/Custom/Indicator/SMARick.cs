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
	/// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
	/// </summary>
	[Description("The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color")]
	public class SMARick : Indicator
	{
		#region Variables
			private int		period	= 14;
		
			private Color upColor			= Color.DarkGreen;
			private Color downColor			= Color.Red;
			private int plot0Width 			= 2;
			private PlotStyle plot0Style	= PlotStyle.Line;
			private DashStyle dash0Style	= DashStyle.Solid;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Green, "SMA"));
			PlotsConfigurable = false;	

			Overlay = true;
		}

		protected override void OnStartUp()
		{
			Plots[0].Pen.Width = plot0Width;
			Plots[0].PlotStyle = plot0Style;
			Plots[0].Pen.DashStyle = dash0Style;
			Plots[0].Pen.Color = Color.Gray;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick).
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				Value.Set(Input[0]);
			else
			{
				double last = Value[1] * Math.Min(CurrentBar, Period);

				if (CurrentBar >= Period)
				{
					Value.Set((last + Input[0] - Input[Period]) / Math.Min(CurrentBar, Period));
					
					if (Rising(Value))
					{
						PlotColors[0][0] = UpColor;
					}
					else
					{
						PlotColors[0][0] = DownColor;
					}
				}
				else
					Value.Set((last + Input[0]) / (Math.Min(CurrentBar, Period) + 1));
				
			}
		}

		#region Properties
		[Description("Select color for Rising Trend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Uptrend")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Select color for downtrend")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("Downtrend")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}

		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
			set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("Width for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Line Width")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}
		
		[Description("PlotStyle for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Plot Style Line")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		[Description("DashStyle for Line")]
		[Category("Plots")]
		[Gui.Design.DisplayNameAttribute("Dash Style Line")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		} 

		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
        private SMARick[] cacheSMARick = null;

        private static SMARick checkSMARick = new SMARick();

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color
        /// </summary>
        /// <returns></returns>
        public SMARick SMARick(int period)
        {
            return SMARick(Input, period);
        }

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color
        /// </summary>
        /// <returns></returns>
        public SMARick SMARick(Data.IDataSeries input, int period)
        {
            if (cacheSMARick != null)
                for (int idx = 0; idx < cacheSMARick.Length; idx++)
                    if (cacheSMARick[idx].Period == period && cacheSMARick[idx].EqualsInput(input))
                        return cacheSMARick[idx];

            lock (checkSMARick)
            {
                checkSMARick.Period = period;
                period = checkSMARick.Period;

                if (cacheSMARick != null)
                    for (int idx = 0; idx < cacheSMARick.Length; idx++)
                        if (cacheSMARick[idx].Period == period && cacheSMARick[idx].EqualsInput(input))
                            return cacheSMARick[idx];

                SMARick indicator = new SMARick();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                SMARick[] tmp = new SMARick[cacheSMARick == null ? 1 : cacheSMARick.Length + 1];
                if (cacheSMARick != null)
                    cacheSMARick.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSMARick = tmp;
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
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMARick SMARick(int period)
        {
            return _indicator.SMARick(Input, period);
        }

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color
        /// </summary>
        /// <returns></returns>
        public Indicator.SMARick SMARick(Data.IDataSeries input, int period)
        {
            return _indicator.SMARick(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMARick SMARick(int period)
        {
            return _indicator.SMARick(Input, period);
        }

        /// <summary>
        /// The SMA (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time. This indicator was modified from NT's indicator to add slope color
        /// </summary>
        /// <returns></returns>
        public Indicator.SMARick SMARick(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SMARick(input, period);
        }
    }
}
#endregion
