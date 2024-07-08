using AxChampionCommAgentLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public class AxChampionCommAgent2 : AxChampionCommAgent, IChampionCommAgent
    {
        private string apiAgentModulePath;
        private string gbcode_cod;
        private List<string> codeList;

        public AxChampionCommAgent2()
        {
            this.BeginInit();
            new System.Windows.Forms.GroupBox().Controls.Add(this);
            // 
            // axChampionCommAgent
            // 
            this.Enabled = true;
            this.Location = new System.Drawing.Point(168, 46);
            this.Name = "axChampionCommAgent1";
            this.Size = new System.Drawing.Size(46, 22);
            this.TabIndex = 16;
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif
            this.EndInit();
#if DEBUG
            Console.WriteLine("AxChampionCommAgent2..ctor():EndInit():" + sw.Elapsed);
#endif
        }

        public override string GetApiAgentModulePath()
        {
            if(!string.IsNullOrWhiteSpace(apiAgentModulePath))
            {
                return apiAgentModulePath;
            }

            apiAgentModulePath = base.GetApiAgentModulePath();
            if (!string.IsNullOrWhiteSpace(apiAgentModulePath))
            {
                return apiAgentModulePath;
            }

            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.OpenSubKey("Software\\EugeneFN\\Champion", true);
            if (regkey == null)
            {
                throw new KeyNotFoundException("프로그램의 위치를 찾지 못했습니다.");
            }
            
            Object objVal = regkey.GetValue("PATH");
            if (objVal == null)
            {
                throw new DirectoryNotFoundException("OpenApi의 위치를 찾지 못했습니다.");
            }

            apiAgentModulePath = Convert.ToString(objVal);
            if (string.IsNullOrWhiteSpace(apiAgentModulePath))
            {
                throw new NullReferenceException();
            }

            return apiAgentModulePath;
        }

        public IReadOnlyList<string> GetCodeList()
        {
            if (gbcode_cod == null)
            {
                gbcode_cod = Path.Combine(GetApiAgentModulePath(), "mst", "gbcode.cod");
                if (!File.Exists(gbcode_cod))
                {
                    throw new FileNotFoundException();
                }
            }
            if (codeList == null)
            {
                codeList = new List<string>();
                string[] allLines = File.ReadAllLines(gbcode_cod);
                foreach (var line in allLines)
                {
                    string code = line.Substring(0, 20);
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        code = code.Trim();
                        this.codeList.Add(code);
                    }
                }
            }
            return codeList;
        }
    }
}
