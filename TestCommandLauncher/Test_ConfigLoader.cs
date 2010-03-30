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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using CommandLauncher;
using System.IO;

namespace CommandLauncher.Tests
{
    [TestFixture]
    public class Test_ConfigLoader
    {
        StreamReader StringToStreamReader(string str)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(str)));
        }

        [Test]
        public void TestCorrectConfig()
        {
            ConfigLoader cl = new ConfigLoader();
            cl.ParseConfig(StringToStreamReader(@"
[section1]
hoge = fuga
asdf =aassddff
[section2]
hoge = fuga asdf

[section3]
hoge=fuga ahoaho

[section4]
h = fuga
ho = fuga 
hog = fuga  
hoge = fuga   "));

            // セクションの取得
            List<string> sections = cl.GetSections();
            Assert.AreEqual(4, sections.Count);

            // note : 順序は保証されない
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section1"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section2"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section3"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section4"; }));
            Assert.AreEqual(false, sections.Exists(delegate(string s) { return s == "section5"; }));
            Assert.AreEqual(false, sections.Exists(delegate(string s) { return s == "section"; }));
            Assert.AreEqual(false, sections.Exists(delegate(string s) { return s == ""; }));

            // セクションから項目取得
            {
                Hashtable conf = cl.GetConfig("section1");
                Assert.AreEqual(2, conf.Count);
                Assert.AreEqual("fuga", (string)conf["hoge"]);
                Assert.AreEqual("aassddff", (string)conf["asdf"]);
            }
            {
                Hashtable conf = cl.GetConfig("section2");
                Assert.AreEqual(1, conf.Count);
                Assert.AreEqual("fuga asdf", (string)conf["hoge"]);
            }
            {
                Hashtable conf = cl.GetConfig("section3");
                Assert.AreEqual(1, conf.Count);
                Assert.AreEqual("fuga ahoaho", (string)conf["hoge"]);
            }
            {
                Hashtable conf = cl.GetConfig("section4");
                Assert.AreEqual(4, conf.Count);
                Assert.AreEqual("fuga", (string)conf["h"]);
                Assert.AreEqual("fuga", (string)conf["ho"]);
                Assert.AreEqual("fuga", (string)conf["hog"]);
                Assert.AreEqual("fuga", (string)conf["hoge"]);
            }
        }

        [Test]
        public void TestIncorrectConfig()
        {
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.ParseConfig(StringToStreamReader(@"
hoge = fuga

[section]
foo = bar

 "));
                }
                catch (ConfigLoaderNotExistsSectionException)
                {
                    catched = true;
                }
                finally
                {
                    Assert.AreEqual(true, catched);
                }
            }
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.ParseConfig(StringToStreamReader(@"
[section]
hoge = fuga
hoge = aho

"));
                }
                catch (ConfigLoaderMultiKeyException)
                {
                    catched = true;
                }
                finally
                {
                    Assert.AreEqual(true, catched);
                }
            }
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.ParseConfig(StringToStreamReader(@"
[section1]
hoge

[section2]
foo = bar

"));
                }
                catch (ConfigLoaderNotKeyValueException)
                {
                    catched = true;
                }
                finally
                {
                    Assert.AreEqual(true, catched);
                }
            }
            {
                ConfigLoader cl = new ConfigLoader();

                bool catched = false;
                try
                {
                    cl.ParseConfig(StringToStreamReader(@"
[section]
hoge = fuga

[section]
foo = bar

"));
                }
                catch (ConfigLoaderSameSectionException)
                {
                    catched = true;
                }
                finally
                {
                    Assert.AreEqual(true, catched);
                }
            }
        }

        void TestLoadingMulticonfig()
        {
            ConfigLoader cl = new ConfigLoader();

            cl.ParseConfig(StringToStreamReader(@"
[section1]
hoge = fuga
asdf =aassddff
[section2]
hoge = fuga asdf

[section3]
hoge=fuga ahoaho

[section4]
h = fuga
ho = fuga 
hog = fuga  
hoge = fuga   

"));
            cl.ParseConfig(StringToStreamReader(@"
[addedsection]
hoku = hoku
poka = poka

"));

            // セクションの取得
            List<string> sections = cl.GetSections();
            Assert.AreEqual(5, sections.Count);

            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section1"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section2"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section3"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "section4"; }));
            Assert.AreEqual(true, sections.Exists(delegate(string s) { return s == "addedsection"; }));
            Assert.AreEqual(false, sections.Exists(delegate(string s) { return s == "section5"; }));
            Assert.AreEqual(false, sections.Exists(delegate(string s) { return s == "section"; }));
            Assert.AreEqual(false, sections.Exists(delegate(string s) { return s == ""; }));

            // セクションから項目取得
            {
                Hashtable conf = cl.GetConfig("section1");
                Assert.AreEqual(2, conf.Count);
                Assert.AreEqual("fuga", (string)conf["hoge"]);
                Assert.AreEqual("aassddff", (string)conf["asdf"]);
            }
            {
                Hashtable conf = cl.GetConfig("addedsection");
                Assert.AreEqual(2, conf.Count);
                Assert.AreEqual("hoku", (string)conf["hoku"]);
                Assert.AreEqual("poka", (string)conf["poka"]);
            }
        }
    }
}
