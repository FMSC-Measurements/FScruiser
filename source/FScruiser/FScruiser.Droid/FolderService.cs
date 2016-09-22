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
    public class FolderService : ICruiseFolderService
    {
        public IEnumerable<string> CruiseFolders
        {
            get
            {
                File docDir = null;
                File downloadDir = null;
                try
                {
                    docDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
                }
                catch (Exception) { }
                try
                {
                    downloadDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                }
                catch (Exception) { }

                foreach (var path in ListCruiseFilePaths(docDir))
                { yield return path; }

                foreach (var path in ListCruiseFilePaths(downloadDir))
                { yield return path; }
            }
        }

        IEnumerable<string> ListCruiseFilePaths(File folder)
        {
            if (folder == null) { return new string[0]; }
            else
            {
                return (from file in
                            folder.ListFiles() ?? new File[0]
                        where (file.IsFile && System.IO.Path.GetExtension(file.AbsolutePath) == ".cruise")
                        select file.AbsolutePath);
            }
        }
    }
}