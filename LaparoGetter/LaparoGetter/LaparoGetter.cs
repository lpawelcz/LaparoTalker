using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaparoTalker
{
    public class LaparoGetter
    {
        public float[] Values = new float[7];

        LaparoTalker program = new LaparoTalker();
        public void Init()
        {
            program.Main();
        }
        public void FileInit(string filepath)
        {
            program.FromFileBegin(filepath);
        }

        public void GetVals(ref float[] Values)
        {
            program.GetValues(ref Values);
        }

        public void End()
        {
            program.End();
        }

        public void FileEnd()
        {
            program.FromFileEnd();
        }
    }
}
