using System;
using UnityEngine;

namespace Core
{
    public class Position
    {
        public float X => Coordinates.x;
        public float Y => Coordinates.y;
        public float Z => Coordinates.z;
        public float Orientation => Rotation.y;

        public Vector3 Coordinates { get; protected set; }
        public Vector3 Rotation { get; protected set; }

        public Position(float x = 0, float y = 0, float z = 0, float o = 0)
        {
            Coordinates = new Vector3(x, y, z);
            Rotation = new Vector3(0, NormalizeOrientation(o), 0);
        }

        public Position(Position loc)
        {
            Relocate(loc);
        }
    

        public void Relocate(float x, float y)
        {
            Coordinates = new Vector3(x, y, Coordinates.z);
        }

        public void Relocate(float x, float y, float z)
        {
            Coordinates = new Vector3(x, y, z);
        }

        public void Relocate(float x, float y, float z, float orientation)
        {
            Coordinates = new Vector3(x, y, z);
            SetOrientation(orientation);
        }

        public void Relocate(Position pos)
        {
            Coordinates = pos.Coordinates;
            SetOrientation(pos.Orientation);
        }

        public void Relocate(Vector3 pos)
        {
            Coordinates = pos;
        }

        public void RelocateOffset(Position offset)
        {
            float newX = X + (offset.X * Mathf.Cos(Orientation) + offset.Y * Mathf.Sin(Orientation + Mathf.PI));
            float newY = Y + (offset.Y * Mathf.Cos(Orientation) + offset.X * Mathf.Sin(Orientation));
            float newZ = Z + offset.Z;
            Coordinates = new Vector3(newX, newY, newZ);
            SetOrientation(Orientation + offset.Orientation);
        }

        public void SetOrientation(float orientation)
        {
            Rotation = new Vector3(0, NormalizeOrientation(orientation), 0);
        }


