using System;

namespace gtktest
{
	public interface MainInterface
	{
		void OnStart(string url);
		void OnStop();
		void OnReloadTimeChange(int time);
		void OnColorChange(Gdk.Color color);
	}
}

