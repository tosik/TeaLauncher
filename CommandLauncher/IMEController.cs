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

        const int IME_CMODE_NOCONVERSION = 0x0100;
        const int IME_CMODE_FULLSHAPE = 0x0008;
        const int IME_CMODE_ALPHANUMERIC = 0x0000;
        const int IME_CMODE_NATIVE = 0x0001;
        const int IME_CMODE_ROMAN = 0x0010;
        const int IME_CMODE_LANGUAGE = 0x0003;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        public void On()
        {
            IntPtr ime_handle = (IntPtr)ImmGetContext(this.Handle);
            int result = ImmSetConversionStatus(ime_handle, IME_CMODE_NATIVE | IME_CMODE_FULLSHAPE | IME_CMODE_ROMAN, 0);
        }
        public void Off()
        {
            IntPtr ime_handle = (IntPtr)ImmGetContext(this.Handle);
            uint conv = 0, sent = 0;
            ImmGetConversionStatus(ime_handle, ref conv, ref sent);
            uint new_conv = conv;
            //uint new_conv = conv ^ IME_CMODE_NOCONVERSION;
            //uint new_conv = conv ^ IME_CMODE_FULLSHAPE;
            //int result = ImmSetConversionStatus(ime_handle, new_conv, sent);
            int result = ImmSetConversionStatus(ime_handle, IME_CMODE_ALPHANUMERIC, sent);
        }
    }
}
