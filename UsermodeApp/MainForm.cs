using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsermodeApp
{
    public unsafe partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        [DllImport("win32u.dll")]
        public static extern ulong NtUserGetObjectInformation(ulong a1, ulong a2, void* a3, ulong a4, ulong a5);
        
        private void CallBtn_Click(object sender, EventArgs e)
        {
            SyscallData data = new SyscallData()
            {
                Magic = 0x69420
            };

            NtUserGetObjectInformation(0, 0, &data, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SyscallData
        {
            public uint Magic;
        }
    }
}
