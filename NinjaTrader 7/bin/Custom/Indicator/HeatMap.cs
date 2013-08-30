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
    /// Uses other periods to confirm trend
	/// Written By Rick Cromer 2013
    /// </summary>
    [Description("Uses other instruments to confirm trend")]
    public class HeatMap : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period1 = 5; // Default setting for Period1
            private int period2 = 10; // Default setting for Period2
            private int period3 = 20; // Default setting for Period1
            private int period4 = 50; // Default setting for Period2
            private string instrument1 = @""; // Default setting for Instrument1
            private string instrument2 = @""; // Default setting for Instrument2
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear0"));	// 0
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear3"));

			Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull0"));  // 4
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull3"));

            Add(new Plot(Color.FromKnownColor(KnownColor.White), PlotStyle.Block, "Marker"));  // 8

			Add(new Plot(new Pen(Color.Red, 8), PlotStyle.Block, "SumBearStrong")); 
            Add(new Plot(new Pen(Color.Pink, 8), PlotStyle.Block, "SumBearWeak"));  // 10
            Add(new Plot(new Pen(Color.Green, 8), PlotStyle.Block, "SumBullStrong"));
            Add(new Plot(new Pen(Color.LimeGreen, 8), PlotStyle.Block, "SumBullWeak")); // 12
            Add(new Plot(new Pen(Color.Gray, 8), PlotStyle.Block, "SumNeutral"));
						
			Plots[0].Pen.Width = 3;
			Plots[1].Pen.Width = 3;
			Plots[2].Pen.Width = 3;
			Plots[3].Pen.Width = 3;

			Plots[4].Pen.Width = 3;
			Plots[5].Pen.Width = 3;
			Plots[6].Pen.Width = 3;
			Plots[7].Pen.Width = 3;
			
			Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			
			int strength = 0;
			
			if (Rising(HMA(period1))) {
				Bull0.Set(3);
				strength++;
			} else {
				Bear0.Set(3);
			}
			
			if (Rising(HMA(period2))) {
				Bull1.Set(2);
				strength++;
			} else {
				Bear1.Set(2);
			}
			
			if (Rising(HMA(period3))) {
				Bull2.Set(1);
				strength++;
			} else {
				Bear2.Set(1);
			}
			
			if (Rising(HMA(period4))) {
				Bull3.Set(0);
				strength++;
			} else {
				Bear3.Set(0);
			}

			Print(Time + " strength: " + strength);
			
			switch ((int) (strength)) {
				case 0:
					SumBearStrong.Set(5);					
					break;
				case 1:
					SumBearWeak.Set(5);
					break;
				case 2:
					SumNeutral.Set(5);
					break;
				case 3:
					SumBullWeak.Set(5);
					break;
				case 4:
					SumBullStrong.Set(5);
					break;

				default:					
					break;
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bear0
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bear1
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bear2
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bear3
        {
            get { return Values[3]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bull0
        {
            get { return Values[4]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bull1
        {
            get { return Values[5]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bull2
        {
            get { return Values[6]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Bull3
        {
            get { return Values[7]; }
        }

		// -------------------------------------------------------------------------
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Marker
        {
            get { return Values[8]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SumBearStrong
        {
            get { return Values[9]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SumBearWeak
        {
            get { return Values[10]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SumBullStrong
        {
            get { return Values[11]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SumBullWeak
        {
            get { return Values[12]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SumNeutral
        {
            get { return Values[13]; }
        }

		// -------------------------------------------------------------------------
		
        [Description("HMA Period 1")]
        [GridCategory("Parameters")]
        public int Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }

        [Description("HMA Period 2")]
        [GridCategory("Parameters")]
        public int Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        [Description("HMA Period 3")]
        [GridCategory("Parameters")]
        public int Period3
        {
            get { return period3; }
            set { period3 = Math.Max(1, value); }
        }

        [Description("HMA Period 4")]
        [GridCategory("Parameters")]
        public int Period4
        {
            get { return period4; }
            set { period4 = Math.Max(1, value); }
        }

        [Description("Instrument to compare with")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayName("Instrument 1")]
        public string Instrument1
        {
            get { return instrument1; }
            set { instrument1 = value; }
        }

        [Description("Instrument to compare with")]
        [GridCategory("Parameters")]
        public string Instrument2
        {
            get { return instrument2; }
            set { instrument2 = value; }
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
        private HeatMap[] cacheHeatMap = null;

        private static HeatMap checkHeatMap = new HeatMap();

        /// <summary>
        /// Uses other instruments to confirm trend
        /// </summary>
        /// <returns></returns>
        public HeatMap HeatMap(string instrument1, string instrument2, int period1, int period2, int period3, int period4)
        {
            return HeatMap(Input, instrument1, instrument2, period1, period2, period3, period4);
        }

        /// <summary>
        /// Uses other instruments to confirm trend
        /// </summary>
        /// <returns></returns>
        public HeatMap HeatMap(Data.IDataSeries input, string instrument1, string instrument2, int period1, int period2, int period3, int period4)
        {
            if (cacheHeatMap != null)
                for (int idx = 0; idx < cacheHeatMap.Length; idx++)
                    if (cacheHeatMap[idx].Instrument1 == instrument1 && cacheHeatMap[idx].Instrument2 == instrument2 && cacheHeatMap[idx].Period1 == period1 && cacheHeatMap[idx].Period2 == period2 && cacheHeatMap[idx].Period3 == period3 && cacheHeatMap[idx].Period4 == period4 && cacheHeatMap[idx].EqualsInput(input))
                        return cacheHeatMap[idx];

            lock (checkHeatMap)
            {
                checkHeatMap.Instrument1 = instrument1;
                instrument1 = checkHeatMap.Instrument1;
                checkHeatMap.Instrument2 = instrument2;
                instrument2 = checkHeatMap.Instrument2;
                checkHeatMap.Period1 = period1;
                period1 = checkHeatMap.Period1;
                checkHeatMap.Period2 = period2;
                period2 = checkHeatMap.Period2;
                checkHeatMap.Period3 = period3;
                period3 = checkHeatMap.Period3;
                checkHeatMap.Period4 = period4;
                period4 = checkHeatMap.Period4;

                if (cacheHeatMap != null)
                    for (int idx = 0; idx < cacheHeatMap.Length; idx++)
                        if (cacheHeatMap[idx].Instrument1 == instrument1 && cacheHeatMap[idx].Instrument2 == instrument2 && cacheHeatMap[idx].Period1 == period1 && cacheHeatMap[idx].Period2 == period2 && cacheHeatMap[idx].Period3 == period3 && cacheHeatMap[idx].Period4 == period4 && cacheHeatMap[idx].EqualsInput(input))
                            return cacheHeatMap[idx];

                HeatMap indicator = new HeatMap();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Instrument1 = instrument1;
                indicator.Instrument2 = instrument2;
                indicator.Period1 = period1;
                indicator.Period2 = period2;
                indicator.Period3 = period3;
                indicator.Period4 = period4;
                Indicators.Add(indicator);
                indicator.SetUp();

                HeatMap[] tmp = new HeatMap[cacheHeatMap == null ? 1 : cacheHeatMap.Length + 1];
                if (cacheHeatMap != null)
                    cacheHeatMap.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheHeatMap = tmp;
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
        /// Uses other instruments to confirm trend
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HeatMap HeatMap(string instrument1, string instrument2, int period1, int period2, int period3, int period4)
        {
            return _indicator.HeatMap(Input, instrument1, instrument2, period1, period2, period3, period4);
        }

        /// <summary>
        /// Uses other instruments to confirm trend
        /// </summary>
        /// <returns></returns>
        public Indicator.HeatMap HeatMap(Data.IDataSeries input, string instrument1, string instrument2, int period1, int period2, int period3, int period4)
        {
            return _indicator.HeatMap(input, instrument1, instrument2, period1, period2, period3, period4);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Uses other instruments to confirm trend
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.HeatMap HeatMap(string instrument1, string instrument2, int period1, int period2, int period3, int period4)
        {
            return _indicator.HeatMap(Input, instrument1, instrument2, period1, period2, period3, period4);
        }

        /// <summary>
        /// Uses other instruments to confirm trend
        /// </summary>
        /// <returns></returns>
        public Indicator.HeatMap HeatMap(Data.IDataSeries input, string instrument1, string instrument2, int period1, int period2, int period3, int period4)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.HeatMap(input, instrument1, instrument2, period1, period2, period3, period4);
        }
    }
}
#endregion
