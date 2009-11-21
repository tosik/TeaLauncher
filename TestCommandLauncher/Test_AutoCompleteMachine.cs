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
using NUnit.Framework;
using CommandLauncher;

namespace CommandLauncher.Tests
{
    [TestFixture]
    public class Test_AutoCompleteMachine
    {
        [Test]
        public void TestRegistration()
        {
            AutoCompleteMachine cm = new AutoCompleteMachine();

            // 登録チェック
            cm.RegisterWord("test");
            Assert.AreEqual(true, cm.HasItem("test"));

            // 削除チェック
            cm.RemoveWord("test");
            Assert.AreEqual(false, cm.HasItem("test"));

            // 削除後登録チェック
            cm.RegisterWord("test");
            Assert.AreEqual(true, cm.HasItem("test"));

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
                    Assert.AreEqual(true, catched_multiple_registering_exception);
                    Assert.AreEqual(true, cm.HasItem("test"));
                    Assert.AreEqual(1, cm.GetNumOfWords());
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
                    Assert.AreEqual(true, catched_multiple_removing_exception);
                    Assert.AreEqual(false, cm.HasItem("test"));
                }
            }
        }
        
        [Test]
        public void TestAutoComplete()
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
            Assert.AreEqual("test",      cm.AutoCompleteWord("t"));
            Assert.AreEqual("test",      cm.AutoCompleteWord("te"));
            Assert.AreEqual("",          cm.AutoCompleteWord("ts"));
            Assert.AreEqual("hog",       cm.AutoCompleteWord("h"));
            Assert.AreEqual("testest",   cm.AutoCompleteWord("teste"));
            Assert.AreEqual("fu",        cm.AutoCompleteWord("f"));
            Assert.AreEqual("fu",        cm.AutoCompleteWord("fu"));
            Assert.AreEqual("fuf",       cm.AutoCompleteWord("fuf"));
            Assert.AreEqual("",          cm.AutoCompleteWord(""));

            // 候補
            {
                List<string> cand = cm.GetCandidates("t");
                Assert.AreEqual(2, cand.Count);
                Assert.AreEqual("test", cand[0]);
                Assert.AreEqual("testest", cand[1]);
            }
            {
                List<string> cand = cm.GetCandidates("h");
                Assert.AreEqual(2, cand.Count);
                Assert.AreEqual("hoge", cand[0]);
                Assert.AreEqual("hoghog", cand[1]);
            }
            {
                List<string> cand = cm.GetCandidates("f");
                Assert.AreEqual(3, cand.Count);
                Assert.AreEqual("fuga", cand[0]);
                Assert.AreEqual("fuf", cand[1]); 
                Assert.AreEqual("fuffy", cand[2]);
            }
            {
                List<string> cand = cm.GetCandidates("fu");
                Assert.AreEqual(3, cand.Count);
                Assert.AreEqual("fuga", cand[0]);
                Assert.AreEqual("fuf", cand[1]); 
                Assert.AreEqual("fuffy", cand[2]);
            }
            {
                List<string> cand = cm.GetCandidates("fug");
                Assert.AreEqual(1, cand.Count);
                Assert.AreEqual("fuga", cand[0]);
            }
            {
                List<string> cand = cm.GetCandidates("fuf");
                Assert.AreEqual(2, cand.Count);
                Assert.AreEqual("fuf", cand[0]); 
                Assert.AreEqual("fuffy", cand[1]);
            }
        }
    }
}
