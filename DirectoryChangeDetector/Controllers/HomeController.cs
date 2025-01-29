using Microsoft.AspNetCore.Mvc;
using DirectoryChangeDetector.Models;
using System.Text.Json;

namespace DirectoryChangeDetector.Controllers
{
	public class HomeController : Controller
	{
		private static string DataFileName = "DirectoryData.json";

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Analyze(string directoryPath)
		{
			//☺ Neplatná cesta k adresáři
			if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
			{
				ViewBag.Error = "Zadaný adresář neexistuje.";
				return View("Index");
			}

			DataFileName = Path.GetFileName(directoryPath) + ".json";  //☺ Nastavení názvu souboru pro uložení dat

			var currentData = AnalyzeDirectory(directoryPath);  //☺ Analýza souborů v zadaném adresáři
			var storedData = LoadStoredData();  //☺ Načteme uložená data
			var changes = CompareData(storedData, currentData);  //☺ Porovnáme aktuální a uložená data

			SaveData(currentData);  //☺ Uložíme aktuální data

			ViewBag.Changes = changes;  //☺ Zobrazíme změny na stránce
			return View("Index");
		}

		//☺ Funkce pro analýzu souborů v adresáři
		private Dictionary<string, FileData> AnalyzeDirectory(string path)
		{
			var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
			var fileData = new Dictionary<string, FileData>();

			foreach (var file in files)
			{
				string relativePath = Path.GetRelativePath(path, file);
				var lastModified = System.IO.File.GetLastWriteTime(file);
				fileData[relativePath] = new FileData { Path = relativePath, LastModified = lastModified, Version = 1 };
			}

			return fileData;
		}

		//☺ Funkce pro načtení uložených dat
		private Dictionary<string, FileData> LoadStoredData()
		{
			if (!System.IO.File.Exists(DataFileName))
			{
				return new Dictionary<string, FileData>();
			}

			string json = System.IO.File.ReadAllText(DataFileName);
			return JsonSerializer.Deserialize<Dictionary<string, FileData>>(json);
		}

		//☺ Funkce pro uložení dat do souboru
		private void SaveData(Dictionary<string, FileData> data)
		{
			string json = JsonSerializer.Serialize(data);
			System.IO.File.WriteAllText(DataFileName, json);
		}

		//☺ Funkce pro porovnání starých a nových dat a detekci změn
		private Changes CompareData(Dictionary<string, FileData> oldData, Dictionary<string, FileData> newData)
		{
			var changes = new Changes();

			foreach (var file in newData)
			{
				if (!oldData.ContainsKey(file.Key))
				{
					changes.Added.Add($"{file.Key}_v{file.Value.Version}");  //☺ Nový soubor
				}
				else if (oldData[file.Key].LastModified != file.Value.LastModified)
				{
					file.Value.Version = oldData[file.Key].Version + 1;  //☺ Zvýšení verze souboru
					changes.Modified.Add($"{file.Key}_v{file.Value.Version}");  //☺ Změněný soubor
				}
			}

			foreach (var file in oldData.Keys.Except(newData.Keys))
			{
				changes.Deleted.Add($"{file}_v{oldData[file].Version}");  //☺ Smazaný soubor
			}

			return changes;
		}
	}
}
