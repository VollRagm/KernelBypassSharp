using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        Process TargetProcess;

        public MainForm()
        {
            InitializeComponent();
        }

        [DllImport("win32u.dll")]
        public static extern ulong NtUserGetObjectInformation(ulong a1, ulong a2, void* a3, ulong a4, ulong a5);
        
        private void ModuleBaseBtn_Click(object sender, EventArgs e)
        {
            ulong moduleBase = 0;

            SyscallData data = GetSyscallData(Operation.Base);
            data.TargetAddress = (ulong)&moduleBase;

            NtUserGetObjectInformation(0, 0, &data, 0, 0);

            MessageBox.Show($"Main Module Base -> {moduleBase:X8}");
        }

        private SyscallData GetSyscallData(Operation op)
        {
            return new SyscallData { Op = op, ProcessId = (uint)TargetProcess.Id, Magic = 0x69420 };
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SyscallData
        {
            public uint Magic;
            public uint ProcessId;
            public Operation Op;
            public ulong SourceAddress;
            public ulong TargetAddress;
            public ulong Size;
        }

        enum Operation
        {
            Read,
            Write,
            Base
        }

        private void selectProcBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TargetProcess = Process.GetProcessesByName(procName.Text)[0];
            }
            catch
            {
                MessageBox.Show("Could not select target process!");
            }
        }

        private void readBtn_Click(object sender, EventArgs e)
        {
            ulong result = 0;
            ulong sourceAddress = ulong.Parse(readAddrTb.Text, System.Globalization.NumberStyles.HexNumber);

            SyscallData data = GetSyscallData(Operation.Read);
            data.TargetAddress = (ulong)&result;
            data.SourceAddress = sourceAddress;
            data.Size = sizeof(ulong);

            NtUserGetObjectInformation(0, 0, &data, 0, 0);

            MessageBox.Show($"Read result -> {result}");
        }

        private void writeBtn_Click(object sender, EventArgs e)
        {
            long value = long.Parse(writeValTb.Text);
            ulong targetAddress = ulong.Parse(writeAddrTb.Text, System.Globalization.NumberStyles.HexNumber);

            SyscallData data = GetSyscallData(Operation.Write);
            data.SourceAddress = (ulong)&value;
            data.TargetAddress = targetAddress;
            data.Size = sizeof(ulong);

            NtUserGetObjectInformation(0, 0, &data, 0, 0);
        }
    }
}
