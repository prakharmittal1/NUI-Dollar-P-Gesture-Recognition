using System;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using PDollarGestureRecognizer;

namespace pdollar{

	internal class DollarRecognizer{

		private static void Main(string[] args){

			int size;
			List<Point> l1;
			List<Gesture> l2;
			Initialize(args, out size, out l1, out l2);

			if (size != 0){

				if (args[0] == "-t"){

					string gestureName1 = args[1];
					Console.WriteLine();
					Console.WriteLine(gestureName1);

					if (!File.Exists(gestureName1)){

						Console.WriteLine("File Unavailable, Try again");
						return;
					}

					using (StreamReader getGesture = new StreamReader(gestureName1)){

						string marker;
						int index = 0;

						string gestureName = getGesture.ReadLine();

						marker = FindStartEnd(l1, getGesture, ref index);
						addGesture();
						GestureIO.WriteGesture(l1.ToArray(), gestureName, string.Format
							("{0}\\Added Gestures\\Gestures\\{1}.xml", Application.StartupPath, gestureName));

						Console.WriteLine("This Gesture template is Added Successfully");
						return;
					}
				}
				if (args[0] == "-r"){

					FileInfo[] data = new DirectoryInfo(string.Format("{0}\\Added Gestures\\Gestures\\", Application.StartupPath)).GetFiles();
					int size2 = data.Length;

					for (int i = 0; i < size2; i++){

						FileInfo d1 = data[i];
						if (d1.Extension == ".xml"){

							d1.Delete();
						}
					}
					Console.WriteLine("Gesture Template Removed ");
					return;
				}
				else{
					string path = args[0];
					if (File.Exists(path))
					{
						RemoveFile(l1, l2, path);
						return;
					}
					Console.WriteLine("File Unavailable in current Directory");
				}
			}
			else{
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

		private static void addGesture(){

			if (!Directory.Exists(Application.StartupPath+"\\Added Gestures\\Gestures")){

				Directory.CreateDirectory(Application.StartupPath+"\\Added Gestures\\Gestures");
			}
		}

		private static void Initialize(string[] args, out int size, out List<Point> l1, out List<Gesture> l2){

			size = args.Length;
			l1 = new List<Point>();
			l2 = new List<Gesture>();
		}

		private static string FindStartEnd(List<Point> l1, StreamReader streamReader, ref int index){

			string marker;
			while ((marker = streamReader.ReadLine()) != null){

				if (marker == "BEGIN"){

					index++;
				}
				else if (!(marker == "END")){

					string[] splitString = marker.Split(new char[]{','});

					float cordx = float.Parse(splitString[0], CultureInfo.InvariantCulture.NumberFormat);
					float cordy = float.Parse(splitString[1], CultureInfo.InvariantCulture.NumberFormat);
					Point item = new Point(cordx, cordy, index);

					l1.Add(item);
				}
			}

			return marker;
		}

		private static void RemoveFile(List<Point> l1, List<Gesture> l2, string path){
			using (StreamReader getString= new StreamReader(path)){

				int item = 0;
				string pointer;
				while ((pointer = getString.ReadLine()) != null){

					if (pointer == "MOUSEDOWN"){
						item++;
					}
					else if (!(pointer == "MOUSEUP"))
					{
						RecognitionSuccess(l1, l2, item, pointer);
					}
				}
				return;
			}
		}

		private static void RecognitionSuccess(List<Point> l1, List<Gesture> l2, int item, string pointer)
		{
			if (pointer == "RECOGNIZE")
			{

				if (!Directory.Exists(Application.StartupPath +
					"\\Added Gestures\\Gestures"))
				{

					Console.WriteLine("Gestures need to be registered.");
					Directory.CreateDirectory(Application.StartupPath +
						"\\Added Gestures\\Gestures");
				}

				string[] file1 = Directory.GetDirectories(Application.StartupPath +
					"\\Added Gestures");
				int stringSize1 = file1.Length;

				for (int j = 0; j < stringSize1; j++)
				{

					string[] readDirectory = Directory.GetFiles(file1[j], "*.xml");

					int stringSize = readDirectory.Length;

					for (int i = 0; i < stringSize; i++)
					{
						string name = readDirectory[i];

						l2.Add(GestureIO.ReadGesture(name));
					}
				}
				Gesture[] recognizer = l2.ToArray();
				string str = PointCloudRecognizer.Classify(new Gesture(l1.ToArray(), ""), recognizer);
				Console.WriteLine("RESULT: " + str);
			}

			else
			{
				string[] addFile = pointer.Split(new char[] { ',' });

				float cord1 = float.Parse(addFile[0], CultureInfo.InvariantCulture.NumberFormat);
				float cord2 = float.Parse(addFile[1], CultureInfo.InvariantCulture.NumberFormat);

				Point item2 = new Point(cord1, cord2, item);
				l1.Add(item2);
			}
		}
	}
}
