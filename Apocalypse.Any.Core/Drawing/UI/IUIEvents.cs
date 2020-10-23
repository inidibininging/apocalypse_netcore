using System;
using Apocalypse.Any.Core;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IUIEvents
    {
        void OnClick(object sender, EventArgs args);
        void OnMouseEnter(object sender, EventArgs args);
        
        void OnMouseLeave(object sender, EventArgs args);
        
    }
}