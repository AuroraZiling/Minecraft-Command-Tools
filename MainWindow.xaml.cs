using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
            //初始化 -> 自定义对话框
            MessageExt.Instance.ShowDialog = ShowDialog;
            MessageExt.Instance.ShowYesNo = ShowYesNo;
            //初始化 -> 各窗口隐藏
            AboutPage.Visibility = Visibility.Hidden;
            BasicPage.Visibility = Visibility.Hidden;
            BasicGamemodeGrid.Visibility = Visibility.Hidden;
            BasicGameruleGrid.Visibility = Visibility.Hidden;
            BasicDifficultyGrid.Visibility = Visibility.Hidden;
            BasicSpawnpointGrid.Visibility = Visibility.Hidden;
            //获取嵌入的updateLog内容 -> About的更新日志
            Assembly updateLogAssembly = Assembly.GetExecutingAssembly();
            Stream updateLogConfigStream = updateLogAssembly.GetManifestResourceStream("CommandTools.updateLog.txt");
            byte[] updateLogTmp = new byte[updateLogConfigStream.Length];
            _ = updateLogConfigStream.Read(updateLogTmp, 0, updateLogTmp.Length);
            string updateLog = Encoding.UTF8.GetString(updateLogTmp);
            AboutUpdateLogText.Document = new FlowDocument(new Paragraph(new Run(updateLog)));
            FlowDocument updateLogDoc = new FlowDocument();
            Paragraph updateLogP = new Paragraph();
            Run updateLogR = new Run(updateLog);
            updateLogP.Inlines.Add(updateLogR);
            updateLogDoc.Blocks.Add(updateLogP);
            AboutUpdateLogText.Document = updateLogDoc;
            //获取嵌入的Licenses内容 -> About的许可证告示
            Assembly licensesAssembly = Assembly.GetExecutingAssembly();
            Stream licensesConfigStream = licensesAssembly.GetManifestResourceStream("CommandTools.licenses.txt");
            byte[] licensesTmp = new byte[licensesConfigStream.Length];
            _ = licensesConfigStream.Read(licensesTmp, 0, licensesTmp.Length);
            string licenses = Encoding.UTF8.GetString(licensesTmp);
            AboutLicencesText.Document = new FlowDocument(new Paragraph(new Run(licenses)));
            FlowDocument licensesDoc = new FlowDocument();
            Paragraph licensesP = new Paragraph();
            Run licensesR = new Run(licenses);
            licensesP.Inlines.Add(licensesR);
            licensesDoc.Blocks.Add(licensesP);
            AboutLicencesText.Document = licensesDoc;
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
                BasicGameruleGrid.Visibility = Visibility.Hidden;
                BasicDifficultyGrid.Visibility = Visibility.Hidden;
                BasicSpawnpointGrid.Visibility = Visibility.Hidden;
                BasicGamemodeGrid.Visibility = Visibility.Visible;
                BasicGamemodeOnWorldRadio.IsChecked = true;
            }
            else if (BasicListBox.SelectedItem.ToString().EndsWith("游戏规则"))
            {
                BasicGamemodeGrid.Visibility = Visibility.Hidden;
                BasicDifficultyGrid.Visibility = Visibility.Hidden;
                BasicSpawnpointGrid.Visibility = Visibility.Hidden;
                BasicGameruleGrid.Visibility = Visibility.Visible;
                BasicGameruleComboBox.SelectedIndex = 0;
            }
            else if (BasicListBox.SelectedItem.ToString().EndsWith("世界难度"))
            {
                BasicGamemodeGrid.Visibility = Visibility.Hidden;
                BasicGameruleGrid.Visibility = Visibility.Hidden;
                BasicSpawnpointGrid.Visibility = Visibility.Hidden;
                BasicDifficultyGrid.Visibility = Visibility.Visible;
            }
            else if (BasicListBox.SelectedItem.ToString().EndsWith("出生点"))
            {
                BasicGamemodeGrid.Visibility = Visibility.Hidden;
                BasicGameruleGrid.Visibility = Visibility.Hidden;
                BasicDifficultyGrid.Visibility = Visibility.Hidden;
                BasicSpawnpointGrid.Visibility = Visibility.Visible;
                BasicSpawnpointOnWorldRadio.IsChecked = true;
                BasicSpawnpointMode1ARadio.IsChecked = true;
                BasicSpawnpointMode2ARadio.IsChecked = true;
                BasicSpawnpointMode3ARadio.IsChecked = true;
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

        /// <summary>
        /// 基础命令 - 游戏规则
        /// </summary>
        public string basicGameruleBaseStr = "/gamerule ";
        
        private void BasicGameruleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            basicGameruleBaseStr = "/gamerule ";
            List<string> rules = new List<string>() { "announceAdvancements", "commandBlockOutput",
                    "disableElytraMovementCheck", "disableRaids", "doDaylightCycle", "doEntityDrops",
                    "doFireTick", "doInsomnia", "doImmediateRespawn", "doLimitedCrafting", "doMobLoot",
                    "doMobSpawning", "doPatrolSpawning", "doTileDrops", "doTraderSpawning", "doWeatherCycle",
                    "drowningDamage", "fallDamage", "fireDamage", "forgiveDeadPlayers", "freezeDamage", "keepInventory",
                    "logAdminCommands", "mobGriefing", "naturalRegeneration", "reducedDebugInfo", "sendCommandFeedback",
                    "showDeathMessages", "spectatorsGenerateChunks", "universalAnger", "maxCommandChainLength",
                    "maxEntityCramming", "playersSleepingPercentage", "randomTickSpeed", "spawnRadius" };

            if (BasicGameruleComboBox.SelectedIndex <= 29) //布尔值
            {
                BasicGameruleControlBoolGrid.Visibility = Visibility.Visible;
                BasicGameruleControlIntGrid.Visibility = Visibility.Hidden;
            }
            else //整型
            {
                BasicGameruleControlBoolGrid.Visibility = Visibility.Hidden;
                BasicGameruleControlIntGrid.Visibility = Visibility.Visible;
            }
            basicGameruleBaseStr += rules[BasicGameruleComboBox.SelectedIndex] + " ";
        }

        private void BasicGameruleOptBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (BasicGameruleComboBox.SelectedIndex <= 29) //布尔值
            {
                BasicGameruleOptTextBox.Text = basicGameruleBaseStr + BasicGameruleTrueRadio.IsChecked.ToString().ToLower();
            }
            else //整型
            {
                if (BasicGameruleControlIntTextBox.Text == "")
                {
                    MessageExt.Instance.ShowDialog("您没有填入数值", "错误");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(BasicGameruleControlIntTextBox.Text, @"^\d*$"))
                {
                    MessageExt.Instance.ShowDialog("您输入的数值不是整数", "错误");
                }
                else
                {
                    BasicGameruleOptTextBox.Text = basicGameruleBaseStr + BasicGameruleControlIntTextBox.Text;
                }
            }

        }

        /// <summary>
        /// 基础命令 - 世界难度
        /// </summary>
        private void BasicDifficultyOptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BasicDifficultyPeacefulRadio.IsChecked == true)
            {
                BasicDifficultyOptTextBox.Text = "/difficulty peaceful";
            }
            else if (BasicDifficultyEasyRadio.IsChecked == true)
            {
                BasicDifficultyOptTextBox.Text = "/difficulty easy";
            }
            else if (BasicDifficultyNormalRadio.IsChecked == true)
            {
                BasicDifficultyOptTextBox.Text = "/difficulty normal";
            }
            else if (BasicDifficultyHardRadio.IsChecked == true)
            {
                BasicDifficultyOptTextBox.Text = "/difficulty hard";
            }
        }

        /// <summary>
        /// 基础命令 - 出生点
        /// </summary>
        private void BasicSpawnpointOnWorldRadio_Checked(object sender, RoutedEventArgs e)
        {
            BasicSpawnpointSelectorGrid.Visibility = Visibility.Hidden;
        }

        private void BasicSpawnpointOnPlayerRadio_Checked(object sender, RoutedEventArgs e)
        {
            BasicSpawnpointSelectorGrid.Visibility = Visibility.Visible;
        }

        private void BasicSpawnpointComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BasicSpawnpointTextBox.Text = BasicSpawnpointComboBox.SelectedItem.ToString()[45] + "" + BasicSpawnpointComboBox.SelectedItem.ToString()[46];
        }

        private void BasicSpawnpointOptBtn_Click(object sender, RoutedEventArgs e)
        {
            string baseStr = "/setworldspawn ";
            string positive = " ";
            if (BasicSpawnpointOnPlayerRadio.IsChecked == true)
            {
                baseStr = "/spawnpoint ";
            }
            if (BasicSpawnpointTextBox.Text == "" && BasicSpawnpointOnPlayerRadio.IsChecked == true)
            {
                MessageExt.Instance.ShowDialog("您没有填入目标选择器", "错误");
            }
            else if (BasicSpawnpointXTextBox.Text == "")
            {
                MessageExt.Instance.ShowDialog("您没有填入X坐标", "错误");
            }
            else if (BasicSpawnpointYTextBox.Text == "")
            {
                MessageExt.Instance.ShowDialog("您没有填入Y坐标", "错误");
            }
            else if (BasicSpawnpointZTextBox.Text == "")
            {
                MessageExt.Instance.ShowDialog("您没有填入Z坐标", "错误");
            }
            else
            {
                if (BasicSpawnpointMode1ARadio.IsChecked == true)
                {
                    positive += BasicSpawnpointXTextBox.Text;
                }
                else
                {
                    positive += "~" + BasicSpawnpointXTextBox.Text;
                }
                positive += " ";
                if (BasicSpawnpointMode2ARadio.IsChecked == true)
                {
                    positive += BasicSpawnpointYTextBox.Text;
                }
                else
                {
                    positive += "~" + BasicSpawnpointYTextBox.Text;
                }
                positive += " ";
                if (BasicSpawnpointMode3ARadio.IsChecked == true)
                {
                    positive += BasicSpawnpointZTextBox.Text;
                }
                else
                {
                    positive += "~" + BasicSpawnpointZTextBox.Text;
                }
                if (BasicSpawnpointOnPlayerRadio.IsChecked == true)
                {
                    BasicSpawnpointOptTextBox.Text = baseStr + BasicSpawnpointTextBox.Text + positive;
                }
                else
                {
                    BasicSpawnpointOptTextBox.Text = baseStr + positive;
                }
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

        /// <summary>
        /// 关于
        /// </summary>

        private void OpenAboutPage(object sender, RoutedEventArgs e)
        {
            AboutPage.Visibility = Visibility.Visible;
        }

        private void AboutMinecraftWikiBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = System.Diagnostics.Process.Start("https://minecraft.fandom.com/wiki/Minecraft_Wiki");
        }

        private void AboutMCModBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = System.Diagnostics.Process.Start("https://www.mcmod.cn/tools/cbcreator/");
        }

        private void AboutCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutPage.Visibility = Visibility.Hidden;
        }

        private void OpenGithubSite(object sender, RoutedEventArgs e) // 打开Github Repo
        {
            _ = System.Diagnostics.Process.Start("https://github.com/AuroraZiling/Minecraft-Command-Tools");
        }

        
    }
}
