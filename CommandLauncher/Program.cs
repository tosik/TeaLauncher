#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace CommandLauncher
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if TEST
            Debug.WriteLine("テスト開始");
            {
                new Test_AutoCompleteMachine();
                new Test_ConfigLoader();
            }
            Debug.WriteLine("テスト終了");
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string conf_filename = args.Length >= 1 ? args[0] : "conf/my.conf";
            Application.Run(new MainWindow(conf_filename));
        }
    }
}
