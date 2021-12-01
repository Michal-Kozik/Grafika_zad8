using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace Grafika_zad8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool IsPictureLoaded()
        {
            if (imgSource.Source == null)
                return false;
            else
                return true;
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                imgSource.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void Dilatation(object sender, RoutedEventArgs e)
        {
            if (!IsPictureLoaded())
            {
                MessageBox.Show("Nie załadowano obrazka.", "Brak obrazka", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bitmap imgSourceBitmap = ConvertImgToBitmap(imgSource);
            BitmapData sourceBitmapData = imgSourceBitmap.LockBits(new Rectangle(0, 0, imgSourceBitmap.Width, imgSourceBitmap.Height),
                                                            ImageLockMode.ReadOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            byte[] pixelBufferResult = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferResult, 0, pixelBufferResult.Length);
            imgSourceBitmap.UnlockBits(sourceBitmapData);
            
            // Analiza obrazu.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBuffer.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        int sum = 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride - 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride + 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i - 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride - 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride + 4] < 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Obiekt.
                            pixelBufferResult[i] = 0;
                            pixelBufferResult[i + 1] = 0;
                            pixelBufferResult[i + 2] = 0;
                        }
                        else
                        {
                            // Tlo.
                            pixelBufferResult[i] = 255;
                            pixelBufferResult[i + 1] = 255;
                            pixelBufferResult[i + 2] = 255;
                        }
                    }
                    catch { }
                }
            }

            // Rezultat.
            Bitmap imgResultBitmap = new Bitmap(imgSourceBitmap.Width, imgSourceBitmap.Height);
            BitmapData resultBitmapData = imgResultBitmap.LockBits(new Rectangle(0, 0, imgResultBitmap.Width, imgResultBitmap.Height),
                                                            ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBufferResult, 0, resultBitmapData.Scan0, pixelBufferResult.Length);
            imgResultBitmap.UnlockBits(resultBitmapData);
            imgResult.Source = ConvertBitmapToImageSource(imgResultBitmap);
        }

        private void Erosion(object sender, RoutedEventArgs e)
        {
            if (!IsPictureLoaded())
            {
                MessageBox.Show("Nie załadowano obrazka.", "Brak obrazka", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bitmap imgSourceBitmap = ConvertImgToBitmap(imgSource);
            BitmapData sourceBitmapData = imgSourceBitmap.LockBits(new Rectangle(0, 0, imgSourceBitmap.Width, imgSourceBitmap.Height),
                                                            ImageLockMode.ReadOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            byte[] pixelBufferResult = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferResult, 0, pixelBufferResult.Length);
            imgSourceBitmap.UnlockBits(sourceBitmapData);

            // Analiza obrazu.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBuffer.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        int sum = 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Tlo.
                            pixelBufferResult[i] = 255;
                            pixelBufferResult[i + 1] = 255;
                            pixelBufferResult[i + 2] = 255;
                        }
                        else
                        {
                            // Obiekt.
                            pixelBufferResult[i] = 0;
                            pixelBufferResult[i + 1] = 0;
                            pixelBufferResult[i + 2] = 0;
                        }
                    }
                    catch { }
                }
            }

            // Rezultat.
            Bitmap imgResultBitmap = new Bitmap(imgSourceBitmap.Width, imgSourceBitmap.Height);
            BitmapData resultBitmapData = imgResultBitmap.LockBits(new Rectangle(0, 0, imgResultBitmap.Width, imgResultBitmap.Height),
                                                            ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBufferResult, 0, resultBitmapData.Scan0, pixelBufferResult.Length);
            imgResultBitmap.UnlockBits(resultBitmapData);
            imgResult.Source = ConvertBitmapToImageSource(imgResultBitmap);
        }

        private void Opening(object sender, RoutedEventArgs e)
        {
            if (!IsPictureLoaded())
            {
                MessageBox.Show("Nie załadowano obrazka.", "Brak obrazka", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bitmap imgSourceBitmap = ConvertImgToBitmap(imgSource);
            BitmapData sourceBitmapData = imgSourceBitmap.LockBits(new Rectangle(0, 0, imgSourceBitmap.Width, imgSourceBitmap.Height),
                                                            ImageLockMode.ReadOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            byte[] pixelBufferResult = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferResult, 0, pixelBufferResult.Length);
            imgSourceBitmap.UnlockBits(sourceBitmapData);

            // Erozja.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBuffer.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        int sum = 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Tlo.
                            pixelBufferResult[i] = 255;
                            pixelBufferResult[i + 1] = 255;
                            pixelBufferResult[i + 2] = 255;
                        }
                        else
                        {
                            // Obiekt.
                            pixelBufferResult[i] = 0;
                            pixelBufferResult[i + 1] = 0;
                            pixelBufferResult[i + 2] = 0;
                        }
                    }
                    catch { }
                }
            }
            // Dylatacja.
            for (int i = 0; i + 4 < pixelBufferResult.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBufferResult.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        int sum = 0;
                        sum += (pixelBufferResult[i - sourceBitmapData.Stride - 4] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i - sourceBitmapData.Stride] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i - sourceBitmapData.Stride + 4] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i - 4] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + 4] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + sourceBitmapData.Stride - 4] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + sourceBitmapData.Stride] < 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + sourceBitmapData.Stride + 4] < 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Obiekt.
                            pixelBuffer[i] = 0;
                            pixelBuffer[i + 1] = 0;
                            pixelBuffer[i + 2] = 0;
                        }
                        else
                        {
                            // Tlo.
                            pixelBuffer[i] = 255;
                            pixelBuffer[i + 1] = 255;
                            pixelBuffer[i + 2] = 255;
                        }
                    }
                    catch { }
                }
            }

            // Rezultat.
            Bitmap imgResultBitmap = new Bitmap(imgSourceBitmap.Width, imgSourceBitmap.Height);
            BitmapData resultBitmapData = imgResultBitmap.LockBits(new Rectangle(0, 0, imgResultBitmap.Width, imgResultBitmap.Height),
                                                            ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultBitmapData.Scan0, pixelBuffer.Length);
            imgResultBitmap.UnlockBits(resultBitmapData);
            imgResult.Source = ConvertBitmapToImageSource(imgResultBitmap);
        }

        private void ClosingOperation(object sender, RoutedEventArgs e)
        {
            if (!IsPictureLoaded())
            {
                MessageBox.Show("Nie załadowano obrazka.", "Brak obrazka", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bitmap imgSourceBitmap = ConvertImgToBitmap(imgSource);
            BitmapData sourceBitmapData = imgSourceBitmap.LockBits(new Rectangle(0, 0, imgSourceBitmap.Width, imgSourceBitmap.Height),
                                                            ImageLockMode.ReadOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            byte[] pixelBufferResult = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferResult, 0, pixelBufferResult.Length);
            imgSourceBitmap.UnlockBits(sourceBitmapData);

            // Dylatacja.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBuffer.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        int sum = 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride - 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride + 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i - 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride - 4] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride] < 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride + 4] < 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Obiekt.
                            pixelBufferResult[i] = 0;
                            pixelBufferResult[i + 1] = 0;
                            pixelBufferResult[i + 2] = 0;
                        }
                        else
                        {
                            // Tlo.
                            pixelBufferResult[i] = 255;
                            pixelBufferResult[i + 1] = 255;
                            pixelBufferResult[i + 2] = 255;
                        }
                    }
                    catch { }
                }
            }
            // Erozja.
            for (int i = 0; i + 4 < pixelBufferResult.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBufferResult.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        int sum = 0;
                        sum += (pixelBufferResult[i - sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i - sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i - sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i - 4] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + 4] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBufferResult[i + sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Tlo.
                            pixelBuffer[i] = 255;
                            pixelBuffer[i + 1] = 255;
                            pixelBuffer[i + 2] = 255;
                        }
                        else
                        {
                            // Obiekt.
                            pixelBuffer[i] = 0;
                            pixelBuffer[i + 1] = 0;
                            pixelBuffer[i + 2] = 0;
                        }
                    }
                    catch { }
                }
            }

            // Rezultat.
            Bitmap imgResultBitmap = new Bitmap(imgSourceBitmap.Width, imgSourceBitmap.Height);
            BitmapData resultBitmapData = imgResultBitmap.LockBits(new Rectangle(0, 0, imgResultBitmap.Width, imgResultBitmap.Height),
                                                            ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultBitmapData.Scan0, pixelBuffer.Length);
            imgResultBitmap.UnlockBits(resultBitmapData);
            imgResult.Source = ConvertBitmapToImageSource(imgResultBitmap);
        }

        private void HitOrMiss(object sender, RoutedEventArgs e)
        {
            if (!IsPictureLoaded())
            {
                MessageBox.Show("Nie załadowano obrazka.", "Brak obrazka", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bitmap imgSourceBitmap = ConvertImgToBitmap(imgSource);
            BitmapData sourceBitmapData = imgSourceBitmap.LockBits(new Rectangle(0, 0, imgSourceBitmap.Width, imgSourceBitmap.Height),
                                                            ImageLockMode.ReadOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            byte[] pixelBufferInvert = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferInvert, 0, pixelBufferInvert.Length);

            byte[] pixelBufferHitOrMiss1 = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferHitOrMiss1, 0, pixelBufferHitOrMiss1.Length);

            byte[] pixelBufferHitOrMiss2 = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferHitOrMiss2, 0, pixelBufferHitOrMiss2.Length);

            byte[] pixelBufferHitOrMiss = new byte[sourceBitmapData.Stride * sourceBitmapData.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixelBufferHitOrMiss, 0, pixelBufferHitOrMiss.Length);

            imgSourceBitmap.UnlockBits(sourceBitmapData);

            // Erozja nr 1.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBuffer.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        // 0 1 0
                        // 1 1 1
                        // 0 1 0
                        int sum = 0;
                        //sum += (pixelBuffer[i - sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - sourceBitmapData.Stride] > 127) ? 1 : 0;
                        //sum += (pixelBuffer[i - sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + 4] > 127) ? 1 : 0;
                        //sum += (pixelBuffer[i + sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        sum += (pixelBuffer[i + sourceBitmapData.Stride] > 127) ? 1 : 0;
                        //sum += (pixelBuffer[i + sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Tlo.
                            pixelBufferHitOrMiss1[i] = 255;
                            pixelBufferHitOrMiss1[i + 1] = 255;
                            pixelBufferHitOrMiss1[i + 2] = 255;
                        }
                        else
                        {
                            // Obiekt.
                            pixelBufferHitOrMiss1[i] = 0;
                            pixelBufferHitOrMiss1[i + 1] = 0;
                            pixelBufferHitOrMiss1[i + 2] = 0;
                        }
                    }
                    catch { }
                }
            }
            // Odwrocenie obrazu.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                if (pixelBuffer[i] < 127)
                {
                    pixelBufferInvert[i] = 255;
                    pixelBufferInvert[i + 1] = 255;
                    pixelBufferInvert[i + 2] = 255;
                }
                else
                {
                    pixelBufferInvert[i] = 0;
                    pixelBufferInvert[i + 1] = 0;
                    pixelBufferInvert[i + 2] = 0;
                }
            }
            // Erozja nr 2.
            for (int i = 0; i + 4 < pixelBuffer.Length; i += 4)
            {
                // Pierwszy wiersz.
                if (i <= sourceBitmapData.Stride)
                {
                    continue;
                }
                // Ostatni wiersz.
                else if (i >= pixelBuffer.Length - sourceBitmapData.Stride)
                {
                    continue;
                }
                // Pierwsza kolumna.
                else if (i % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                // Ostatnia kolumna.
                else if ((i - 4) % sourceBitmapData.Stride == 0)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        // 1 0 1
                        // 0 0 0
                        // 1 0 1
                        int sum = 0;
                        sum += (pixelBufferInvert[i - sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        //sum += (pixelBufferInvert[i - sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBufferInvert[i - sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        //sum += (pixelBufferInvert[i - 4] > 127) ? 1 : 0;
                        //sum += (pixelBufferInvert[i] > 127) ? 1 : 0;
                        //sum += (pixelBufferInvert[i + 4] > 127) ? 1 : 0;
                        sum += (pixelBufferInvert[i + sourceBitmapData.Stride - 4] > 127) ? 1 : 0;
                        //sum += (pixelBufferInvert[i + sourceBitmapData.Stride] > 127) ? 1 : 0;
                        sum += (pixelBufferInvert[i + sourceBitmapData.Stride + 4] > 127) ? 1 : 0;
                        if (sum > 0)
                        {
                            // Tlo.
                            pixelBufferHitOrMiss2[i] = 255;
                            pixelBufferHitOrMiss2[i + 1] = 255;
                            pixelBufferHitOrMiss2[i + 2] = 255;
                        }
                        else
                        {
                            // Obiekt.
                            pixelBufferHitOrMiss2[i] = 0;
                            pixelBufferHitOrMiss2[i + 1] = 0;
                            pixelBufferHitOrMiss2[i + 2] = 0;
                        }
                    }
                    catch { }
                }
            }
            // Czesc wspolna.
            for (int i = 0; i + 4 < pixelBufferHitOrMiss1.Length; i += 4)
            {
                if (pixelBufferHitOrMiss1[i] == pixelBuffer[i] && pixelBufferHitOrMiss2[i] != pixelBuffer[i])
                {
                    // Obiekt.
                    pixelBufferHitOrMiss[i] = 0;
                    pixelBufferHitOrMiss[i + 1] = 0;
                    pixelBufferHitOrMiss[i + 2] = 0;
                }
                else
                {
                    // Tlo.
                    pixelBufferHitOrMiss[i] = 255;
                    pixelBufferHitOrMiss[i + 1] = 255;
                    pixelBufferHitOrMiss[i + 2] = 255;
                }
            }

            // Rezultat.
            Bitmap imgResultBitmap = new Bitmap(imgSourceBitmap.Width, imgSourceBitmap.Height);
            BitmapData resultBitmapData = imgResultBitmap.LockBits(new Rectangle(0, 0, imgResultBitmap.Width, imgResultBitmap.Height),
                                                            ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBufferHitOrMiss, 0, resultBitmapData.Scan0, pixelBufferHitOrMiss.Length);
            imgResultBitmap.UnlockBits(resultBitmapData);
            imgResult.Source = ConvertBitmapToImageSource(imgResultBitmap);
        }

        private Bitmap ConvertImgToBitmap(System.Windows.Controls.Image source)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)source.ActualWidth, (int)source.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
            source.Measure(new System.Windows.Size((int)source.ActualWidth, (int)source.ActualHeight));
            source.Arrange(new Rect(new System.Windows.Size((int)source.ActualWidth, (int)source.ActualHeight)));
            renderTargetBitmap.Render(source);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            encoder.Save(stream);
            Bitmap bitmap = new Bitmap(stream);
            stream.Close();
            renderTargetBitmap.Clear();
            return bitmap;
        }

        private BitmapImage ConvertBitmapToImageSource(Bitmap bitmap)
        {
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
