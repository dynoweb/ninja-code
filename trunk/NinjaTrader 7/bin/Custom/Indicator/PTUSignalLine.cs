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
	[Description("Premier Trader University Signal Line Indicator")]
  	[DisplayName("PTU Signal Line")]
    public class PTUSignalLine : Indicator
    {
        #region Variables
		private int npTrailLength = 3;
		private int BOMBDATE = 22130630;
		private bool securityValid;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			this.Add(new Plot(new Pen(Color.Teal, 1f), (PlotStyle) 6, "PTU_SignalLine"));
			this.CalculateOnBarClose = true;
			this.Overlay = true;
			this.PriceTypeSupported = false;
			//this.BOMBDATE = this.get_Instrument().get_MasterInstrument().get_Name() == "AAPL" || this.get_Instrument().get_MasterInstrument().get_Name() == "DIA" || (this.get_Instrument().get_MasterInstrument().get_Name() == "GOOGL" || this.get_Instrument().get_MasterInstrument().get_Name() == "NFLX") ? 22131231 : 20000101;
			this.securityValid = true;
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (this.CurrentBar == 0)
			{
				this.NP_Trail[0] = 0.0;
			}
			else
			{
				int num = Math.Min(this.NPTrailLength, this.CurrentBar);
				double val1_1 = this.High[0];
				double val1_2 = this.Low[0];
				for (int index = 0; index < num; ++index)
				{
				val1_1 = Math.Max(val1_1, this.High[index]);
				val1_2 = Math.Min(val1_2, this.Low[index]);
				}
				this.NP_Trail[0] = (val1_1 + val1_2) / 2.0;
			}
        }

        #region Properties
		
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries NP_Trail
		{
		get
		{
			return this.Values[0];
		}
		}

		[Description("The Trail Length")]
		[GridCategory("Parameters")]
		public int NPTrailLength
		{
		get
		{
			return this.npTrailLength;
		}
		set
		{
			this.npTrailLength = Math.Max(1, value);
		}
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
        private PTUSignalLine[] cachePTUSignalLine = null;

        private static PTUSignalLine checkPTUSignalLine = new PTUSignalLine();

        /// <summary>
        /// Premier Trader University Signal Line Indicator
        /// </summary>
        /// <returns></returns>
        public PTUSignalLine PTUSignalLine(int nPTrailLength)
        {
            return PTUSignalLine(Input, nPTrailLength);
        }

        /// <summary>
        /// Premier Trader University Signal Line Indicator
        /// </summary>
        /// <returns></returns>
        public PTUSignalLine PTUSignalLine(Data.IDataSeries input, int nPTrailLength)
        {
            if (cachePTUSignalLine != null)
                for (int idx = 0; idx < cachePTUSignalLine.Length; idx++)
                    if (cachePTUSignalLine[idx].NPTrailLength == nPTrailLength && cachePTUSignalLine[idx].EqualsInput(input))
                        return cachePTUSignalLine[idx];

            lock (checkPTUSignalLine)
            {
                checkPTUSignalLine.NPTrailLength = nPTrailLength;
                nPTrailLength = checkPTUSignalLine.NPTrailLength;

                if (cachePTUSignalLine != null)
                    for (int idx = 0; idx < cachePTUSignalLine.Length; idx++)
                        if (cachePTUSignalLine[idx].NPTrailLength == nPTrailLength && cachePTUSignalLine[idx].EqualsInput(input))
                            return cachePTUSignalLine[idx];

                PTUSignalLine indicator = new PTUSignalLine();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.NPTrailLength = nPTrailLength;
                Indicators.Add(indicator);
                indicator.SetUp();

                PTUSignalLine[] tmp = new PTUSignalLine[cachePTUSignalLine == null ? 1 : cachePTUSignalLine.Length + 1];
                if (cachePTUSignalLine != null)
                    cachePTUSignalLine.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePTUSignalLine = tmp;
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
        /// Premier Trader University Signal Line Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PTUSignalLine PTUSignalLine(int nPTrailLength)
        {
            return _indicator.PTUSignalLine(Input, nPTrailLength);
        }

        /// <summary>
        /// Premier Trader University Signal Line Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.PTUSignalLine PTUSignalLine(Data.IDataSeries input, int nPTrailLength)
        {
            return _indicator.PTUSignalLine(input, nPTrailLength);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Premier Trader University Signal Line Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.PTUSignalLine PTUSignalLine(int nPTrailLength)
        {
            return _indicator.PTUSignalLine(Input, nPTrailLength);
        }

        /// <summary>
        /// Premier Trader University Signal Line Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.PTUSignalLine PTUSignalLine(Data.IDataSeries input, int nPTrailLength)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PTUSignalLine(input, nPTrailLength);
        }
    }
}
#endregion
