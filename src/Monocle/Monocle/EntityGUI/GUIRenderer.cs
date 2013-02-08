﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle.Graphics;
using OpenTK;
using OpenTK.Graphics;
using Monocle.Utils;

namespace Monocle.EntityGUI
{
    class GUIRenderer : IGUIRenderer
    {
        private readonly ISpriteBuffer batch;
        private readonly Frame pixel;

        private Matrix4 matrix;


        public GUIRenderer(ISpriteBuffer batch, Frame pixel)
        {
            this.batch = batch;
            this.pixel = pixel;
        }

        public void DrawRect(ref Rect rect, Color color)
        {
            this.batch.BufferFrame(pixel, rect, color);         
        }

        public void DrawFrame(Frame frame, ref Rect rect, Color color)
        {
            this.batch.BufferFrame(frame, rect, color);
        }

        public void DrawString(Font textureFont, string text, ref Rect rect, Color color, TextAlignment textAlignment)
        {
            string canDraw = textureFont.BestFit(text, rect.W);
            Vector2 align = GetAlignment(textureFont, canDraw, ref rect, textAlignment);
            this.batch.BufferString(textureFont, canDraw, new Vector2(rect.X, rect.Y), color, align);
        }

        public void DrawMarkedString(Font textureFont, TextEditor text, ref Rect rect, Color color, Color selectionColor, TextAlignment textAlignment)
        {
            string canDraw = textureFont.BestFitBackWards(text.ToString(), text.MarkerIndex, rect.W);
            float markerPos;
            if (canDraw.Length < text.Length)
                markerPos = rect.W - 1;
            else
            {
                markerPos = textureFont.MessureSubstring(canDraw, 0, text.MarkerIndex - (text.Length - canDraw.Length)).X;
                markerPos = MathHelper.Clamp(0, rect.W - 1, markerPos);
            }

            Vector2 align = GetAlignment(textureFont, canDraw, ref rect, textAlignment);

            if (text.Selected)
            {
                float selectionStart = textureFont.MessureSubstring(canDraw, 0, text.SelectionIndex - (text.Length - canDraw.Length)).X;
                this.batch.BufferFrame(this.pixel, new Rect(rect.X + selectionStart, rect.Y, markerPos - selectionStart, rect.H), selectionColor * 0.6f);
                this.batch.BufferFrame(this.pixel, new Rect(rect.X + markerPos, rect.Y, 1, rect.H), color);
                this.batch.BufferString(textureFont, canDraw, new Vector2(rect.X, rect.Y), color, align);
            }
            else
            {
                this.batch.BufferFrame(this.pixel, new Rect(rect.X + markerPos, rect.Y, 1, rect.H), color);
                this.batch.BufferString(textureFont, canDraw, new Vector2(rect.X, rect.Y), color, align);   
            }
        }

        public void DrawMultiLineString(Font font, string text, ref Rect rect, Color color, ref Vector2 offset)
        {
            this.batch.BufferString(font, text, new Vector2(rect.X, rect.Y), color, offset);
        }

        public void DrawMarkedMultiLineString(Font font, TextEditor textEditor, ref Rect rect,ref Vector2 offset, Color color, Color selectionColor)
        {
            int realIndex;
            int lineIndex = FindLineIndex(textEditor.ToString(), textEditor.MarkerIndex, out realIndex);

            float markerIndex = font.MessureSubstring(textEditor.ToString(), realIndex, textEditor.MarkerIndex - realIndex).X;

            this.batch.BufferString(font, textEditor.ToString(), new Vector2(rect.X, rect.Y), color, offset);
            this.batch.BufferFrame(this.pixel, new Rect(rect.X + markerIndex - offset.X, rect.Y - offset.Y + lineIndex * font.LineHeight, 1, font.Size), color);
        }

        private unsafe int FindLineIndex(string p, int p_2, out int realIndex)
        {
            realIndex = 0;
            int index = 0;
            fixed (char* ptr = p)
            {
                for (int i = 0; i < p_2; i++)
                {
                    char c = ptr[i];
                    if (c == '\n' || c == '\r')
                    {
                        realIndex = i + 1;
                        index++;
                    }
                }
            }

            return index;
        }



        private Vector2 GetAlignment(Font textureFont, string text, ref Rect bounds, TextAlignment textAlignment)
        {
            Vector2 textSize = textureFont.MessureString(text);
            Vector2 offset = Vector2.Zero;
        
            switch (textAlignment)
            {
                case TextAlignment.Left:
                    break;
                case TextAlignment.Right:
                    offset.X = -bounds.W + textSize.X;
                    break;
                case TextAlignment.Center:
                    offset.X = textSize.X / 2 - (bounds.W) / 2;
                    offset.Y = 0;
                    break;
            }

            return offset;
        }


        public void Draw(GUIContainer container, ref Matrix4 projection)
        {
            this.matrix = projection;

            Vector2 _offset = container.Position;
            Rect _drawableArea = container.Bounds;
            this.SetSubRectDrawableArea(ref _drawableArea, ref _drawableArea, out _drawableArea);

            container.Draw(ref _drawableArea, this);


            this.batch.Draw(ref this.matrix);
        }

        public bool SetSubRectDrawableArea(ref Rect currentDrawable, ref Rect innerDrawable, out Rect subRect)
        {
            int x = (int)Math.Max(currentDrawable.Left, innerDrawable.Left);
            int y = (int)Math.Max(currentDrawable.Top, innerDrawable.Top);
            int w = (int)Math.Min(currentDrawable.Right, innerDrawable.Right) - x;
            int h = (int)Math.Min(currentDrawable.Bottom, innerDrawable.Bottom) - y;

            if (w <= 0 || h <= 0)
            {
                subRect = Rect.Zero;
                return false;
            }

            this.batch.Draw(ref this.matrix);
            subRect = new Rect(x, y, w, h);
            this.batch.GraphicsContext.Scissor = subRect;
            return true;
        }
    }
}
