using FScruiser.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FScruiser.Droid.Services
{
    public class CruiseFileService : ICruiseFileService
    {
        public static string GetDocsDir()
        {
            try
            {
                Java.IO.File file = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
                return file.AbsolutePath;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static string GetDownloadsDir()
        {
            try
            {
                var file = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                return file.AbsolutePath;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static IEnumerable<FileGroup> GetCruiseFileGroups()
        {
            var docsDir = GetDocsDir();
            if (docsDir != null && System.IO.Directory.Exists(docsDir))
            {
                var files = System.IO.Directory.GetFiles(docsDir, "*.cruise", System.IO.SearchOption.AllDirectories)
                    .Select(x => new FileInfo(x)).ToArray();
                yield return new FileGroup() { GroupName = "Documents", CruiseFiles = files };
            }

            var downloadDir = GetDownloadsDir();
            if (downloadDir != null && System.IO.Directory.Exists(downloadDir))
            {
                var files = System.IO.Directory.GetFiles(downloadDir, "*.cruise", System.IO.SearchOption.AllDirectories)
                    .Select(x => new FileInfo(x)).ToArray();
                yield return new FileGroup() { GroupName = "Downloads", CruiseFiles = files };
            }

            yield break;
        }

        public IEnumerable<FileGroup> CruiseFilesGroups
        {
            get
            {
                return GetCruiseFileGroups();
            }
        }
    }
}