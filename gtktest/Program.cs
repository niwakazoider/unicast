using System;
using Gtk;
using Gdk;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace gtktest
{
	class MainClass : MainInterface, ShitarabaInterface
	{
		int wx=0, wy=0;
		MainWindow mw;
		ImageWindow iw;
		Shitaraba shitaraba;
		Thread queuethread;
		List<string> queue = new List<string>();
		Gdk.Color textcolor = new Gdk.Color(0,234, 255);
		int reloadtime = 30;
		WindowController controller = new WindowController();


		public static void Main() {
			new MainClass ();
		}

		public MainClass(){
			
			Application.Init();

			Util.ThreadInit ();

			mw = new MainWindow ();
			mw.DeleteEvent += OnClose;
			mw.AddEventListener(this);
			mw.Move (200, 200);
			mw.ShowAll ();
			mw.SetColor (textcolor);

			controller.AddWindow (mw);

			queuethread = new Thread (Run);
			queuethread.Start ();

			Application.Run();

		}

		void Run(){

			GLib.Idle.Add (new GLib.IdleHandler (delegate() {
				iw = controller.AddImageWindow (Util.CurrentDir()+"assets"+Util.DirSP()+"unicast.png");
				controller.AddWindow (iw);
				//controller.AddTextWindow ("test", mw.GetColor(), 100, 100);
				//controller.AddImageWindow ("../../image/test.png");
				return false;
			}));

			while (true) {
				Thread.Sleep (500);
				GLib.Idle.Add (new GLib.IdleHandler (
					process
				));
				controller.CloseWindow ();
			}

		}

		private bool process(){
			if(iw!=null){
				iw.GetPosition (out wx, out wy);
			}

			if (controller.Count () > 0) {
				return false;
			}

			string msg = ShiftRes();
			if (msg != null) {
				Sound sound = new Sound ();
				sound.Aplay (Util.GetRandomSoundSrc ());
				ShowText (msg);
			}

			return false;
		}

		private string ShiftRes(){
			string msg = null;
			lock (queue) {
				if(queue.Count>0){
					msg = queue [0];
					queue.RemoveAt (0);
				}
			}
			return msg;
		}

		private void ShowText (string msg){
				TextWindow tw = controller.AddTextWindow (msg, mw.GetColor(), wx, wy);
				tw.Fade ();
		}

		private void ShowColorSample(){
			GLib.Idle.Add(new GLib.IdleHandler(delegate() {
				ShowText ("text color");
				return false;
			}));
		}
		
		public void OnStart (string url)
		{
			if (shitaraba != null) {
				shitaraba.Stop ();
			}

			//"http://shitaraba.net/bbs/rawmode.cgi/game/32713/1406136363/"
			shitaraba = new Shitaraba (url);
			shitaraba.AddEventListener (this);
			shitaraba.SetReloadTime (mw.GetReloadTime());
			shitaraba.Start ();
		}

		public void OnStop(){
			if (shitaraba != null) {
				shitaraba.Stop ();
				shitaraba = null;
			}
		}

		public void OnReloadTimeChange(int time){
			reloadtime = time;
			if (shitaraba != null) {
				shitaraba.SetReloadTime (reloadtime);
			}
		}

		public void OnNewRes(string[] res){
			for (int i = 0; i < res.Length; i++) {
				string msg = res[i];
				queue.Add (msg);
			}
		}

		public void OnRes1000(){

		}

		public void OnServerError(){
			GLib.Idle.Add(new GLib.IdleHandler(delegate() {
				mw.OnError();
				return false;
			}));
		}

		public void OnColorChange(Gdk.Color color){
			textcolor = color;
			ShowColorSample();
		}

		void OnClose (object obj, DeleteEventArgs args)
		{
			controller.Fin ();

			if (queuethread!=null) {
				queuethread.Abort ();
			}
			if (shitaraba != null) {
				shitaraba.Stop ();
			}

			Application.Quit ();
		}

	}
}