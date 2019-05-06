namespace AMazeCS
{
    public class Position
    {
        public int X { private set; get; }
        public int Y { private set; get; }
        public Position From { set; get; }
        public Position(int x, int y, Position from)
        {
            X = x;
            Y = y;
            From = from;
        }
        public Position(int x, int y) : this(x, y, null) { }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(!(obj is Position))
            {
                return false;
            }
            var other = obj as Position;
            return this.X == other.X && this.Y == other.Y;
        }
        public override int GetHashCode()
        {
            return X * Y * X.GetHashCode() * Y.GetHashCode() * 47;
        }
    }
}
