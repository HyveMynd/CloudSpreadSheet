using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SS;
using System.Text.RegularExpressions;
using System.IO;
using SpreadsheetUtilities;
using SpreadsheetModel;
using System.Threading;

namespace SS
{
    /// <summary>
    /// Makes a spreadsheet with menus
    /// </summary>
    public partial class Form1 : Form
    {
        private int numWindows = 0;
        private Spreadsheet mySheet;
        private string myFileName = null;
        private string myVersion = null;
        private string myLength = null;
        private string myName = null;
        private SpreadsheetModel.SSModel myModel;
        private Spreadsheet myopenSheet = null;
        private int myRow2 = 0;
        private int mycol2 = 0;
        private string myval2 = null;
        private int myRow3 = 0;
        private int mycol3 = 0;
        private string myVal3 = null;
        private int myRow4 = 0;
        private int mycol4 = 0;
        private string myVal4 = null;
        private int myRow5 = 0;
        private int mycol5 = 0;
        private string myVal5 = null;
        
        /// <summary>
        /// Creates a new empty spreadsheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            myModel = new SpreadsheetModel.SSModel();
            myModel.CreateOK += ValidSS;
            myModel.CreateFail += InvalidSS;
            myModel.JoinOK += successJoin;
            myModel.JoinFail += failJoin;
            myModel.ChangeOk += successChange;
            myModel.ChangeWait += waitChange;
            myModel.ChangeFail += failChange;
            myModel.UndoOk += successUndo;
            myModel.UndoEnd += endUndo;
            myModel.UndoWait += waitUndo;
            myModel.UndoFail += failUndo;
            myModel.Update += update;
            myModel.SaveOk += successSave;
            myModel.SaveFail += failSave;
            myModel.Error += error;
            myModel.Test += tester;
            myModel.noConnection += connectError;
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new System.Drawing.Size(0, 1);
            
            mySheet = new Spreadsheet(s => true/*Regex.IsMatch(s, "(^([a-z]|[A-Z])\\d+$)")*/, s => s.ToUpper(), "ps6");
            numWindows++;
            int mycol;
            int myRow;
            int colLetter;
            string myVal;
            object content;
            //this.textBox1.Invoke(new Action(()=>textBox1.Clear()));
            textBox1.Clear();
            
