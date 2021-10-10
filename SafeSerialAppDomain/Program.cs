using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace SafeSerialAppDomain {
    class Program {
        public delegate void SerialDataReceivedEventHandler(string message);
        public event SerialDataReceivedEventHandler OnDataReceived;
        private SafeSerialPort sp1;
        public string portName = "COM4";
        public bool isOpen
        {
            get
            {
                return isRunning;
            }
        }
        private Thread thread_;
        private bool isRunning = false;
        private static string message;
        private bool isNewMessageReceived = false;
        
        static void Main(string[] args) {
            Console.WriteLine();
            Console.Write("\tPush Any Key\n\t>>");
            Console.ReadKey();
            Application.Run(new Open());
            while (true) {
                
            }
        }

        private static void Ssp1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
            try {
                var data = message.Split(new string[] { "E" }, System.StringSplitOptions.None);
                if (data.Length < 2) return;
                //Debug.Log(data[0]);
                Console.WriteLine(data[0]);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        private void initialize() {
            sp1.DataReceived += Ssp1_DataReceived;
            sp1 = new SafeSerialPort();
            sp1.PortName = portName;
        }
        public void Open() {
            if (sp1 == null) {
                initialize();
                try {
                    sp1.Open();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                if (sp1.IsOpen == true) {
                    isRunning = true;
                    thread_ = new Thread(Read);
                    thread_.Start();
                }
                else {
                    Disposer();
                    Console.WriteLine("Open Denied");
                }
            }
            else {
                Disposer();
                Console.WriteLine("Already Open!!");
            }
        }
        public void Loop() {
            while (true) {
                if (isNewMessageReceived) {
                    OnDataReceived(message);
                }
            }
        }
        public void Close() {
            if (isRunning == true) {
                isRunning = false;
            }
            else {
                Console.WriteLine("does not running");
            }
        }
        private void Disposer() {
            if (sp1 != null) {
                try {
                    sp1.Dispose();
                    sp1 = null;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            else {
                Console.WriteLine("Already Disposed!!");
            }
        }
        private void Read() {
            while (isRunning == true && sp1.BreakState == false) {
                try {
                    if (sp1.ReadBufferSize > 0) {
                        message = sp1.ReadLine();
                        isNewMessageReceived = true;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            isNewMessageReceived = false;
            Disposer();
            Console.WriteLine("end read");
        }
    }
}
