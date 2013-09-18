Development work being managed on google code, but you won't need this right away.
SVN Repository - TortoiseSVN 1.8.2
https://code.google.com/p/ninja-code/

When you get ready to checkout the code ninja uses

c:\users\username\documents\Ninja Trader 7

Ninja Trader 7 is the root of the checked in code.  You should be able to do a check-out right on top of your existing files.


Trading Platform
http://www.ninjatrader.com/

Getting started on Ninja Notes
You will need to get a free license code to register Ninja to get it to work.  There's no catch getting that except they get your email address.  You opt out of any email subscriptions if you need to.  I haven't yet and I've been using it for 2 years.  I've purchased a lifetime single broker license which is required to trade live. With the free license you'll only be able to end of day stock and futures prices. You can code against end of day prices to start off while you're learning.  Once you're ready, I've captured 5 years of tick data for a couple of indexes.

Once you enter your license and restart Ninja, it's ready to use.  

Some initial navigation tips.  

You'll want to connect to "Kinetick - End Of Day (Free)" by selecting File/Connect and picking Kinetick, once connected you should see a green status indicator on the bottom left of the Control Center, the mail Ninja window, which should say Connected - Kinetick.  Once connected, you can open a chart and the price data will download and be stored on your local database. It only downloads the data it needs, so if you're looking at AAPL with end of day prices it will download daily data for one year.  

To see a chart select File/New/Chart.  When a new chart opens, it defaults to 1 min, since you don't have minute data, it will be blank.  You will need to change the period to daily.  The first drop down allows you to change the instrument i.e. AAPL and the second the time frame.  Once daily is selected you should see the chart plotted. 

The icons on the top of the chart control most of the chart.  From left to right, Instrument, Period, Bar Type, Drawing Tools, Zoom selection, un-zoom, pointer type, data box, chart trader, data series, indicators, strategies and properties.

The two you should get to know in detail are
Data Series - you can control the end date, time period
Indicators - you can add indicators to your chart to show things like moving average, volume traded etc.

Tools/Instrument Manager allows you to add instruments.  Since you'll be using Kinetick at first, lets just deal with equities (stocks).  If you want to have a stock in your default list so you can just change it by using the drop down, you do that by first searching for it in the Instrument Manager, then clicking on the left diamond on the bottom to add it to your list on the left. Once it's listed under default, then you can select that instrument from the first icon on the chart window.

There are two areas for programmers to develop applications, Strategies and Indicators.  Strategies are used to auto trade.  In the Ninja Trader 7 folder referenced above, you can navigate to the supplied samples.  It's under Ninja Trader 7/bin/Custom/Indicator | Strategy.  They are in C# source code and named with an @ for the first character.  That is what is used to distinguished the supplied samples and our own created code (no @).  

There are 4 sample strategies with very little code in them so they are easy to look at what they are doing.  Open them with the Ninja code editor by selecting Tools/Edit Ninja Script/Strategy | Indicator.  You may notice two methods that are common, Initialize and OnBarUpdate.  It's pretty obvious what those are there for.  Use the on-line help by pressing F1 and go to the bottom of the contents and expand the NinjaScript section.  There are a couple quick tutorials to get you started, plus all of the Ninja methods are documented. 

Hopefully you can come up to speed with the tool pretty quick so we can start working on developing a money making retirement machine.
