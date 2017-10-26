using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace imageUpdaterV2._0
{
    public partial class watcher
    {

        private static db database; //get local instance of db class
        private static imageUpdater upd;


        //give full access to directory
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]

        public static void main()
        {
            
            database = new db();
            
            //using GetCurrentDirectory() get directory where executable is located
            //string path = System.IO.Directory.GetCurrentDirectory();
            //********* or *****
            //using user defined path
            string dir = "C:\\Users\\Yuri\\USBWebserver v8.5\\8.5\\root\\img";
            //upd = new imageUpdater();
            //string path = "";

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.IncludeSubdirectories = true; //allow to watch for changes in subdirectories

            watcher.Path = dir;
            /* Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch image files 
            watcher.Filter = "*.jp*";// watch only for files with extensions *.jpeg, *.jpg; 

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

        }//end of Main method



        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            upd = new imageUpdater();
            string cam_no = upd.getListViewItems();
            
            string filename = e.Name.ToString();//turn e.Name to string
            filename = filename.Remove(filename.LastIndexOf(".")); //remove the file extention
            filename = filename.Substring(filename.IndexOf("10.")); //remove any directory names or anything else before the image name to leave only ip_address and reg_no: 10.0.14.17_AY15USN

            string reg_no = filename.Remove(0, 11);

            
            string ip_address; //get the ip address of camera from the name

            ip_address = filename.Remove(filename.LastIndexOf("_"));

            if (cam_no.Contains("ANPR1"))
            {
                cam_no = "ANPR1";
            }
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine(e.FullPath + " " + e.ChangeType + " " + filename);

            //just for the test purpose write the name without extension again
            Console.WriteLine(filename);
            Console.WriteLine(ip_address);
            Console.WriteLine(reg_no);



            //06-10-2017 when file changed , created, or deleted create text file and make record
           /* using (StreamWriter write = new StreamWriter("report.txt", true))
            {

                write.WriteLine(e.FullPath + " " + e.ChangeType + " " + filename + " " + DateTime.Now);
                //just for the test purpose write the name without extension again
                write.WriteLine(filename);
                write.WriteLine(ip_address);
                write.Close();


            }*/

            var path = e.FullPath;
            path = path.Replace(@"\", @"\\").Replace("'", @"\'"); //otherwise path is stored in db without escape \'s

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                
                string query = "INSERT into `images` VALUES (null, current_timestamp(), '" + reg_no + "','" + path + "','" + cam_no + "','" + e.FullPath + "');";
                database.Insert(query);
               
            }

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                
                string query = "UPDATE images SET `reg_no` = '" + reg_no + "' WHERE `reg_no` = '" + reg_no + "'";
                database.Insert(query);
               
            }

            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                string query = "DELETE FROM `images` WHERE reg_no = '" +reg_no + "';";
                database.Delete(query);
            }

        }//end of onChange 

        public static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            MessageBox.Show(("File" + e.OldName + e.Name));

            //06-10-2017 when file is renamed create 
            using (StreamWriter write = new StreamWriter("onChange.txt", true))
            {
                write.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath + " " + DateTime.Now);
                write.Close();

            }


        }
    }
}

