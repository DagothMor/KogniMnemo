﻿using CogniMnemo.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CogniMnemo
{
	/// <summary>
	/// Data base initialization.
	/// </summary>
	public static class DataBaseInitialization
	{
		public static void Start()
		{
			var readylist = new List<string>();
			var filtratedqueue = new Queue<string>();

			CheckForExistFolder();

			var bufferlist = FolderController.GetAllFileNamesInDataBase();

			for (int i = 0; i < bufferlist.Count; i++)
			{
				if (TextController.ManualInsertCardTextValidation(bufferlist[i]))
				{
					filtratedqueue.Enqueue(bufferlist[i]);
					continue;
				}
				if (TextController.AutomatedInsertCardTextValidation(bufferlist[i]))
				{
					readylist.Add(bufferlist[i]);
					continue;
				}
				else
				{
					if (bufferlist[i] != FolderController.CreateNewNameForFilePath(bufferlist[i], "incorrect"))
					{
						string bufferstart = FolderController.CreateNewNameForFilePath(bufferlist[i], "incorrect");
						string bufferend;
						Console.WriteLine($"Bad card on path {bufferlist[i]},Please edit card by rules");
						if (File.Exists(bufferstart))
						{
							bufferend = FolderController.SafeFileRename(bufferlist[i], FolderController.CreateNewNameForFilePath(bufferlist[i], "incorrect"));
							bufferlist[bufferlist.IndexOf(bufferstart)] = bufferend;
							bufferlist[i] = FolderController.CreateNewNameForFilePath(bufferlist[i], "incorrect");
						}
						else
						{
							FolderController.SafeFileRename(bufferlist[i], FolderController.CreateNewNameForFilePath(bufferlist[i], "incorrect"));
							bufferlist[i] = FolderController.CreateNewNameForFilePath(bufferlist[i], "incorrect");
						}
					}
				}
			}

			_ = readylist.OrderBy(q => q);

			foreach (string path in filtratedqueue)
			{
				CardController.CreateWorkCardFromManualInsertedCard(path);
			}
			while (filtratedqueue.Count != 0)
			{
				readylist.Add(filtratedqueue.Dequeue());
			}
			for (int i = 0; i < readylist.Count; i++)
			{
				if (readylist[i] != FolderController.CreateNewNameForFilePath(readylist[i], i.ToString()))
				{
					if (File.Exists(FolderController.CreateNewNameForFilePath(readylist[i], i.ToString())))
					{
						string bufferstart = FolderController.CreateNewNameForFilePath(readylist[i], i.ToString());
						string bufferend = FolderController.SafeFileRename(readylist[i], FolderController.CreateNewNameForFilePath(readylist[i], i.ToString()));
						readylist[readylist.IndexOf(bufferstart)] = bufferend;
					}
					else
					{
						FolderController.SafeFileRename(readylist[i], FolderController.CreateNewNameForFilePath(readylist[i], i.ToString()));
					}
				}
			}
		}
		/// <summary>
		/// Checking for exist folder
		/// </summary>
		public static void CheckForExistFolder()
		{
			if (!Directory.Exists($"{AppContext.BaseDirectory}" + @"CorgiMnemoDataBase\"))
			{
				Directory.CreateDirectory($"{AppContext.BaseDirectory}" + @"CorgiMnemoDataBase\");
			}
		}

	}
}
