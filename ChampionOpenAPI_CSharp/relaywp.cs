﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChampionOpenAPI_CSharp
{

    public delegate void wpdele(ref Message m);

    public class relaywp:Form
    {
        private readonly wpdele __wndProc;
        public relaywp(wpdele wndProc)
        {
            __wndProc = wndProc;
        }
        protected override void WndProc(ref Message m)
        {
            __wndProc?.Invoke(ref m);
            base.WndProc(ref m);
        }
    }

}
