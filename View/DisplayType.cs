using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexEditor.View
{
    public enum DisplayType : byte
    {
        UNKNOWN = 0,

        FUNC_Selectable = 1,
        FUNC_Selected = 2,
        
        GAME_SimpleGround = 11,
        GAME_HasContent = 12
    }
}
