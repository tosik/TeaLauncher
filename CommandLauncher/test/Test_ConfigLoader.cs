using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CommandLauncher
{
    class Test_ConfigLoader
    {
        public Test_ConfigLoader()
        {
            TestCorrectConfig();
            TestIncorrectConfig();
            TestLoadingMulticonfig();
        }

        void TestCorrectConfig()
        {
            ConfigLoader cl = new ConfigLoader();

            cl.LoadConfigFile("conf/test_correctcase.conf");

            // セクションの取得
            List<string> sections = cl.GetSections();
            Debug.Assert(sections.Count == 4);

            // note : 順序は保証されない
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section1"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section2"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section3"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section4"; }));
            Debug.Assert(!sections.Exists(delegate(string s) { return s == "section5"; }));
            Debug.Assert(!sections.Exists(delegate(string s) { return s == "section"; }));
            Debug.Assert(!sections.Exists(delegate(string s) { return s == ""; }));

            // セクションから項目取得
            {
                Hashtable conf = cl.GetConfig("section1");
                Debug.Assert(conf.Count == 2);
                Debug.Assert((string)conf["hoge"] == "fuga");
                Debug.Assert((string)conf["asdf"] == "aassddff");
            }
            {
                Hashtable conf = cl.GetConfig("section2");
                Debug.Assert(conf.Count == 1);
                Debug.Assert((string)conf["hoge"] == "fuga asdf");
            }
            {
                Hashtable conf = cl.GetConfig("section3");
                Debug.Assert(conf.Count == 1);
                Debug.Assert((string)conf["hoge"] == "fuga ahoaho");
            }
            {
                Hashtable conf = cl.GetConfig("section4");
                Debug.Assert(conf.Count == 4);
                Debug.Assert((string)conf["h"] == "fuga");
                Debug.Assert((string)conf["ho"] == "fuga");
                Debug.Assert((string)conf["hog"] == "fuga");
                Debug.Assert((string)conf["hoge"] == "fuga");
            }
        }

        void TestIncorrectConfig()
        {
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.LoadConfigFile("conf/test_incorrectcase_notexistssection.conf");
                }
                catch (ConfigLoaderNotExistsSectionException)
                {
                    catched = true;
                }
                finally
                {
                    Debug.Assert(catched);
                }
            }
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.LoadConfigFile("conf/test_incorrectcase_multikey.conf");
                }
                catch (ConfigLoaderMultiKeyException)
                {
                    catched = true;
                }
                finally
                {
                    Debug.Assert(catched);
                }
            }
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.LoadConfigFile("conf/test_incorrectcase_notkeyvalue.conf");
                }
                catch (ConfigLoaderNotKeyValueException)
                {
                    catched = true;
                }
                finally
                {
                    Debug.Assert(catched);
                }
            }
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.LoadConfigFile("conf/test_incorrectcase_samesection.conf");
                }
                catch (ConfigLoaderSameSectionException)
                {
                    catched = true;
                }
                finally
                {
                    Debug.Assert(catched);
                }
            }
        }

        void TestLoadingMulticonfig()
        {
            ConfigLoader cl = new ConfigLoader();

            cl.LoadConfigFile("conf/test_correctcase.conf");
            cl.LoadConfigFile("conf/test_correctcase2.conf");

            // セクションの取得
            List<string> sections = cl.GetSections();
            Debug.Assert(sections.Count == 5);

            Debug.Assert(sections.Exists(delegate(string s) { return s == "section1"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section2"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section3"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "section4"; }));
            Debug.Assert(sections.Exists(delegate(string s) { return s == "addedsection"; }));
            Debug.Assert(!sections.Exists(delegate(string s) { return s == "section5"; }));
            Debug.Assert(!sections.Exists(delegate(string s) { return s == "section"; }));
            Debug.Assert(!sections.Exists(delegate(string s) { return s == ""; }));

            // セクションから項目取得
            {
                Hashtable conf = cl.GetConfig("section1");
                Debug.Assert(conf.Count == 2);
                Debug.Assert((string)conf["hoge"] == "fuga");
                Debug.Assert((string)conf["asdf"] == "aassddff");
            }
            {
                Hashtable conf = cl.GetConfig("addedsection");
                Debug.Assert(conf.Count == 2);
                Debug.Assert((string)conf["hoku"] == "hoku");
                Debug.Assert((string)conf["poka"] == "poka");
            }
        }
    }
}