        public void GetPosition(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        public void GetPosition(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public void GetPosition(out float x, out float y, out float z, out float o)
        {
            x = X;
            y = Y;
            z = Z;
            o = Orientation;
        }

        public float GetExactDist2DSq(float x, float y)
        { 
            var dx = X - x;
            var dy = Y - y; 
            return dx * dx + dy * dy;
        }

        public float GetExactDist2D(float x, float y)
        {
            return Mathf.Sqrt(GetExactDist2DSq(x, y));
        }

        public float GetExactDist2DSq(Position pos)
        {
            var dx = X - pos.X;
            var dy = Y - pos.Y;
            return dx * dx + dy * dy;
        }

        public float GetExactDist2D(Position pos)
        {
            return Mathf.Sqrt(GetExactDist2DSq(pos));
        }

        public float GetExactDistSq(float x, float y, float z)
        {
            var dz = Z - z;
            return GetExactDist2DSq(x, y) + dz * dz;
        }

        public float GetExactDist(float x, float y, float z)
        {
            return Mathf.Sqrt(GetExactDistSq(x, y, z));
        }

        public float GetExactDistSq(Position pos)
        {
            var dx = X - pos.X;
            var dy = Y - pos.Y;
            var dz = Z - pos.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        public float GetExactDist(Position pos)
        {
            return Mathf.Sqrt(GetExactDistSq(pos));
        }


        public void GetPositionOffsetTo(Position endPos, Position retOffset)
        {
            var dx = endPos.X - X;
            var dy = endPos.Y - Y;

            var newX = dx * Mathf.Cos(Orientation) + dy * Mathf.Sin(Orientation);
            var newY = dy * Mathf.Cos(Orientation) - dx * Mathf.Sin(Orientation);
            var newZ = endPos.Z - Z;

            retOffset.Relocate(newX, newY, newZ);
            retOffset.SetOrientation(endPos.Orientation - Orientation);
        }

        public Position GetPositionWithOffset(Position offset)
        {
            var newPosition = new Position(this);
            newPosition.RelocateOffset(offset);
            return newPosition;
        }


        public float GetAngle(Position pos)
        {
            return pos == null ? 0 : GetAngle(pos.X, pos.Y);
        }

        public float GetAngle(float x, float y)
        {
            var dx = x - X;
            var dy = y - Y;
            var ang = Mathf.Atan2(dy, dx);

            return ang >= 0 ? ang : 2 * Mathf.PI + ang;
        }

        public float GetRelativeAngle(Position pos)
        {
            return GetAngle(pos) - Orientation;
        }

        public float GetRelativeAngle(float x, float y)
        {
            return GetAngle(x, y) - Orientation;
        }

        public void GetSinCos(float x, float y, out float vsin, out float vcos)
        {
            var dx = X - x;
            var dy = Y - y;

            if (Mathf.Abs(dx) < 0.001f && Mathf.Abs(dy) < 0.001f)
            {
                var angle = (float)RandomHelper.NextDouble() * (2 * Mathf.PI);
                vcos = Mathf.Cos(angle);
                vsin = Mathf.Sin(angle);
            }
            else
            {
                var dist = Mathf.Sqrt(dx * dx + dy * dy);
                vcos = dx / dist;
                vsin = dy / dist;
            }
        }


        public bool IsInDist2D(float x, float y, float dist)
        {
            return GetExactDist2DSq(x, y) < dist * dist;
        }

        public bool IsInDist2D(Position pos, float dist)
        {
            return GetExactDist2DSq(pos) < dist * dist;
        }

        public bool IsInDist(float x, float y, float z, float dist)
        {
            return GetExactDistSq(x, y, z) < dist* dist;
        }

        public bool IsInDist(Position pos, float dist)
        {
            return GetExactDistSq(pos) < dist* dist;
        }

        public bool IsWithinBox(Position center, float xradius, float yradius, float zradius)
        {
            // rotate the WorldObject position instead of rotating the whole cube, that way we can make a simplified
            // is-in-cube check and we have to calculate only one point instead of 4

            // keep in mind that ingame orientation is counter-clockwise
            double rotation = 2 * Mathf.PI - center.Orientation;
            double sinVal = Math.Sin(rotation);
            double cosVal = Math.Cos(rotation);

            float boxDistX = X - center.X;
            float boxDistY = Y - center.Y;

            float rotX = (float)(center.X + boxDistX * cosVal - boxDistY * sinVal);
            float rotY = (float)(center.Y + boxDistY * cosVal + boxDistX * sinVal);

            // box edges are parallel to coordiante axis, so we can treat every dimension independently :D
            float dz = Z - center.Z;
            float dx = rotX - center.X;
            float dy = rotY - center.Y;

            return !(Mathf.Abs(dx) > xradius) && !(Mathf.Abs(dy) > yradius) && !(Mathf.Abs(dz) > zradius);
        }

        public bool HasInArc(float arc, Position pos, float border = 2.0f)
        {
            // always have self in arc
            if (pos == this)
                return true;

            // move arc to range 0.. 2*pi
            arc = NormalizeOrientation(arc);
            float angle = GetAngle(pos) - Orientation;

            // move angle to range -pi ... +pi
            angle = NormalizeOrientation(angle);
            if (angle > Mathf.PI)
                angle -= 2.0f * Mathf.PI;

            float lborder = -1 * (arc / border);                        // in range -pi..0
            float rborder = (arc / border);                             // in range 0..pi
            return (angle >= lborder && angle <= rborder);
        }

        public bool HasInLine(Position pos, float width)
        {
            if (!HasInArc(Mathf.PI, pos))
                return false;

            return Mathf.Abs(Mathf.Sin(GetRelativeAngle(pos))) * GetExactDist2D(pos.X, pos.Y) < width;
        }


        public static float NormalizeOrientation(float o)
        {
            var mod = o % (2.0f * Mathf.PI);
            if (mod < 0.0f)
                return mod + 2.0f * Mathf.PI;

            return mod;
        }

        public static bool operator ==(Position a, Position b)
        {
            if (ReferenceEquals(null, a))
                return ReferenceEquals(null, b);
            if (ReferenceEquals(null, b))
                return false;

            return Mathf.Approximately(a.X, b.X) && Mathf.Approximately(a.Y, b.Y) &&
                   Mathf.Approximately(a.Z, b.Z) && Mathf.Approximately(a.Orientation, b.Orientation);
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            var other = (Position)obj;
            return Mathf.Approximately(X, other.X) && Mathf.Approximately(Y, other.Y) &&
                   Mathf.Approximately(Z, other.Z) && Mathf.Approximately(Orientation, other.Orientation);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ Orientation.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y} Z:{Z} O:{Orientation}";
        }
    }
}