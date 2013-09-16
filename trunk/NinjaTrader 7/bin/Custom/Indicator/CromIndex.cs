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
    /// Indicator to use for confirmation against the indexes.  
	/// Written by Rick Cromer (dynoweb) - 2013
	/// 
	/// Not complate
	/// 
	/// 
    /// </summary>
    [Description("Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013")]
    public class CromIndex : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period1 = 5; // Default setting for Period1
            private int period2 = 10; // Default setting for Period2
            private int period3 = 20; // Default setting for Period1
            private int period4 = 50; // Default setting for Period2
            private string instrument1 = @"ES 09-13"; // Default setting for Instrument1
            private string instrument2 = @"ES 09-13"; // Default setting for Instrument2
            private string instrument3 = @"YM 09-13"; // Default setting for Instrument3
            private string instrument4 = @"TF 09-13"; // Default setting for Instrument4
        // User defined variables (add any user defined variables below)
			private int				periodD	= 3;	// SlowDperiod
			private int				periodK	= 5;	// Kperiod
			private int				smooth	= 2;	// SlowKperiod
		    Color[] plotColor = new Color[4];
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            //Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "Plot0"));
			
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear0"));	// 0
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "Bear3"));

			Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull0"));  // 4
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull1"));
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull2"));
            Add(new Plot(Color.FromKnownColor(KnownColor.MediumSeaGreen), PlotStyle.Block, "Bull3"));

            Add(new Plot(Color.FromKnownColor(KnownColor.White), PlotStyle.Block, "Marker"));  // 8

			Add(new Plot(new Pen(Color.Red, 2), PlotStyle.Block, "SumBearStrong")); 
            Add(new Plot(new Pen(Color.Pink, 2), PlotStyle.Block, "SumBearWeak"));  // 10
            Add(new Plot(new Pen(Color.Green, 2), PlotStyle.Block, "SumBullStrong"));
            Add(new Plot(new Pen(Color.LimeGreen, 2), PlotStyle.Block, "SumBullWeak")); // 12
            Add(new Plot(new Pen(Color.Gray, 2), PlotStyle.Block, "SumNeutral"));
						
			
			Plots[0].Pen.Width = 3;
			Plots[1].Pen.Width = 3;
			Plots[2].Pen.Width = 3;
			Plots[3].Pen.Width = 3;

			Plots[4].Pen.Width = 3;
			Plots[5].Pen.Width = 3;
			Plots[6].Pen.Width = 3;
			Plots[7].Pen.Width = 3;
			
			Add(Instrument1, BarsPeriod.Id, BarsPeriod.Value);
			Add(Instrument2, BarsPeriod.Id, BarsPeriod.Value);
			Add(Instrument3, BarsPeriod.Id, BarsPeriod.Value);
			Add(Instrument4, BarsPeriod.Id, BarsPeriod.Value);
			
			Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            //Plot0.Set(Close[0]);
			
			// Checks to ensure all Bars objects contain enough bars before beginning
		    if (CurrentBars[0] <= BarsRequired || 
				CurrentBars[1] <= BarsRequired || 
				CurrentBars[2] <= BarsRequired || 
				CurrentBars[3] <= BarsRequired)
        		return;
			
			int strength = 0;
			//Stochastics stoc0 = Stochastics(BarsArray[0], periodD, periodK, smooth);
			
			if (BarsInProgress == 0) 
			{
				strength = plotStoc(BarsInProgress);				
				Bear0.Set(BarsInProgress);
				Print(Time + " BarsInProgress: " + BarsInProgress + " strength: " + strength);
			} 
			else if (BarsInProgress == 1)
			{
				strength += plotStoc(BarsInProgress);				
				Print(Time + " BarsInProgress: " + BarsInProgress + " strength: " + strength);
			}
			else
			{
				return;
			}
				
			/*
			if (Rising(Stochastics(BarsArray[0], periodD, periodK, smooth).D)) {
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
			
			switch ((int) (strength)) {
				case -2:
					//SumBearStrong.Set(5);
					Plots[0].Pen.Color = Color.Red; 
					Plots[0] = 5;
					break;
				case -1:
					//SumBearWeak.Set(5);
					break;
				case 0:
					//SumNeutral.Set(5);
					break;
				case 1:
					//SumBullWeak.Set(5);
					break;
				case 2:
					Plots[0].Pen.Color  = Color.Blue; 
					Plots[0].Set(5);
					//SumBullStrong.Set(5);
					//Bull1.Set(5);
					break;

				default:					
					break;
			}
*/

		}

		private int plotStoc(int bip) 
		{
			//Print(Time + " BarsInProgress: " + bip);
			
			int strength = 0;
			Stochastics stoc = Stochastics(BarsArray[bip], periodD, periodK, smooth);

			// TODO: add color properties
			
						// faster line is K, starting up
			if (Rising(stoc.K) 
				&& stoc.K[1] <= 50 
				&& Rising(stoc.D)
				&& stoc.K[0] > stoc.D[0]
				) {
				PlotColors[bip][0] = Color.Green;
			//	Plots[bip][0]=(2);
				strength = 2;
				return strength;
			} 
				
			// fast up, d hopefully following
			if (Rising(stoc.K)
				&& stoc.K[1] <= 50
				&& Falling(stoc.D)
				&& stoc.K[0] > stoc.D[0]
				) {
				PlotColors[bip][0] = Color.LimeGreen;
				//Bear0.Set(2);
				//SumBullWeak.Set(bip);
				strength = 1;
				return strength;
			} 
		
			// ending up
			if (Rising(stoc.K)
				&& stoc.K[1] >= 50
				&& Rising(stoc.D)
				&& stoc.K[0] > stoc.D[0]
				) {
				PlotColors[bip][0] = Color.LimeGreen;
				//SumBullWeak.Set(bip);
				strength = 1;
				return strength;
			} 
		
			// starting confirmed down
			if (Falling(stoc.K) 
				&& stoc.K[1] >= 50
				&& Falling(stoc.D)
				&& stoc.K[0] < stoc.D[0]
				) {
				PlotColors[bip][0] = Color.Red;
				//SumBearStrong.Set(bip);
				strength = -2;
				return strength;
			}

			if (Falling(stoc.K)
				&& stoc.K[1] >= 50
				&& Rising(stoc.D)
				&& stoc.K[0] < stoc.D[0]
				) {
				PlotColors[bip][0] = Color.Pink;
				//SumBearWeak.Set(bip);
				strength = -1;
				return strength;
			} 

			if (Falling(stoc.K)
				&& stoc.K[1] < 50
				&& Falling(stoc.D)
				&& stoc.K[0] < stoc.D[0]
				) {
				PlotColors[bip][0] = Color.Pink;
				//SumBearWeak.Set(bip);
				strength = -1;
				return strength;
			} 

/*			
			// faster line is K, starting up
			if (Rising(stoc.K) 
				&& stoc.K[1] <= 50 
				&& Rising(stoc.D)
				&& stoc.K[0] > stoc.D[0]
				) {
				SumBullStrong.Set(bip);
				strength = 2;
				return strength;
			} 
				
			// fast up, d hopefully following
			if (Rising(stoc.K)
				&& stoc.K[1] <= 50
				&& Falling(stoc.D)
				&& stoc.K[0] > stoc.D[0]
				) {
				SumBullWeak.Set(bip);
				strength = 1;
				return strength;
			} 
		
			// ending up
			if (Rising(stoc.K)
				&& stoc.K[1] >= 50
				&& Rising(stoc.D)
				&& stoc.K[0] > stoc.D[0]
				) {
				SumBullWeak.Set(bip);
				strength = 1;
				return strength;
			} 
		
			// starting confirmed down
			if (Falling(stoc.K) 
				&& stoc.K[1] >= 50
				&& Falling(stoc.D)
				&& stoc.K[0] < stoc.D[0]
				) {
				SumBearStrong.Set(bip);
				strength = -2;
				return strength;
			}

			if (Falling(stoc.K)
				&& stoc.K[1] >= 50
				&& Rising(stoc.D)
				&& stoc.K[0] < stoc.D[0]
				) {
				SumBearWeak.Set(bip);
				strength = -1;
				return strength;
			} 

			if (Falling(stoc.K)
				&& stoc.K[1] < 50
				&& Falling(stoc.D)
				&& stoc.K[0] < stoc.D[0]
				) {
				SumBearWeak.Set(bip);
				strength = -1;
				return strength;
			} 
*/
				
				//Print(Time + " Rising(stoc.K): " + Rising(stoc.K) + " Rising(stoc.D): " + Rising(stoc.D)
				//    + " Falling(stoc.K): " + Falling(stoc.K) + " Falling(stoc.D): " + Falling(stoc.D));
			
/*
			// faster line is K
			if (Rising(stoc.K) && Rising(stoc.D)) {
				SumBullStrong.Set(bip);
				strength = 2;
			} 
			if (Rising(stoc.K) && Falling(stoc.D)) {
				SumBullWeak.Set(bip);
				//SumBullStrong.Set(bip);
				strength = 1;
				//Print(Time + " Rising(stoc.K): " + Rising(stoc.K) + " Rising(stoc.D): " + Rising(stoc.D)
				//    + " Falling(stoc.K): " + Falling(stoc.K) + " Falling(stoc.D): " + Falling(stoc.D));
			} 
		
			if (Falling(stoc.K) && Rising(stoc.D)) {
				SumBearWeak.Set(bip);
				strength = -1;
			} 

			if (Falling(stoc.K) && Falling(stoc.D)) {
				SumBearStrong.Set(bip);
				strength = -2;
			}
*/			
			PlotColors[bip][0] = Color.Gray;
			return 0;
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

        [Description("Instrument to compare with")]
        [GridCategory("Parameters")]
        public string Instrument3
        {
            get { return instrument3; }
            set { instrument3 = value; }
        }

        [Description("Instrument to compare with")]
        [GridCategory("Parameters")]
        public string Instrument4
        {
            get { return instrument4; }
            set { instrument4 = value; }
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
        private CromIndex[] cacheCromIndex = null;

        private static CromIndex checkCromIndex = new CromIndex();

        /// <summary>
        /// Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013
        /// </summary>
        /// <returns></returns>
        public CromIndex CromIndex(string instrument1, string instrument2, string instrument3, string instrument4, int period1, int period2, int period3, int period4)
        {
            return CromIndex(Input, instrument1, instrument2, instrument3, instrument4, period1, period2, period3, period4);
        }

        /// <summary>
        /// Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013
        /// </summary>
        /// <returns></returns>
        public CromIndex CromIndex(Data.IDataSeries input, string instrument1, string instrument2, string instrument3, string instrument4, int period1, int period2, int period3, int period4)
        {
            if (cacheCromIndex != null)
                for (int idx = 0; idx < cacheCromIndex.Length; idx++)
                    if (cacheCromIndex[idx].Instrument1 == instrument1 && cacheCromIndex[idx].Instrument2 == instrument2 && cacheCromIndex[idx].Instrument3 == instrument3 && cacheCromIndex[idx].Instrument4 == instrument4 && cacheCromIndex[idx].Period1 == period1 && cacheCromIndex[idx].Period2 == period2 && cacheCromIndex[idx].Period3 == period3 && cacheCromIndex[idx].Period4 == period4 && cacheCromIndex[idx].EqualsInput(input))
                        return cacheCromIndex[idx];

            lock (checkCromIndex)
            {
                checkCromIndex.Instrument1 = instrument1;
                instrument1 = checkCromIndex.Instrument1;
                checkCromIndex.Instrument2 = instrument2;
                instrument2 = checkCromIndex.Instrument2;
                checkCromIndex.Instrument3 = instrument3;
                instrument3 = checkCromIndex.Instrument3;
                checkCromIndex.Instrument4 = instrument4;
                instrument4 = checkCromIndex.Instrument4;
                checkCromIndex.Period1 = period1;
                period1 = checkCromIndex.Period1;
                checkCromIndex.Period2 = period2;
                period2 = checkCromIndex.Period2;
                checkCromIndex.Period3 = period3;
                period3 = checkCromIndex.Period3;
                checkCromIndex.Period4 = period4;
                period4 = checkCromIndex.Period4;

                if (cacheCromIndex != null)
                    for (int idx = 0; idx < cacheCromIndex.Length; idx++)
                        if (cacheCromIndex[idx].Instrument1 == instrument1 && cacheCromIndex[idx].Instrument2 == instrument2 && cacheCromIndex[idx].Instrument3 == instrument3 && cacheCromIndex[idx].Instrument4 == instrument4 && cacheCromIndex[idx].Period1 == period1 && cacheCromIndex[idx].Period2 == period2 && cacheCromIndex[idx].Period3 == period3 && cacheCromIndex[idx].Period4 == period4 && cacheCromIndex[idx].EqualsInput(input))
                            return cacheCromIndex[idx];

                CromIndex indicator = new CromIndex();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Instrument1 = instrument1;
                indicator.Instrument2 = instrument2;
                indicator.Instrument3 = instrument3;
                indicator.Instrument4 = instrument4;
                indicator.Period1 = period1;
                indicator.Period2 = period2;
                indicator.Period3 = period3;
                indicator.Period4 = period4;
                Indicators.Add(indicator);
                indicator.SetUp();

                CromIndex[] tmp = new CromIndex[cacheCromIndex == null ? 1 : cacheCromIndex.Length + 1];
                if (cacheCromIndex != null)
                    cacheCromIndex.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheCromIndex = tmp;
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
        /// Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CromIndex CromIndex(string instrument1, string instrument2, string instrument3, string instrument4, int period1, int period2, int period3, int period4)
        {
            return _indicator.CromIndex(Input, instrument1, instrument2, instrument3, instrument4, period1, period2, period3, period4);
        }

        /// <summary>
        /// Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013
        /// </summary>
        /// <returns></returns>
        public Indicator.CromIndex CromIndex(Data.IDataSeries input, string instrument1, string instrument2, string instrument3, string instrument4, int period1, int period2, int period3, int period4)
        {
            return _indicator.CromIndex(input, instrument1, instrument2, instrument3, instrument4, period1, period2, period3, period4);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.CromIndex CromIndex(string instrument1, string instrument2, string instrument3, string instrument4, int period1, int period2, int period3, int period4)
        {
            return _indicator.CromIndex(Input, instrument1, instrument2, instrument3, instrument4, period1, period2, period3, period4);
        }

        /// <summary>
        /// Indicator to use for confirmation against the indexes.  Written by Rick Cromer (dynoweb) - 2013
        /// </summary>
        /// <returns></returns>
        public Indicator.CromIndex CromIndex(Data.IDataSeries input, string instrument1, string instrument2, string instrument3, string instrument4, int period1, int period2, int period3, int period4)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.CromIndex(input, instrument1, instrument2, instrument3, instrument4, period1, period2, period3, period4);
        }
    }
}
#endregion
