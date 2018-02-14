using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TimeIsUp {
	class Line {
		public List<Vector2> Points { get; set; }
		public Color Color { get; set; }
		public Line(Color color, params Vector2[] points) {
			Points = new List<Vector2>();
			foreach (var p in points) {
				Points.Add(p);
			}
			Color = color;
		}
		public Line(int x1, int y1, int x2, int y2, Color color) {
			Points = new List<Vector2>() { new Vector2(x1, y1), new Vector2(x2, y2) };
			Color = color;
		}
		public override bool Equals(object obj) {
			return obj.GetHashCode() == GetHashCode();
		}
		public override int GetHashCode() {
			unchecked {
				int hash = 97;
				hash = hash * 127 + Points.GetHashCode();
				hash = hash * 127 + Color.GetHashCode();
				return hash;
			}
		}
	}
}
