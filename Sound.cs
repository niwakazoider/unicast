using System;
using System.Media;

namespace gtktest
{
	public class Sound
	{
		SoundPlayer player;
		public Sound ()
		{
			player = new SoundPlayer();
			player.SoundLocation = "sound.wav";
			player.Load();
		}
		public void play(){
			if (player.IsLoadCompleted)
			{
				player.Play();
			}
		}
	}
}

