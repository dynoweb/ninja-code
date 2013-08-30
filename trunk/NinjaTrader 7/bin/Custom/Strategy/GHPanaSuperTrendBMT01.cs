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
    /// Strategy developed using the anaSuperTrend Indicator Rev 1
    /// </summary>
    [Description("Strategy developed using the anaSuperTrend Indicator Rev 1")]
    public class GHPanaSuperTrendBMT01 : Strategy
    {
        //#region Variables
		private double	m_Mult			= 1.0;
		private int		m_ATRPeriod		= 5;
		private int		m_Median		= 5;
		//#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            
            CalculateOnBarClose = true;
			EntryHandling		= EntryHandling.UniqueEntries;
	
			Add(anaSuperTrend(m_Mult,m_ATRPeriod,m_Median));		
        }
		
		private void GoLong()
		{
			EnterLong("target1");	
		}
		
		private void GoShort()
		{
			EnterShort("target1");	
		}
		
		private void ManageOrders()
		{
			//Nothing to do here yet, but kept for structure
		
		}

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			bool	m_EnterLong=false;
			bool	m_EnterShort=false;
			
            EntryHandling		= EntryHandling.UniqueEntries;
			
			if ((anaSuperTrend(m_Mult,m_ATRPeriod,m_Median).UpTrend[0]) && (!anaSuperTrend(m_Mult,m_ATRPeriod,m_Median).UpTrend[1]))
				m_EnterLong=true;  // Uptrend Arrow would be printed in Indicator, enter Long
			
			if ((!anaSuperTrend(m_Mult,m_ATRPeriod,m_Median).UpTrend[0]) && (anaSuperTrend(m_Mult,m_ATRPeriod,m_Median).UpTrend[1]))
				m_EnterShort=true; // Downtrend Arrow would be printed in Indicator, enter Short
			
			

			ManageOrders();
			
			if (m_EnterLong && (Position.MarketPosition != MarketPosition.Long))
				GoLong();
			else
			if (m_EnterShort && (Position.MarketPosition != MarketPosition.Short))
				GoShort();
			
        }


        #region Properties
        [Description("anaSuperTrend Median Value")]
        [GridCategory("Parameters")]
        public int PeriodMedian
        {
            get { return m_Median; }
            set { m_Median = Math.Max(1, value); }
        }
		
		[Description("anaSuperTrend ATR Period Value")]
        [GridCategory("Parameters")]
        public int ATRPeriod
        {
            get { return m_ATRPeriod; }
            set { m_ATRPeriod = Math.Max(1, value); }
        }
		
		[Description("anaSuperTrend Multiplier (double) Value")]
        [GridCategory("Parameters")]
        public double Multiplier
        {
            get { return m_Mult; }
            set { m_Mult = value; }
        }
        #endregion
    }
}
