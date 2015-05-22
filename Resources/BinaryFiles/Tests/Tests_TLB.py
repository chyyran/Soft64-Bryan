from Soft64 import Machine
from Soft64.MipsR4300.CP0 import TLBCache
from Soft64.MipsR4300.CP0 import TLBEntry
from Soft64.MipsR4300 import WordSize
from Soft64.MipsR4300.CP0 import VirtualPageNumber2
from Soft64.MipsR4300.CP0 import PageFrameNumber
from System import Console


tlb = Machine.Current.DeviceCPU.VirtualMemoryStream.TLB

entry = TLBEntry()
entry.VPN2 = VirtualPageNumber2(2, 0x345)
tlb.AddEntry(0, entry)

entry = TLBEntry()
entry.VPN2 = VirtualPageNumber2(2, 0x645)
entry.PfnEven = PageFrameNumber(0x2000);
entry.PfnEven.IsValid = True;
tlb.AddEntry(1, entry)

entry = TLBEntry()
entry.VPN2 = VirtualPageNumber2(2, 0x445)
entry.PfnOdd = PageFrameNumber(0x3000);
entry.PfnOdd.IsValid = True;
tlb.AddEntry(2, entry)

Console.WriteLine("TLB Test complete!")
