using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SS.NetworkController;
using System.Threading;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    /// This class handles communication with the server
    /// Including formatting messages to the server
    /// and recieving and parsing data from the server
    /// </summary>
    public class Controller
    {
        public delegate void FileListRecievedHandler(List<String> files);
        public event FileListRecievedHandler fileListRecieved;

        //public delegate void CloseOpenFormsHandler();
        //public event CloseOpenFormsHandler CloseAllOpenForms;

        //could hold the spreadsheet?
        //public WorldModel world
        //{
        //    get;

        //    protected set;
        //}

        public string userName
        {
            get;

            protected set;
        }

        //private int playerID = -1;
        private int clientID = -1;
        private Dictionary<int, Form1> openSheets;
        private string filename = "";
        //private int width = -1;
        //private int height = -1;
        public bool ssReady
        {
            get;
            protected set;
        }

        private SocketState state;
        public object ssLock = new object();

        /// <summary>
        /// create a new instance of game control
        /// </summary>
        /// <param name="name">player name</param>
        /// <param name="serverAddress">server address</param>
        public Controller(String username, String serverAddress)
        {
            //Reasoning: We only need the control when we connect to a server, so we can just create a new control object when connecting to a server
            //And each unique connection needs a new instance of a worldmodel anyways, so just create it with each Control object
            this.userName = username;
            ssReady = false;

            state = Networking.ConnectToServer(ConnectionEstablished, serverAddress);
        }

        public void requestFileList()
        {
            Networking.Send(state.theSocket, "0\n");
        }

        /// <summary>
        /// send a command to the server
        /// </summary>
        /// <param name="data">commmand to be sent</param>
        public void SendCommand(string data)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////
            //This is going to get changed a lot, possibly seperated into different functions
            Networking.Send(state.theSocket, data);
        }

        /// <summary>
        /// Connection has been established
        /// </summary>
        /// <param name="ss">Current Socket State</param>
        private void ConnectionEstablished(SocketState ss)
        {
            ss.handleConnection = Initialization;
        }

        /// <summary>
        /// initializese the world object when information first starts coming from the server
        /// </summary>
        /// <param name="ss">active socket state</param>
        private void Initialization(SocketState ss)
        {
            string IDMessage = ss.sb.ToString();
            IDMessage.Trim();
            Int32.TryParse(IDMessage, out clientID);
            openSheets = new Dictionary<int, Form1>();
            ss.sb.Remove(0, IDMessage.Length);
            ss.handleConnection = ReceiveSSData;

            //Send the player name here
            Networking.Send(ss.theSocket, userName + "\n");

            ssReady = true;
        }

        /// <summary>
        /// handles incoming game data from the server
        /// </summary>
        /// <param name="ss">current socket state</param>
        private void ReceiveSSData(SocketState ss)
        {
            string totalData = ss.sb.ToString();
            string[] parts = Regex.Split(totalData, @"(\n)");
            int numberOfValidMessages = parts.Length;
            if (totalData[totalData.Length - 1] != '\n')
            {
                numberOfValidMessages--;
            }

            //if the last 
            lock (ssLock)
            {
                for(int i = 0; i < parts.Length;i++)
                {
                    string p = parts[i];
                    // Ignore empty strings added by the regex splitter
                    if (p.Length == 0)
                        continue;
                    // The regex splitter will include the last string even if it doesn't end with a '\n',
                    // So we need to ignore it if this happens. 
                    string[] messageParts = Regex.Split(p, @"\t");
                    if (messageParts[0] == "0")
                    {
                        handleReceiveFileList(p);
                    }
                    else if (messageParts[0] == "1" || messageParts[0] == "2")
                    {
                        createNewSpreadsheet(int.Parse(messageParts[1]));
                    }
                    else if (messageParts[0] == "3")
                    {
                        //cell update
                        int id = int.Parse(messageParts[1]); 
                        openSheets[id].recieveSSEdit(messageParts[2], messageParts[3]);
                    }
                    else if (messageParts[0] == "4")
                    {
                        //valid edit
                        //does this even need to do anything?
                        //i dont think so
                    }
                    else if (messageParts[0] == "5")
                    {
                        //invalid edit
                        //need to tell the user they fucked up
                        MessageBox.Show("Invalid edit sent to server, you likely sent a formula that caused a circular exception. Your edit has been rejected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (messageParts[0] == "6")
                    {
                        //rename
                        //need to see about making things propagate to the server
                        int id = int.Parse(messageParts[1]);
                        openSheets[id].ChangeName(messageParts[2]);
                    }
                    else if (messageParts[0] == "7")
                    {
                        //save
                        //same as above
                        int id = int.Parse(messageParts[1]);
                        openSheets[id].showSaved();
                        
                    }
                    else if (messageParts[0] == "8")
                    {
                        //Valid rename
                        //probably also doesnt need to do anything
                    }
                    else if (messageParts[0] == "9")
                    {
                        //invalid rename
                        //also tell the user they fucked up
                        MessageBox.Show("Invalid name sent to the server. The name already exist. Your renaming has been rejected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    }
                    else if (messageParts[0] == "A")
                    {
                        //who's editing what
                        //need to see about how we're going to do this
                        int id = int.Parse(messageParts[1]);
                        string cell = messageParts[2];
                        int userId = int.Parse(messageParts[3]);
                        string name = messageParts[4];
                        //issue with threads possibly, will have to see about this
                        openSheets[id].ShowOthers(cell, userId, name);
                    }
                    // Then remove it from the SocketState's growable buffer
                    ss.sb.Remove(0, p.Length);
                    string currentData = ss.sb.ToString();
                }
                string currentData2 = ss.sb.ToString();
            }
        }

        private void createNewSpreadsheet(int DocID)
        {
            //CloseAllOpenForms();
            
            
            Thread theSpreadsheet = new Thread(() =>
            {
                Form1 newSSWindow = new Form1(this, DocID);
                openSheets.Add(DocID, newSSWindow);
                newSSWindow.ShowDialog();
                
            });
            theSpreadsheet.Start();

        }
        
        public void handleReceiveFileList(string fileListMessage)
        {
            string[] parts = Regex.Split(fileListMessage, @"\t");
            List<string> files = new List<string>();
            for (int i = 1; i < parts.Length; i++)
            {
                files.Add(parts[i].Trim());
            }
            if (fileListRecieved != null)
            {
                fileListRecieved(files);
            }
        }
    }
}