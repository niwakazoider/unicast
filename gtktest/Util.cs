using System;
using System.IO;
using System.Reflection;

namespace gtktest
{
	public class Util
	{
		public Util ()
		{
			/*
			Pango.Context context = w.CreatePangoContext();
			Pango.FontFamily[] fam = context.Families;
			foreach (Pango.FontFamily ff in fam) {
				//System.Console.WriteLine(ff.Name);
			}
			*/
		}
		public static char DirSP(){
			return System.IO.Path.DirectorySeparatorChar;
		}

		public static string CurrentDir(){
			Assembly myAssembly = Assembly.GetEntryAssembly();
			string path = myAssembly.Location;
			return System.IO.Path.GetDirectoryName (path) + System.IO.Path.DirectorySeparatorChar;
		}
		public static bool IsWindows(){
			System.OperatingSystem os = System.Environment.OSVersion;
			if (os.ToString ().ToLower ().IndexOf ("windows") >= 0) {
				return true;
			}
			return false;
		}
		public static bool IsLinux(){
			System.OperatingSystem os = System.Environment.OSVersion;
			if (os.ToString ().ToLower ().IndexOf ("unix") >= 0) {
				return true;
			}
			return false;
		}
		public static bool IsMac(){
			System.OperatingSystem os = System.Environment.OSVersion;
			if (os.ToString ().ToLower ().IndexOf ("mac") >= 0) {
				return true;
			}
			return false;
		}
		public static string GetSoundSrc(){
			return "assets"+Util.DirSP()+"sound.wav";
		}
		public static string GetRandomSoundSrc(){
			string dir = "assets"+Util.DirSP()+"sound"+Util.DirSP();
			string[] files = Directory.GetFiles(dir);
			Random rnd = new Random();
			int i = rnd.Next (0, files.Length);
			string src = files [i];
			return src;
		}
		public static void ThreadEnter(){
			if (!IsWindows ()) {
				//Gdk.Threads.Enter ();
			}
		}
		public static void ThreadLeave(){
			if (!IsWindows ()) {
				//Gdk.Threads.Leave ();
			}
		}
		public static void ThreadInit(){
			if (!IsWindows ()) {
				if (!GLib.Thread.Supported) {
					GLib.Thread.Init ();
				}
				Gdk.Threads.Init ();
			}
		}
	}
}

