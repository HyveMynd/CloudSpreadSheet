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

namespace SS
{
    /// <summary>
    /// Makes a spreadsheet with menus
    /// </summary>
    public partial class Form1 : Form
    {
        private int numWindows = 0;
        private Spreadsheet mySheet;
        private bool saveOnClose = false;
        private bool closeNow = false;
        private string myFileName = null;
        /// <summary>
        /// Creates a new empty spreadsheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            mySheet = new Spreadsheet(s => true/*Regex.IsMatch(s, "(^([a-z]|[A-Z])\\d+$)")*/, s => s.ToUpper(), "ps6");
            numWindows++;
            int mycol;
            int myRow;
            int colLetter;
            string myVal;
            object content;
                textBox1.Clear();
                spreadsheetPanel1.GetSelection(out mycol, out myRow);
                content = mySheet.GetCellContents(GetCellName(mycol, myRow));
                if (content is Formula)
                {
                    textBox1.Text = "=" + content.ToString();
                }
                else
                {
                    textBox1.Text = content.ToString();
                }
                spreadsheetPanel1.GetValue(mycol, myRow, out myVal);

                colLetter = mycol + 65;
                textBox2.Text = ((char)colLetter).ToString() + (myRow + 1).ToString() + "= " + myVal;
            this.Text = "Spreadsheet";
        }
        /// <summary>
        /// Creates a spreadsheet that is populated from a file
        /// </summary>
        /// <param name="openSheet"></param>
        /// <param name="filename"></param>
        public Form1(Spreadsheet openSheet,string filename)
        {
            
            InitializeComponent();
            numWindows++;
            mySheet = openSheet;

            foreach (string s in openSheet.GetNamesOfAllNonemptyCells())
            {
                int myCol;
                int myRow;
                GetRowColoumn(s, out myCol, out myRow);
                if (myCol <= 25 | myRow <= 99)
                {
                    

                    spreadsheetPanel1.SetValue(myCol, myRow, mySheet.GetCellValue(s).ToString());
                }
            }
            int mycol;
            int myrow;
            int colLetter;
            string myVal;
            object content;
          
                textBox1.Clear();
                spreadsheetPanel1.GetSelection(out mycol, out myrow);
                content = mySheet.GetCellContents(GetCellName(mycol, myrow));
                if (content is Formula)
                {
                    textBox1.Text = "=" + content.ToString();
                }
                else
                {
                    textBox1.Text = content.ToString();
                }
                spreadsheetPanel1.GetValue(mycol, myrow, out myVal);

                colLetter = mycol + 65;
                textBox2.Text = ((char)colLetter).ToString() + (myrow + 1).ToString() + "= " + myVal;
            myFileName = filename;
            this.Text = filename;
        }
        

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
          
        }
        /// <summary>
        /// Puts up a save as file dialog box and makes sure the file name is ok
        /// If an overwrite will occor warns the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            try
            {
                   /* if (File.Exists(saveFileDialog1.FileName))
                    {

                        /*if (MessageBox.Show("File exists overwrite?", "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            mySheet.Save(saveFileDialog1.FileName);
                            this.Text = saveFileDialog1.FileName;

                        }
                        else
                        {
                           
                        }
                    }

                else
                {*/
                    mySheet.Save(saveFileDialog1.FileName);
                    this.Text = saveFileDialog1.FileName;
                    myFileName = saveFileDialog1.FileName;
               // }

                if (saveOnClose)
                {
                    closeNow = true;
                    
                    this.Close();
                }
            }
            catch (SpreadsheetReadWriteException ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// When new is clicked creates a new empty spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiApplicationContext.getAppContext().RunForm(new Form1());
            
            numWindows++;
            this.Text = "Spreadsheet";
        }
        /// <summary>
        /// When save is clicked saves the current spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (myFileName != null && File.Exists(myFileName))
            {
                mySheet.Save(saveFileDialog1.FileName);
                this.Text = this.Text.TrimEnd('*');
            }
            else
            {
                saveFileDialog1.ShowDialog();
            }
        }
        /// <summary>
        /// When open is clicked opens a new spreadsheet from a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.ShowDialog();
                Spreadsheet myopenSheet = new Spreadsheet(openFileDialog1.FileName, s => true, s => s.ToUpper(), "ps6");
                GuiApplicationContext.getAppContext().RunForm(new Form1(myopenSheet, openFileDialog1.FileName.ToString()));
                numWindows++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// When close is clicked closes current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            this.Close();
        }
        /// <summary>
        /// When exit is clicked asks user to close all windows then closes based on users choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numWindows >= 1)
            {
                DialogResult myClose = MessageBox.Show("Close all Windows?", "Close Windows", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (myClose == DialogResult.Yes)
                {
                    Application.Exit();
                    saveOnClose = true;
                    /*try
                    {

                        Application.Exit();
                    }
                    catch (InvalidOperationException)
                    {
                    }*/
                    
                }
                if (myClose == DialogResult.No)
                {
                   // Application.Exit();
                    //closeNow = true;
                }
            }
            else
            {
                saveOnClose = true;
                Application.Exit();
            }
        }
        /// <summary>
        /// When a window starts to close figures out if that window needs to be saved and prompts the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closeNow)
            {
              
                if (mySheet.Changed)
                {
                    
                    this.Focus();
                    DialogResult myResult=MessageBox.Show("Save Changes?", "Spreadsheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if  (myResult==DialogResult.Yes)
                    {
                        saveToolStripMenuItem_Click(sender, e);
                        this.Hide();
                    }
                    if (myResult == DialogResult.No)
                    {
                        e.Cancel = false;
                        this.Hide();
                        /*if (exitAll)
                        {
                            exitAll = false;
                            this.Close();

                        }*/
                    }
                }
            }
            else if (closeNow)
            {
                e.Cancel = false;
                saveOnClose = false;
                closeNow = false;
                this.Close();
                numWindows--;
            }
        }
        /// <summary>
        /// Detects when user changes the selection and populates the Text boxes
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SS.SpreadsheetPanel sender)
        {
            int mycol;
            int myRow;
            int colLetter;
            string myVal;
            object content;
            try
            {

                textBox1.Clear();
                spreadsheetPanel1.GetSelection(out mycol, out myRow);
                content = mySheet.GetCellContents(GetCellName(mycol, myRow));
                if (content is Formula)
                {
                    textBox1.Text = "=" + content.ToString();
                }
                else
                {
                    textBox1.Text = content.ToString();
                }
                spreadsheetPanel1.GetValue(mycol, myRow, out myVal);

                colLetter = mycol + 65;
                textBox2.Text = ((char)colLetter).ToString() + (myRow + 1).ToString() + "= " + myVal;
               
            }
            catch (CircularException)
            {
                MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                int mycol;
                int myRow;
                string myVal;
                int colLetter;
                object content;
                object value;
                
                try
                {
                   
                    int chRow;
                    int chcol;
                   // bool fail=false;
                    spreadsheetPanel1.GetSelection(out mycol, out myRow);
                    spreadsheetPanel1.GetValue(mycol, myRow, out myVal);
                    colLetter = mycol + 65;
                    
                            foreach (string s in mySheet.SetContentsOfCell(GetCellName(mycol, myRow), textBox1.Text))
                            {
                                GetRowColoumn(s, out chcol, out chRow);

                                spreadsheetPanel1.SetValue(chcol, chRow, mySheet.GetCellValue(s).ToString());
                            }
                      //  }
                        content = mySheet.GetCellContents(GetCellName(mycol, myRow));

                        value = mySheet.GetCellValue(GetCellName(mycol, myRow));
                        spreadsheetPanel1.SetValue(mycol, myRow, value.ToString());
                        textBox2.Text = ((char)colLetter).ToString() + (myRow + 1).ToString() + "= " + value.ToString();
                        if (content is Formula)
                        {
                            textBox1.Text = "=" + content.ToString();
                        }
                        else
                        {
                            textBox1.Text = content.ToString();
                        }
                        if (mySheet.Changed && !this.Text.EndsWith("*"))
                        {
                            this.Text = this.Text + "*";
                        }
                        if (!mySheet.Changed)
                        {
                            this.Text = this.Text.TrimEnd('*');
                        }
                    
                }
                catch (CircularException)
                {
                    MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                }
            }
        }
        /// <summary>
        /// Saves when save as is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }
        /// <summary>
        /// Sets cell value when button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int mycol;
            int myRow;
            string myVal;
            int colLetter;
            object content;
            object value;
            spreadsheetPanel1.GetSelection(out mycol, out myRow);
            try
            {
                
                
                int chRow;
                int chcol;
                //bool fail = false;
                spreadsheetPanel1.GetValue(mycol, myRow, out myVal);
                colLetter = mycol + 65;

                        foreach (string s in mySheet.SetContentsOfCell(GetCellName(mycol, myRow), textBox1.Text))
                        {
                            GetRowColoumn(s, out chcol, out chRow);

                            spreadsheetPanel1.SetValue(chcol, chRow, mySheet.GetCellValue(s).ToString());
                        }
                   // }
                    content = mySheet.GetCellContents(GetCellName(mycol, myRow));
                    
                    value = mySheet.GetCellValue(GetCellName(mycol, myRow));
                    textBox2.Text = ((char)colLetter).ToString() + (myRow + 1).ToString() + "= " + value;
                    spreadsheetPanel1.SetValue(mycol, myRow, value.ToString());
                    if (content is Formula)
                    {
                        textBox1.Text = "=" + content.ToString();
                    }
                    else
                    {
                        textBox1.Text = content.ToString();
                    }
                    if (mySheet.Changed && !this.Text.EndsWith("*"))
                    {
                        this.Text = this.Text + "*";
                    }
                    if (!mySheet.Changed)
                    {
                        this.Text = this.Text.TrimEnd('*');
                    }
                
            }
            catch (CircularException)
            {
                MessageBox.Show("Cannot use current cell as part of formula", "CircularException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
               
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
        
    }
}
