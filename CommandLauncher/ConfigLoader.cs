using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommandLauncher
{
    // exceptions
    class ConfigLoaderNotExistsSectionException : Exception { }
    class ConfigLoaderMultiKeyException : Exception { }
    class ConfigLoaderNotKeyValueException : Exception { }
    class ConfigLoaderSameSectionException : Exception { }

    class ConfigLoader
    {
        Hashtable m_Conf = new Hashtable();

        public void LoadConfigFile(string filename)
        {
            StreamReader stream = new StreamReader(filename);

            string section = null;

            string line = "";
            while (true)
            {
                line = stream.ReadLine();

                if (line == null)
                    break;

                string trimed = line.Trim();

                if (trimed == "")
                    continue;

                // セクションのときの処理
                if (trimed[0] == '[' && trimed[trimed.Length - 1] == ']')
                {
                    section = trimed.Substring(1, trimed.Length - 2);

                    if (m_Conf[section] != null)
                        throw new ConfigLoaderSameSectionException();
                }
                else // キーと値のときの処理
                {
                    if (section == null)
                        throw new ConfigLoaderNotExistsSectionException();

                    string[] splitted = line.Split(new char[] { '=' });
                    if (splitted.Length == 1)
                        throw new ConfigLoaderNotKeyValueException();
                    if (splitted.Length > 2)
                    {
                        for (int i = 2; i < splitted.Length; i++)
                        {
                            splitted[1] += "=" + splitted[i];
                        }
                    }

                    string key = splitted[0].Trim();
                    string value = splitted[1].Trim();

                    // 無かったら登録
                    if (m_Conf[section] == null)
                        m_Conf[section] = new Hashtable();

                    // 登録
                    Hashtable ht = (Hashtable)(m_Conf[section]);
                    try
                    {
                        ht.Add(key, value);
                    }
                    catch (Exception)
                    {
                        throw new ConfigLoaderMultiKeyException();
                    }
                }
            }

            stream.Close();
        }

        public List<string> GetSections()
        {
            List<string> list = new List<string>();
            foreach (DictionaryEntry d in m_Conf)
                list.Add((string)d.Key);
            return list;
        }

        public Hashtable GetConfig(string section)
        {
            return (Hashtable)m_Conf[section];
        }
    }
}
