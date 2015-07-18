using System;
using System.Threading;
using Gtk;
using Gdk;
using System.Collections;
using System.Collections.Generic;

namespace gtktest
{
	public class WindowController
	{
		private List<Gtk.Window> windows = new List<Gtk.Window> ();
		public WindowController ()
		{
		}
		public ImageWindow AddImageWindow(string path){
			//Util.ThreadEnter ();
			ImageWindow iw = new ImageWindow (path);
			//Util.ThreadLeave ();
			AddWindow (iw);
			return iw;
		}
		public TextWindow AddTextWindow(string text, Gdk.Color color){
			TextWindow tw = new TextWindow (text, color);
			AddWindow (tw);
			return tw;
		}
		public TextWindow AddTextWindow(string text, Gdk.Color color, int x, int y){
			TextWindow tw = new TextWindow (text, color, x, y);
			AddWindow (tw);
			return tw;
		}
		public void AddWindow(Gtk.Window w){
			windows.Add(w);
		}
		public void Fin(){
			foreach (var item in windows) {
				Gtk.Window w = (Gtk.Window)item;
				if (w != null) {
					w.HideAll ();
					w.Destroy ();
				}
			}
		}
		public int Count(){
			int i = 0;
			foreach (var item in windows) {
				if (item!= null) {
					TextWindow tw = item as TextWindow;
					if (tw!= null) {
						if (!tw.Timeout ()) {
							i++;
						}
					}
				}
			}
			return i;
		}
		public void CloseWindow(){
			List<Gtk.Window> d = new List<Gtk.Window>();
			foreach (var item in windows) {
				if (item!= null) {
					TextWindow tw = item as TextWindow;
					if (tw!= null) {
						if (tw.Timeout ()) {
							tw.Close ();
							d.Add (tw);
						}
					}
				}
			}
			windows.RemoveAll (delegate(Gtk.Window w) {
				TextWindow tw = w as TextWindow;
				if(tw!=null && tw.Timeout()){
					return true;
				}else{
					return false;
				}
			});
		}

		public static void OnButtonDragPress(object o, ButtonPressEventArgs args){
			Gdk.Window w = ((Gtk.Widget)o).GdkWindow.Toplevel;
			if (((Gdk.EventButton)args.Event).Type == Gdk.EventType.TwoButtonPress) {
			} else {
				int x, y;
				Gdk.Display.Default.GetPointer (out x, out y);
				w.BeginMoveDrag (1, x, y, 0);
			}
		}

		public static void OnButtonPress(object o, ButtonPressEventArgs args){
			Gdk.Window w = ((Gtk.Widget)o).GdkWindow.Toplevel;
			if (((Gdk.EventButton)args.Event).Type == Gdk.EventType.TwoButtonPress) {
				w.Destroy ();
			} else {
				int x, y;
				Gdk.Display.Default.GetPointer (out x, out y);
				w.BeginMoveDrag (1, x, y, 0);
			}
		}

		public static void HandleMyWinScreenChanged (object o, ScreenChangedArgs args)
		{
			Gtk.Widget widget = (Gtk.Widget)o;
			Gdk.Colormap colormap = widget.Screen.RgbaColormap;

			if(colormap == null) {
				colormap = widget.Screen.RgbColormap;
			}

			widget.Colormap = colormap; 
		}

		public static void Fade(Gtk.Window w){
			//Fadein (w);
			//Thread.Sleep (5000);
			//Fadeout (w);
			//w.HideAll ();
			//w.Destroy ();
			Thread th = new Thread (delegate() {
				Thread.Sleep(5000);
				w.HideAll ();
				w.Destroy ();
			});
			th.Start ();
		}

		private static void Fadeout(Gtk.Window w){
			for (double i = 1.0; i >= 0.2; i-=0.4) {
				w.Opacity = i;
				Thread.Sleep (100);
			}
		}

		public static void Fadein(Gtk.Window w){
			for (double i = 0.2; i <= 1; i+=0.4) {
				w.Opacity = i;
				Thread.Sleep (100);
			}
		}

		public static void SetWindowShape(Gtk.Window w, Gtk.Image image){
			Pixmap pixmap, pixmask;
			image.Pixbuf.RenderPixmapAndMask (out pixmap, out pixmask, 1);
			w.ShapeCombineMask (pixmask, 0, 0);
		}

		public static void SetWindowShapeFromPixbuf(Gtk.Window w, Gdk.Pixbuf pixbuf){
			Pixmap pixmap, pixmask;
			pixbuf.RenderPixmapAndMask (out pixmap, out pixmask, 1);
			w.ShapeCombineMask (pixmask, 0, 0);
		}

		public static void DrawPixbuf(Cairo.Context cr, Pixbuf pixbuf){
			cr.SetSourceRGBA(0.0, 0.0, 0.0, 1.0);
			cr.Operator = Cairo.Operator.DestIn;
			Gdk.CairoHelper.SetSourcePixbuf (cr, pixbuf, 0, 0);
			cr.Paint ();
			cr.Operator = Cairo.Operator.Over;
			cr.Paint ();
		}

	}
}

