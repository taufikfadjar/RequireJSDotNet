using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace RequireJsNet.Compressor.Helper
{
		internal class FileVirtualPathProvider : VirtualPathProvider
		{
			private bool _ensureExists = true;

			public string ApplicationPath { get; set; }

			internal bool EnsureExists
			{
				get
				{
					return this._ensureExists;
				}
				set
				{
					this._ensureExists = value;
				}
			}

			public FileVirtualPathProvider(string applicationPath)
			{
				if (string.IsNullOrEmpty(applicationPath))
					throw new ArgumentNullException("applicationPath");
				this.ApplicationPath = applicationPath;
			}

			public string MapPath(string virtualPath)
			{
				string oldValue = this.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? "~/" : "~";
				return virtualPath.Replace(oldValue, this.ApplicationPath);
			}

			public override bool FileExists(string virtualPath)
			{
				if (this.EnsureExists)
					return File.Exists(this.MapPath(virtualPath));
				else
					return true;
			}

			public override bool DirectoryExists(string virtualDir)
			{
				if (this.EnsureExists)
					return Directory.Exists(this.MapPath(virtualDir));
				else
					return true;
			}

			public override VirtualFile GetFile(string virtualPath)
			{
				string str = this.MapPath(virtualPath);
				return (VirtualFile)new FileVirtualPathProvider.FileInfoVirtualFile(str, new FileInfo(str));
			}

			public override VirtualDirectory GetDirectory(string virtualDir)
			{
				string str = this.MapPath(virtualDir);
				return (VirtualDirectory)new FileVirtualPathProvider.DirectoryInfoVirtualDirectory(str, new DirectoryInfo(str));
			}

			internal class FileInfoVirtualFile : VirtualFile
			{
				public FileInfo File { get; set; }

				public FileInfoVirtualFile(string virtualPath, FileInfo file)
					: base(virtualPath)
				{
					this.File = file;
				}

				public override Stream Open()
				{
					return (Stream)this.File.OpenRead();
				}
			}

			internal class DirectoryInfoVirtualDirectory : VirtualDirectory
			{
				public DirectoryInfo Directory { get; set; }

				public override IEnumerable Files
				{
					get
					{
						List<VirtualFile> list = new List<VirtualFile>();
						foreach (FileInfo file in this.Directory.GetFiles())
							list.Add((VirtualFile)new FileVirtualPathProvider.FileInfoVirtualFile(file.FullName, file));
						return (IEnumerable)list;
					}
				}

				public override IEnumerable Children
				{
					get
					{
						throw new NotImplementedException();
					}
				}

				public override IEnumerable Directories
				{
					get
					{
						throw new NotImplementedException();
					}
				}

				public DirectoryInfoVirtualDirectory(string virtualPath, DirectoryInfo directory)
					: base(virtualPath)
				{
					this.Directory = directory;
				}
			}
		}
	}

