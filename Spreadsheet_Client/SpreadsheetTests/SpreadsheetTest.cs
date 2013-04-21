using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    
    
    /// <summary>
    ///This is a test class for SpreadsheetTest and is intended
    ///to contain all SpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpreadsheetTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Spreadsheet Constructor
        ///</summary>
        [TestMethod()]
        public void SpreadsheetConstructorTest()
        {
            string myPath = string.Empty; // TODO: Initialize to an appropriate value
            Func<string, bool> myValid = null; // TODO: Initialize to an appropriate value
            Func<string, string> myNormalize = null; // TODO: Initialize to an appropriate value
            string myVersion = string.Empty; // TODO: Initialize to an appropriate value
            Spreadsheet target = new Spreadsheet(myPath, myValid, myNormalize, myVersion);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Spreadsheet Constructor
        ///</summary>
        [TestMethod()]
        public void SpreadsheetConstructorTest1()
        {
            Func<string, bool> myValid = s=>true; 
            Func<string, string> myNormalize = s=>s;
            string myVersion = "version1";
            Spreadsheet target = new Spreadsheet(myValid, myNormalize, myVersion);
        }

        /// <summary>
        ///A test for Spreadsheet Constructor
        ///</summary>
        [TestMethod()]
        public void SpreadsheetConstructorTest2()
        {
            Spreadsheet target = new Spreadsheet();
           // Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetCellContents
        ///</summary>
        [TestMethod()]
        public void GetCellContentsTest()
        {
            Spreadsheet target = new Spreadsheet();
            Formula myformula=new Formula("a1+2",s=>true,s=>s);
            target.SetContentsOfCell("b1", "=a1+2");
            string name = "b1"; 
            object expected = myformula; 
            object actual;
            actual = target.GetCellContents(name);
            Assert.AreEqual(expected, actual);
           
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsNullName()
        {
            Spreadsheet target = new Spreadsheet();
            target.GetCellContents(null);
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsBadName()
        {
            Spreadsheet target = new Spreadsheet();
            target.GetCellContents("a");
        }
        [TestMethod()]
        public void GetCellContentsNameNotinFile()
        {
            Spreadsheet target = new Spreadsheet();
            target.GetCellContents("a1");
        }

        /// <summary>
        ///A test for GetCellValue
        ///</summary>
        [TestMethod()]
        public void GetCellValueTest()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1", "2");
            double mydouble = 2;
            string name = "a1"; 
            object expected = mydouble; 
            object actual;
            actual = target.GetCellValue(name);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetNamesOfAllNonemptyCells
        ///</summary>
        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest()
        {
            Spreadsheet target = new Spreadsheet();
            List<string> myList = new List<string>();
            List<string> myactual = new List<string>();
            myList.Add("a1");
            myList.Add("a2");
            myList.Add("b2");
            myList.Add("c2"); 
            target.SetContentsOfCell("a1", "2");
            target.SetContentsOfCell("a2", "Total");
            target.SetContentsOfCell("b2", "3");
            target.SetContentsOfCell("c2", "=a1+b2");
            IEnumerable<string> expected = myList; 
            IEnumerable<string> actual;
            actual = target.GetNamesOfAllNonemptyCells();
            foreach (string s in actual)
            {
                myactual.Add(s);
            }
            for (int i = 0; i < myList.Count - 1; i++)
            {
                Assert.AreEqual(myList[i], myactual[i]);
            }
            
        }

        /// <summary>
        ///A test for GetSavedVersion
        ///</summary>
        [TestMethod()]
        public void GetSavedVersionTest()
        {
            Spreadsheet target = new Spreadsheet(); // TODO: Initialize to an appropriate value
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetSavedVersion(filename);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            Spreadsheet target = new Spreadsheet(); // TODO: Initialize to an appropriate value
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            target.Save(filename);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SetContentsOfCell
        ///</summary>
        [TestMethod()]
        public void SetContentsOfCellTest()
        {
            List<string> myexpected = new List<string>();
            List<string> myactual = new List<string>();
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1", "2");
            target.SetContentsOfCell("b1", "2");
            target.SetContentsOfCell("c1", "total");
            //target.SetContentsOfCell("d1", "=a1+b1");
            string name = "d1";
            string content = "=a1+b1";
            myexpected.Add("d1");
            int count = myexpected.Count;
            //ISet<string> expected = null; 
            ISet<string> actual;
            actual = target.SetContentsOfCell(name, content);
            foreach (string s in actual)
            {
                myactual.Add(s);
            }
            
            for (int i = 0; i < count-1 ; i++)
            {
                Assert.AreEqual(myexpected[i], myactual[i]);
            }
        }

        /// <summary>
        ///A test for myEvaluate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Spreadsheet.dll")]
        public void myEvaluateTest()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor(); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.myEvaluate(name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for myLookup
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("Spreadsheet.dll")]
        public void myLookupTest()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor(); // TODO: Initialize to an appropriate value
            string name = "a1"; // TODO: Initialize to an appropriate value
           // double expected = 0F; // TODO: Initialize to an appropriate value
            double actual;
            actual = target.myLookup(name);
        }
        
        [TestMethod()]
        [ExpectedException(typeof (InvalidNameException))]
        public void GetCellContentsExceptionTest()
        {
            Spreadsheet target = new Spreadsheet();
            target.GetCellContents("a");
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsNullTest()
        {
            Spreadsheet target = new Spreadsheet();
            string myName = null;
            target.GetCellContents(myName);
        }

       
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectDependentsNullTest()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor();
            string name=null;
            target.GetDirectDependents(name);
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependentsInvalidName()
        {
            Spreadsheet_Accessor target = new Spreadsheet_Accessor();
            target.GetDirectDependents("a0a");
        }
    

        
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleExceptionTest()
        {
            Spreadsheet target = new Spreadsheet();
            string name = null;
            target.SetContentsOfCell(name, "56");
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsInvalidnameDouble()
        {
            Spreadsheet target = new Spreadsheet();
            string name = "a";
            target.SetContentsOfCell(name, "46");
        }


       
        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void SetCellContentsFormulaNull()
        {
            Spreadsheet target = new Spreadsheet();
            Formula formula = new Formula("a1+3", s => true, s => s);
            formula = null;
            target.SetContentsOfCell("a7", formula.ToString());
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaInvalidName()
        {
            Spreadsheet target = new Spreadsheet();
            Formula formula = new Formula("i4+78-98.7", s => true, s => s);
            target.SetContentsOfCell("a0a", formula.ToString());
        }
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsCircularException()
        {
            Spreadsheet target = new Spreadsheet();
            Formula formula = new Formula("b1+b1", s => true, s => s);
            target.SetContentsOfCell("b1", "=b1+b1");
            formula=new Formula("b1+b1",s=>true,s=>s);
            target.SetContentsOfCell("a1", formula.ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsTextNull()
        {
            Spreadsheet target = new Spreadsheet();
            string text = null;
            target.SetContentsOfCell("a4", text);
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTextNameError()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("f", "Total");
        }

    
    }
}
