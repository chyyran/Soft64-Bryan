from Soft64 import Machine
from Soft64.Engines import EmulatorEngine
from System import String
from System.Text import StringBuilder
from System.Threading import Thread
from System.Windows import MessageBox


scheduler = Machine.Current.CurrentEngine.CurrentScheduler
threads = scheduler.GetThreads()
str = StringBuilder()

for thread in threads:
    str.Append(String.Format("Thread {0}: {1}", thread.ManagedThreadId.ToString(), thread.ThreadState))

MessageBox.Show(str.ToString())