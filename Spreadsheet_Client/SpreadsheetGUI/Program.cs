using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SS
{
    class GuiApplicationContext : ApplicationContext
    {
        private int formCount = 0;
        private static GuiApplicationContext appContext;
       
        private GuiApplicationContext()
        {
            
        }
        public static GuiApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new GuiApplicationContext();
            }
            return appContext;
        }
        public void RunForm(Form form)
        {
            formCount++;
            form.FormClosed += (o, e) => { if (--formCount <= 0)ExitThread(); };
            form.Show();
        }
    }
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GuiApplicationContext appCotext = GuiApplicationContext.getAppContext();
            appCotext.RunForm(new Form1());
            Application.Run(appCotext);
        }
    }
}
