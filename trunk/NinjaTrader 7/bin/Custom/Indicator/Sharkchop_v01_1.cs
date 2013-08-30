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
	[Description ("Enter the description of your new custom indicator here")]
	public class Sharkchop_v01_1 : Indicator//Sharkchop_v01
	{
		#region Variables
		// Wizard generated variables
		private int longLeg = 13; // Default setting for LongLeg
		private int shortLeg = 16; // Default setting for ShortLeg
		private int multiLeg = 1; // Default setting for MultiLeg
		private int multiW = 1; // Default setting for MultiW
		// User defined variables (add any user defined variables below)
		#endregion

		double SyncShort, SyncLong, SyncMulti;
		double ShortWeight, LongWeight, MultiWeight;

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize ()
		{
			//Add (new Line (Color.Black, 0, "ZeroLine"));
			Add(new Line(new Pen(Color.Black, 35), 0, "Zero line"));
			
//			Add (new Plot (Color.FromKnownColor (KnownColor.Red), PlotStyle.Line, "MultiOsc"));
//			Add (new Plot (Color.FromKnownColor (KnownColor.Transparent), PlotStyle.Line, "Netural"));
//			Add (new Plot (Color.FromKnownColor (KnownColor.Lime), PlotStyle.Line, "MultiLow"));
			Add(new Plot(new Pen(Color.Red,4), PlotStyle.Line, "Short"));//0MultiOsc
			Add(new Plot(new Pen(Color.Lime,4), PlotStyle.Line, "Long"));//1MultiLow
			
			CalculateOnBarClose = false;
			Overlay = false;
			PriceTypeSupported = true;
			PriceType = PriceType.Close;

			ShortWeight = 2.0 / (ShortLeg + 1);
			LongWeight = 2.0 / (LongLeg + 1);
			MultiWeight = 2.0 / (MultiLeg + 1);
		}


		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate ()
		{
			if (FirstTickOfBar) // Ducman, added
			{
				if (SyncShort == 0)
				{
					SyncShort = Close [0];
					SyncLong = Close [0];
				}
				else
				{
					SyncShort = SyncShort * (1 - ShortWeight) + (ShortWeight * Close [0]);
					SyncLong = SyncLong * (1 - LongWeight) + (LongWeight * Close [0]);
				}

				double multiOsc = 100 * ((SyncShort / SyncLong) - 1);
				if (SyncMulti == 0)
					SyncMulti = multiOsc;
				else
					SyncMulti = Math.Abs (SyncMulti) * (1 - MultiWeight) + (MultiWeight * multiOsc);

//				MultiHigh.Set (SyncMulti * MultiW);
//				MultiLow.Set (-SyncMulti * MultiW);
//				MultiOsc.Set (multiOsc);
//				MultiHigh.Set (SyncMulti * MultiW);
				Long.Set (-SyncMulti * MultiW);
				Short.Set (multiOsc);
			
//				Print("Time is: " + Time[0]);
//				Print("Close[0] is: " + Close[0]);
//				Print("SyncShort is: " + SyncShort);
//				Print("SyncLong is: " + SyncLong);
//				Print("SyncMulti is: " + SyncMulti);						
//				Print("multiOsc is: " + multiOsc);		
			}
		}
		#region Properties
		
//		[Browsable (false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//		[XmlIgnore ()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//		public DataSeries MultiOsc
//		{
//			get { return Values [0]; }
//		}

		[Browsable(false)]		[XmlIgnore()]	public DataSeries Short			{	get { return Values[0]; }	}
		[Browsable(false)]		[XmlIgnore()]	public DataSeries Long		{	get { return Values[1]; }	}
		
//		[Browsable (false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//		[XmlIgnore ()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//		public DataSeries MultiHigh
//		{
//			get { return Values [1]; }
//		}
//
//		[Browsable (false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//		[XmlIgnore ()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//		public DataSeries MultiLow
//		{
//			get { return Values [2]; }
//		}



		[Description ("")]
		[Category ("Parameters")]
		public int LongLeg
		{
			get { return longLeg; }
			set { longLeg = Math.Max (1, value); }
		}

		[Description ("")]
		[Category ("Parameters")]
		public int ShortLeg
		{
			get { return shortLeg; }
			set { shortLeg = Math.Max (1, value); }
		}

		[Description ("")]
		[Category ("Parameters")]
		public int MultiLeg
		{
			get { return multiLeg; }
			set { multiLeg = Math.Max (1, value); }
		}

		[Description ("")]
		[Category ("Parameters")]
		public int MultiW
		{
			get { return multiW; }
			set { multiW = Math.Max (1, value); }
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
        private Sharkchop_v01_1[] cacheSharkchop_v01_1 = null;

        private static Sharkchop_v01_1 checkSharkchop_v01_1 = new Sharkchop_v01_1();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Sharkchop_v01_1 Sharkchop_v01_1(int longLeg, int multiLeg, int multiW, int shortLeg)
        {
            return Sharkchop_v01_1(Input, longLeg, multiLeg, multiW, shortLeg);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Sharkchop_v01_1 Sharkchop_v01_1(Data.IDataSeries input, int longLeg, int multiLeg, int multiW, int shortLeg)
        {
            if (cacheSharkchop_v01_1 != null)
                for (int idx = 0; idx < cacheSharkchop_v01_1.Length; idx++)
                    if (cacheSharkchop_v01_1[idx].LongLeg == longLeg && cacheSharkchop_v01_1[idx].MultiLeg == multiLeg && cacheSharkchop_v01_1[idx].MultiW == multiW && cacheSharkchop_v01_1[idx].ShortLeg == shortLeg && cacheSharkchop_v01_1[idx].EqualsInput(input))
                        return cacheSharkchop_v01_1[idx];

            lock (checkSharkchop_v01_1)
            {
                checkSharkchop_v01_1.LongLeg = longLeg;
                longLeg = checkSharkchop_v01_1.LongLeg;
                checkSharkchop_v01_1.MultiLeg = multiLeg;
                multiLeg = checkSharkchop_v01_1.MultiLeg;
                checkSharkchop_v01_1.MultiW = multiW;
                multiW = checkSharkchop_v01_1.MultiW;
                checkSharkchop_v01_1.ShortLeg = shortLeg;
                shortLeg = checkSharkchop_v01_1.ShortLeg;

                if (cacheSharkchop_v01_1 != null)
                    for (int idx = 0; idx < cacheSharkchop_v01_1.Length; idx++)
                        if (cacheSharkchop_v01_1[idx].LongLeg == longLeg && cacheSharkchop_v01_1[idx].MultiLeg == multiLeg && cacheSharkchop_v01_1[idx].MultiW == multiW && cacheSharkchop_v01_1[idx].ShortLeg == shortLeg && cacheSharkchop_v01_1[idx].EqualsInput(input))
                            return cacheSharkchop_v01_1[idx];

                Sharkchop_v01_1 indicator = new Sharkchop_v01_1();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LongLeg = longLeg;
                indicator.MultiLeg = multiLeg;
                indicator.MultiW = multiW;
                indicator.ShortLeg = shortLeg;
                Indicators.Add(indicator);
                indicator.SetUp();

                Sharkchop_v01_1[] tmp = new Sharkchop_v01_1[cacheSharkchop_v01_1 == null ? 1 : cacheSharkchop_v01_1.Length + 1];
                if (cacheSharkchop_v01_1 != null)
                    cacheSharkchop_v01_1.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSharkchop_v01_1 = tmp;
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
        public Indicator.Sharkchop_v01_1 Sharkchop_v01_1(int longLeg, int multiLeg, int multiW, int shortLeg)
        {
            return _indicator.Sharkchop_v01_1(Input, longLeg, multiLeg, multiW, shortLeg);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.Sharkchop_v01_1 Sharkchop_v01_1(Data.IDataSeries input, int longLeg, int multiLeg, int multiW, int shortLeg)
        {
            return _indicator.Sharkchop_v01_1(input, longLeg, multiLeg, multiW, shortLeg);
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
        public Indicator.Sharkchop_v01_1 Sharkchop_v01_1(int longLeg, int multiLeg, int multiW, int shortLeg)
        {
            return _indicator.Sharkchop_v01_1(Input, longLeg, multiLeg, multiW, shortLeg);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.Sharkchop_v01_1 Sharkchop_v01_1(Data.IDataSeries input, int longLeg, int multiLeg, int multiW, int shortLeg)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.Sharkchop_v01_1(input, longLeg, multiLeg, multiW, shortLeg);
        }
    }
}
#endregion
