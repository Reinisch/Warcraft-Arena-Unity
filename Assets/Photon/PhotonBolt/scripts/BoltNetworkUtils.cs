using System;

public static class BoltNetworkUtils
{
	public static Action Combine(this Action self, Action action)
	{
		return (Action)Delegate.Combine(self, action);
	}
}
