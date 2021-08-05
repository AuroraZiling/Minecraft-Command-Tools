using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace CommandTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static string current_path = Environment.CurrentDirectory.Replace("\\", "/") + "/";

        public MainWindow()
        {
            InitializeComponent();
            //获取嵌入的updateLog内容 -> About的更新日志
            Assembly updateLogAssembly = Assembly.GetExecutingAssembly();
            Stream updateLogConfigStream = updateLogAssembly.GetManifestResourceStream("CommandTools.updateLog.txt");
            byte[] updateLogTmp = new byte[updateLogConfigStream.Length];
            _ = updateLogConfigStream.Read(updateLogTmp, 0, updateLogTmp.Length);
            string updateLog = Encoding.UTF8.GetString(updateLogTmp);
            AboutUpdateLogText.Document = new FlowDocument(new Paragraph(new Run(updateLog)));
            FlowDocument updateLogDoc = new FlowDocument();
            Paragraph updateLogP = new Paragraph();
            Run r = new Run(updateLog);
            updateLogP.Inlines.Add(r);
            updateLogDoc.Blocks.Add(updateLogP);
            AboutUpdateLogText.Document = updateLogDoc;
        }
        private void BasicCommandBtn_Click(object sender, RoutedEventArgs e)
        {
            BasicPage.Visibility = Visibility.Visible;
        }

        private void BasicCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            BasicPage.Visibility = Visibility.Hidden;
        }

        public static void ExtractResFile(string resFileName, string outputFile) //将嵌入资源复制至外部文件夹
        {
            BufferedStream inStream = null;
            FileStream outStream = null;
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                inStream = new BufferedStream(asm.GetManifestResourceStream(resFileName));
                outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[1024];
                int length;

                while ((length = inStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, length);
                }
                outStream.Flush();
            }
            finally
            {
                if (outStream != null)
                {
                    outStream.Close();
                }
                if (inStream != null)
                {
                    inStream.Close();
                }
            }
        }
        private void OpenAboutPage(object sender, RoutedEventArgs e)
        {
            AboutPage.Visibility = Visibility.Visible;
        }

        private void AboutCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutPage.Visibility = Visibility.Hidden;
        }

        private void OpenGithubSite(object sender, RoutedEventArgs e) // 打开Github Repo
        {
            _ = System.Diagnostics.Process.Start("");
        }

        
    }
}
