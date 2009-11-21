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

namespace CommandLauncher
{
    public class AutoCompleteMachineMultipleRegisteringException : Exception { }
    public class AutoCompleteMachineMultipleRemovingException : Exception { }

    public class AutoCompleteMachine
    {
        private List<string> m_WordList = new List<string>();

        public void RegisterWord(string word)
        {
            // 多重登録できないようにする
            if (HasItem(word))
                throw new AutoCompleteMachineMultipleRegisteringException();

            m_WordList.Add(word);
        }

        public void RemoveWord(string word)
        {
            // 存在しないアイテムの削除は例外
            if (!HasItem(word))
                throw new AutoCompleteMachineMultipleRemovingException();

            m_WordList.Remove(word);
        }

        public void ClearWords()
        {
            m_WordList.Clear();
        }

        public string AutoCompleteWord(string chars)
        {
            List<string> list = GetCandidates(chars);

            if (list.Count == 0)
                return "";

            // 前方一致のなかで前方から共通する部分までの文字列を取得
            {
                int length = 0;
                string sample = list[0];

                {
                    string part_of_sample = "";
                    for (int i = 0; i < sample.Length; i++)
                    {
                        bool ok = true;
                        part_of_sample += sample[i];
                        foreach (string str in list)
                        {
                            if (!str.StartsWith(part_of_sample))
                            {
                                ok = false;
                            }
                        }
                        if (ok)
                            length = i + 1;
                    }
                }

                string comp_str = "";
                for (int i = 0; i < length; i++)
                {
                    comp_str += sample[i];
                }

                return comp_str;
            }
        }

        public List<string> GetCandidates(string chars)
        {
            // 前方一致する文字列をリストで取得
            List<string> list = m_WordList.FindAll(
                delegate(string str)
                {
                    return str.StartsWith(chars);
                }
            );

            return list;
        }

        public bool HasItem(string item)
        {
            return m_WordList.Exists(
                delegate(string str)
                {
                    return str == item;
                }
            );
        }

        public int GetNumOfWords()
        {
            return m_WordList.Count;
        }
    }
}
