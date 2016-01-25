using Gtk;
using Weland;
using System;
using System.Collections.Generic;

namespace Hopper {
  public class Hop5DPoly {
    bool valid;
    public short minX;
    public short maxX;
    public short minY;
    public short maxY;
    public short minZ;
    public short maxZ;
    public List<Point> points;
    
    public Hop5DPoly(Level level, int index) {
      Polygon poly = level.Polygons[index];
      this.valid = true;
      this.points = new List<Point>();
      minZ = poly.FloorHeight;
      maxZ = poly.CeilingHeight;
      for (short i = 0; i < poly.VertexCount; ++i) {
        Point p = level.Endpoints[poly.EndpointIndexes[i]];
        points.Add(p);
        if (i == 0) {
          minX = maxX = p.X;
          minY = maxY = p.Y;
        } else {
          if (p.X < minX) minX = p.X;
          if (p.X > maxX) maxX = p.X;
          if (p.Y < minY) minY = p.Y;
          if (p.Y > maxY) maxY = p.Y;
        }
      }
      if (minX >= maxX || minY >= maxY || minZ >= maxZ)
        valid = false;
      
      int det = 0;
      for (int i = 0; valid && i < poly.VertexCount; ++i) {
        det = Determinant(points[i],
                          points[(i + 1) % poly.VertexCount],
                          points[(i + 2) % poly.VertexCount]);
        if (det != 0)
          break;
      }
      if (det == 0)
        valid = false;
      if (det > 0)
        points.Reverse();
    }
    
    static int Determinant(Point p1, Point p2, Point p3) {
      return ((p2.X - p1.X) * (p3.Y - p1.Y)) -
             ((p3.X - p1.X) * (p2.Y - p1.Y));
    }
    
    bool HasSeparatingAxis(Hop5DPoly other) {
      for (int i = 0; i < points.Count; ++i) {
        bool clean = true;
        for (int j = 0; j < other.points.Count; ++j) {
          int det = Determinant(points[i],
                                points[(i + 1) % points.Count],
                                other.points[j]);
          if (det < 0)
            clean = false;
        }
        if (clean)
          return true;
      }
      return false;
    }
    
    public bool Intersects(Hop5DPoly other) {
      if (!valid || !other.valid)
        return false;
      
      if (minX >= other.maxX || other.minX >= maxX ||
          minY >= other.maxY || other.minY >= maxY ||
          minZ >= other.maxZ || other.minZ >= maxZ)
        return false;
      
      if (this.HasSeparatingAxis(other))
        return false;
      if (other.HasSeparatingAxis(this))
        return false;
      
      return true;
    }
  }

  public static class Hop5DSpace {
    public static List<Tuple<int, int>> Intersections(Level level) {
      List<Hop5DPoly> polys = new List<Hop5DPoly>();
      for (int i = 0; i < level.Polygons.Count; ++i) {
        polys.Add(new Hop5DPoly(level, i));
      }
      List<Tuple<int, int>> result = new List<Tuple<int, int>>();
      for (int i = 0; i < polys.Count; ++i) {
        for (int j = i + 1; j < polys.Count; ++j) {
          if (polys[i].Intersects(polys[j]))
            result.Add(new Tuple<int, int>(i, j));
        }
      }
      return result;
    }
  }

}
