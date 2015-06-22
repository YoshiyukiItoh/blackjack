using System;

namespace blackjack
{
	public interface OutputInterface
	{
		void Write(string msg,Object[] args = null);
		void WriteLine(string msg,Object[] args = null);
	}
}

