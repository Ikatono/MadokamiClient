using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MadokamiDownloader
{
    public partial class MainWindow : Form
    {
        const string baseURL = "https://manga.madokami.al";
        Dictionary<string, string[]> pagetopage = new Dictionary<string, string[]>();
        HttpClientHandler Handler = new HttpClientHandler();
        public MainWindow()
        {
            InitializeComponent();
            
        }

        

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
