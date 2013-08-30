//Vers. 0.1 translated by Gumphrie

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
	/// Trigger Lines
	/// </summary>
	[Description("Trigger Lines.")]
	[Gui.Design.DisplayName("Trigger Lines")]
	public class TriggerLines : Indicator
	{
		#region Variables
		private int	length = 20; //80; //20;
		private int trigAvg = 8; //5; //8;
		private	DataSeries		value1;
		private	DataSeries		value2;
		private bool			bTrigWasRising = false;
		
		
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Cyan, "Plot1_Up"));
			Add(new Plot(Color.Magenta, "Plot1_Dn"));
			Add(new Plot(Color.Cyan, "Plot2_Up"));
			Add(new Plot(Color.Magenta, "Plot2_Dn"));


			value1	= new DataSeries(this);
			value2	= new DataSeries(this);

            Overlay				= true;
			PriceTypeSupported	= true;
			
			BarsRequired = length+trigAvg;
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			value1.Set(LinReg(base.Input,length)[0]);
			value2.Set(EMA(value1,trigAvg)[0]);
			
			
			if (value2[0]>=value1[0])
			{
				if (bTrigWasRising)
				{
					Plot1_Up.Set(value1[0]);
					Plot2_Up.Set(value2[0]);
				}
				Plot1_Dn.Set(value1[0]);
				Plot2_Dn.Set(value2[0]);
				bTrigWasRising = false;
			}
			else
			{
				if (!bTrigWasRising)
				{
					Plot1_Dn.Set(value1[0]);
					Plot2_Dn.Set(value2[0]);
				}
				Plot1_Up.Set(value1[0]);
				Plot2_Up.Set(value2[0]);
				bTrigWasRising = true;
			}				
		}

		#region Properties		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot1_Up
		{
			get { return Values[0]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot1_Dn
		{
			get { return Values[1]; }
		}
		
        	/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot2_Up
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Plot2_Dn
		{
			get { return Values[3]; }
		}

		/// <summary>
		/// </summary>
		[Description("Period")]
		[Category("Parameters")]
		public int Period
		{
			get { return length; }
			set { length = Math.Max(1, value); }
		}
		/// <summary>
		/// </summary>
		[Description("Trigger Average")]
		[Category("Parameters")]
		public int TrigAvg
		{
			get { return trigAvg; }
			set { trigAvg = Math.Max(1, value); }
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
        private TriggerLines[] cacheTriggerLines = null;

        private static TriggerLines checkTriggerLines = new TriggerLines();

        /// <summary>
        /// Trigger Lines.
        /// </summary>
        /// <returns></returns>
        public TriggerLines TriggerLines(int period, int trigAvg)
        {
            return TriggerLines(Input, period, trigAvg);
        }

        /// <summary>
        /// Trigger Lines.
        /// </summary>
        /// <returns></returns>
        public TriggerLines TriggerLines(Data.IDataSeries input, int period, int trigAvg)
        {
            if (cacheTriggerLines != null)
                for (int idx = 0; idx < cacheTriggerLines.Length; idx++)
                    if (cacheTriggerLines[idx].Period == period && cacheTriggerLines[idx].TrigAvg == trigAvg && cacheTriggerLines[idx].EqualsInput(input))
                        return cacheTriggerLines[idx];

            lock (checkTriggerLines)
            {
                checkTriggerLines.Period = period;
                period = checkTriggerLines.Period;
                checkTriggerLines.TrigAvg = trigAvg;
                trigAvg = checkTriggerLines.TrigAvg;

                if (cacheTriggerLines != null)
                    for (int idx = 0; idx < cacheTriggerLines.Length; idx++)
                        if (cacheTriggerLines[idx].Period == period && cacheTriggerLines[idx].TrigAvg == trigAvg && cacheTriggerLines[idx].EqualsInput(input))
                            return cacheTriggerLines[idx];

                TriggerLines indicator = new TriggerLines();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.TrigAvg = trigAvg;
                Indicators.Add(indicator);
                indicator.SetUp();

                TriggerLines[] tmp = new TriggerLines[cacheTriggerLines == null ? 1 : cacheTriggerLines.Length + 1];
                if (cacheTriggerLines != null)
                    cacheTriggerLines.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTriggerLines = tmp;
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
        /// Trigger Lines.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TriggerLines TriggerLines(int period, int trigAvg)
        {
            return _indicator.TriggerLines(Input, period, trigAvg);
        }

        /// <summary>
        /// Trigger Lines.
        /// </summary>
        /// <returns></returns>
        public Indicator.TriggerLines TriggerLines(Data.IDataSeries input, int period, int trigAvg)
        {
            return _indicator.TriggerLines(input, period, trigAvg);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Trigger Lines.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TriggerLines TriggerLines(int period, int trigAvg)
        {
            return _indicator.TriggerLines(Input, period, trigAvg);
        }

        /// <summary>
        /// Trigger Lines.
        /// </summary>
        /// <returns></returns>
        public Indicator.TriggerLines TriggerLines(Data.IDataSeries input, int period, int trigAvg)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TriggerLines(input, period, trigAvg);
        }
    }
}
#endregion
