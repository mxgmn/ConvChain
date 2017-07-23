//This is free and unencumbered software released into the public domain.

using System;
using System.Drawing;
using System.Xml.Linq;
using System.ComponentModel;

class Program
{
	static void Main()
	{
		XDocument xdoc = XDocument.Load("samples.xml");
		int pass = 1;

		foreach (XElement xelem in xdoc.Root.Elements("sample"))
		{
			string name = xelem.Get("name", "");
			bool[,] sample = new Bitmap($"Samples/{name}.png").ToArray();
			int receptorSize = xelem.Get("receptorSize", 2), outputSize = xelem.Get("outputSize", 32), iterations = xelem.Get("iterations", 2);
			double temperature = xelem.Get("temperature", 1.0);

			for (int k = 0; k < xelem.Get("screenshots", 1); k++)
			{
				Console.WriteLine($"> {name} {k}");
				Bitmap output = ConvChain(sample, receptorSize, temperature, outputSize, iterations).ToBitmap();
				output.Save($"{pass} {name} t={temperature} i={iterations} {k}.png");
			}

			pass++;
		}
	}

	static bool[,] ConvChain(bool[,] sample, int N, double temperature, int size, int iterations)
	{
		bool[,] field = new bool[size, size];
		double[] weights = new double[1 << (N * N)];
		Random random = new Random();

		for (int y = 0; y < sample.GetLength(1); y++) for (int x = 0; x < sample.GetLength(0); x++)
			{
				Pattern[] p = new Pattern[8];

				p[0] = new Pattern(sample, x, y, N);
				p[1] = p[0].Rotated();
				p[2] = p[1].Rotated();
				p[3] = p[2].Rotated();
				p[4] = p[0].Reflected();
				p[5] = p[1].Reflected();
				p[6] = p[2].Reflected();
				p[7] = p[3].Reflected();

				for (int k = 0; k < 8; k++) weights[p[k].Index()] += 1;
			}

		for (int k = 0; k < weights.Length; k++) if (weights[k] <= 0) weights[k] = 0.1;
		for (int y = 0; y < size; y++) for (int x = 0; x < size; x++) field[x, y] = random.Next(2) == 1;

		double energyExp(int i, int j)
		{
			double value = 1.0;
			for (int y = j - N + 1; y <= j + N - 1; y++) for (int x = i - N + 1; x <= i + N - 1; x++) value *= weights[new Pattern(field, x, y, N).Index()];
			return value;
		};

		void metropolis(int i, int j)
		{
			double p = energyExp(i, j);
			field[i, j] = !field[i, j];
			double q = energyExp(i, j);

			if (Math.Pow(q / p, 1.0 / temperature) < random.NextDouble()) field[i, j] = !field[i, j];
		};

		for (int k = 0; k < iterations * size * size; k++) metropolis(random.Next(size), random.Next(size));
		return field;
	}
}

class Pattern
{
	public bool[,] data;

	private int Size() => data.GetLength(0);
	private void Set(Func<int, int, bool> f) { for (int j = 0; j < Size(); j++) for (int i = 0; i < Size(); i++) data[i, j] = f(i, j); }

	public Pattern(int size, Func<int, int, bool> f) { data = new bool[size, size];	Set(f);	}

	public Pattern(bool[,] field, int x, int y, int size) : this(size, (i, j) => false) {
		Set((i, j) => field[(x + i + field.GetLength(0)) % field.GetLength(0), (y + j + field.GetLength(1)) % field.GetLength(1)]);	}

	public Pattern Rotated() => new Pattern(Size(), (x, y) => data[Size() - 1 - y, x]);
	public Pattern Reflected() => new Pattern(Size(), (x, y) => data[Size() - 1 - x, y]);

	public int Index()
	{
		int result = 0;
		for (int y = 0; y < Size(); y++) for (int x = 0; x < Size(); x++) result += data[x, y] ? 1 << (y * Size() + x) : 0;
		return result; 
	}
}

static class Stuff
{
	public static T Get<T>(this XElement xelem, string attribute, T defaultT = default(T))
	{
		XAttribute a = xelem.Attribute(attribute);
		return a == null ? defaultT : (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(a.Value);
	}

	public static bool[,] ToArray(this Bitmap bitmap)
	{
		bool[,] result = new bool[bitmap.Width, bitmap.Height];
		for (int y = 0; y < result.GetLength(1); y++) for (int x = 0; x < result.GetLength(0); x++) result[x, y] = bitmap.GetPixel(x, y).R > 0;
		return result;
	}

	public static Bitmap ToBitmap(this bool[,] array)
	{
		Bitmap result = new Bitmap(array.GetLength(0), array.GetLength(1));
		for (int y = 0; y < result.Height; y++) for (int x = 0; x < result.Width; x++) result.SetPixel(x, y, array[x, y] ? Color.LightGray : Color.Black);
		return result;
	}
}
