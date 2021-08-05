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
using MahApps.Metro.Controls.Dialogs;

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
            MessageExt.Instance.ShowDialog = ShowDialog;
            MessageExt.Instance.ShowYesNo = ShowYesNo;
            AboutPage.Visibility = Visibility.Hidden;
            BasicPage.Visibility = Visibility.Hidden;
            BasicGamemodeGrid.Visibility = Visibility.Hidden;
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
        /// <summary>
        /// 基础命令
        /// </summary>
        private void BasicCommandBtn_Click(object sender, RoutedEventArgs e)
        {
            BasicPage.Visibility = Visibility.Visible;
        }
        private void BasicListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BasicListBox.SelectedItem.ToString().EndsWith("游戏模式"))
            {
                BasicGamemodeGrid.Visibility = Visibility.Visible;
                BasicGamemodeOnWorldRadio.IsChecked = true;
            }
        }
        /// <summary>
        /// 基础命令 - 游戏模式
        /// </summary>
        private void BasicGamemodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BasicGamemodeTextBox.Text = BasicGamemodeComboBox.SelectedItem.ToString()[45] + "" + BasicGamemodeComboBox.SelectedItem.ToString()[46];
        }

        private void BasicGamemodeOnPlayerRadio_Checked(object sender, RoutedEventArgs e)
        {
            BasicGamemodeSelectorGrid.Visibility = Visibility.Visible;
        }
        private void BasicGamemodeOnWorldRadio_Checked(object sender, RoutedEventArgs e)
        {
            BasicGamemodeSelectorGrid.Visibility = Visibility.Hidden;
        }
        private void BasicGamemodeOptBtn_Click(object sender, RoutedEventArgs e)
        {
            string effectMode = "survival ";
            string effectPlayer = "none";
            string baseStr = "/defaultgamemode ";
            if (BasicGamemodeOnPlayerRadio.IsChecked == true)
            {
                effectPlayer = BasicGamemodeTextBox.Text;
                baseStr = "/gamemode ";
            }
            if (BasicGamemodeCreativeRadio.IsChecked == true)
            {
                effectMode = "creative ";
            }
            else if(BasicGamemodeAdvantureRadio.IsChecked == true)
            {
                effectMode = "advanture ";
            }
            else if(BasicGamemodeSpectatorRadio.IsChecked == true)
            {
                effectMode = "spectator ";
            }
            baseStr += effectMode;
            if (effectPlayer == "none")
            {
                BasicGamemodeOptTextBox.Text = baseStr;
            }
            else
            {
                BasicGamemodeOptTextBox.Text = baseStr + effectPlayer;
            }
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

        public sealed class MessageExt // // CustomMsgBox
        {
            /// CustomMsgBox使用方法:
            /// MessageExt.Instance.ShowDialog("查询", "提示");
            /// MessageExt.Instance.ShowYesNo("查询", "提示", new Action(() => {
            ///    MessageBox.Show("我来了");
            /// }));
            private static readonly MessageExt instance = new MessageExt();
            private MessageExt()
            {
            }

            public static MessageExt Instance
            {
                get
                {
                    return instance;
                }
            }
            public Action<string, string> ShowDialog { get; set; }
            public Action<string, string, Action> ShowYesNo { get; set; }
        }

        public async void ShowDialog(string message, string title) // CustomMsgBox的Box
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "关闭",
                ColorScheme = MetroDialogColorScheme.Theme
            };
            _ = await this.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, mySettings);
        }
        public async void ShowYesNo(string message, string title, Action action) // CustomMsgBox的YNBox
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Cancel",
                ColorScheme = MetroDialogColorScheme.Theme
            };
            MessageDialogResult result = await this.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, mySettings);
            if (result == MessageDialogResult.Affirmative)
                await Task.Factory.StartNew(action);
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
