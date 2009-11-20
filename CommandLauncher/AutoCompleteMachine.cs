using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLauncher
{
    class AutoCompleteMachineMultipleRegisteringException : Exception { }
    class AutoCompleteMachineMultipleRemovingException : Exception { }

    class AutoCompleteMachine
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
