using System;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using PDollarGestureRecognizer;

namespace pdollar
{
	internal class DollarRecognizer
	{
		private static void Main(string[] args)
		{
			int size;
			List<Point> l1;
			List<Gesture> l2;
			Initialize(args, out size, out l1, out l2);

			if (size != 0)
			{
				if (args[0] == "-t")
				{
					string gestureName1 = args[1];
					Console.WriteLine();
					Console.WriteLine(gestureName1);

					if (!File.Exists(gestureName1))
					{
						Console.WriteLine("File Unavailable, Try again");
						return;
					}

					using (StreamReader getGesture = new StreamReader(gestureName1))
					{
						string marker;
						int index = 0;

						string gestureName = getGesture.ReadLine();

						marker = FindStartEnd(l1, getGesture, ref index);
						addGesture();
						GestureIO.WriteGesture(l1.ToArray(), gestureName, string.Format
							("{0}\\GestureSet\\NewGestures\\{1}.xml", Application.StartupPath, gestureName));

						Console.WriteLine("This Gesture template is Added Successfully");
						return;
					}
				}
				if (args[0] == "-r")
				{
					FileInfo[] data = new DirectoryInfo(string.Format("{0}\\GestureSet\\NewGestures\\", Application.StartupPath)).GetFiles();
					int size2 = data.Length;

					for (int i = 0; i < size2; i++)
					{
						FileInfo d1 = data[i];

						if (d1.Extension == ".xml")
						{
							d1.Delete();
						}
					}
					Console.WriteLine("Gesture Template Removed ");
					return;
				}
				else
				{
					string path = args[0];
					if (File.Exists(path))
					{
						RemoveFile(l1, l2, path);
						return;
					}
					Console.WriteLine("File Unavailable in current Directory");
				}
			}
			else
			{
				Console.WriteLine();

				Console.WriteLine("	Help");
				Console.WriteLine();

				Console.WriteLine("	pdollar –t <gesturefile> : Adds the gesture file to the list of templates");
				Console.WriteLine();

				Console.WriteLine("	pdollar ‐r : Clears the templates");
				Console.WriteLine();

				Console.WriteLine("	pdollar <eventstream> : Prints the name of gestures as they are recognized from the event stream");
				Console.WriteLine();

				return;
			}
		}

		private static void addGesture()
		{
			if (!Directory.Exists(Application.StartupPath + "\\GestureSet\\NewGestures"))
			{
				Directory.CreateDirectory(Application.StartupPath + "\\GestureSet\\NewGestures");
			}
		}

		private static void Initialize(string[] args, out int size, out List<Point> l1, out List<Gesture> l2)
		{
			size = args.Length;
			l1 = new List<Point>();
			l2 = new List<Gesture>();
		}

		private static string FindStartEnd(List<Point> l1, StreamReader streamReader, ref int index)
		{
			string marker;
			while ((marker = streamReader.ReadLine()) != null)
			{
				if (marker == "BEGIN")
				{
					index++;
				}
				else if (!(marker == "END"))
				{
					string[] splitString = marker.Split(new char[]
					{','
					});
					float cordx = float.Parse(splitString[0], CultureInfo.InvariantCulture.NumberFormat);
					float cordy = float.Parse(splitString[1], CultureInfo.InvariantCulture.NumberFormat);
					Point item = new Point(cordx, cordy, index);
					l1.Add(item);
				}
			}

			return marker;
		}

		private static void RemoveFile(List<Point> l1, List<Gesture> l2, string path)
		{
			using (StreamReader streamReader2 = new StreamReader(path))
			{
				int num2 = 0;
				string pointer;
				while ((pointer = streamReader2.ReadLine()) != null)
				{
					if (pointer == "MOUSEDOWN")
					{
						num2++;
					}
					else if (!(pointer == "MOUSEUP"))
					{
						if (pointer == "RECOGNIZE")
						{
							if (!Directory.Exists(Application.StartupPath + "\\GestureSet\\NewGestures"))
							{
								Console.WriteLine("Gestures need to be registered.");
								Directory.CreateDirectory(Application.StartupPath + "\\GestureSet\\NewGestures");
							}
							string[] directories = Directory.GetDirectories(Application.StartupPath + "\\GestureSet");
							for (int i = 0; i < directories.Length; i++)
							{
								string[] files2 = Directory.GetFiles(directories[i], "*.xml");
								for (int j = 0; j < files2.Length; j++)
								{
									string fileName = files2[j];
									l2.Add(GestureIO.ReadGesture(fileName));
								}
							}
							Gesture[] trainingSet = l2.ToArray();
							string str = PointCloudRecognizer.Classify(new Gesture(l1.ToArray(), ""), trainingSet);
							Console.WriteLine("RESULT: " + str);
						}
						else
						{
							string[] expr_339 = pointer.Split(new char[]
							{
									','
							});
							float x2 = float.Parse(expr_339[0], CultureInfo.InvariantCulture.NumberFormat);
							float y2 = float.Parse(expr_339[1], CultureInfo.InvariantCulture.NumberFormat);
							Point item2 = new Point(x2, y2, num2);
							l1.Add(item2);
						}
					}
				}
				return;
			}
		}
	}
}