            //this.spreadsheetPanel1.Invoke(new Action(()=>spreadsheetPanel1.GetSelection(out mycol1, out myRow1)));
            spreadsheetPanel1.GetSelection(out mycol, out myRow);
                content = mySheet.GetCellContents(GetCellName(mycol, myRow));
                if (content is Formula)
                {
                    //this.textBox1.Invoke(new Action(()=>textBox1.Text = "=" + content.ToString()));
                    textBox1.Text = "=" + content.ToString();
                }
                else
                {
                    //this.textBox1.Invoke(new Action(()=>textBox1.Text = content.ToString()));
                    textBox1.Text = content.ToString();
                }
            //this.spreadsheetPanel1.Invoke(new Action(()=>spreadsheetPanel1.GetValue(mycol1, myRow1, out myal1)));
                spreadsheetPanel1.GetValue(mycol, myRow, out myVal);
                colLetter = mycol + 65;
            //this.textBox2.Invoke(new Action(()=>textBox2.Text = ((char)colLetter).ToString() + (myRow1 + 1).ToString() + "= " + myval1));
            textBox2.Text=((char)colLetter).ToString()+(myRow+1).ToString()+"= "+myVal;
            //this.Invoke(new Action(()=>this.Text = "Spreadsheet"));
            this.Text = "Spreadsheet";
        }
       
        
        /// <summary>
        /// When save is clicked saves the current spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myModel.Save(myName);
            
        }
        
        /// <summary>
        /// When close is clicked closes current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myModel.Leave(myName);
            this.Close();
            Application.Exit();
        }
        
       
        /// <summary>
        /// Detects when user changes the selection and populates the Text boxes
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SS.SpreadsheetPanel sender)
        {
            //int mycol;
            //int myRow;
            int colLetter;
            //string myVal;
            object content;
            try
            {
               //this.textBox1.Invoke(new Action(()=>this.textBox1.Clear()));
                //textBox1.Clear();
               this.textBox1.Clear();
                this.spreadsheetPanel1.Invoke(new Action(()=>
                spreadsheetPanel1.GetSelection(out mycol2, out myRow2)));
                content = mySheet.GetCellContents(GetCellName(mycol2, myRow2));
                if (content is Formula)
                {
                    //this.textBox1.Invoke(new Action(()=>textBox1.Text = "=" + content.ToString()));
                    this.textBox1.Text = "=" + content.ToString();
                }
                else
                {
                    //this.textBox1.Invoke(new Action(()=>textBox1.Text = content.ToString()));
                    this.textBox1.Text = content.ToString();
                }
                this.spreadsheetPanel1.Invoke(new Action(()=>
                spreadsheetPanel1.GetValue(mycol2, myRow2, out myval2)));

                colLetter = mycol2 + 65;
                this.textBox2.Invoke(new Action(()=>
                textBox2.Text = ((char)colLetter).ToString() + (myRow2 + 1).ToString() + "= " + myval2));
               
            }
            catch (CircularException)
            {
                MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR FROM SPREADSHEET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Takes in the cell name and returns the appropriate rows and coloumns
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void GetRowColoumn(string cellName, out int col, out int row)
        {
            int myRow=999;
            int myCol = 999;
            foreach (string t in Regex.Split(cellName.ToUpper(), "^([a-z]|[A-Z])"))
            {
                
                if (t.Length > 0 && t.Length < 2)
                {
                    myCol=((int)((char)cellName.ToUpper()[0]) - 65);
                }
                if (Int32.TryParse(t, out myRow))
                {
                    row=myRow;
                }
            }
            col = myCol;
            row = myRow-1;
        }
        /// <summary>
        /// Takes in a column and row and returns a cell name
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetCellName(int col, int row)
        {
            char letter;
            letter = ((char)('A' + col));
            string cell = letter.ToString() + (row+1).ToString();
            return cell.ToUpper();
        }
        private void checkChange(string cellname, string content1)
    {
        int chcol;
        int chRow;
        object content;
        object value;
        string sendContent;
        string length;
        try
        {

            foreach (string s in mySheet.SetContentsOfCell(cellname, content1))
            {
                GetRowColoumn(s, out chcol, out chRow);
                this.spreadsheetPanel1.Invoke(new Action(() =>
                spreadsheetPanel1.SetValue(chcol, chRow, mySheet.GetCellValue(s).ToString())));
            }
            content = mySheet.GetCellContents(GetCellName(mycol3, myRow3));

            value = mySheet.GetCellValue(GetCellName(mycol3, myRow3));
            this.spreadsheetPanel1.Invoke(new Action(() =>
                spreadsheetPanel1.SetValue(mycol3, myRow3, value.ToString())));
            this.textBox2.Invoke(new Action(() =>
                textBox2.Text = (cellname + "= " + value.ToString())));
            if (content is Formula)
            {
                this.textBox1.Invoke(new Action(() =>
                textBox1.Text = "=" + content.ToString()));
                sendContent = "=" + content.ToString();
            }
            else
            {
                this.textBox1.Invoke(new Action(() =>
                textBox1.Text = content.ToString()));
                sendContent = content.ToString();
            }
            if (mySheet.Changed && !this.Text.EndsWith("*"))
            {
                this.Invoke(new Action(() =>
                this.Text = this.Text + "*"));
            }
            if (!mySheet.Changed)
            {
                this.Invoke(new Action(() =>
                this.Text = this.Text.TrimEnd('*')));
            }

            length = sendContent.Length.ToString();
            myModel.Change(myName, myVersion, GetCellName(mycol3, myRow3), length, sendContent);
        }
        catch (CircularException)
        {
            MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERROR FROM SPREADSHEET", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }




    }
        /// <summary>
        /// Sets cell after enter is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                //int mycol;
                //int myRow;
                //string myVal;
                int colLetter;
                object content;
                object value;
                string sendContent;
                string length;
                
                try
                {
                   
                    int chRow;
                    int chcol;
                    this .spreadsheetPanel1.Invoke(new Action(()=>
                    spreadsheetPanel1.GetSelection(out mycol3, out myRow3)));
                    this.spreadsheetPanel1.Invoke(new Action(()=>
                    spreadsheetPanel1.GetValue(mycol3, myRow3, out myVal3)));
                    colLetter = mycol3 + 65;
                    //string text;
                        //this.textBox1.Invoke(new Action(()=>text=textBox1.Text));
                            foreach (string s in mySheet.SetContentsOfCell(GetCellName(mycol3, myRow3), this.textBox1.Text))
                            {
                                GetRowColoumn(s, out chcol, out chRow);
                                this.spreadsheetPanel1.Invoke(new Action(()=>
                                spreadsheetPanel1.SetValue(chcol, chRow, mySheet.GetCellValue(s).ToString())));
                            }
                        content = mySheet.GetCellContents(GetCellName(mycol3, myRow3));

                        value = mySheet.GetCellValue(GetCellName(mycol3, myRow3));
                    this.spreadsheetPanel1.Invoke(new Action(()=>
                        spreadsheetPanel1.SetValue(mycol3, myRow3, value.ToString())));
                    this.textBox2.Invoke(new Action(()=>
                        textBox2.Text = ((char)colLetter).ToString() + (myRow3 + 1).ToString() + "= " + value.ToString()));
                        if (content is Formula)
                        {
                            this.textBox1.Invoke(new Action(()=>
                            textBox1.Text = "=" + content.ToString()));
                            sendContent = "=" + content.ToString();
                        }
                        else
                        {
                            this.textBox1.Invoke(new Action(()=>
                            textBox1.Text = content.ToString()));
                            sendContent = content.ToString();
                        }
                        if (mySheet.Changed && !this.Text.EndsWith("*"))
                        {
                            this.Invoke(new Action(()=>
                            this.Text = this.Text + "*"));
                        }
                        if (!mySheet.Changed)
                        {
                            this.Invoke(new Action(()=>
                            this.Text = this.Text.TrimEnd('*')));
                        }
                        length = sendContent.Length.ToString();
                        myModel.Change(myName, myVersion, GetCellName(mycol3, myRow3), length, sendContent);

                }
                catch (CircularException)
                {
                    MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR FROM SPREADSHEET", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                }
            }
        }
        
        /// <summary>
        /// Sets cell value when button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
           // int mycol;
            //int myRow;
            //string myVal;
            int colLetter;
            object content;
            object value;
            string sendContent;
            string length;
            this.spreadsheetPanel1.Invoke(new Action(()=>
            spreadsheetPanel1.GetSelection(out mycol4, out myRow4)));
            try
            {
                
                
                int chRow;
                int chcol;
                //bool fail = false;
                this.spreadsheetPanel1.Invoke(new Action(()=>
                spreadsheetPanel1.GetValue(mycol4, myRow4, out myVal4)));
                colLetter = mycol4 + 65;

                 
                        foreach (string s in mySheet.SetContentsOfCell(GetCellName(mycol4, myRow4), this.textBox1.Text))
                        {
                            GetRowColoumn(s, out chcol, out chRow);
                            this.spreadsheetPanel1.Invoke(new Action(()=>
                            spreadsheetPanel1.SetValue(chcol, chRow, mySheet.GetCellValue(s).ToString())));
                        }

                    content = mySheet.GetCellContents(GetCellName(mycol4, myRow4));
                    
                    value = mySheet.GetCellValue(GetCellName(mycol4, myRow4));
                this.textBox2.Invoke(new Action(()=>
                    textBox2.Text = ((char)colLetter).ToString() + (myRow4 + 1).ToString() + "= " + value));
                    spreadsheetPanel1.SetValue(mycol4, myRow4, value.ToString());
                   

                    if (content is Formula)
                    {
                        this.textBox1.Invoke(new Action(()=>
                        textBox1.Text = "=" + content.ToString()));
                        sendContent = "=" + content.ToString();
                    }
                    else
                    {
                        this.textBox1.Invoke(new Action(()=>
                        textBox1.Text = content.ToString()));
                        sendContent = content.ToString();
                    }
                    if (mySheet.Changed && !this.Text.EndsWith("*"))
                    {
                        this.Invoke(new Action(()=>
                        this.Text = this.Text + "*"));
                    }
                    if (!mySheet.Changed)
                    {
                        this.Invoke(new Action(()=>
                        this.Text = this.Text.TrimEnd('*')));
                    }
                    length = sendContent.Length.ToString();
                    myModel.Change(myName, myVersion, GetCellName(mycol4, myRow4), length, sendContent);

            }
            catch (CircularException)
            {
                MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR FROM SPREEDSHEET", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
               
            }

        }
        /// <summary>
        /// Tells the user how to use the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select cell with mouse" + "\r\n" + "Enter a word number or formula in box labled Contents Of Cell" + "\r\n"
                + "formula must start with = or it is treated as a word" + "\r\n" + "After you have typed your content press enter key or Enter Content button"
                + "\r\n" + "Open file with File menu and Open" + "\r\n" + "Save file with File menu and Save or Save As" + "\r\n"
                + "Open new blank window with File menu and new"
                + "\r\n" + "Close current window with File menu and Close" + "\r\n" + "Close all windows with File menu and Exit"
                + "\r\n" + "Get help form help menu", "Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
                
        }
        private void tester(string test)
        {
            testTexBox.Invoke(new Action(() => testTexBox.Text = test));
        }
        private void connectError(string error)
        {
            MessageBox.Show("ERROR NOT CONNECTED", error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ValidSS(string creds)
        {
            
        }
        private void InvalidSS(string name, string message)
        {
            MessageBox.Show(message, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void successJoin(string name,string version,string length, string filename, string xml)
        {
            myVersion = version;
            myLength = length;
            myName = name;
            
            myopenSheet = new Spreadsheet(filename, s => true, s => s.ToUpper(), "ps6");
            //this.Invoke(new Action(()=>OpenNew(myopenSheet,name)));
                //this.textBox1.Invoke(new Action(()=>textBox1.Clear()));
            OpenNew(myopenSheet, name);
            //GuiApplicationContext.getAppContext().RunForm(new Form1(myopenSheet, name));
            //MessageBox.Show(xml, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.tabControl1.Invoke(new Action(()=>tabControl1.SelectedIndex=1));
            
            
        }
        private void failJoin(string name, string message)
        {
            MessageBox.Show(message, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            
        }
        private void successChange(string name, string version)
        {
            myName = name;
            myVersion = version;
            
        }
        private void waitChange(string name, string version, string cellName, string cellcontent)
        {
            
           
            if(myVersion==version)
            {
                checkChange(cellName, cellcontent);
            }
           
        }
        private void failChange(string name, string message)
        {
            MessageBox.Show(message, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }
        private void successUndo(string name, string version, string cell, string length, string content)
        {
            myName = name;
            myVersion = version;
            myLength = length;
            changeCell(name, version, cell, length, content);
            this.textBox1.Invoke(new Action(()=>textBox1.Clear()));
            
            
        }
        private void endUndo(string name, string version)
        {
            MessageBox.Show("Nothing to Undo", name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            myName = name;
            myVersion = version;
        }
        private void failUndo(string name,string message)
        {
            MessageBox.Show(message, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void waitUndo(string name, string version, string cellName, string cellcontent)
        {

            if (myVersion == version)
            {
                checkChange(cellName, cellcontent);
            }
           
        }
        private void update(string name,string version, string cell, string length, string content)
        {
           
            changeCell(name, version, cell, length, content);
        }
        private void successSave(string name)
        {
            MessageBox.Show(name+" Saved", name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void failSave(string name, string message)
        {
            MessageBox.Show(message, name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void error(string error)
        {
            MessageBox.Show(error, error, MessageBoxButtons.OK, MessageBoxIcon.Information);
           
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int port = 0;
            if (Int32.TryParse(editPort.Text, out port))
            {
                myModel.Connect(editHost.Text, port);
            }
            
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (editFilename.Text != null && editPasswd.Text != null)
            {
                myModel.Create(editFilename.Text, editPasswd.Text);
            }
            else
            {
                 MessageBox.Show("Filename and Password cannot be blank", "Missing filename or password", MessageBoxButtons.OK, MessageBoxIcon.Error);
           
            }
        }

        private void JoinButton_Click(object sender, EventArgs e)
        {
            if (editFilename.Text != null && editPasswd.Text != null)
            {
                myModel.Join(editFilename.Text, editPasswd.Text);
            }
            else
            {
                MessageBox.Show("Filename and Password cannot be blank", "Missing filename or password", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myModel.Undo(myName, myVersion);
        }
        private void changeCell(string name, string version, string cell, string length, string content)
        {
           
            int mycol;
            int myRow;
            
            object cellContent;
            object value;

            try
            {


                int chRow;
                int chcol;
               
                foreach (string s in mySheet.SetContentsOfCell(cell, content))
                {
                    GetRowColoumn(s, out chcol, out chRow);
                    this.spreadsheetPanel1.Invoke(new Action(()=>
                    spreadsheetPanel1.SetValue(chcol, chRow, mySheet.GetCellValue(s).ToString())));
                }

                cellContent = mySheet.GetCellContents(cell);

                value = mySheet.GetCellValue(cell);
                GetRowColoumn(cell, out mycol, out myRow);
                this.spreadsheetPanel1.Invoke(new Action(()=>
                spreadsheetPanel1.SetValue(mycol, myRow, value.ToString())));

            }
            catch (CircularException)
            {
                MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR FROM SPREADSHEET", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeToolStripMenuItem_Click(sender, e);
            
        }
        private void OpenNew(Spreadsheet openSheet, string filename)
        {

           
            mySheet = openSheet;
            


            foreach (string s in openSheet.GetNamesOfAllNonemptyCells())
            {
                int myCol;
                int myRow;
                GetRowColoumn(s, out myCol, out myRow);
                if (myCol <= 25 | myRow <= 99)
                {

                    this.spreadsheetPanel1.Invoke(new Action(()=>
                    spreadsheetPanel1.SetValue(myCol, myRow, mySheet.GetCellValue(s).ToString())));
                }
            }
            
            int colLetter;
            object content;
            this.textBox1.Invoke(new Action(() => textBox1.Clear()));
            this.spreadsheetPanel1.Invoke(new Action(()=>
            spreadsheetPanel1.GetSelection(out mycol5, out myRow5)));
            content = mySheet.GetCellContents(GetCellName(mycol5, myRow5));
            if (content is Formula)
            {
                this.textBox1.Invoke(new Action(()=>
                textBox1.Text = "=" + content.ToString()));
            }
            else
            {
                this.textBox1.Invoke(new Action(()=>
                textBox1.Text = content.ToString()));
            }
            spreadsheetPanel1.GetValue(mycol5, myRow5, out myVal5);

            colLetter = mycol5 + 65;
            this.textBox2.Invoke(new Action(()=>
            textBox2.Text = ((char)colLetter).ToString() + (myRow5 + 1).ToString() + "= " + myVal5));
            myFileName = filename;
            this.Invoke(new Action(() => this.Text = filename));
        }
    }
}
