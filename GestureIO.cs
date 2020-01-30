using PDollarGestureRecognizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace pdollar
{
	public class GestureIO
	{
		public static Gesture ReadGesture(string fileName)
		{
			List<Point> list = new List<Point>();
			XmlTextReader xmlTextReader = null;
			int num = -1;
			string text = "";
			try
			{
				xmlTextReader = new XmlTextReader(File.OpenText(fileName));
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType == XmlNodeType.Element)
					{
						string name = xmlTextReader.Name;
						if (!(name == "Gesture"))
						{
							if (!(name == "Stroke"))
							{
								if (name == "Point")
								{
									list.Add(new Point(float.Parse(xmlTextReader["X"]), float.Parse(xmlTextReader["Y"]), num));
								}
							}
							else
							{
								num++;
							}
						}
						else
						{
							text = xmlTextReader["Name"];
							if (text.Contains("~"))
							{
								text = text.Substring(0, text.LastIndexOf('~'));
							}
							if (text.Contains("_"))
							{
								text = text.Replace('_', ' ');
							}
						}
					}
				}
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
			return new Gesture(list.ToArray(), text);
		}

		public static void WriteGesture(Point[] points, string gestureName, string fileName)
		{
			using (StreamWriter streamWriter = new StreamWriter(fileName))
			{
				streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
				streamWriter.WriteLine("<Gesture Name = \"{0}\">", gestureName);
				int num = -1;
				for (int i = 0; i < points.Length; i++)
				{
					if (points[i].StrokeID != num)
					{
						if (i > 0)
						{
							streamWriter.WriteLine("\t</Stroke>");
						}
						streamWriter.WriteLine("\t<Stroke>");
						num = points[i].StrokeID;
					}
					streamWriter.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\" T = \"0\" Pressure = \"0\" />", points[i].X, points[i].Y);
				}
				streamWriter.WriteLine("\t</Stroke>");
				streamWriter.WriteLine("</Gesture>");
			}
		}
	}
}
