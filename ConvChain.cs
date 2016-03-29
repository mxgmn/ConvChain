//This is free and unencumbered software released into the public domain.

using System;
using System.Xml;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

class Program
{
	static void Main(string[] args)
	{
		var xdoc = new XmlDocument();
		xdoc.Load("samples.xml");

		for (var xnode = xdoc.FirstChild.FirstChild; xnode != null; xnode = xnode.NextSibling)
			for (int k = 0; k < xnode.Get("screenshots", 1); k++)
			{
				string name = xnode.Get("name", "");
				Bitmap output = ConvChain(new Bitmap("Samples/" + name + ".bmp"), xnode.Get("receptorSize", 2),
					xnode.Get("temperature", 0.02), xnode.Get("outputSize", 32), xnode.Get("iterations", 2));
				output.Save(name + " " + k + ".bmp");
			}
	}

	static Bitmap ConvChain(Bitmap sample, int N, double temperature, int size, int iterations)
	{
		int[,] field = new int[size, size];
		double[] weights = new double[1 << (N * N)];
		Random random = new Random();

		for (int i = 0; i < weights.Length; i++) weights[i] = 0;

		for (int y = 0; y < sample.Size.Height; y++) for (int x = 0; x < sample.Size.Width; x++)
			{
				Pattern[] p = new Pattern[8];

				p[0] = new Pattern(sample, x, y, N);
				p[1] = p[0].Rotated;
				p[2] = p[1].Rotated;
				p[3] = p[2].Rotated;
				p[4] = p[0].Reflected;
				p[5] = p[1].Reflected;
				p[6] = p[2].Reflected;
				p[7] = p[3].Reflected;

				for (int k = 0; k < 8; k++) weights[p[k].Index] += 1;
			}

		double sum = 8 * sample.Size.Width * sample.Size.Height;
		for (int k = 0; k < weights.Length; k++) weights[k] /= sum;

		for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) field[x, y] = random.Next(2);

		Func<int, int, int, double> energy = (color, i, j) =>
		{
			double value = 0;
			int oldColor = field[i, j];
			field[i, j] = color;

			for (int y = j - N + 1; y <= j + N - 1; y++) for (int x = i - N + 1; x <= i + N - 1; x++)
					value += weights[new Pattern(field, x, y, N).Index];

			field[i, j] = oldColor;
			return value;
		};

		Action<int, int> heatBath = (i, j) =>
		{
			var probabilities = new Dictionary<int, double>();
			for (int color = 0; color < 2; color++) probabilities.Add(color, Math.Exp(energy(color, i, j) / temperature));
			field[i, j] = probabilities.Random(random);
		};

		for (int k = 0; k < iterations * size * size; k++) heatBath(random.Next(size), random.Next(size));

		var result = new Bitmap(size, size);
		for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) result.SetPixel(x, y, field[x, y] == 0 ? Color.LightGray : Color.Black);
		return result;
	}
}

class Pattern
{
	public int[,] data;

	private int Size { get { return data.GetLength(0); } }
	private void Set(Func<int, int, int> f) { for (int j = 0; j < Size; j++) for (int i = 0; i < Size; i++) data[i, j] = f(i, j); }

	public Pattern(int size, Func<int, int, int> f)	{
		data = new int[size, size];
		Set(f);	}

	public Pattern(Bitmap bitmap, int x, int y, int size) : this(size, (i, j) => 0) {
		Set((i, j) => bitmap.GetPixel((x + i) % bitmap.Width, (y + j) % bitmap.Height).R == 0 ? 1 : 0); }

	public Pattern(int[,] field, int x, int y, int size) : this(size, (i, j) => 0) {
		Set((i, j) => field[(x + i + field.GetLength(0)) % field.GetLength(0), (y + j + field.GetLength(1)) % field.GetLength(1)]);	}

	public Pattern Rotated { get { return new Pattern(Size, (x, y) => data[Size - 1 - y, x]); } }
	public Pattern Reflected { get { return new Pattern(Size, (x, y) => data[Size - 1 - x, y]);	} }

	public int Index
	{
		get
		{
			int result = 0, power = 1;
			for (int y = 0; y < Size; y++) for (int x = 0; x < Size; x++)
				{
					result += data[x, y] * power;
					power *= 2;
				}
			return result;
		}
	}
}

static class Stuff
{
	public static T Get<T>(this XmlNode node, string attribute, T defaultT = default(T))
	{
		string s = ((XmlElement)node).GetAttribute(attribute);
		var converter = TypeDescriptor.GetConverter(typeof(T));
		return s == "" ? defaultT : (T)converter.ConvertFromString(s);
	}

	public static T Random<T>(this Dictionary<T, double> dic, Random random)
	{
		if (dic.Count == 0) return default(T);

		double r = random.NextDouble();
		var keys = dic.Keys.ToList();
		double[] values = dic.Values.ToArray();

		double sum = values.Sum();
		for (int j = 0; j < values.Count(); j++) values[j] /= sum;

		int i = 0;
		double x = 0;

		while (i < dic.Count)
		{
			x += values[i];
			if (r <= x) return keys[i];
			i++;
		}

		return default(T);
	}
}
