using Microsoft.Xna.Framework;

namespace TimeIsUp {
	struct Line {
		public Vector2 Point1 { get; set; }
		public Vector2 Point2 { get; set; }
		public Line(Vector2 p1, Vector2 p2) {
			Point1 = p1;
			Point2 = p2;
		}
		public Line(int x1, int y1, int x2, int y2) {
			Point1 = new Vector2(x1, y1);
			Point2 = new Vector2(x2, y2);
		}
	}
}
