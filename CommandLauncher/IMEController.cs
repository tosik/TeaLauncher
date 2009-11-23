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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace CommandLauncher
{
    class IMEController : Form
    {
        [DllImport("Imm32.dll")]
        private static extern int ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        private static extern int ImmSetConversionStatus(IntPtr hIMC, uint fdwConversion, uint fdwSentence);

        [DllImport("Imm32.dll")]
        private static extern int ImmGetConversionStatus(IntPtr hIMC, ref uint fdwConversion, ref uint fdwSentence);

        [DllImport("Imm32.dll")]
        private static extern int ImmSetOpenStatus(IntPtr hIMC, int fOpen);

        const int IME_CMODE_NOCONVERSION = 0x0100;
        const int IME_CMODE_FULLSHAPE = 0x0008;
        const int IME_CMODE_ALPHANUMERIC = 0x0000;
        const int IME_CMODE_NATIVE = 0x0001;
        const int IME_CMODE_ROMAN = 0x0010;
        const int IME_CMODE_LANGUAGE = 0x0003;

        const int TRUE = 1;
        const int FALSE = 0;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        public void On()
        {
            IntPtr ime_handle = (IntPtr)ImmGetContext(this.Handle);
            ImmSetOpenStatus(ime_handle, TRUE);
        }
        public void Off()
        {
            IntPtr ime_handle = (IntPtr)ImmGetContext(this.Handle);
            ImmSetOpenStatus(ime_handle, TRUE);
            ImmSetOpenStatus(ime_handle, FALSE);
        }

        public void Alphanumeric()
        {
            IntPtr ime_handle = (IntPtr)ImmGetContext(this.Handle);
            uint conv = 0, sent = 0;
            ImmGetConversionStatus(ime_handle, ref conv, ref sent);
            int result = ImmSetConversionStatus(ime_handle, IME_CMODE_ALPHANUMERIC, sent);
        }
    }
}
