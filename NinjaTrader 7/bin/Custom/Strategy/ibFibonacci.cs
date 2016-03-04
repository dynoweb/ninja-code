#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
using System.Drawing.Drawing2D;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    partial class Strategy
    {
		public enum FibLevels {_0, _236, _270, _382, _400, _500, _618, _700, _764, _880, _1000, _1270, _1380, _1500, _1618};  
		
		#region variables 
		
//		private FibLevels fibLevels_Entry       	= FibLevels._500;
//		private FibLevels fibLevels_TakeProfit		= FibLevels._382;
//		private FibLevels fibLevels_StopLoss		= FibLevels._618; 
		
		#endregion //closes variables 
		
		#region methods
		
		public double _getFibLevelFromEnum(FibLevels fLevel)
		{
			switch (fLevel)
			{
				case FibLevels._0:	
					return(0.0d); 
				case FibLevels._236:	
					return(0.236d); 
				case FibLevels._270:	
					return(0.27d); 
				case FibLevels._382:	
					return(0.382d); 		
				case FibLevels._400:	
					return(0.40d); 
				case FibLevels._500:	
					return(0.50d); 
				case FibLevels._618:	
					return(0.618d); 
				case FibLevels._700:	
					return(0.70d); 
				case FibLevels._764:	
					return(0.764d); 
				case FibLevels._880:	
					return(0.88d);
				case FibLevels._1000:	
					return(1.0d); 
				case FibLevels._1270:	
					return(1.27d); 
				case FibLevels._1380:	
					return(1.38d); 
				case FibLevels._1500:	
					return(1.5d); 
				case FibLevels._1618:	
					return(1.618d);
				default:
					return(0.50d); 
			}
		}
		
		#endregion //closes methods		
    }
}
