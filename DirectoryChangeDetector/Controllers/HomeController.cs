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
			DataFileName = Path.GetFileName(directoryPath) + ".json";

			if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
			{
				ViewBag.Error = "Zadaný adresáø neexistuje.";
				return View("Index");
			}

			var currentData = AnalyzeDirectory(directoryPath);
			var storedData = LoadStoredData();
			var changes = CompareData(storedData, currentData);

			SaveData(currentData);

			ViewBag.Changes = changes;
			return View("Index");
		}

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

		private Dictionary<string, FileData> LoadStoredData()
		{
			if (!System.IO.File.Exists(DataFileName))
			{
				return new Dictionary<string, FileData>();
			}

			string json = System.IO.File.ReadAllText(DataFileName);
			return JsonSerializer.Deserialize<Dictionary<string, FileData>>(json);
		}

		private void SaveData(Dictionary<string, FileData> data)
		{
			string json = JsonSerializer.Serialize(data);
			System.IO.File.WriteAllText(DataFileName, json);
		}

		private Changes CompareData(Dictionary<string, FileData> oldData, Dictionary<string, FileData> newData)
		{
			var changes = new Changes();

			foreach (var file in newData)
			{
				if (!oldData.ContainsKey(file.Key))
				{
					changes.Added.Add($"{file.Key}_v{file.Value.Version}");
				}
				else if (oldData[file.Key].LastModified != file.Value.LastModified)
				{
					file.Value.Version = oldData[file.Key].Version + 1;
					changes.Modified.Add($"{file.Key}_v{file.Value.Version}");
				}
			}

			foreach (var file in oldData.Keys.Except(newData.Keys))
			{
				changes.Deleted.Add($"{file}_v{oldData[file].Version}");
			}

			return changes;
		}
	}
}
