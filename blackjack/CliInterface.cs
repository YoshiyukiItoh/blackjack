using System;

namespace blackjack
{
	public class CliInterface:OutputInterface
	{
		public void Write(string msg, Object[] args = null)
		{
			if (args == null) 
			{
				Console.Write (msg);
			} 
			else
			{
				Console.Write (msg, args);
			}
		}
		public void WriteLine(string msg, Object[] args = null)
		{
			if (args == null) 
			{
				Console.Write (msg);
			} 
			else
			{
				Console.Write (msg, args);
			}
		}
	}
}

