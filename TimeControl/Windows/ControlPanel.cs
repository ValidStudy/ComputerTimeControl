﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeControl.AppControl;
using TimeControl.Data;
using TimeControl.Tools;
using Windows.ApplicationModel.Contacts;

namespace TimeControl.Windows
{
    public partial class ControlPanel : Form
    {
        private bool hide = false;//指示启动后是否需要隐藏
        private bool isClosable = false;//指示当前是否可以关闭
        private string unlockPasswordHash = "";//密码哈希值，用作比对
        private AppController appController;//列表、计时控制器
        private TimeData timeData;//数据
        public ControlPanel(bool hide)
        {
            InitializeComponent();
            this.hide = hide;
            //屏保
            if (File.Exists(TimeControlFile.WhiteAppLocation))
                whiteProcessBox.Text = File.ReadAllText(TimeControlFile.WhiteAppLocation);
            if (File.Exists(TimeControlFile.TempTimeFile))
            {
                MessageBox.Show("恢复屏保");
                StartLock(unlockPasswordHash);
            }
            //程序计时
            if (!Directory.Exists(TimeControlFile.BaseLocation))
            {
                Directory.CreateDirectory(TimeControlFile.BaseLocation);
            }
            appController = new(usageBox, processMonitorTimer);
            fileSaveTimer.Start();
            //自动关机
            if (File.Exists(TimeControlFile.ShutdownSpan))
            {
                string[] shutdownTimeLines = File.ReadAllLines(TimeControlFile.ShutdownSpan);
                TimeOnly startTime = TimeOnly.Parse(shutdownTimeLines[0]);
                TimeOnly endTime = TimeOnly.Parse(shutdownTimeLines[1]);
                TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
                if (now >= startTime && now <= endTime)
                {
                    SystemControl.Shutdown();
                }
                Application.Exit();
            }
            //数据记录
            if(File.Exists(TimeControlFile.SavedData))
            {
                timeData = TimeControlFile.ReadTimeData();
                RefreshAndSaveData();
            }
            else
            {
                timeData = new();
                RefreshAndSaveData();
            }
            //密码
            if (File.Exists(TimeControlFile.PassLocation))//加载密码哈希值
            {
                unlockPasswordHash = File.ReadAllText(TimeControlFile.PassLocation);
                PasswordSet();
            }
            else
                unlockPasswordRemoveButton.Enabled = false;
        }

        #region Form

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)//打开界面
        {
            Show();
        }

