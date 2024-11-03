using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;
using ImageProcess2;
using System.Diagnostics;
using Emgu.CV.Structure;
using Emgu.CV;
namespace WindowsFormsApp1
{
    
    public partial class Form1 : Form
    {
        Bitmap loaded, processed,imageA,imageB,colorGreen;
        Device[] devices;
        bool stream = false;
        Capture capture;
        bool on = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk_1(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            imageA = new Bitmap(openFileDialog1.FileName);

            pictureBox1.Image = loaded;
            
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for(int x =0;x < loaded.Width; x++)
            {
                for(int y=0;y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x,y);
                    processed.SetPixel(x,y,pixel);
                }
            }
            //pictureBox2.Image = processed;
            pictureBox2.Image = pictureBox1.Image;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();  
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void grayScalingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int avrg;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    avrg =(int) (pixel.R+pixel.G+pixel.B)/3;
                    Color gray = Color.FromArgb(avrg,avrg,avrg);
                    processed.SetPixel(x, y, gray);
                }
            }
            //pictureBox2.Image = processed;
            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int avrg;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    
                    Color invert = Color.FromArgb(255-pixel.R, 255-pixel.G, 255-pixel.B);
                    processed.SetPixel(x, y, invert);
                }
            }
            pictureBox2.Image = processed;
        }

        
        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Hist(ref loaded, ref processed);
            pictureBox2.Image = processed;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            BasicDIP.Brightness(ref loaded,ref processed,trackBar1.Value);
            pictureBox2.Image = processed;
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Equalisation(ref loaded, ref processed, trackBar2.Value);
            pictureBox2.Image = processed;
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel((loaded.Width-1)-x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void mirrorVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, (loaded.Height-1)-y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            BasicDIP.Rotate(ref loaded, ref processed, trackBar3.Value);
            pictureBox2.Image = processed;
        }

        private void scalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Scale(ref loaded, ref processed, 100, 100);
            pictureBox2.Image = processed;
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int avrg;
            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    avrg = (int)(pixel.R + pixel.G + pixel.B) / 3;
                    if(avrg < 180)
                    {
                        processed.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        processed.SetPixel(x, y, Color.White);
                    }
                }
            }
            pictureBox2.Image = processed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //devices = DeviceManager.GetAllDevices();
            stream = false;
        }
        private void streaming(object sender,System.EventArgs e)
        {
            var image = capture.QueryFrame().ToImage<Bgr,byte> ();
            var bmp = image.Bitmap;
            loaded = new Bitmap(bmp.Width, bmp.Height);
            pictureBox1.Image = bmp;
            loaded = bmp;
            imageA = bmp;
        }

        private void cameraOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //devices[0].ShowWindow(pictureBox1); 
            
            if (!on)
            {
                pictureBox1.Image = null;

                capture = new Capture();

                Application.Idle += streaming;

                stream = true;
            }
            on = true;
        }

        private void cameraOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //devices[0].Stop();
            if (on)
            {
                capture.Dispose();

                Application.Idle -= streaming;

                stream = false;
                pictureBox1.Image = null;
            }
            on=false;
           
        }

        private void greyScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stream == true)
            {
                var image = capture.QueryFrame().ToImage<Bgr, byte>();
                var bmp = image.Bitmap;
                loaded = new Bitmap(bmp.Width, bmp.Height);
                pictureBox1.Image = bmp;
                loaded = bmp;
                imageA = bmp;
                Bitmap b = new Bitmap(bmp);

                BitmapFilter.GrayScale(b);
                pictureBox2.Image = b;
            }
            else
            {
                pictureBox2.Image = null;
            }           
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            Color pixel;
            int tr, tg, tb;

            for (int i = 0; i < loaded.Width; i++)
            {
                for (int j = 0; j < loaded.Height; j++)
                {
                    pixel = loaded.GetPixel(i, j);


                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;


                    tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                    tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                    tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);


                    if (tr > 255) tr = 255;
                    if (tg > 255) tg = 255;
                    if (tb > 255) tb = 255;

                    Color sepia = Color.FromArgb(tr, tg, tb);
                    processed.SetPixel(i, j, sepia);
                }
            }
            pictureBox2.Image = processed;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap resultiamge = new Bitmap(imageB.Width, imageB.Height);

            Color myg = Color.FromArgb(0, 0, 255);
            int greygreen = (myg.R + myg.G + myg.B) / 3;
            int threshold = 5;
            

            for(int i = 0; i < imageB.Width; i++)
            {
                for(int j = 0; j < imageB.Height; j++)
                {
                    Color pixel = imageB.GetPixel(i, j);
                    Color backpixel = imageA.GetPixel(i, j);
                    int grey = (pixel.R + pixel.G + pixel.B)/3;
                    int subtractvalue = Math.Abs(grey - greygreen);

                    if(subtractvalue > threshold)
                    {
                        resultiamge.SetPixel(i, j, pixel);
                    }
                    else
                    {
                        resultiamge.SetPixel(i, j, backpixel);
                    }
                }
            }
            pictureBox2.Image = resultiamge;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog2.FileName);
            pictureBox3.Image = imageB;
        }

        private void inversionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            Color pixel;
            int avrg;
            if (stream == true)
            {
                var image = capture.QueryFrame().ToImage<Bgr, byte>();
                var bmp = image.Bitmap;
                loaded = new Bitmap(bmp.Width, bmp.Height);
                pictureBox1.Image = bmp;
                loaded = bmp;
                imageA = bmp;
                Bitmap b = new Bitmap(bmp);

                for (int x = 0; x < loaded.Width; x++)
                {
                    for (int y = 0; y < loaded.Height; y++)
                    {
                        pixel = loaded.GetPixel(x, y);

                        Color invert = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                        b.SetPixel(x, y, invert);
                    }
                }
                pictureBox2.Image = b;
            }
            else
            {
                pictureBox2.Image = null;
            }
           


        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void contrastToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer3.Enabled = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (stream == true)
            {
                var image = capture.QueryFrame().ToImage<Bgr, byte>();
                var bmp = image.Bitmap;
                loaded = new Bitmap(bmp.Width, bmp.Height);
                pictureBox1.Image = bmp;
                loaded = bmp;
                imageA = bmp;
                Bitmap b = new Bitmap(bmp);

                BasicDIP.Equalisation(ref b, ref loaded, trackBar2.Value);
                pictureBox2.Image = b;
            }
            else
            {
                pictureBox2.Image = null;
            }
        }

        private void sepiaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer4.Enabled = true;
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            Color pixel;
            int tr, tg, tb;
            if (stream == true)
            {
                var image = capture.QueryFrame().ToImage<Bgr, byte>();
                var bmp = image.Bitmap;
                loaded = new Bitmap(bmp.Width, bmp.Height);
                pictureBox1.Image = bmp;
                loaded = bmp;
                imageA = bmp;
                Bitmap bitmap = new Bitmap(bmp);

                for (int i = 0; i < loaded.Width; i++)
                {
                    for (int j = 0; j < loaded.Height; j++)
                    {
                        pixel = loaded.GetPixel(i, j);

                        int r = pixel.R;
                        int g = pixel.G;
                        int b = pixel.B;

                        tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                        tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                        tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);


                        if (tr > 255) tr = 255;
                        if (tg > 255) tg = 255;
                        if (tb > 255) tb = 255;

                        Color sepia = Color.FromArgb(tr, tg, tb);
                        bitmap.SetPixel(i, j, sepia);
                    }
                }
                pictureBox2.Image = bitmap;
            }
            else
            {
                pictureBox2.Image = null;
            }
        }

        private void subtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer5.Enabled = true;
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            Bitmap resultiamge = new Bitmap(imageB.Width, imageB.Height);
            Color myg = Color.FromArgb(0, 0, 255);
            int greygreen = (myg.R + myg.G + myg.B) / 3;
            int threshold = 5;

            if (stream == true)
            {
                var image = capture.QueryFrame().ToImage<Bgr, byte>();
                var bmp = image.Bitmap;
                loaded = new Bitmap(bmp.Width, bmp.Height);
                pictureBox1.Image = bmp;
                loaded = bmp;
                imageA = bmp;
                Bitmap bitmap = new Bitmap(bmp);

                for (int i = 0; i < imageB.Width; i++)
                {
                    for (int j = 0; j < imageB.Height; j++)
                    {
                        Color pixel = imageB.GetPixel(i, j);
                        Color backpixel = imageA.GetPixel(i, j);
                        int grey = (pixel.R + pixel.G + pixel.B) / 3;
                        int subtractvalue = Math.Abs(grey - greygreen);

                        if (subtractvalue > threshold)
                        {
                            resultiamge.SetPixel(i, j, pixel);
                        }
                        else
                        {
                            resultiamge.SetPixel(i, j, backpixel);
                        }
                    }
                }
                pictureBox2.Image = resultiamge;

            }
            else
            {
                pictureBox2.Image = null;
            }
            
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stream)
            {
               cameraOffToolStripMenuItem_Click(this, EventArgs.Empty);
            }
           
            openFileDialog1.ShowDialog();
        }

    }
}
