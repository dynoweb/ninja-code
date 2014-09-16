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
    [Description("Enter the description of your strategy here")]
    public class TinyGP : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (Position.MarketPosition == MarketPosition.Long)
				ExitLong();
			
			double X1 = Open[0];
			double X2 = Close[1];
			double X3 = Open[1];
			double X4 = Close[2];
			double X5 = Open[2];
			double X6 = Close[3];
			double X7 = Open[3];
			double X8 = Close[4];
			double X9 = Open[4];
			double X10 = Volume[0];
			
			// Fitness of 29.18 after 150 iterations, target was gain on day[0] - not even close
			double answer = ( X2  *  (  ( X7  + -0.050149989689918684 )  +  (  (  (  (  ( X4  * X8  )  * X2  )  +  (  (  (  (  ( X4  * X8  )  * X4  )  /  (  ( -0.35281377613985465 + X9  )  *  ( X7  + X8  )  )  )  -  ( X8  - X6  )  )  +  (  (  ( 0.10566584116448063 + -0.050149989689918684 )  *  (  ( X5  *  (  ( X1  - -0.050149989689918684 )  *  (  (  ( X3  +  (  (  (  ( X4  + X9  )  + X6  )  / X2  )  +  (  (  (  ( X5  *  (  ( X6  + -0.050149989689918684 )  +  (  (  (  (  (  ( 0.10566584116448063 + X4  )  *  ( -0.050149989689918684 + X7  )  )  + X3  )  +  (  (  (  ( X4  * X8  )  * X3  )  /  (  ( -0.35281377613985465 + X9  )  *  ( X7  + X8  )  )  )  +  (  (  (  ( X4  * X6  )  - X5  )  /  (  ( X5  + X10  )  *  ( X7  - X8  )  )  )  +  ( X3  * X2  )  )  )  )  -  ( X4  + X8  )  )  + X6  )  )  )  * X7  )  *  (  ( X4  * X9  )  +  (  (  ( X6  +  (  (  (  ( X4  - X8  )  * X4  )  /  (  ( -0.35281377613985465 + X9  )  *  ( X7  + X8  )  )  )  +  (  ( X5  * X5  )  * X4  )  )  )  -  ( X4  + X8  )  )  + X6  )  )  )  *  ( X4  / X9  )  )  )  )  -  ( X1  / X7  )  )  + X6  )  )  )  + X5  )  )  / X2  )  )  )  -  ( X4  + X8  )  )  + X6  )  )  ) ;
			// Fitness of 1.95 after 100 iterations and close[0] in dataset
			//double answer = ( X2  *  (  ( X2  *  (  ( X2  *  (  ( -0.09205591398651224 - X2  )  *  ( X2  /  ( X2  *  (  ( 0.006931050160770447 - X2  )  *  ( -0.09205591398651224 + X2  )  )  )  )  )  )  +  ( X2  - 0.09006299268837101 )  )  )  /  ( 0.09006299268837101 -  (  ( 0.09006299268837101 - X2  )  *  (  ( X2  - 0.09006299268837101 )  +  (  ( X2  *  (  ( -0.09205591398651224 - X2  )  *  ( X2  *  ( X2  +  (  ( 0.006931050160770447 - X2  )  /  ( -0.09205591398651224 + X2  )  )  )  )  )  )  +  (  ( X1  + -0.05714003126085816 )  + X2  )  )  )  )  )  )  ) ;
			// Fitness of 27 after 50 iterations
			//double answer = (  ( X8  / X8  )  /  (  (  (  ( X4  + X9  )  - X3  )  *  (  (  (  ( X10  / X9  )  *  ( X8  +  (  ( X5  + X5  )  - X7  )  )  )  / X2  )  *  ( X9  +  (  ( X9  + X5  )  - X7  )  )  )  )  *  (  ( X10  *  (  ( X5  /  ( X4  / X3  )  )  / X7  )  )  -  ( X10  +  ( X9  * X1  )  )  )  )  ) ;
//			double x = (((
//					(0.12395519105633301 + ((X9  * ((-0.06785158441447159 > X6) > ((-0.06785158441447159 * (X7  < (X5  + X1))) > X2))) * (X6  / X2)))
//				* ((-0.06785158441447159 / X7) + (X3  < X2))) + (X5  < (X7  < X5))) < ((X5  * (((X6  / X3) > X6) < (-0.06785158441447159 - X9))) - (X3  + X9)));
//			double answer = 	(
//							(
//								(
//									(0.12395519105633301 + 
//										(
//											(X9 * 
//												(
//													((-0.06785158441447159 > X6) ? 0 : 1) 
//													> 
//													(
//														(-0.06785158441447159 * 
//															(
//																X7 < (X5  + X1) ? 0 : 1
//															)
//														) > X2 ? 0 : 1
//													) ? 0 : 1
//												)
//											) * (X6  / X2)
//										)
//									)
//									* 
//									(
//										(-0.06785158441447159 / X7) 
//										+ 
//										((X3 < X2) ? 0 : 1)
//									)
//								)
//								+ 
//								(((
//									X5  
//									< 
//									((X7 < X5) ? 0 : 1)
//								) ? 0 : 1)
//								)
//							)
//							< 
//							(
//								(
//									X5  * ((
//										(((X6  / X3) > X6) ? 1 : 0)
//										< 
//										(-0.06785158441447159 - X9)) ? 1 : 0)
//								) 
//								- (X3  + X9)
//							) ? 1 : 0
//						);
			Print(Time + " " + answer);
			if (answer > 0.5)
				this.EnterLong("EnterLong" + answer);
			if (answer < -0.5)
				this.EnterShort("EnterShort" + answer);
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MyInput0
        {
            get { return myInput0; }
            set { myInput0 = Math.Max(1, value); }
        }
        #endregion
    }
}
