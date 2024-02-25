using Cosmos.Core.Memory;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
namespace HydrixOS.Core.Graphics
{
    public class GraphicsHandler
    {
        [ManifestResourceStream(ResourceName = "HydrixOS.Images.cursor.bmp")]
        private static byte[] cursor;
        public bool isgraphicsrunning = true;
        public void Start(Canvas canvas)
        {
            Heap.Collect();
            canvas = new SVGAIICanvas();
            canvas.Mode = new Mode(1920u, 1080u, ColorDepth.ColorDepth32);
            canvas.Clear(Color.Black);
            Sys.MouseManager.ScreenWidth = 1920;
            Sys.MouseManager.ScreenHeight = 1080;
            Sys.MouseManager.X = 1920 / 2;
            Sys.MouseManager.Y = 1080 / 2;

            while (isgraphicsrunning)
            {
                //clear and display
                canvas.Clear(Color.White);
                //draw dot at mouse position
                canvas.DrawImageAlpha(new Bitmap(cursor), (int)Sys.MouseManager.X, (int)Sys.MouseManager.Y);
                //check if left mouse button is down

                canvas.Display();
                Heap.Collect();
            }

        }
        public class basicwindow
        {
            public string title;
            public int x;
            public int y;
            public int width;
            public int height;
            public basicwindow(string title, int x, int y, int width, int height)
            {
                this.title = title;
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }
            public void Draw(SVGAIICanvas canvas)
            {
                canvas.DrawRectangle(Color.White, x, y, width, height);
            }
        }
        public class Button
        {
            int x;
            int y;
            int width;
            int height;
            string text;
            public Button(int x, int y, int width, int height, string text)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.text = text;
            }
            public void Draw(SVGAIICanvas canvas)
            {
                canvas.DrawRectangle(Color.White, x, y, width, height);
                canvas.DrawString(text, PCScreenFont.Default, Color.Black, x, y);
            }
        }
    }
}
