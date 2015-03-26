from Soft64 import Machine
from Soft64.MipsR4300.CP0 import TLBCache
from Soft64.MipsR4300.CP0 import TLBEntry
from Soft64.MipsR4300 import WordSize
from System import Console


tlb = Machine.Current.CPU.VirtualMemoryStream.TLB

entryA = TLBEntry(WordSize.MIPS32)

tlb.AddEntry(0, entryA)

Console.WriteLine("TLB Test complete!")
