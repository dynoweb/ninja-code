#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This file holds all user defined strategy methods.
    /// </summary>
    partial class Strategy
    {
		/*
		public enum OrderPurpose {Flatten, HandleRejection, NewEntry, StopExit, None}; 
		
		public int _adjustOpenContracts(int alreadyOpenContracts, int qtyFilled, OrderAction oa)
		{
			int factor = 1;
			if (oa == OrderAction.Sell) factor = -1;
			int adjustment = factor * qtyFilled; 
			
			return(alreadyOpenContracts + adjustment);
		}
		
		public void _submitNewEntryOrder(ref IOrder newOrder, OrderAction orderAction, double orderPrice)
		{
			this._CanSendNewEntry   = false;
			string msg 		= _createOrderString(this._StratName, OrderPurpose.NewEntry, orderAction, this._TradeNumber); 
			
			switch (this._EntryOrderType)
			{
				case(OrderType.Limit):	
					newOrder = SubmitOrder(0, orderAction, OrderType.Limit, this._NumContracts, orderPrice, 0, String.Empty, msg);
					break;
				case(OrderType.Stop):	
					newOrder = SubmitOrder(0, orderAction, OrderType.Stop, this._NumContracts, 0, orderPrice, String.Empty, msg);
					break;
				case(OrderType.StopLimit):	
					newOrder = SubmitOrder(0, orderAction, OrderType.Stop, this._NumContracts, 0, orderPrice, String.Empty, msg);
					break;
				default:	
					newOrder = SubmitOrder(0, orderAction, OrderType.Market, this._NumContracts, 0, 0, String.Empty, msg);
					break;
			}
		}
		
		public void _processCanceledOrder(ref IOrder cxlOrder, CancelReason clxRsn)
		{
			double orderPx = cxlOrder.LimitPrice;
			if ((cxlOrder.OrderType == OrderType.Stop) || (cxlOrder.OrderType == OrderType.StopLimit)) orderPx = cxlOrder.StopPrice; 
			
			Print(String.Format("{0} Order to {1} at {2} (submitted at: {3:T}) canceled at: {4:T}. Reason: {5}", 
				this._StratName, cxlOrder.OrderAction, orderPx, this._OrderSubmissionTime, this._EventTime, clxRsn));
			
			this._CanSendNewEntry = true; 
			
			CancelOrder(cxlOrder);
			cxlOrder  = null;
		}
		
		public void _flattenPositions(string reason)
		{
			if (Position.MarketPosition != MarketPosition.Flat)
			{
				OrderAction oa = OrderAction.Buy; 
				if (Position.MarketPosition == MarketPosition.Long) oa = OrderAction.Sell; 
				SubmitOrder(0, oa, OrderType.Market, Position.Quantity, 0, 0, String.Empty, reason);	
			}	
		}
		
		public OrderAction _reversePosition(Position p)
        {
            OrderAction oa = OrderAction.Buy; 
            if (p.MarketPosition == MarketPosition.Long) oa = OrderAction.Sell;

            return (oa);
		}
		
		public OrderAction _reversePosition(IOrder order)
        {
            OrderAction oa = OrderAction.Buy; 
            if (order.OrderAction == OrderAction.Buy) oa = OrderAction.Sell;

            return (oa);
		}
*/		
    }
}