        private void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)//处理关闭逻辑
        {
            appController.Save();
            if (!isClosable)//隐藏窗口
            {
                e.Cancel = true;
                Hide();
            }
            else//退出前关闭保护进程
            {
                Process[] processes = Process.GetProcessesByName("TimeControlConsole");
                foreach (Process process in processes)
                {
                    process.Kill();
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)//正常退出程序
        {
            PasswordInput passwordInput = new(unlockPasswordHash);
            if (!string.IsNullOrEmpty(unlockPasswordHash))//检测是否设置了管理码
            {
                if (passwordInput.ShowDialog() == DialogResult.OK)
                    ForceClose();
            }
            else
                ForceClose();
        }

        private void ForceClose()//可以正常关闭
        {
            isClosable = true;
            processMonitorTimer.Stop();
            Close();
        }

        private void ControlPanel_Shown(object sender, EventArgs e)//启动隐藏参数支持
        {
            if (hide)
            {
                Hide();
            }

            processMonitorTimer.Start();
        }

        #endregion Form

        #region LockPage

        private void StartButton_Click(object sender, EventArgs e)//启动屏保程序
        {
            StartLock(unlockPasswordHash, (int)timeBox.Value);
        }

        private void StartLock(string unlockPasswordHash, int minutes = 0)
        {
            IntPtr nowDesktop = Dllimport.GetThreadDesktop(Dllimport.GetCurrentThreadId());
            IntPtr newDesktop = Dllimport.CreateDesktop("Lock", null, null, 0, Dllimport.ACCESS_MASK.GENERIC_ALL, IntPtr.Zero);
            Dllimport.SwitchDesktop(newDesktop);
            Task.Factory.StartNew(() =>
            {
                Dllimport.SetThreadDesktop(newDesktop);
                Lock _lock;
                if (minutes != 0)
                    _lock = new(minutes, unlockPasswordHash);
                else
                    _lock = new(unlockPasswordHash);
                Application.Run(_lock);
            }).Wait();
            Dllimport.SwitchDesktop(nowDesktop);
            Dllimport.CloseDesktop(newDesktop);
            int index = dataGridView.Rows.Add();
            timeData.AddTime(Lock.TempTimeSpan);
            RefreshAndSaveData();
        }

        private void WhiteProcessBox_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(TimeControlFile.WhiteAppLocation, whiteProcessBox.Text);
        }

        #endregion

        #region ProcessPage

        private void FileSaveTimer_Tick(object sender, EventArgs e)
        {
            processMonitorTimer.Stop();
            appController.Save();
            processMonitorTimer.Start();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//添加链接
        {
            Process.Start("explorer.exe", "https://icons8.com/icon/19614/icon");
        }

        private void ClearButton_Click(object sender, EventArgs e)//移除所有的已添加窗口
        {
            if (PasswordCheck())
            {
                appController.RemoveAll();
            }
        }

        private void AppAddButton_Click(object sender, EventArgs e)//添加进程
        {
            TimeInput timeInput = new(appController);//打开进程限时控制窗口
            timeInput.ShowDialog();
        }

        private void RemoveButton_Click(object sender, EventArgs e)//移除窗口
        {
            //检测密码设置
            if (PasswordCheck())
            {
                appController.Remove();
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)//重新获取所有软件所用时间
        {
            appController.Refresh();
        }

        private void ProcessMonitorTimer_Tick(object sender, EventArgs e)
        {
            appController.Run();
            if (autoRefreshBox.Checked)
                appController.Refresh();
            if (Process.GetProcessesByName("TimeControlConsole").Length == 0)//检查保护程序状态
            {
                ProcessStartInfo process = new();
                process.FileName = "TimeControlConsole.exe";
                Process.Start(process);
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            appController.Reset();
        }

        #endregion

        #region ShutdownPage
        private void shutdownSetButton_Click(object sender, EventArgs e)
        {
            TimeOnly startTime = new((int)startShutdownHour.Value,
                (int)startShutdownMinute.Value, 0);
            TimeOnly endTime = new((int)endShutdownHour.Value,
                (int)endShutdownMinute.Value, 0);
            if (startTime < endTime)
                File.WriteAllText(TimeControlFile.ShutdownSpan,
                    startTime + Environment.NewLine + endTime);
            else
                MessageBox.Show("时间输入非法。", "错误"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void shutdownRemoveButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(TimeControlFile.ShutdownSpan))
                File.Delete(TimeControlFile.ShutdownSpan);
        }
        #endregion

        #region ProtectPage

        #region Password

        private void UnloackPasswordSetButton_Click(object sender, EventArgs e)//保存密码
        {
            unlockPasswordHash = Password.ComputeHash(unlockPasswordBox.Text);//保存哈希值
            File.WriteAllText(TimeControlFile.PassLocation, unlockPasswordHash.ToString());//保存哈希值到文件
            PasswordSet();
        }

        private void UnlockPasswordRemoveButton_Click(object sender, EventArgs e)
        {
            if (PasswordCheck())
            {
                File.Delete(TimeControlFile.PassLocation);
                unlockPasswordHash = "";
                unlockPasswordBox.Text = "";
                unlockPasswordBox.Enabled = true;
                unlockPasswordSetButton.Enabled = true;
                unlockPasswordRemoveButton.Enabled = false;
                removeBootButton.Enabled = true;
            }
        }

        private void PasswordSet()//密码设置后调用
        {
            unlockPasswordBox.Text = "";
            unlockPasswordBox.Enabled = false;
            unlockPasswordSetButton.Enabled = false;
            unlockPasswordRemoveButton.Enabled = true;
            removeBootButton.Enabled = false;
        }

        private bool PasswordCheck()//检测密码是否正确
        {
            if (!string.IsNullOrEmpty(unlockPasswordHash))
            {
                PasswordInput passwordInput = new(unlockPasswordHash);
                if (passwordInput.ShowDialog() == DialogResult.OK)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        #endregion

        private void AddBootButton_Click(object sender, EventArgs e)
        {
            TaskSchedulerControl.AddBoot();
        }

        private void RemoveBootButton_Click(object sender, EventArgs e)
        {
            TaskSchedulerControl.RemoveBoot();
        }

        #endregion

        #region AboutPage
        private void AuthorButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Resources.words);
        }

        private void GithubLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/SamHou0/ComputerTimeControl");
        }

        private void GiteeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/Sam-Hou/ComputerTimeControl");
        }

        private void HelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe",
                "https://gitee.com/Sam-Hou/ComputerTimeControl/wikis/%E5%B8%B8%E8%A7%81%E9%97%AE%E9%A2%98&%E4%BD%BF%E7%94%A8%E8%AF%B4%E6%98%8E");
        }
        #endregion

        #region DataPage
        private void RefreshAndSaveData()
        {
            //刷新列表
            //屏保合计
            dataGridView.Rows.Clear();
            dataGridView.Rows.Add();
            dataGridView.Rows[0].Cells[0].Value = timeData.Time.Hours;
            dataGridView.Rows[0].Cells[1].Value = timeData.Time.Minutes;
            dataGridView.Rows[0].Cells[2].Value = timeData.Time.Seconds;
            dataGridView.Rows[0].Cells[3].Value = "屏保合计";
            //保存
            TimeControlFile.SaveTimeData(timeData);
        }

        #endregion

    }
}