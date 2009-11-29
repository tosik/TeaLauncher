/*
 * TeaLauncher. Simple command launcher.
 * Copyright (C) Toshiyuki Hirooka <toshi.hirooka@gmail.com> http://wasabi.in/
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CommandLauncher
{
    class Command
    {
        public string command;
        public string execution;
    }

    interface ICommandManagerInitializer
    {
        void Reinitialize();
    }

    interface ICommandManagerFinalizer
    {
        void Exit();
    }

    interface ICommandManagerDialogShower
    {
        void ShowVersionInfo();
        void ShowError(string message);
    }

    class CommandManager
        : AutoCompleteMachine
    {
        private List<Command> m_Commands = new List<Command>();

        ICommandManagerInitializer m_Initializer;
        ICommandManagerFinalizer m_Finalizer;
        ICommandManagerDialogShower m_DialogShower;

        public CommandManager(
            ICommandManagerInitializer initializer,
            ICommandManagerFinalizer finalizer,
            ICommandManagerDialogShower dialogshower
            )
        {
            m_Initializer = initializer;
            m_Finalizer = finalizer;
            m_DialogShower = dialogshower;
        }

        public void RegisterCommand(Command command)
        {
            // 無ければ登録
            if (!m_Commands.Exists(
                delegate(Command cmd)
                {
                    return (command.command == cmd.command);
                }
                )
            )
            {
                m_Commands.Add(command);
                RegisterWord(command.command);
            }
        }

        public void RemoveCommand(string command)
        {
            string item = GetExecution(command);

            // 見つけ出して取り除く
            m_Commands.ForEach(
                delegate(Command cmd)
                {
                    if (item == cmd.command)
                    {
                        m_Commands.Remove(cmd);
                        RemoveWord(cmd.command);
                    }
                }
            );
        }

        private bool IsPath(string str)
        {
            // FIXME : 決め打ちよくない

            if (str.StartsWith("http://"))
                return true;
            if (str.StartsWith("https://"))
                return true;
            if (str.StartsWith("ftp://"))
                return true;
            if (str.Length >= 3)
            {
                // X:\ から始まる
                if (str[1] == ':' && str[2] == '\\')
                    return true;
            }

            return false;
        }

        private bool IsSpecialCommand(string str)
        {
            if (str.Length >= 2)
                if (str[0] == '!')
                    return true;

            return false;
        }

        public void Run(string command)
        {
            {
                string filename;
                string args;

                if (IsPath(command))
                {
                    // パスならそのまま
                    filename = GetExecution(command);
                    args = String.Join(" ", GetArguments(command).ToArray());
                }
                else
                {
                    // 登録されたコマンド名ならコマンドからファイル名を取得する
                    string item = GetExecution(command);

                    // コマンドを探す
                    Command finding_cmd = m_Commands.Find(
                         delegate(Command cmd)
                         {
                             return (item == cmd.command);
                         }
                    );

                    filename = GetExecution(finding_cmd.execution);
                    args = String.Join(" ", GetArguments(finding_cmd.execution).ToArray()) + " " + String.Join(" ", GetArguments(command).ToArray());
                }

                if (IsSpecialCommand(filename))
                {
                    Debug.WriteLine("SpecialCommand : " + filename);
                    RunSpecialCommand(filename);
                }
                else
                {
                    Debug.WriteLine("Execute : " + filename + " " + args);
                    Process.Start(filename, args);
                }
            }
        }

        private void RunSpecialCommand(string cmd)
        {
            switch (cmd)
            {
                case "!reload":
                    RequestReinitialize();
                    break;
                case "!exit":
                    RequestExitApplication();
                    break;
                case "!version":
                    RequestShowVersionInfo();
                    break;
                default:
                    RequestShowError("not found the command : " + cmd);
                    break;
            }
        }

        private List<string> Split(string str)
        {
            List<string> args = new List<string>();

            string work_str = "";
            bool is_begin_quotation = false;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case ' ':
                        if (!is_begin_quotation)
                        {
                            args.Add(work_str);
                            work_str = "";
                        }
                        else
                        {
                            work_str += " ";
                        }
                        break;

                    case '"':
                    case '\'':
                        if (is_begin_quotation == false)
                            is_begin_quotation = true;
                        else
                            is_begin_quotation = false;
                        break;

                    default:
                        work_str += str[i];
                        break;
                }
            }

            args.Add(work_str);

            return args;
        }

        private string GetExecution(string command)
        {
            List<string> strlist = Split(command);
            if (strlist.Count >= 1)
                return strlist[0];
            else
                return null;
        }

        private List<string> GetArguments(string command)
        {
            List<string> strlist = Split(command);
            strlist.Remove(GetExecution(command));
            return strlist;
        }

        public bool HasCommand(string command)
        {
            if (IsPath(command))
                return true;
            string item = GetExecution(command);
            return HasItem(item);
        }

        public void ClearCommands()
        {
            m_Commands.Clear();
            ClearWords();
        }

        private void RequestReinitialize()
        {
            // 再初期化を依頼する
            m_Initializer.Reinitialize();
        }

        private void RequestExitApplication()
        {
            // アプリケーション終了を依頼する
            m_Finalizer.Exit();
        }

        private void RequestShowVersionInfo()
        {
            // バージョン情報の表示を依頼する
            m_DialogShower.ShowVersionInfo();
        }

        private void RequestShowError(string message)
        {
            // エラー表示を依頼する
            m_DialogShower.ShowError(message);
        }
    }
}
