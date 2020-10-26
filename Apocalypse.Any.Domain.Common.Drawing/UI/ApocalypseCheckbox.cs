using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Domain.Common.Drawing.UI
{
    public class ApocalypseCheckbox<TContext> 
        : ApocalypseButton<TContext>
    {
        private const string CheckedSymbol = "x";
        public bool IsChecked { get; private set; }
        public ApocalypseCheckbox(Dictionary<(int frame, int x, int y), Rectangle> frames) : base(frames)
        {
            
        }

        public override void OnClick(object sender, EventArgs args)
        {
            Text = (IsChecked ^= true) == true ? CheckedSymbol : "";
            base.OnClick(sender, args);
        }
        
    }
}