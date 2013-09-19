#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("This strategy was created by a Cartesian Genetic Programming engine discuss here: http://www.bigmiketrading.com/elite-trading-journals/24691-my-hunt-automated-holy-grail-11.html#post329069")]
    public class CGPOscillator : Strategy
    {
        private int varlength1 		= 14;
		private int varlength2		= 30;
		private int	varlength3		= 3;
		private int	varlength4		= 7;
		
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
			IncludeCommission	= true;
	//		Slippage			= 2;   Use BOTB defaults
			Enabled				= true;
        }

        protected override void OnBarUpdate()
        {
			if (CurrentBar < Math.Max(VarLength3, VarLength4)) return;
			
			Double Output=(EMA(VarLength1)[VarLength3]-EMA(VarLength1)[0]) - (EMA(VarLength2)[VarLength4]-EMA(VarLength2)[0]);
			
			if ((Output > 0.05) && (Position.MarketPosition!=MarketPosition.Long))
				EnterLong();
			if ((Output <= 0.01) && (Position.MarketPosition==MarketPosition.Long))
				ExitLong();

			if ((Output < -0.05) && (Position.MarketPosition!=MarketPosition.Short))
				EnterShort();
			if ((Output >= -0.01) && (Position.MarketPosition==MarketPosition.Short))
				ExitShort();
			
			
        }

        #region Properties
        [Description("01. Var Length 1")]
        [GridCategory("Parameters")]
        public int VarLength1
        {
            get { return varlength1; }
            set { varlength1 = Math.Max(1, value); }
        }
		[Description("02. Var Length 2")]
        [GridCategory("Parameters")]
        public int VarLength2
        {
            get { return varlength2; }
            set { varlength2 = Math.Max(1, value); }
        }
		[Description("03. Var Length 3")]
        [GridCategory("Parameters")]
        public int VarLength3
        {
            get { return varlength3; }
            set { varlength3 = Math.Max(1, value); }
        }
		[Description("04. Var Length 4")]
        [GridCategory("Parameters")]
        public int VarLength4
        {
            get { return varlength4; }
            set { varlength4 = Math.Max(1, value); }
        }
        #endregion
    }
}
