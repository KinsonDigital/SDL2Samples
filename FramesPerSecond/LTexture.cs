﻿using System;
using System.Runtime.InteropServices;
using SDL2;

namespace FramesPerSecond
{
    //Texture wrapper class
    class LTexture
    {

        //Initializes variables
        public LTexture()
        {
            //Initialize
            mTexture = IntPtr.Zero;
            mWidth = 0;
            mHeight = 0;
        }

        //Deallocates memory
        ~LTexture()
        {
            free();
        }

        //Loads image at specified path
        public bool loadFromFile(string path)
        {
            //Get rid of preexisting texture
            free();

            //The final texture
            IntPtr newTexture = IntPtr.Zero;

            //Load image at specified path
            IntPtr loadedSurface = SDL_image.IMG_Load(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
            }
            else
            {
                var s = Marshal.PtrToStructure<SDL.SDL_Surface>(loadedSurface);

                //Color key image
                SDL.SDL_SetColorKey(loadedSurface, (int)SDL.SDL_bool.SDL_TRUE, SDL.SDL_MapRGB(s.format, 0, 0xFF, 0xFF));

                //Create texture from surface pixels
                newTexture = SDL.SDL_CreateTextureFromSurface(Program.gRenderer, loadedSurface);
                if (newTexture == IntPtr.Zero)
                {
                    Console.WriteLine("Unable to create texture from {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                }
                else
                {
                    //Get image dimensions
                    mWidth = s.w;
                    mHeight = s.h;
                }

                //Get rid of old loaded surface
                SDL.SDL_FreeSurface(loadedSurface);
            }

            //Return success
            mTexture = newTexture;
            return mTexture != IntPtr.Zero;
        }

        public bool loadFromRenderedText(string textureText, SDL.SDL_Color textColor)
        {
            //Get rid of preexisting texture
            free();

            //Render text surface
            IntPtr textSurface = SDL_ttf.TTF_RenderText_Solid(Program.gFont, textureText, textColor);
            if (textSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to render text surface! SDL_ttf Error: {0}", SDL.SDL_GetError());
            }
            else
            {
                //Create texture from surface pixels
                mTexture = SDL.SDL_CreateTextureFromSurface(Program.gRenderer, textSurface);
                if (mTexture == IntPtr.Zero)
                {
                    Console.WriteLine("Unable to create texture from rendered text! SDL Error: {0}", SDL.SDL_GetError());
                }
                else
                {
                    var s = Marshal.PtrToStructure<SDL.SDL_Surface>(textSurface);

                    //Get image dimensions
                    mWidth = s.w;
                    mHeight = s.h;
                }

                //Get rid of old surface
                SDL.SDL_FreeSurface(textSurface);
            }

            //Return success
            return mTexture != IntPtr.Zero;
        }

        //Deallocates texture
        public void free()
        {
            //Free texture if it exists
            if (mTexture != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(mTexture);
                mTexture = IntPtr.Zero;
                mWidth = 0;
                mHeight = 0;
            }
        }

        public void setColor(byte red, byte green, byte blue)
        {
            //Modulate texture
            SDL.SDL_SetTextureColorMod(mTexture, red, green, blue);
        }

        public void setBlendMode(SDL.SDL_BlendMode blending)
        {
            //Set blending function
            SDL.SDL_SetTextureBlendMode(mTexture, blending);
        }

        public void setAlpha(byte alpha)
        {
            //Modulate texture alpha
            SDL.SDL_SetTextureAlphaMod(mTexture, alpha);
        }

        //Renders texture at given point
        public void render(int x, int y, SDL.SDL_Rect? clip = null, double angle = 0, SDL.SDL_Point? center = null, SDL.SDL_RendererFlip flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE)
        {
            //Set rendering space and render to screen
            SDL.SDL_Rect renderQuad = new SDL.SDL_Rect { x = x, y = y, w = mWidth, h = mHeight };

            var myCenter = center ?? new SDL.SDL_Point();

            //Set clip rendering dimensions
            if (clip != null)
            {
                renderQuad.w = clip.Value.w;
                renderQuad.h = clip.Value.h;

                var myClip = clip.Value;
                
                SDL.SDL_RenderCopyEx(Program.gRenderer, mTexture, ref myClip, ref renderQuad, angle, ref myCenter, flip);
                return;
            }

            SDL.SDL_RenderCopyEx(Program.gRenderer, mTexture, IntPtr.Zero, ref renderQuad, angle, ref myCenter, flip);
        }

        //Gets image dimensions
        public int getWidth()
        {
            return mWidth;
        }

        public int getHeight()
        {
            return mHeight;
        }


        //The actual hardware texture
        private IntPtr mTexture;

        //Image dimensions
        private int mWidth;

        private int mHeight;
    };

}