using System;
using System.Media;
using System.Threading;
using System.IO;

namespace gtktest
{
	public class Sound
	{

		public void Play(string src){
			using(SoundPlayer sp = new SoundPlayer(src)) {
				sp.PlaySync ();
				sp.Dispose ();
			}
		}

		public void Aplay(string src){
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = true; 
			proc.StartInfo.FileName = "aplay";
			proc.StartInfo.Arguments = src;
			proc.Start();
			proc.WaitForExit ();
			proc.Close ();
		}

	}
}

