﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle.Graphics;
using OpenTK.Input;

namespace Monocle.GUI
{
    class ImageBox : GUIControl
    {
        public Texture2D Image
        {
            get;
            set;
        }

        public Rect SrcRect
        {
            get;
            set;
        }

        public ImageBox() : base() 
        {
        }
    }
}
