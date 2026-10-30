using System;
using TwitchToolkit.Utilities;
using Verse;

namespace TwitchToolkit.Votes;

public static class VoteHelper
{
	private static DateTime GetLastEvenTime()
	{
		Ticker ticker = Current.Game.GetComponent<Ticker>();
        return ticker.lastEvent;
    }

	public static bool TimeForEventVote()
	{
		return ToolkitSettings.TimedStorytelling && TimeHelper.MinutesElapsed(GetLastEvenTime()) >= ToolkitSettings.TimeBetweenStorytellingEvents;
	}
}
