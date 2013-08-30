//###
//### Calculate Risk Reward Ratio from use Drawn price levels
//### Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target' respectively
//### May have multipl targets named 'Target<x>' where <x> can be any text that makes all targets unique
//### Risk/Reward will be calculated based on the price location of these lines
//###
//### User		Date 		Description
//### ------	-------- 	-------------
//### Gaston	Feb 2011	Created
//###
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

namespace NinjaTrader.Indicator
{
    [Description("Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique")]
    public class RiskReward : Indicator
    {
        #region Variables
		double entryPrice  = 0;
		double stopPrice   = 0;
		double targetPrice = 0;
		double risk        = 0;
		double reward      = 0;
		double riskReward  = 0;
		
        IHorizontalLine entry;
        IHorizontalLine stop;
        IHorizontalLine target;
		
		System.Drawing.Font boldFont 	= new Font("Courier", 9,System.Drawing.FontStyle.Bold);
		
        #endregion

        protected override void Initialize() {
            Overlay				= true;
        }

        protected override void OnBarUpdate() {
		}
		
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max) {
			base.Plot(graphics, bounds, min, max);
			int y=0;
			StringFormat format	= new StringFormat();
			
			format.Alignment = StringAlignment.Far;
			
			foreach (IDrawObject draw in DrawObjects) {
    			if (draw.UserDrawn && draw.DrawType == DrawType.HorizontalLine) {
        
					if ( draw.Tag.ToUpper().Contains("TARGET") ) {
			        	target = (IHorizontalLine) draw;
						targetPrice = Instrument.MasterInstrument.Round2TickSize(target.Y);
						y = ChartControl.GetYByValue(Bars, targetPrice);
						risk       = Math.Abs(entryPrice-stopPrice);
						reward     = Math.Abs(targetPrice-entryPrice);
						riskReward = ( risk > 0 ) ? reward/risk : 0;
						graphics.DrawString("T: " +targetPrice.ToString("0.0000") +"\nR/R  " +riskReward.ToString("0.##") +":1", boldFont, target.Pen.Brush, bounds.Width, y, format);
					}
					else 
					if ( draw.Tag.ToUpper().CompareTo("ENTRY") == 0 ) {
			        	entry = (IHorizontalLine) draw;
						entryPrice = Instrument.MasterInstrument.Round2TickSize(entry.Y);
						y = ChartControl.GetYByValue(Bars, entryPrice);
						graphics.DrawString("E: " +entryPrice.ToString("0.0000"), boldFont, entry.Pen.Brush, bounds.Width, y, format);
					}
					else 
					if ( draw.Tag.ToUpper().CompareTo("STOP") == 0 ) {
			        	stop = (IHorizontalLine) draw;
						stopPrice = Instrument.MasterInstrument.Round2TickSize(stop.Y);
						y = ChartControl.GetYByValue(Bars, stopPrice);
						graphics.DrawString("S: " +stopPrice.ToString("0.0000"), boldFont, stop.Pen.Brush, bounds.Width, y, format);
					}
			    }
			}
		}

        #region Properties
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private RiskReward[] cacheRiskReward = null;

        private static RiskReward checkRiskReward = new RiskReward();

        /// <summary>
        /// Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique
        /// </summary>
        /// <returns></returns>
        public RiskReward RiskReward()
        {
            return RiskReward(Input);
        }

        /// <summary>
        /// Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique
        /// </summary>
        /// <returns></returns>
        public RiskReward RiskReward(Data.IDataSeries input)
        {
            if (cacheRiskReward != null)
                for (int idx = 0; idx < cacheRiskReward.Length; idx++)
                    if (cacheRiskReward[idx].EqualsInput(input))
                        return cacheRiskReward[idx];

            lock (checkRiskReward)
            {
                if (cacheRiskReward != null)
                    for (int idx = 0; idx < cacheRiskReward.Length; idx++)
                        if (cacheRiskReward[idx].EqualsInput(input))
                            return cacheRiskReward[idx];

                RiskReward indicator = new RiskReward();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                RiskReward[] tmp = new RiskReward[cacheRiskReward == null ? 1 : cacheRiskReward.Length + 1];
                if (cacheRiskReward != null)
                    cacheRiskReward.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheRiskReward = tmp;
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
        /// Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RiskReward RiskReward()
        {
            return _indicator.RiskReward(Input);
        }

        /// <summary>
        /// Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique
        /// </summary>
        /// <returns></returns>
        public Indicator.RiskReward RiskReward(Data.IDataSeries input)
        {
            return _indicator.RiskReward(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.RiskReward RiskReward()
        {
            return _indicator.RiskReward(Input);
        }

        /// <summary>
        /// Calculate Risk/Reward Ratio. Draw 3 Horizontal lines, edit each and change the Tag Names to 'Entry', 'Stop', 'Target', 'Target2',... 'Target<x>' respectively, where <x> can be any text that makes all targets unique
        /// </summary>
        /// <returns></returns>
        public Indicator.RiskReward RiskReward(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.RiskReward(input);
        }
    }
}
#endregion
