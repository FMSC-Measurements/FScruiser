using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FScruiser.Droid
{
    public class FolderService : IFolderService
    {
        public IEnumerable<string> CruiseFolders
        {
            get
            {
                var docDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
                var downloadDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);

                return (from file in
                        (docDir.ListFiles() ?? new File[] { }).Union(downloadDir.ListFiles() ?? new File[] { })
                        where (file.IsFile && System.IO.Path.GetExtension(file.AbsolutePath) == ".cruise")
                        select file.AbsolutePath);
            }
        }
    }
}