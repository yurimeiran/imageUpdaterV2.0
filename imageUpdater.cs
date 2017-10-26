using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace imageUpdaterV2._0
{
    public partial class imageUpdater : Form
    {
        private db dbConn; //use db class to connect to database and use its functions
        
        int j;
        List<string>[] list;
        string name;
        int cnt = 0;
       
        public imageUpdater()
        {
            InitializeComponent();

            dbConn = new db();
            listView1.Font = new System.Drawing.Font("Calibri", 10 );
            get_cam_toList();
            get_data();
            watcher.main();
            
        }

        //method of getting all cam records from db and put them inside listview
        public void get_cam_toList()
        {
            string sel = "select * from cam_data";
            MySqlDataAdapter ad = new MySqlDataAdapter(sel, dbConn._connection);
            DataTable dt = new DataTable();
            ad.Fill(dt);

            listView1.View = View.Details;
            ListViewItem item;


            foreach (DataRow row in dt.Rows)
            {

                item = new ListViewItem();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0)
                        item.Text = row.ItemArray[i].ToString();
                    else
                        item.SubItems.Add(row.ItemArray[i].ToString());

                }

                listView1.Items.Add(item);

                dbConn._connection.Close();

            }


        } //end of cam_toList

        public string getListViewItems()

        {


            string text = listView1.Items[0].SubItems[0].ToString();

            return text;
        
        }
        
        //adding new cameras to a list and creating new record in db
        private void button1_Click(object sender, EventArgs e)
        {
            
            string anpr = "ANPR";     //give default name to the camera
            list = dbConn.Select();   //method from db class for getting all camera data
            //dataGW.Rows.Clear();

            FolderBrowserDialog fd = new FolderBrowserDialog(); //display dialog box for choosing directory where cam images are stored
            fd.ShowDialog();

            string pathRaw = fd.SelectedPath.ToString(); //store directory selected into string variable
            string path = pathRaw.Replace(@"\", @"\\").Replace("'", @"\'"); //otherwise path is stored in db without escape \'s

            try
            {
                int count = 0; //initiate count
                for (j = 0; j < list[0].Count; j++)
                {

                    cnt = list[0].Count; //get count of cameras in the list
               
                    do             
                    {
                        count++; //count until "while" expression won't be true

                    }

                    //this will compare count of cameras in db and when count is greater
                    //add this number to the camera name, like ANPR + number that is gerater then "cnt"
                    while (count > cnt); //loop, while count won't be greater that number of cameras added
                    //for some reason it stops when count is = to cnt?

                }

                //when cnt is = to count add +1 and store result in the database
                if (count == cnt)
                {
                    cnt = cnt + 1; //make number that is greater that the name of last camera in the list

                    string query = "INSERT into `cam_data` values ('" + anpr + cnt + "','" + path + "','" + cnt + "')"; //add new camera to db
                    dbConn.Insert(query); //pass squery string to insert method in db class

                    ListViewItem cam_n = new ListViewItem(anpr + cnt); //create listview and add camera name : ANPR + number
                    //string cam = cam_n.ToString(); 
                    cam_n.SubItems.Add(pathRaw); //display camera directory
                    listView1.Items.Add(cam_n);
                    

                   //get_cam_toList();//update the list of the cameras in the datagrid view
                }
                
            } //end of try

            catch (MySqlException ex)
            {
                    MessageBox.Show(ex.Message, "Trying different name!"); //display error message

                    if (ex.Number == 1062) //if error message  = 1062 (duplicate key entry, record exists)
                    {                      
                        dbConn.index();//execute index method (string) that will get biggest number from cam_no column

                        name = dbConn.index(); //return the number and assign it to string "name"
                        int id = Int32.Parse(name); //parse number returned in string format to integer

                        //now try to execute query again
                        try
                        {
                            cnt = id + 1; 

                            string query = "INSERT into `cam_data` values ('" + anpr + cnt + "','" + path + "','" + cnt + "')"; //add new camera to db
                            dbConn.Insert(query); //pass squery string to insert method in db class

                            ListViewItem cam_n = new ListViewItem(anpr + cnt); //create listview and add camera name : ANPR + number
                            //string cam = cam_n.ToString(); 
                            cam_n.SubItems.Add(pathRaw); //display camera directory
                            listView1.Items.Add(cam_n); //display camera name and directory in the listview
                        }

                        //handle any errors appeared
                        catch (MySqlException ex1)
                        {
                            MessageBox.Show(ex1.Message);
                            throw ex1;
                        }

                        finally { 
     
                        }
                       
                    }

            }
                return;
               
        } //end of button_click1 - adding cam to list


        //method of displaying all the records from images database
        public void get_data()
        {         
            List<string>[] list;
            list = dbConn.RegNoSelect();

            dataGW.Rows.Clear();
            for(int i = 0; i < list[0].Count; i++)
            {
                int name = dataGW.Rows.Add();
                dataGW.Rows[name].Cells[0].Value = list[1][i]; //display all reg_no's
                dataGW.Rows[name].Cells[1].Value = list[0][i]; //display all path's
         
            }

           
        }

        //remove all cameras from the list
        private void button2_Click(object sender, EventArgs e)
        {
            //when remove all buttin cklicked - ask is user is sure
            //if user pressed "OK" - delete all data from cam_data table
            if (MessageBox.Show("Delete all cameras from list?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) 
            {
                //show dialog form to enter admin credentials
                //doesn't work yet
                loginDialog frm = new loginDialog(); 
                frm.Show();

                string query = "truncate cam_data;"; //removes all data from selected table
                dbConn.Insert(query); 
                listView1.Items.Clear(); //clear listview from all items
                MessageBox.Show("All cameras removed!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information); //display confirmation message
            }
            else
            {
                // user clicked no
                //do something
            }

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (listView1.Items.Count != 0)
            {
                try
                {
                    //MessageBox.Show(listView1.SelectedItems[0].SubItems[0].Text);
                    string item = listView1.SelectedItems[0].SubItems[0].Text.ToString();
                    MessageBox.Show(item);

                    string query = "DELETE FROM cam_data WHERE cam_name = '" + item + "';";
                    dbConn.Delete(query);
                    listView1.MultiSelect = false;
                    listView1.SelectedItems[0].Remove();

                }

                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                
                }
            }
        } //end of tooltip click


    } //end of imageUpdater : Form

        

       
        
        
    }

