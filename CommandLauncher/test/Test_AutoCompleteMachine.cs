using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CommandLauncher
{
    class Test_AutoCompleteMachine
    {
        public Test_AutoCompleteMachine()
        {
            TestRegistration();
            TestAutoComplete();
        }

        void TestRegistration()
        {
            AutoCompleteMachine cm = new AutoCompleteMachine();

            // 登録チェック
            cm.RegisterWord("test");
            Debug.Assert(cm.HasItem("test"));

            // 削除チェック
            cm.RemoveWord("test");
            Debug.Assert(!cm.HasItem("test"));

            // 削除後登録チェック
            cm.RegisterWord("test");
            Debug.Assert(cm.HasItem("test"));

            // 多重登録禁止チェック
            {
                bool catched_multiple_registering_exception = false;
                try
                {
                    cm.RegisterWord("test");
                }
                catch (AutoCompleteMachineMultipleRegisteringException)
                {
                    catched_multiple_registering_exception = true;
                }
                finally
                {
                    Debug.Assert(catched_multiple_registering_exception);
                    Debug.Assert(cm.HasItem("test"));
                    Debug.Assert(cm.GetNumOfWords() == 1);
                }
            }

            // 存在しないアイテムの削除チェック
            {
                bool catched_multiple_removing_exception = false;
                try
                {
                    cm.RemoveWord("test");
                    cm.RemoveWord("test");
                }
                catch (AutoCompleteMachineMultipleRemovingException)
                {
                    catched_multiple_removing_exception = true;
                }
                finally
                {
                    Debug.Assert(catched_multiple_removing_exception);
                    Debug.Assert(!cm.HasItem("test"));
                }
            }
        }

        void TestAutoComplete()
        {
            AutoCompleteMachine cm = new AutoCompleteMachine();

            // キーワード登録
            cm.RegisterWord("test");
            cm.RegisterWord("hoge");
            cm.RegisterWord("fuga");
            cm.RegisterWord("hoghog");
            cm.RegisterWord("testest");
            cm.RegisterWord("fuf");
            cm.RegisterWord("fuffy");

            // 補完
            Debug.Assert(cm.AutoCompleteWord("t") == "test");
            Debug.Assert(cm.AutoCompleteWord("te") == "test");
            Debug.Assert(cm.AutoCompleteWord("ts") == "");
            Debug.Assert(cm.AutoCompleteWord("h") == "hog");
            Debug.Assert(cm.AutoCompleteWord("teste") == "testest");
            Debug.Assert(cm.AutoCompleteWord("f") == "fu");
            Debug.Assert(cm.AutoCompleteWord("fu") == "fu");
            Debug.Assert(cm.AutoCompleteWord("fuf") == "fuf");
            Debug.Assert(cm.AutoCompleteWord("") == "");

            // 候補
            {
                List<string> cand = cm.GetCandidates("t");
                Debug.Assert(cand.Count == 2);
                Debug.Assert(cand[0] == "test");
                Debug.Assert(cand[1] == "testest");
            }
            {
                List<string> cand = cm.GetCandidates("h");
                Debug.Assert(cand.Count == 2);
                Debug.Assert(cand[0] == "hoge");
                Debug.Assert(cand[1] == "hoghog");
            }
            {
                List<string> cand = cm.GetCandidates("f");
                Debug.Assert(cand.Count == 3);
                Debug.Assert(cand[0] == "fuga");
                Debug.Assert(cand[1] == "fuf");
                Debug.Assert(cand[2] == "fuffy");
            }
            {
                List<string> cand = cm.GetCandidates("fu");
                Debug.Assert(cand.Count == 3);
                Debug.Assert(cand[0] == "fuga");
                Debug.Assert(cand[1] == "fuf");
                Debug.Assert(cand[2] == "fuffy");
            }
            {
                List<string> cand = cm.GetCandidates("fug");
                Debug.Assert(cand.Count == 1);
                Debug.Assert(cand[0] == "fuga");
            }
            {
                List<string> cand = cm.GetCandidates("fuf");
                Debug.Assert(cand.Count == 2);
                Debug.Assert(cand[0] == "fuf");
                Debug.Assert(cand[1] == "fuffy");
            }
        }
    }
}
