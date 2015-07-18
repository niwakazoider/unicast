using System;
using Gtk;
using System.Collections;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	private List<gtktest.MainInterface> listeners = new List<gtktest.MainInterface> ();
	public void AddEventListener(gtktest.MainInterface listener){
		listeners.Add(listener);
	}
	public void RemoveEventListener(gtktest.MainInterface listener){
		listeners.Remove(listener);
	}

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	public void OnError(){
		togglebutton1.Active = false;
		togglebutton1.Label = "Start";
	}

	public Gdk.Color GetColor(){
		return colorbutton1.Color;
	}

	public void SetColor(Gdk.Color color){
		colorbutton1.Color = color;
	}

	public string GetUrl (){
		return entry4.Text.ToString ();
	}

	public int GetReloadTime(){
		return int.Parse (combobox1.ActiveText);
	}

	protected void OnToggleClicked (object sender, EventArgs e)
	{
		ToggleButton button = (ToggleButton)sender;
		foreach (var listener in listeners) {
			if (button.Active) {
				button.Label = "Stop";
				listener.OnStart (GetUrl());
			} else {
				button.Label = "Start";
				listener.OnStop ();
			}
		}
	}

	protected void OnReloadTimeChanged (object sender, EventArgs e)
	{
		//ComboBox combobox = (ComboBox)sender;
		int time = GetReloadTime ();
		foreach (var listener in listeners) {
			listener.OnReloadTimeChange (time);
		}
	}

	protected void OnColorChange (object sender, EventArgs e)
	{
		//ColorButton combobox = (ColorButton)sender;
		Gdk.Color c = GetColor();
		foreach (var listener in listeners) {
			listener.OnColorChange (c);
		}
	}
}
