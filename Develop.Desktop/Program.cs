#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;
#elif __IOS__ || __TVOS__
using Foundation;
using UIKit;
#endif
#endregion

namespace Develop
{
	static class Program
	{

		static void Main (string [] args)
		{
			using (var game = new Game1 ()) {
				game.Run ();
			}
		}
	}
}

