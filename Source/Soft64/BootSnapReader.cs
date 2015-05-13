using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Soft64
{
    public sealed class BootSnapReader : IDisposable
    {
        private Stream m_BootSnapStream;
        private Boolean m_Disposed;

        private struct GprRegWrite
        {
            public Int32 index;
            public UInt64 value;
        }

        private struct Mem32Write
        {
            public Int64 position;
            public Int32 index;
            public UInt32 value;
        }
        
        public BootSnapReader(Stream xmlSource)
        {
            m_BootSnapStream = xmlSource;
        }

        private String HexFix(String hex)
        {
            if (hex.StartsWith("0x"))
            {
                return hex.Substring(2, hex.Length - 2);
            }

            return hex;
        }

        public void LoadBootSnap(CICKeyType cic, RegionType region)
        {
            List<GprRegWrite> regs = new List<GprRegWrite>();
            List<Mem32Write> mem32 = new List<Mem32Write>();
            BinaryWriter bw = new BinaryWriter(Machine.Current.RCP.SafeN64Memory);

            StreamReader reader = new StreamReader(m_BootSnapStream);
            XDocument doc = XDocument.Load(reader, LoadOptions.None);
            XElement root = doc.Root;

            var results = GetSnapElements(root, cic, region);
                    
            foreach (var result in results)
            {
                if (result.Name == "GPRRegSet")
                {
                    regs.Add(
                        new GprRegWrite
                        {
                            index = Int32.Parse(result.Attribute("Index").Value),
                            value = UInt64.Parse(HexFix(result.Value), NumberStyles.AllowHexSpecifier)
                        });
                }

                if (result.Name == "Mem32")
                {
                    mem32.Add(
                        new Mem32Write
                        {
                            index = Int32.Parse(result.Attribute("Index").Value),
                            position = Int64.Parse(HexFix(result.Attribute("Offset").Value), NumberStyles.AllowHexSpecifier),
                            value = UInt32.Parse(HexFix(result.Value), NumberStyles.AllowHexSpecifier),
                        });
                }
            }

            /* Now we do the actual writes to the emulator */
            foreach (var reg in regs)
            {
                Machine.Current.CPU.State.GPRRegs64[reg.index] = reg.value;
            }

            foreach (var m in mem32)
            {
                bw.BaseStream.Position = m.position + (4 * m.index);
                bw.Write(m.value);
            }
                    
        }

        private Boolean CompareCic (String cicName, CICKeyType cic)
        {
            return cic.ToString().Contains(cicName);
        }

        private Boolean CompareRegion(String regionName, RegionType region)
        {
            return region.ToString().Contains(regionName);
        }

        private IEnumerable<XElement> GetSnapElements(XElement snap, CICKeyType cic, RegionType region)
        {
            var condition = snap.Attribute("Condition");
            var value = snap.Attribute("Value");
            Boolean valid = false;

            if (condition != null)
            {
                switch (condition.Value) {
                    case "CIC": valid = CompareCic(value.Value, cic); break;
                    case "Region": valid = CompareRegion(value.Value, region); break;
                    default: break;
                }

                if (!valid)
                    return Enumerable.Empty<XElement>();
            }

            List<XElement> writes = new List<XElement>();

            writes.AddRange(snap.Elements("GPRRegSet"));
            writes.AddRange(snap.Elements("Mem32"));
            writes.AddRange(snap.Elements("BootSnap").SelectMany(s => GetSnapElements(s, cic, region)));

            return writes;
        }

        private void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    m_BootSnapStream.Dispose();
                }

                m_Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
