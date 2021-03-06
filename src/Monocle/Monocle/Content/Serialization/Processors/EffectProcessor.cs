﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle.Content.Serialization.ImportedContent;
using Monocle.Graphics;
using System.IO;

namespace Monocle.Content.Serialization.Processors
{
    [Processor(typeof(EffectContent), true)]
    public class EffectProcessor : Processor<EffectContent, ShaderProgram>
    {
        public override ShaderProgram Process(EffectContent input, IResourceContext context)
        {
            string vertexSource = LoadShaderSource(input.VertexShaderPath, context);
            string fragmentSource = LoadShaderSource(input.FragmentShaderPath, context);

            var gc = context.Locator.GetService<IGraphicsContext>();


            return ShaderProgram.CreateProgram(gc, vertexSource, fragmentSource);
        }

        private string LoadShaderSource(string path, IResourceContext context)
        {
            string correctPath = Path.GetDirectoryName(this.ResourcePath) + path;
            return context.LoadAsset<string>(correctPath);
        }
    }
}
