using System.Collections.Generic;

namespace Mono.Terminal
{
	public class HBox : Box
	{
		public HBox()
			: base()
		{
		}

		public HBox(int w, int h)
			: base(w, h)
		{
		}

		public HBox(int x, int y, int w, int h)
			: base(x, y, w, h)
		{
		}

		public override void Redraw()
		{
			int totalwidth = 0;
			int fillcount = 0;

			foreach (var element in widgets) {
				var widget = element.Widget;
				if (element.Setting == Setting.Size) {
					totalwidth += widget.Width;
				} else {
					fillcount++;
				}
			}

			int x = 0;
			int fillsize = (Width - totalwidth) / fillcount;
			foreach (var element in widgets) {
				var widget = element.Widget;

				widget.Y = Y;
				widget.Height = Height;

				widget.X = x;

				if (element.Setting == Setting.Fill) {
					widget.Width = fillsize;
				}

				x += widget.Width;

				widget.Redraw();
			}
		}
	}

	public class VBox : Box
	{
		public VBox()
			: base()
		{
		}

		public VBox(int w, int h)
			: base(w, h)
		{
		}

		public VBox(int x, int y, int w, int h)
			: base(x, y, w, h)
		{
		}

		public override void Redraw()
		{
			int totalheight = 0;
			int fillcount = 0;

			foreach (var element in widgets) {
				var widget = element.Widget;
				if (element.Setting == Setting.Size) {
					totalheight += widget.Height;
				} else {
					fillcount++;
				}
			}

			int y = 0;
			int fillsize = (Height - totalheight) / fillcount;
			foreach (var element in widgets) {
				var widget = element.Widget;

				widget.X = X;
				widget.Width = Width;

				widget.Y = y;

				if (element.Setting == Setting.Fill) {
					widget.Height = fillsize;
				}

				y += widget.Height;

				widget.Redraw();
			}
		}
	}

	public abstract class Box : Container, IEnumerable<Widget>
	{
		public enum Setting
		{
			Fill,
			Size
		}

		protected class Element
		{
			public Widget Widget { get; set; }
			public Setting Setting { get; set; }
		}

		protected List<Element> widgets = new List<Element>();

		public Box()
			: base()
		{
		}

		public Box(int w, int h)
			: base(w, h)
		{
		}

		public Box(int x, int y, int w, int h)
			: base(x, y, w, h)
		{
		}

		public override bool ProcessKey(int key)
		{
			if (CurrentFocus != null) {
				return CurrentFocus.ProcessKey(key);
			} else {
				return false;
			}
		}

		public override void SetCursorPosition()
		{
			if (CurrentFocus != null) {
				CurrentFocus.SetCursorPosition();
			}
		}

		public Widget CurrentFocus { get; set; }

		public void FocusNext()
		{
			CurrentFocus.HasFocus = false;
			CurrentFocus = null;

			bool next = true;

			foreach (var element in widgets) {
				var widget = element.Widget;
				if (widget.CanFocus) {
					if (widget == CurrentFocus) {
						next = true;
					} else if (next) {
						CurrentFocus = widget;
					}
				}
			}

			if (CurrentFocus == null) {
				CurrentFocus = GetFirstFocusable();
			}

			if (CurrentFocus != null) {
				CurrentFocus.HasFocus = true;
			}
		}

		public Widget GetFirstFocusable()
		{
			foreach (var element in widgets) {
				var widget = element.Widget;
				if (widget.CanFocus) {
					return widget;
				}
			}
			return null;
		}

		public void Add(Widget widget, Setting setting)
		{
			widgets.Add(new Element() {
				Widget = widget,
				Setting = setting
			});

			if (CurrentFocus == null && widget.CanFocus) {
				CurrentFocus = widget;
			}
		}

		public override bool CanFocus {
			get {
				foreach (var element in widgets) {
					if (element.Widget.CanFocus) {
						return true;
					}
				}
				return false;
			}
		}

		public override bool HasFocus {
			get {
				foreach (var element in widgets) {
					if (element.Widget.HasFocus) {
						return true;
					}
				}
				return false;
			}
			set {
			}
		}

		#region IEnumerable[Widget] implementation
		public IEnumerator<Widget> GetEnumerator()
		{
			foreach (var element in widgets) {
				yield return element.Widget;
			}
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

