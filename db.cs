using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GriffithElder.Database.MySql;
using GriffithElder.Utils;
using MySql.Data.MySqlClient;
using System.Windows.Forms;


namespace imageUpdaterV2._0
{
    class db
    {
        public MySqlConnection _connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        string id;
        //Constructor
        public db()
        {
            server_setup();
        }

        //Initialize values
        public void server_setup()
        {
            /* server name; either hostname or IP address */
            //server = "192.168.16.1"; 
            //server = "192.168.16.122";
            server = "localhost";
            /********************************************/

            /****** database name ***********************/
            //database = "cameras";
            database = "anpr";
            //database = "gecam";
            /*******************************************/

            /******** database username ****************/
            //uid = "";
            // uid = ""; //allows remote connection
            uid = "root";//only allows connections from local machine

            /************ database password ***************/
            //password = "";
            password = "";
            //password = "";
            /*********************************************/

            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            _connection = new MySqlConnection(connectionString);
        }


        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                _connection.Open();
                Console.WriteLine("connected");
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }//end of OpenConnection

        //Close connection
        public bool CloseConnection()
        {  
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        } //end of CloseConnection

        //Insert statement
        public void Insert(string query)
        {
            //open connection
            //if (this.OpenConnection() == true)
            try
            {
               
                if (this.OpenConnection() == true)
                {
                    
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Executed");
                    this.CloseConnection();
                    

                }
                else {

                    _connection.Open();

                    MySqlCommand cmd = new MySqlCommand(query, _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Executed");
                    this.CloseConnection();
                    _connection.Close();
                
                }
            }

            catch (MySqlException ex)
            {
                //Log.WriteLog(LogType.Database, ex);
                
                //1062 error for duplicate primary key
                 if (ex.Number == 1062)
                {

                }

                 throw ex;
                
            }

            finally
            {
                //close connection
                this.CloseConnection();
                _connection.Close();
            } 


        }//end of insert method

        //Update statement
        public void Update(string query)
        {

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = _connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
                _connection.Close();
            }
        }// end of update method


        public List<string>[] Select()
        {
            //string query = "SELECT id, date_time, reg_no, img_path, cam_no FROM database_name.images";
            string query = "SELECT * FROM cam_data";
            //Create a list to store the result
            //create a new list for every column
            List<string>[] list = new List<string>[2];
            list[0] = new List<string>();
            list[1] = new List<string>();
            

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["cam_name"] + "");
                    list[1].Add(dataReader["img_dir"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();
                _connection.Close();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string>[] RegNoSelect()
        {
            //string query = "SELECT id, date_time, reg_no, img_path, cam_no FROM database_name.images";
            string query = "SELECT * FROM images";
            //Create a list to store the result
            //create a new list for every column
            List<string>[] list1 = new List<string>[2];
            list1[0] = new List<string>();
            list1[1] = new List<string>();


            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    list1[0].Add(dataReader["date_time"] + "");
                    list1[1].Add(dataReader["reg_no"] + "");
                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();
                _connection.Close();

                //return list to be displayed
                return list1;
            }
            else
            {
                return list1;
            }
        }

        public void Delete(string query)
        {

            //Open connection
            if (this.OpenConnection() == true)
            {
                
                    MySqlCommand cmd = new MySqlCommand(query, _connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                    _connection.Close();
              
            }
        }// end of delete method

        public void get_cam_names()  
        {

            string query = "select * from cam_data";
            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = _connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                //this.CloseConnection();
                _connection.Close();
               

            }
        } //end of get_cam_name


        public string index()
        {
            //open connection
            //if (this.OpenConnection() == true)
            try
            {
                //open connection
                _connection.Open();

                string query = "SELECT * FROM cam_data ORDER BY cam_no DESC LIMIT 1";
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, _connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                //cmd.ExecuteNonQuery();
                

                while (reader.Read())
                {           
                    id = reader["cam_no"].ToString();
                }

                //Execute command
                reader.Close();
                Console.WriteLine("Executed");
                _connection.Close();
                //this.CloseConnection();

                return id; 
               
            }

            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;

            }

            finally
            {
                //close connection
                this.CloseConnection();
            }
           

        }//end of insert method


    } ///end of db class
    ///

}///end of namespace imageUpdaterV1._0
