using System;
using Gtk;
using Gdk;
using System.Threading;
using Cairo;

namespace gtktest
{
	public class TextWindow:Gtk.Window
	{
		bool ready = false;
		int wx = 100;
		int wy = 100;
		Pixbuf pixbuf;
		Gtk.Image image;
		Cairo.ImageSurface surface;
		Cairo.Context cr;
		Cairo.Context boxcr;
		EventBox box;
		Gdk.Color textcolor = new Gdk.Color();
		DateTime time = DateTime.Now;
		
		public TextWindow(string title) : base(title)
		{
			Init (title, textcolor, wx, wy);
		}

		public TextWindow(string title, Gdk.Color color) : base(title)
		{
			Init (title, color, wx, wy);
		}

		public TextWindow(string title, Gdk.Color color, int x, int y) : base(title)
		{
			Init (title, color, x, y);
		}

		private void Init(string title, Gdk.Color color, int x, int y){
			Title = title;
			//Opacity = 0;
			Decorated = false;
			KeepAbove = true;
			Resize (800, 600);
			Move (x, y);
			textcolor = color;

			pixbuf = PixbufText (Title, 30, true);
			//setWindowShapeFromPixbuf (pixbuf);
			image = new Gtk.Image ();
			image.Pixbuf = pixbuf;

			box = new EventBox ();
			box.ButtonPressEvent += new ButtonPressEventHandler (WindowController.OnButtonPress);
			box.ExposeEvent += HandleMyWinExposeEvent;
			box.Add (image);
			box.ScreenChanged += WindowController.HandleMyWinScreenChanged;
			Add (box);

			WindowController.HandleMyWinScreenChanged(this, null); 
			WindowController.SetWindowShapeFromPixbuf (this, pixbuf);

			/*
			DestroyEvent += new DestroyEventHandler (delegate(object o, DestroyEventArgs args) {
				System.Console.WriteLine ("OnDestroyEvent");
				if(cr!=null){
					cr.Dispose();
				}
				if(boxcr!=null){
					boxcr.Dispose();
				}
			});
			*/

			ShowAll();
		}

		void HandleMyWinExposeEvent (object o, ExposeEventArgs args)
		{
			GLib.Idle.Add (new GLib.IdleHandler (delegate() {
				ready = true;
				if (boxcr == null) {
					Gtk.Widget widget = ((Gtk.Widget)o);
					boxcr = Gdk.CairoHelper.Create (widget.GdkWindow);
				}
				WindowController.DrawPixbuf (boxcr, pixbuf);
				return false;
			}));
		}

		void OnDestroy (object o, EventArgs args)
		{
			System.Console.WriteLine ("OnDeleteEvent");
			if (surface != null) {
				surface.Dispose ();
			}
		}

		public void Fade(){
			//wait
			//while (!ready) {
			//	Thread.Sleep (250);
			//}

			//WindowController.Fade (this);
		}

		public void Close(){
			System.Console.WriteLine ("close");
			var w = this;
			GLib.Idle.Add (new GLib.IdleHandler (delegate() {
				w.HideAll();
				if (cr != null) {
					((IDisposable)cr).Dispose ();
				}
				if(boxcr!=null){
					((IDisposable)boxcr).Dispose();
				}
				if (pixbuf != null) {
					pixbuf.Dispose ();
				}
				if (surface != null) {
					surface.Dispose ();
				}
				w.Dispose ();
				return false;
			}));
		}

		public bool Timeout(){
			var diff = DateTime.Now - time;
			if ( diff.Seconds > 5) {
				//System.Console.WriteLine (DateTime.Now - time);
				return true;
			}
			return false;
		}

		private Pixbuf PixbufText(string text, int size, bool transparent){
			int x=0, y=36;

			surface = new Cairo.ImageSurface(Cairo.Format.Argb32,  800, 600);
			cr = new Cairo.Context(surface);

			if (!transparent) {
				cr.SetSourceRGBA (0.0, 0.0, 0.0, 1.0);
				cr.Rectangle (new Cairo.Rectangle (0, 0, 800, 600));
				cr.Fill ();
			}

			//cr.MoveTo(x, y);
			string font = "sans";
			if (Util.IsWindows ()) {
				font = "MS Gothic";
			}
			if (Util.IsLinux ()) {
				font = "TakaoGothic";
			}
			if (Util.IsMac ()) {
				font = "Hiragino Kaku Gothic ProN";
			}

			cr.SelectFontFace(font, Cairo.FontSlant.Normal, Cairo.FontWeight.Bold);
			cr.SetFontSize(size);

			Cairo.TextExtents extents = cr.TextExtents(text);
			string[] lines = text.Split ('\n');
			int min = size;
			for (int i = 0; i < lines.Length; i++) {
				string line = lines[i];
				for (int j = size; j > 8; j-=4) {
					extents = cr.TextExtents(line);
					if(extents.Width+extents.XBearing<800){
						if(j<min) min = j;
						break;
					}
					cr.SetFontSize(j);
				}
			}
			cr.SetFontSize(min);

			//string[] lines = text.Split ('\n');
			for (int i = 0; i < lines.Length; i++) {
				DrawText (cr, lines[i], x, y);
				y += (int)extents.Height;
				if (i > 5)
					break;
			}

			Pixbuf buf = new Gdk.Pixbuf (surface.Data, true, 8, surface.Width, surface.Height, surface.Stride);
			return buf;

			//buf.Save ("aaa.png", "png");
			//surface.WriteToPng (file);
		}

		private void DrawText(Context cr, string text, int x, int y){
			cr.SetSourceRGBA(0.0, 0.0, 0.0, 0.1);
			int dx = 3, dy = 4;
			cr.MoveTo(x-1+dx, y-1+dy);
			cr.ShowText(text);
			cr.MoveTo(x+1+dx, y+1+dy);
			cr.ShowText(text);
			cr.MoveTo(x-1+dx, y+1+dy);
			cr.ShowText(text);
			cr.MoveTo(x+1+dx, y-1+dy);
			cr.ShowText(text);

			Gdk.Color c = textcolor;
			double b = (double)c.Blue / ushort.MaxValue;
			double g = (double)c.Green / ushort.MaxValue;
			double r = (double)c.Red / ushort.MaxValue;
			if(b>1) b = 1;
			if(g>1) g = 1;
			if(r>1) r = 1;
			cr.SetSourceRGBA(b,g,r, 1);
			cr.MoveTo(x, y);
			cr.Antialias = Antialias.Default;
			cr.ShowText(text);
		}

	}
}

