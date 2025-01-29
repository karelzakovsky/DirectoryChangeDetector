using System;
using System.Collections.Generic;

namespace DirectoryChangeDetector.Models
{

	public class FileData
	{
		public string Path { get; set; }
		public DateTime LastModified { get; set; }
		public int Version { get; set; }
	}

	public class Changes
	{
		public List<string> Added { get; set; } = new List<string>();
		public List<string> Modified { get; set; } = new List<string>();
		public List<string> Deleted { get; set; } = new List<string>();
	}


}
