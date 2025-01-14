﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Windows.Foundation;
using Windows.Graphics.Display;

namespace Uno.Extensions
{
	public static class RectExtensions
	{
		/// <summary>
		/// Creates a transformed <see cref="Rect"/> using a <see cref="Matrix3x2"/>.
		/// </summary>
		/// <param name="rect">The rectangle to transform</param>
		/// <param name="m">The matrix to use to transform the <paramref name="rect"/></param>
		/// <returns>A new rectangle</returns>
		internal static Rect Transform(this Rect rect, Matrix3x2 m)
			=> Matrix3x2Extensions.Transform(m, rect);

		/// <summary>
		/// Returns the orientation of the rectangle.
		/// </summary>
		/// <param name="rect">A rectangle.</param>
		/// <returns>Portrait, Landscape, or None (if the rectangle has an exact 1:1 ratio)</returns>
		public static DisplayOrientations GetOrientation(this Rect rect)
		{
			if (rect.Height > rect.Width)
			{
				return DisplayOrientations.Portrait;
			}
			else if (rect.Width > rect.Height)
			{
				return DisplayOrientations.Landscape;
			}
			else
			{
				return DisplayOrientations.None;
			}
		}

		internal static Rect OffsetRect(this Rect rect, double dx, double dy)
		{
			rect.X += dx;
			rect.Y += dy;

			return rect;
		}

		internal static Rect OffsetRect(this Rect rect, Point offset) => rect.OffsetRect(offset.X, offset.Y);

		internal static bool IsIntersecting(this Rect rect, Rect other)
		{
			rect.Intersect(other);
			return !rect.IsEmpty;
		}

		/// <summary>
		/// Gets the shortest distance from the given point to the edges of rect.
		/// If the rect <see cref="Rect.Contains"/> the point, then distance will be 0.
		/// </summary>
		internal static double GetDistance(this Rect rect, Point point)
		{
			// Note: cf. Contains comment to understand why we do 'point.X - rect.Width <= rect.X'

			var dx = point.X >= rect.X && point.X - rect.Width <= rect.X
				? 0 // Point is vertically aligned with rect
				: Math.Min(Math.Abs(point.X - rect.X), Math.Abs(point.X - rect.Right));
			var dy = point.Y >= rect.Y && point.Y - rect.Height <= rect.Y
				? 0 // Point is horizontally aligned with rect
				: Math.Min(Math.Abs(point.Y - rect.Y), Math.Abs(point.Y - rect.Bottom));

			return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
		}
	}
}
