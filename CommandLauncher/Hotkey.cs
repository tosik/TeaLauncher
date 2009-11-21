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



// from http://www.k4.dion.ne.jp/~anis7742/codevault/index.html

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// グローバルホットキーを登録するクラス。
/// 使用後は必ずDisposeすること。
/// </summary>
public class HotKey : IDisposable
{
    HotKeyForm m_Form;
    /// <summary>
    /// ホットキーが押されると発生する。
    /// </summary>
    public event EventHandler HotKeyPressed;

    /// <summary>
    /// ホットキーを指定して初期化する。
    /// 使用後は必ずDisposeすること。
    /// </summary>
    /// <param name="modKey">修飾キー</param>
    /// <param name="key">キー</param>
    public HotKey(MOD_KEY modKey, Keys key)
    {
        m_Form = new HotKeyForm(modKey, key, raiseHotKeyPressed);
    }

    private void raiseHotKeyPressed()
    {
        if (HotKeyPressed != null)
        {
            HotKeyPressed(this, EventArgs.Empty);
        }
    }

    public void Dispose()
    {
        m_Form.Dispose();
    }

    private class HotKeyForm : Form
    {
        [DllImport("user32.dll")]
        extern static int RegisterHotKey(IntPtr HWnd, int ID, MOD_KEY MOD_KEY, Keys KEY);

        [DllImport("user32.dll")]
        extern static int UnregisterHotKey(IntPtr HWnd, int ID);

        const int WM_HOTKEY = 0x0312;
        int id;
        ThreadStart proc;

        public HotKeyForm(MOD_KEY modKey, Keys key, ThreadStart proc)
        {
            this.proc = proc;
            for (int i = 0x0000; i <= 0xbfff; i++)
            {
                if (RegisterHotKey(this.Handle, i, modKey, key) != 0)
                {
                    id = i;
                    break;
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if ((int)m.WParam == id)
                {
                    proc();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            UnregisterHotKey(this.Handle, id);
            base.Dispose(disposing);
        }
    }
}

/// <summary>
/// HotKeyクラスの初期化時に指定する修飾キー
/// </summary>
public enum MOD_KEY : int
{
    ALT = 0x0001,
    CONTROL = 0x0002,
    SHIFT = 0x0004,
}