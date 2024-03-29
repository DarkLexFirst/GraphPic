﻿using System;
using System.Windows;

namespace GraphPic.UI
{
	public static class VectorExtension
	{
		public static double GetAngleTo(this Vector input)
		{
			var normalized = input;
			normalized.Normalize();

			return Math.Atan2(normalized.Y, normalized.X);
		}

		public static Vector ToVector(this double angle)
		{
			return new Vector(Math.Cos(angle), Math.Sin(angle));
		}

		public static Vector ToVector(this double angle, double multiplier)
		{
			return angle.ToVector() * multiplier;
		}

		public static Vector SwithTop(this Vector vector, UIElement element)
		{
			vector.Y = element.RenderSize.Height - vector.Y;
			return vector;
		}

		public static Point SwithTop(this Point vector, UIElement element)
		{
			vector.Y = element.RenderSize.Height - vector.Y;
			return vector;
		}
	}
}
