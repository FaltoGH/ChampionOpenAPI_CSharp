using AxChampionCommAgentLib;
using System;
using System.Collections.Generic;
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
        public List<string> GetCodeList()
        {
            if (this.apiAgentModulePath == null)
            {
                apiAgentModulePath = GetApiAgentModulePath();
                if (string.IsNullOrWhiteSpace(apiAgentModulePath))
                {
                    throw new ArgumentNullException();
                }
            }
            if (gbcode_cod == null)
            {
                gbcode_cod = Path.Combine(apiAgentModulePath, "gbcode.cod");
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
