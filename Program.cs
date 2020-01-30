using PDollarGestureRecognizer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace pdollar
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			int size = args.Length;
			List<Point> l1 = new List<Point>();
			List<Gesture> l2 = new List<Gesture>();

			if (size == 0)
			{
				Console.WriteLine("Help Menu :");
				Console.WriteLine("Adds the gesture file to the list of templates : ");
				Console.WriteLine("pdollar –t <gesturefile>");

				Console.WriteLine();
				Console.WriteLine("Clears the templates:");
				Console.WriteLine("pdollar ‐r");

				Console.WriteLine();

				Console.WriteLine("Prints the name of gestures as they are recognized from the event stream:");
				Console.WriteLine("pdollar <eventstream>");
				Console.WriteLine();
				return;
			}


			if (args[0] == "-t")
			{
				string text = args[1];
				Console.WriteLine(text);
				if (File.Exists(text))
				{
					using (StreamReader streamReader = new StreamReader(text))
					{
						int num = 0;
						string text2 = streamReader.ReadLine();
						string text3;
						while ((text3 = streamReader.ReadLine()) != null)
						{
							if (text3 == "BEGIN")
							{
								num++;
							}
							else if (!(text3 == "END"))
							{
								string[] expr_DD = text3.Split(new char[]
								{
									','
								});
								float x = float.Parse(expr_DD[0], CultureInfo.InvariantCulture.NumberFormat);
								float y = float.Parse(expr_DD[1], CultureInfo.InvariantCulture.NumberFormat);
								Point item = new Point(x, y, num);
								l1.Add(item);
							}
						}
						if (!Directory.Exists(Application.StartupPath + "\\GestureSet\\NewGestures"))
						{
							Directory.CreateDirectory(Application.StartupPath + "\\GestureSet\\NewGestures");
						}
						GestureIO.WriteGesture(l1.ToArray(), text2, string.Format("{0}\\GestureSet\\NewGestures\\{1}.xml", Application.StartupPath, text2));
						Console.WriteLine("New gesture registered!");
						return;
					}
				}
				Console.WriteLine("File not found!!!!!");
				return;
			}
			if (args[0] == "-r")
			{
				FileInfo[] files = new DirectoryInfo(string.Format("{0}\\GestureSet\\NewGestures\\", Application.StartupPath)).GetFiles();
				for (int i = 0; i < files.Length; i++)
				{
					FileInfo fileInfo = files[i];
					if (fileInfo.Extension == ".xml")
					{
						fileInfo.Delete();
					}
				}
				Console.WriteLine("Gesture templates deleted");
				return;
			}



			string path = args[0];
			if (File.Exists(path))
			{
				using (StreamReader streamReader2 = new StreamReader(path))
				{
					int num2 = 0;
					string text4;
					while ((text4 = streamReader2.ReadLine()) != null)
					{
						if (text4 == "MOUSEDOWN")
						{
							num2++;
						}
						else if (!(text4 == "MOUSEUP"))
						{
							if (text4 == "RECOGNIZE")
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
								Console.WriteLine("Recognized as: " + str);
							}
							else
							{
								string[] expr_339 = text4.Split(new char[]
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
			Console.WriteLine("File Unavailable in current Directory");
		}
	}
}
